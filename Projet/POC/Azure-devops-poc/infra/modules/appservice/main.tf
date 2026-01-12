terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.90.0"
    }
  }
}

resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.location
}

resource "azurerm_service_plan" "asp" {
  name                = "${var.name}-plan"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Linux"
  sku_name            = var.sku
}

resource "azurerm_linux_web_app" "app" {
  name                = var.name
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id     = azurerm_service_plan.asp.id

  site_config {
    linux_fx_version = "DOTNETCORE|8.0"
  }

  app_settings = {
    "ConnectionStrings__DefaultConnection" = module.postgresql.connection_string
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE"  = false
    "WEBSITES_PORT"                        = 8080
  }
}

# -------------------------
# SLOT BLUE
# -------------------------
resource "azurerm_linux_web_app_slot" "blue" {
  name                = "blue"
  app_service_id      = azurerm_linux_web_app.app.id
  resource_group_name = azurerm_resource_group.rg.name

  site_config {
    linux_fx_version = "DOTNETCORE|8.0"
  }

  app_settings = {
    "SLOT_NAME" = "blue"
  }
}

# -------------------------
# SLOT GREEN
# -------------------------
resource "azurerm_linux_web_app_slot" "green" {
  name                = "green"
  app_service_id      = azurerm_linux_web_app.app.id
  resource_group_name = azurerm_resource_group.rg.name

  site_config {
    linux_fx_version = "DOTNETCORE|8.0"
  }

  app_settings = {
    "SLOT_NAME" = "green"
  }
}
