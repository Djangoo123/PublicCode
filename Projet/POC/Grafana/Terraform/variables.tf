variable "region" {
  type    = string
  default = "us-west-2"
}

variable "az" {
  type    = string
  default = "us-west-2a"
}

variable "allowed_ip" {
  description = "Adresse IP autorisée pour Grafana"
  type        = string
  default     = "0.0.0.0/0"
}


variable "az_secondary" {
  description = "Deuxième zone de disponibilité"
  type        = string
  default     = "us-west-2b"
}
