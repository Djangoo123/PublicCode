variable "resource_group_name" {
  type        = string
  description = "Resource group for Terraform state"
}

variable "location" {
  type        = string
  description = "Azure region for storage"
  default     = "westeurope"
}

variable "prefix" {
  type        = string
  description = "Prefix for storage account naming"
  default     = "poc"
}
