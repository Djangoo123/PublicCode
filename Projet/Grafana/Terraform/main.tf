terraform {
  required_version = ">= 1.6.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }
}

provider "aws" {
  region = var.region
}

# ---------------- Zones de disponibilité dynamiques ----------------
data "aws_availability_zones" "available" {
  state = "available"
}

# ---------------- VPC + réseau ----------------
resource "aws_vpc" "this" {
  cidr_block           = "10.1.0.0/16"
  enable_dns_hostnames = true
  enable_dns_support   = true
  tags                 = { Name = "grafana-ecs-vpc" }
}

resource "aws_internet_gateway" "igw" {
  vpc_id = aws_vpc.this.id
}

# Subnet 1 - AZ[0]
resource "aws_subnet" "public" {
  vpc_id                  = aws_vpc.this.id
  cidr_block              = "10.1.1.0/24"
  map_public_ip_on_launch = true
  availability_zone_id    = data.aws_availability_zones.available.zone_ids[0]
  tags                    = { Name = "grafana-ecs-subnet-a" }
}

# Subnet 2 - AZ[1]
resource "aws_subnet" "public_b" {
  vpc_id                  = aws_vpc.this.id
  cidr_block              = "10.1.2.0/24"
  map_public_ip_on_launch = true
  availability_zone_id    = data.aws_availability_zones.available.zone_ids[1]
  tags                    = { Name = "grafana-ecs-subnet-b" }
}

# Table de routage publique (vers Internet Gateway)
resource "aws_route_table" "public" {
  vpc_id = aws_vpc.this.id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.igw.id
  }

  tags = { Name = "grafana-public-rt" }
}

# Associations explicites pour les subnets publics
resource "aws_route_table_association" "public_a" {
  subnet_id      = aws_subnet.public.id
  route_table_id = aws_route_table.public.id
}

resource "aws_route_table_association" "public_b" {
  subnet_id      = aws_subnet.public_b.id
  route_table_id = aws_route_table.public.id
}

# ---------------- Security Group principal ----------------
resource "aws_security_group" "ecs_sg" {
  name        = "grafana-ecs-sg"
  description = "Allow Grafana, Loki and ECS comms"
  vpc_id      = aws_vpc.this.id

  ingress {
    from_port   = 3000
    to_port     = 3000
    protocol    = "tcp"
    cidr_blocks = [var.allowed_ip]
    description = "Grafana access"
  }

  ingress {
    from_port   = 3100
    to_port     = 3100
    protocol    = "tcp"
    cidr_blocks = ["10.1.0.0/16"]
    description = "Loki internal"
  }

  ingress {
    from_port   = 2049
    to_port     = 2049
    protocol    = "tcp"
    cidr_blocks = ["10.1.0.0/16"]
    description = "Allow NFS traffic for EFS"
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = { Name = "grafana-ecs-sg" }
}

# ---------------- ECS Cluster ----------------
resource "aws_ecs_cluster" "this" {
  name = "grafana-cluster"
}

# ---------------- CloudWatch Logs ----------------
resource "aws_cloudwatch_log_group" "grafana" {
  name              = "/ecs/grafana"
  retention_in_days = 7
}

resource "aws_cloudwatch_log_group" "loki" {
  name              = "/ecs/loki"
  retention_in_days = 7
}

# ---------------- IAM Roles ----------------
data "aws_iam_policy_document" "ecs_assume_role" {
  statement {
    actions = ["sts:AssumeRole"]
    principals {
      type        = "Service"
      identifiers = ["ecs-tasks.amazonaws.com"]
    }
  }
}

resource "aws_iam_role" "ecs_task_execution" {
  name               = "ecsTaskExecutionRole-grafana"
  assume_role_policy = data.aws_iam_policy_document.ecs_assume_role.json
}

resource "aws_iam_role_policy_attachment" "ecs_execution_policy" {
  role       = aws_iam_role.ecs_task_execution.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

resource "aws_iam_role" "ecs_task_loki" {
  name               = "ecsTaskRole-loki"
  assume_role_policy = data.aws_iam_policy_document.ecs_assume_role.json
}

resource "aws_iam_role_policy_attachment" "ecs_task_loki_efs" {
  role       = aws_iam_role.ecs_task_loki.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonElasticFileSystemClientReadWriteAccess"
}

# ---------------- Task Definitions ----------------
resource "aws_ecs_task_definition" "grafana" {
  family                   = "grafana-task"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = 512
  memory                   = 1024
  execution_role_arn       = aws_iam_role.ecs_task_execution.arn
  container_definitions    = file("${path.module}/ecs-task-grafana.json")
}

resource "aws_ecs_task_definition" "loki" {
  family                   = "loki-task"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = 512
  memory                   = 1024
  execution_role_arn       = aws_iam_role.ecs_task_execution.arn
  task_role_arn            = aws_iam_role.ecs_task_loki.arn

  container_definitions = jsonencode([
    {
      name      = "loki"
      image     = "grafana/loki:2.9.0"
      essential = true
      portMappings = [
        { containerPort = 3100, hostPort = 3100, protocol = "tcp" }
      ]
      command = ["-config.file=/etc/loki/local-config.yaml"]
      mountPoints = [
        { sourceVolume = "loki-data", containerPath = "/loki" }
      ]
      logConfiguration = {
        logDriver = "awslogs",
        options = {
          "awslogs-group"         = "/ecs/loki"
          "awslogs-region"        = "us-west-2"
          "awslogs-stream-prefix" = "ecs"
        }
      }
    }
  ])

  volume {
    name = "loki-data"
    efs_volume_configuration {
      file_system_id     = aws_efs_file_system.loki_data.id
      transit_encryption = "ENABLED"
      authorization_config {
        access_point_id = aws_efs_access_point.loki_ap.id
        iam             = "ENABLED"
      }
    }
  }
}

# ---------------- EFS pour Loki ----------------
resource "aws_efs_file_system" "loki_data" {
  creation_token   = "loki-data"
  performance_mode = "generalPurpose"
  encrypted        = true
  tags             = { Name = "loki-data" }
}

resource "aws_efs_mount_target" "loki_mount" {
  file_system_id  = aws_efs_file_system.loki_data.id
  subnet_id       = aws_subnet.public.id
  security_groups = [aws_security_group.ecs_sg.id]
}

resource "aws_efs_access_point" "loki_ap" {
  file_system_id = aws_efs_file_system.loki_data.id

  posix_user {
    uid = 10001
    gid = 10001
  }

  root_directory {
    path = "/loki-data"
    creation_info {
      owner_uid   = 10001
      owner_gid   = 10001
      permissions = "0755"
    }
  }

  tags = { Name = "loki-access-point" }
}

# ---------------- Application Load Balancer (Loki) ----------------
resource "aws_security_group" "loki_alb_sg" {
  name        = "loki-alb-sg"
  description = "Allow public access to Loki via Load Balancer"
  vpc_id      = aws_vpc.this.id

  ingress {
    from_port   = 3100
    to_port     = 3100
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = { Name = "loki-alb-sg" }
}

resource "aws_lb" "loki_alb" {
  name               = "loki-alb"
  internal           = false
  load_balancer_type = "application"
  security_groups    = [aws_security_group.loki_alb_sg.id]
  subnets            = [aws_subnet.public.id, aws_subnet.public_b.id]
  tags               = { Name = "loki-alb" }
}

resource "aws_lb_target_group" "loki_tg" {
  name        = "loki-tg"
  port        = 3100
  protocol    = "HTTP"
  vpc_id      = aws_vpc.this.id
  target_type = "ip"

  health_check {
    path                = "/ready"
    interval            = 30
    timeout             = 5
    healthy_threshold   = 2
    unhealthy_threshold = 2
  }

  tags = { Name = "loki-tg" }
}

resource "aws_lb_listener" "loki_listener" {
  load_balancer_arn = aws_lb.loki_alb.arn
  port              = 3100
  protocol          = "HTTP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.loki_tg.arn
  }
}

# ---------------- ECS Services ----------------
resource "aws_ecs_service" "loki" {
  name            = "loki-service"
  cluster         = aws_ecs_cluster.this.id
  task_definition = aws_ecs_task_definition.loki.arn
  desired_count   = 1
  launch_type     = "FARGATE"

  network_configuration {
    subnets          = [aws_subnet.public.id]
    assign_public_ip = true
    security_groups  = [aws_security_group.ecs_sg.id]
  }

  load_balancer {
    target_group_arn = aws_lb_target_group.loki_tg.arn
    container_name   = "loki"
    container_port   = 3100
  }

  depends_on = [
    aws_lb_listener.loki_listener
  ]
}

resource "aws_ecs_service" "grafana" {
  name            = "grafana-service"
  cluster         = aws_ecs_cluster.this.id
  task_definition = aws_ecs_task_definition.grafana.arn
  desired_count   = 1
  launch_type     = "FARGATE"

  network_configuration {
    subnets = [
      aws_subnet.public.id,
      aws_subnet.public_b.id
    ]
    assign_public_ip = true
    security_groups  = [aws_security_group.ecs_sg.id]
  }

  depends_on = [aws_ecs_service.loki]
}
