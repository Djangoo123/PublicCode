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
# NETWORK 
# ----------------------------------------------------
module "network" {
  source = "../modules/network"

  name                = "poc-prod"
  resource_group_name = var.resource_group_name
  location            = var.location

  vnet_cidr                 = "10.30.0.0/16"
  appservice_subnet_cidr    = "10.30.1.0/24"
  containerapps_subnet_cidr = "10.30.2.0/24"
}

# ----------------------------------------------------
# APP SERVICE PROD (Blue/Green)
# ----------------------------------------------------
module "appservice" {
  source = "../modules/appservice"

  name                = var.app_name
  resource_group_name = var.resource_group_name
  location            = var.location

  # Real PRODUCTION tier
  sku = "P1v2"
}

# ----------------------------------------------------
# CONTAINER APPS PROD
# ----------------------------------------------------
module "containerapp" {
  source = "../modules/containerapp"

  name                = "${var.app_name}-aca"
  resource_group_name = var.resource_group_name
  location            = var.location
  environment         = "prod"

  image_name        = var.docker_image_name
  image_tag         = var.docker_image_tag

  acr_login_server  = var.acr_login_server
  acr_username      = var.acr_username
  acr_password      = var.acr_password
}
