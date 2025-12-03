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
# STORAGE ACCOUNT FOR TERRAFORM STATE
# ---------------------------------------------------
resource "random_string" "sa_suffix" {
  length  = 6
  upper   = false
  numeric = true
  special = false
}

resource "azurerm_storage_account" "sa" {
  name                     = lower("${var.prefix}tf${random_string.sa_suffix.result}")
  location                 = azurerm_resource_group.rg.location
  resource_group_name      = azurerm_resource_group.rg.name
  sku                      = "Standard_LRS"
  kind                     = "StorageV2"

  allow_blob_public_access = false
  enable_https_traffic_only = true
}

# ---------------------------------------------------
# BLOB CONTAINER FOR TFSTATE
# ---------------------------------------------------
resource "azurerm_storage_container" "tfstate" {
  name                  = "tfstate"
  storage_account_name  = azurerm_storage_account.sa.name
  container_access_type = "private"
}
