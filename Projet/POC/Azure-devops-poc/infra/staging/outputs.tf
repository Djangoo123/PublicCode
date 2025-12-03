output "resource_group_name" {
  value = var.resource_group_name
}

output "app_service_name" {
  value = module.appservice.app_name
}

output "app_service_slot_blue" {
  value = module.appservice.slot_blue_name
}

output "app_service_slot_green" {
  value = module.appservice.slot_green_name
}

output "app_url" {
  value = module.appservice.app_url
}

output "container_app_url" {
  value = module.containerapp.container_app_url
}
