---
paths:
  - "Assets/_Project/Data/**"
  - "Assets/_Project/Scripts/**/*Definition.cs"
  - "Assets/_Project/Scripts/**/GameCatalog.cs"
  - "Assets/_Project/Scripts/Economy/**"
  - "Assets/_Project/Scripts/Meta/**"
  - "Assets/_Project/Scripts/Gameplay/**"
---

# Données de jeu : ScriptableObjects et valeurs réglables

## Aucune valeur d'équilibrage en dur

- PV, vitesses, dégâts, coûts, taux, délais, plafonds et poids de tirage viennent d'une définition
  ScriptableObject ou de Remote Config, jamais d'un littéral dans le code.
- Valeurs initiales = celles du brief (§ 3.4 à 3.7, § 4) ; un écart avec le brief se signale.
- Les formules (§ 4.2, § 3.7) vivent dans une classe pure (`EconomyCurves`) paramétrée par ces
  données, pas dupliquées dans le gameplay.

## Définitions

- Une définition par concept : `EnemyDefinition`, `UpgradeDefinition`, `WaveDefinition`,
  `IslandDefinition`, `KeeperDefinition`, `BuildingDefinition`.
- `[CreateAssetMenu(menuName = "Phare/<Type>")]` sur chaque définition.
- Champ `Id` en `snake_case` anglais, stable : il est écrit dans la sauvegarde et les événements
  analytics, ne jamais le renommer une fois publié.
- Toutes les définitions sont référencées par le `GameCatalog`, injecté par le composition root ; pas
  de `Resources.Load` ni de recherche d'asset à l'exécution.
- Un ScriptableObject est une donnée en lecture seule à l'exécution : l'état de run ou de partie vit
  dans des objets C# séparés.

## Sérialisation

- Renommer un champ sérialisé perd les valeurs saisies dans les assets : l'éviter, sinon ajouter
  `[FormerlySerializedAs("ancienNom")]`.
- Effets de cartes exprimés en `StatModifier[]` (`PerStack`) ; logique spécifique seulement quand un
  modificateur de stat ne suffit pas (brief § 3.6).
