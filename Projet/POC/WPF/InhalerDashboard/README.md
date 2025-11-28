# 🫁 Inhaler Dashboard – Client lourd WPF (.NET)

## 🚀 Fonctionnalités

### 🔌 Simulation de dispositif
Le simulateur intégré modélise un inhalateur :

- Connexion / déconnexion
- Débit instantané (L/min)
- Batterie (%)
- Doses restantes
- Publication régulière de mesures
- Simulation d’une inhalation complète (30–40 valeurs)
- Détection d’anomalies :
  - Fuite d’air
  - Capteur défaillant
  - Batterie critique
  - Déconnexion

Chaque inhalation génère un **profil de débit réaliste** basé sur une courbe sinusoïdale.

---

## 📊 Interface (WPF)

L’UI affiche en temps réel :

- **Graphique du débit** (LiveChartsCore)
- **État du device** (batterie, doses, statut connexion)
- **Journal des lectures**
- **Statistiques automatiques :**
  - Durée moyenne d’inhalation
  - Volume total / moyen (mL)
  - Intervalle moyen entre inhalations

### Commandes utilisateur :

- ▶️ **Démarrer**  
- ⏹ **Arrêter**  
- 🫁 **Simuler une inhalation**  
- 🔄 **Réinitialiser**


## 🧪 Simulation d’une inhalation

Chaque inhalation est composée de :

- 30–40 échantillons de débit (profil sinusoïdal)
- Génération de volume total inhalé (mL)
- Décrément des doses + batterie
- Événements aléatoires :
  - fuite d’air (~10 %)
  - capteur défaillant (~3 %)
  - batterie faible < 10 %
