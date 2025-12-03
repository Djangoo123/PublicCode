output "storage_account_name" {
  value = azurerm_storage_account.sa.name
}

output "resource_group_name" {
  value = azurerm_resource_group.rg.name
}

output "container_name" {
  value = azurerm_storage_container.tfstate.name
}

output "primary_access_key" {
  value     = azurerm_storage_account.sa.primary_access_key
  sensitive = true
}

output "backend_config" {
  description = "Terraform backend block configuration"
  value = {
    resource_group_name  = azurerm_resource_group.rg.name
    storage_account_name = azurerm_storage_account.sa.name
    container_name       = azurerm_storage_container.tfstate.name
  }
}
