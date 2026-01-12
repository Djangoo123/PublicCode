output "connection_string" {
  value = "Host=${azurerm_postgresql_flexible_server.pg.fqdn};Database=${var.database_name};Username=${var.admin_username};Password=${var.admin_password}"
  sensitive = true
}
