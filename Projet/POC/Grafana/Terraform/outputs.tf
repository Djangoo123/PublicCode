# Nom du cluster ECS
output "cluster_name" {
  description = "Nom du cluster ECS"
  value       = aws_ecs_cluster.this.name
}

# Identifiant du service ECS Grafana
output "grafana_service_name" {
  description = "Nom du service ECS Grafana"
  value       = aws_ecs_service.grafana.name
}

# Identifiant du service ECS Loki
output "loki_service_name" {
  description = "Nom du service ECS Loki"
  value       = aws_ecs_service.loki.name
}

# Nom du Security Group ECS
output "ecs_security_group" {
  description = "Nom du Security Group utilisé par les services ECS"
  value       = aws_security_group.ecs_sg.name
}

# Sous-réseau utilisé
output "subnet_id" {
  description = "ID du sous-réseau public utilisé pour ECS"
  value       = aws_subnet.public.id
}

# IAM Role ECS Task Execution
output "ecs_task_execution_role" {
  description = "Nom du rôle IAM utilisé pour exécuter les tâches ECS"
  value       = aws_iam_role.ecs_task_execution.name
}


# ---------------- Output loki url ----------------
output "loki_alb_dns" {
  description = "URL publique du Loki Load Balancer"
  value       = aws_lb.loki_alb.dns_name
}
