output "resource_group_name" {
  value = azurerm_resource_group.rg.name
}

output "vnet_id" {
  value = azurerm_virtual_network.vnet.id
}

output "appservice_subnet_id" {
  value = azurerm_subnet.appservice.id
}

output "containerapps_subnet_id" {
  value = azurerm_subnet.containerapps.id
}
