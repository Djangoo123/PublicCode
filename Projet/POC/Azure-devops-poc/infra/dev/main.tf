terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.90.0"
    }
  }
}

provider "azurerm" {
  features {}
}

# ----------------------------------------------------
# MODULE STATE
# ----------------------------------------------------
# module "state" {
#   source = "../modules/state"
#
#   resource_group_name = "rg-tfstate-global"
#   location            = var.location
#   prefix              = "poc"
# }

# ----------------------------------------------------
# MODULE NETWORK
# ----------------------------------------------------
module "network" {
  source = "../modules/network"

  name                = "poc-dev"
  resource_group_name = var.resource_group_name
  location            = var.location

  vnet_cidr                 = "10.10.0.0/16"
  appservice_subnet_cidr    = "10.10.1.0/24"
  containerapps_subnet_cidr = "10.10.2.0/24"
}

# ----------------------------------------------------
# MODULE APPSERVICE (Blue/Green)
# ----------------------------------------------------
module "appservice" {
  source = "../modules/appservice"

  name                = var.app_name
  resource_group_name = var.resource_group_name
  location            = var.location
  sku                 = "B1"
}

# ----------------------------------------------------
# MODULE CONTAINER APPS 
# ----------------------------------------------------
module "containerapp" {
  source = "../modules/containerapp"

  name                = "${var.app_name}-aca"
  resource_group_name = var.resource_group_name
  location            = var.location
  environment         = "dev"

  image_name       = var.docker_image_name
  image_tag        = var.docker_image_tag

  acr_login_server = var.acr_login_server
  acr_username     = var.acr_username
  acr_password     = var.acr_password
}

# ----------------------------------------------------
# MODULE POSTGRESQL
# ----------------------------------------------------

module "postgresql" {
  source = "../modules/postgresql"

  name                = "poc-pg-dev"
  resource_group_name = var.resource_group_name
  location            = var.location

  admin_username = var.pg_admin
  admin_password = var.pg_password
  database_name  = "pocdbdev"
}

