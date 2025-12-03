# Azure DevOps – CI/CD + Terraform + Blue/Green – POC

# ✅ Fonctionnalités principales
# Application

API en .NET 8

Endpoint de santé : /health

Tests automatisés (xUnit)

Image Docker multi-stage

# Infrastructure (Terraform)

Architecture modulaire

Environnements DEV, STAGING, PROD

Azure App Service (Linux)

Deux slots : blue et green (Blue/Green deployment)

Azure Virtual Network + Subnets

Container Apps (optionnel)

Stockage du Terraform state dans Azure Storage

# CI Pipeline (ci.yml)

Restauration / Build .NET

Exécution des tests

Publication d’un ZIP pour App Service

# Build Docker

Push de l’image dans Azure Container Registry (ACR)

# CD Pipeline (cd.yml)

Déploiement multi-environnements :

1) DEV

Terraform apply

Déploiement sur le slot blue

2) STAGING

Terraform apply

Déploiement sur le slot green

3) PROD

Terraform apply

Déploiement sur green

Health check automatisé

Swap green → production (Blue/Green)

Rollback si health check KO