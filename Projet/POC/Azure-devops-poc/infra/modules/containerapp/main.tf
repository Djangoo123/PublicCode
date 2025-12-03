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

# ---------------------------------------------------
# RESOURCE GROUP
# ---------------------------------------------------
resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.location
}

# ---------------------------------------------------
# LOG ANALYTICS (required for ACA)
# ---------------------------------------------------
resource "azurerm_log_analytics_workspace" "log" {
  name                = "${var.name}-law"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

# ---------------------------------------------------
# CONTAINER APPS ENVIRONMENT
# ---------------------------------------------------
resource "azurerm_container_app_environment" "env" {
  name                       = "${var.name}-env"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.log.id
}

# ---------------------------------------------------
# CONTAINER APP (the actual microservice)
# ---------------------------------------------------
resource "azurerm_container_app" "app" {
  name                         = var.name
  container_app_environment_id = azurerm_container_app_environment.env.id
  resource_group_name          = azurerm_resource_group.rg.name
  revision_mode                = "Single"

  template {
    container {
      name   = var.name
      image  = "${var.acr_login_server}/${var.image_name}:${var.image_tag}"

      resources {
        cpu    = 0.5
        memory = "1Gi"
      }

      env {
        name  = "ASPNETCORE_ENVIRONMENT"
        value = var.environment
      }
    }

    ingress {
      external_enabled = true
      target_port      = 8080
    }
  }

  registry {
    server   = var.acr_login_server
    username = var.acr_username
    password = var.acr_password
  }
}
