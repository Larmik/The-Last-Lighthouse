---
paths:
  - "Assets/_Project/Scripts/Core/**"
  - "Assets/_Project/Scripts/Economy/**"
  - "Assets/_Project/Scripts/Meta/**"
  - "Assets/_Project/Scripts/Services/**"
---

# Sauvegarde et temps

## Sauvegarde

- Format JSON via Newtonsoft (`JsonUtility` ne gère ni `Dictionary` ni `HashSet`), dans
  `Application.persistentDataPath`.
- Tout changement de `PlayerSave` incrémente `Version` et ajoute une migration de N vers N+1 ; une
  sauvegarde ancienne se charge toujours sans perte.
- Écriture atomique : fichier temporaire puis remplacement ; une sauvegarde illisible ne doit jamais
  écraser la dernière valide.
- Sauvegarder à chaque achat, fin de run et `OnApplicationPause(true)`.
- La sauvegarde stocke des identifiants (`Id` des définitions), jamais de références d'assets.

## Temps

- Tout calcul temporel (énergie, Light Cache hors ligne, récompenses quotidiennes, série, limites
  par jour) lit l'heure via `ITimeProvider` (UTC, injectable en test), jamais `DateTime.Now` ni
  `Time.time`.
- Calcul à partir d'horodatages à la reprise, jamais de timer en mémoire (brief § 4.4).
- Plafonds appliqués au calcul : hors ligne `min(écoulé, offline_cap_hours)`, régénération arrêtée au
  plafond, bonus jamais retirés.
- Un retour en arrière de l'horloge ne crédite rien et ne retire rien.
