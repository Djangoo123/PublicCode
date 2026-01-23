# PursuitSim_v2

PursuitSim_v2 est un simulateur écrit en .NET 8 composé de deux moteurs indépendants :

DroneSim : simulation de poursuite/évasion (drones + équipe au sol).

UrbanSiege : simulation tactique entre attaquants, défenseurs et drones.

Le projet contient également un mode batch avancé permettant d'exécuter automatiquement plusieurs scénarios en série, avec export CSV.

# Fonctionnalités

# DroneSim

Terrain 2D avec obstacles, chemins principaux et chemins alternatifs.

Mines et détections de proximité.

Runners humains avec vitesse, espacement et état (KO/actif).

Drone autonome (patrouille, poursuite, latence, perte de ligne de vue).

Logs temps réel et export CSV détaillé.

Trois scénarios inclus : PlainWithHedges, UrbanGrid, MixedClearingFinal.

# UrbanSiege

Simule un assaut urbain.

Attaquants, défenseurs, drones Hunter/Bomber.

Portées, probabilités de tir et KO.

Fin de partie en fonction du seuil de victoire ou élimination totale.

# Export CSV + affichage console.

# Modes d’utilisation

# 1. Single Run

Exécute une seule simulation pour un scénario donné.
Affiche les logs dans la console et génère un fichier CSV.

# 2. Batch Run (DroneSim)

Exécute plusieurs runs du même scénario dans un mode automatisé.
Génère un résumé en console et un CSV de synthèse.

# 3. Batch Urban Siege

Même fonctionnement que DroneSim.

# 4. Advanced Batch (via fichier JSON)

Un fichier batch_config.json permettant d'exécuter plusieurs scénarios et plusieurs runs automatiquement.

Exemple :

{
  "OutputDir": "out",

  "DroneSimBatch": [
    { "Scenario": "Plain", "Runs": 50 },
    { "Scenario": "UrbanGrid", "Runs": 50 }
  ],

  "UrbanSiegeBatch": [
    { "Runs": 100 }
  ]
}

# Export CSV

Chaque simulation génère :

un fichier CSV détaillé avec toutes les positions et événements

un CSV résumé pour un run unique

un CSV résumé pour un batch de runs

Les fichiers sont organisés par date et par scénario dans le dossier défini dans appsettings.json ou batch_config.json.


# Objectifs du projet

Fournir un moteur de simulation modulaire et extensible.

Permettre la génération de datasets pour des analyses ou modèles ML.NET.

Servir de base pour une future interface visuelle (Blazor).