output "app_name" {
  value = azurerm_linux_web_app.app.name
}

output "resource_group_name" {
  value = azurerm_resource_group.rg.name
}

output "slot_blue_name" {
  value = azurerm_linux_web_app_slot.blue.name
}

output "slot_green_name" {
  value = azurerm_linux_web_app_slot.green.name
}

output "app_url" {
  value = azurerm_linux_web_app.app.default_hostname
}
