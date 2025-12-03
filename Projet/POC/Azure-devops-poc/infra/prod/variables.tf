variable "app_name" {
  type        = string
  description = "Name for the App Service in PROD"
  default     = "poc-api-prod"
}

variable "resource_group_name" {
  type        = string
  description = "Resource group for PROD environment"
  default     = "rg-poc-prod"
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
  default     = "poc-api"
}

variable "docker_image_tag" {
  type        = string
  description = "Tag for prod deployment"
  default     = "prod"
}
