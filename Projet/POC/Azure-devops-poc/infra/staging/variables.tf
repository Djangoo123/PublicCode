variable "app_name" {
  type        = string
  description = "Name for the App Service"
  default     = "poc-api-staging"
}

variable "resource_group_name" {
  type        = string
  description = "Resource group for STAGING environment"
  default     = "rg-poc-staging"
}

variable "location" {
  type        = string
  default     = "westeurope"
}

variable "acr_login_server" {
  type        = string
  description = "ACR login server"
}

variable "acr_username" {
  type        = string
  sensitive   = true
}

variable "acr_password" {
  type        = string
  sensitive   = true
}

variable "docker_image_name" {
  type        = string
  description = "Docker image inside ACR"
  default     = "poc-api"
}

variable "docker_image_tag" {
  type        = string
  description = "Tag for staging deployment"
  default     = "staging"
}
