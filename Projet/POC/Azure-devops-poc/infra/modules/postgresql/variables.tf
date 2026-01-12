variable "name" {}
variable "resource_group_name" {}
variable "location" { default = "westeurope" }

variable "admin_username" { sensitive = true }
variable "admin_password" { sensitive = true }

variable "database_name" { default = "appdb" }
