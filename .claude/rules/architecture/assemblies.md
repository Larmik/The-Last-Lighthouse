---
paths:
  - "Assets/_Project/Scripts/**"
  - "Assets/_Project/Tests/**"
---

# Assemblies : placement du code et dépendances

## Matrice de dépendances (brief § 5.2)

| Assembly | Peut référencer |
|---|---|
| `Game.Core` | rien (`noEngineReferences: true`, pas de `UnityEngine`) |
| `Game.Data` | Core |
| `Game.Economy` | Core, Data |
| `Game.Meta` | Core, Data |
| `Game.Gameplay` | Core, Data, Economy |
| `Game.Services` | Core |
| `Game.UI` | Core, Data, Economy, Meta, Gameplay |
| `Game.Bootstrap` | tout |
| `Game.Tests.EditMode` / `Game.Tests.PlayMode` | les assemblies testées |

- Jamais de dépendance à rebours (ex. Core → Gameplay, Gameplay → UI). Un besoin inverse se résout
  par une interface ou un événement déclaré dans l'assembly basse, implémenté ou écouté plus haut.
- Les paquets tiers (VContainer, UniTask, Newtonsoft…) ne sont référencés que par les assemblies
  qui en ont besoin ; `Game.Core` n'en référence aucun qui dépende de `UnityEngine`.
- Références d'asmdef par nom (pas par GUID).
- Nouvelle assembly `Game.*` ou nouvelle référence entre assemblies : mettre à jour la matrice de
  `Tests/EditMode/AssemblyDefinitionTests.cs` en même temps que celle-ci.
- Chaque assembly applicative garde son `AssemblyInfo.cs` (`InternalsVisibleTo` vers les tests).

## Où placer le code

- Calcul, règle, formule, état sans rendu → `Game.Core` / `Game.Economy` / `Game.Meta`, en C# pur
  testable en EditMode.
- Définition ScriptableObject, `UpgradeEffect`, `GameCatalog`, contrats lus par plusieurs assemblies
  (`IRunContext`) → `Game.Data`. Un champ prefab y est typé `GameObject`, jamais un composant de
  Gameplay.
- `MonoBehaviour`, rendu, entrées, pooling d'objets de scène → `Game.Gameplay` ou `Game.UI`.
- SDK externe (AdMob, IAP, Firebase, Play Games) → uniquement dans `Game.Services`, derrière une
  interface ; aucune autre assembly ne voit le SDK.
- Câblage VContainer, `LifetimeScope`, `BootFlow` → `Game.Bootstrap`.
- Code d'éditeur (menus, outils) → dossier `Editor/` avec asmdef `Game.Editor` limitée à la
  plateforme Editor.
- Namespace = nom de l'assembly (`Game.Core`, `Game.Gameplay`…), sous-namespace optionnel par
  sous-dossier.
