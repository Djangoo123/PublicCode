variable "name" {
  type        = string
  description = "Container App name"
}

variable "resource_group_name" {
  type        = string
  description = "Resource group name"
}

variable "location" {
  type        = string
  default     = "westeurope"
}

variable "image_name" {
  type        = string
  description = "Docker image name stored in ACR"
}

variable "image_tag" {
  type        = string
  description = "Docker image tag"
  default     = "latest"
}

variable "environment" {
  type        = string
  description = "Environment name: dev/staging/prod"
}

# ACR configuration
variable "acr_login_server" {
  type        = string
  description = "ACR login server (e.g. myacr.azurecr.io)"
}

variable "acr_username" {
  type        = string
  description = "ACR username"
  sensitive   = true
}

variable "acr_password" {
  type        = string
  description = "ACR password"
  sensitive   = true
}
