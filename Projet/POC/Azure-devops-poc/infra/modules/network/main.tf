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

# ------------------------------------------
# RESOURCE GROUP
# ------------------------------------------
resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.location
}

# ------------------------------------------
# VIRTUAL NETWORK
# ------------------------------------------
resource "azurerm_virtual_network" "vnet" {
  name                = "${var.name}-vnet"
  address_space       = [var.vnet_cidr]
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
}

# ------------------------------------------
# SUBNET — App Service Integration
# ------------------------------------------
resource "azurerm_subnet" "appservice" {
  name                 = "${var.name}-snet-appsvc"
  resource_group_name  = azurerm_resource_group.rg.name
  virtual_network_name = azurerm_virtual_network.vnet.name
  address_prefixes     = [var.appservice_subnet_cidr]

  delegation {
    name = "appservice"
    service_delegation {
      name    = "Microsoft.Web/serverFarms"
      actions = ["Microsoft.Network/virtualNetworks/subnets/action"]
    }
  }
}

# ------------------------------------------
# SUBNET — Container Apps / Compute
# ------------------------------------------
resource "azurerm_subnet" "containerapps" {
  name                 = "${var.name}-snet-containerapps"
  resource_group_name  = azurerm_resource_group.rg.name
  virtual_network_name = azurerm_virtual_network.vnet.name
  address_prefixes     = [var.containerapps_subnet_cidr]

  delegation {
    name = "containerapps"
    service_delegation {
      name    = "Microsoft.Web/containerApps"
      actions = ["Microsoft.Network/virtualNetworks/subnets/action"]
    }
  }
}
