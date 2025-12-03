variable "name" {
  type        = string
  description = "Name prefix for VNet and subnets"
}

variable "resource_group_name" {
  type        = string
  description = "Resource group name"
}

variable "location" {
  type        = string
  default     = "westeurope"
}

variable "vnet_cidr" {
  type        = string
  description = "CIDR for the VNet"
  default     = "10.0.0.0/16"
}

variable "appservice_subnet_cidr" {
  type        = string
  description = "CIDR for App Service subnet"
  default     = "10.0.1.0/24"
}

variable "containerapps_subnet_cidr" {
  type        = string
  description = "CIDR for Container Apps subnet"
  default     = "10.0.2.0/24"
}
