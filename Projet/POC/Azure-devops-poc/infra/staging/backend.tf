terraform {
  backend "azurerm" {
    resource_group_name  = "rg-tfstate-global"
    storage_account_name = "poctf123456"
    container_name       = "tfstate"
    key                  = "staging.terraform.tfstate"
  }
}
