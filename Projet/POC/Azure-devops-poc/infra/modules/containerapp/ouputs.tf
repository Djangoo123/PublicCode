output "container_app_name" {
  value = azurerm_container_app.app.name
}

output "container_app_url" {
  value = azurerm_container_app.app.latest_revision_fqdn
}

output "container_app_environment" {
  value = azurerm_container_app_environment.env.name
}

output "resource_group" {
  value = azurerm_resource_group.rg.name
}
