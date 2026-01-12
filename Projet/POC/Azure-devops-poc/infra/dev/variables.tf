variable "app_name" {
  type        = string
  description = "Name for the App Service"
  default     = "poc-api-dev"
}

variable "resource_group_name" {
  type        = string
  description = "Resource group for DEV environment"
  default     = "rg-poc-dev"
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
  description = "Name of image inside ACR"
  default     = "poc-api"
}

variable "docker_image_tag" {
  type        = string
  description = "Tag used for dev deployment"
  default     = "dev"
}

variable "pg_admin" {
  type      = string
  sensitive = true
}
variable "pg_password" {
  type      = string
  sensitive = true
}
