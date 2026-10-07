---
paths:
  - "Assets/_Project/Scripts/UI/**"
  - "Assets/_Project/Prefabs/UI/**"
---

# UI : presenters, textes, mobile

## Presenters

- Un presenter par écran ; il observe `Wallet`, services et événements du `RunDirector` et met à jour
  la vue. Aucune règle métier dans un MonoBehaviour d'UI : il appelle la logique, il ne la contient pas.
- Abonnement aux événements C# dans `OnEnable` (ou à l'injection), désabonnement symétrique dans
  `OnDisable` / `OnDestroy`.

## Textes

- Aucun texte affiché en dur : clé de table Unity Localization (anglais au lancement).
- Clés en `snake_case` anglais, préfixées par l'écran (`hub_play_button`, `run_wave_label`).
- TextMeshPro pour tout texte.

## Mobile

- Contenu interactif dans la safe area (encoches) ; boutons d'action dans la zone du pouce, côté
  inversable (option gauche / droite).
- Ignorer la visée du faisceau quand le pointeur est sur un élément d'UI.
- Lisibilité : information jamais portée par la couleur seule (forme, icône, texte).
- Pubs récompensées présentées comme facultatives, avec la récompense affichée avant le choix ; pas
  de faux compte à rebours ni de probabilité cachée (brief, éthique du design).
