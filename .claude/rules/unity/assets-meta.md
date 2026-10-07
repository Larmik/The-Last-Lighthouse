---
paths:
  - "Assets/**"
  - "Packages/**"
  - "ProjectSettings/**"
---

# Assets Unity : .meta, scènes, paquets, réglages

## Fichiers .meta

- Chaque fichier ou dossier sous `Assets/` a son `.meta` versionné (le GUID relie les références).
  Un fichier créé hors de l'éditeur reçoit son `.meta` à l'import : avant un commit, vérifier que
  chaque nouvel asset est accompagné de son `.meta`.
- Déplacer ou renommer un asset avec son `.meta` (`git mv` des deux), jamais l'un sans l'autre.
- Ne jamais modifier le GUID d'un `.meta` existant.

## Scènes, prefabs et assets sérialisés

- Ne pas écrire à la main le YAML d'une scène, d'un prefab ou d'un ScriptableObject non trivial :
  décrire la manipulation à faire dans l'éditeur (étape « action humaine » du résumé), ou fournir un
  menu d'éditeur (`Game.Editor`) qui la réalise.
- Une modification ponctuelle d'une valeur dans un `.asset` existant est tolérée si elle est lisible
  dans le diff.

## Paquets et réglages

- Paquets épinglés à une version précise dans `Packages/manifest.json` ; ajout ou mise à jour
  uniquement quand le ticket le demande.
- `ProjectSettings/` ne change que pour un ticket qui le demande ; signaler tout autre diff produit
  par l'éditeur au lieu de le commiter.
- Les fichiers générés (`*.csproj`, `*.slnx`, `Library/`, `UserSettings/`) ne sont jamais versionnés.
