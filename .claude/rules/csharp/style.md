---
paths:
  - "Assets/_Project/**/*.cs"
---

# Style C# : commentaires, noms, portée, diffs

## Aucun commentaire

- Aucun commentaire dans le code : ni `//`, ni `/* */`, ni `///`, ni `TODO`, ni code commenté. Un hook
  bloque toute écriture d'un `.cs` qui en contient.
- Un besoin d'explication se règle par un meilleur nom (méthode extraite, variable intermédiaire,
  type dédié), pas par un commentaire.

## Noms

- Identifiants en anglais, vocabulaire du glossaire du brief (§ 2.6) : `Light`, `Fragments`,
  `Pearls`, `LampOil`, `Sparks`, `Integrity`, `Flare`, `Keeper`, `Wick`.
- `PascalCase` pour types, méthodes, propriétés, événements et constantes ; `camelCase` pour champs
  privés, paramètres et locales, sans préfixe `_` ni `m_`.
- Le nom décrit le rôle (`enemyRegistry`, `halfArc`), jamais une lettre seule (tolérés : `i`, `dt`).
- Interfaces préfixées `I` ; implémentation de test d'éditeur préfixée `Fake` (`FakeAdService`).

## Portée et forme

- Classes `sealed` par défaut ; `static` pour les fonctions pures sans état (`EconomyCurves`).
- `private` par défaut. Champ édité dans l'inspecteur d'un MonoBehaviour : `[SerializeField] private`,
  jamais `public`. Les définitions ScriptableObject suivent la forme du brief (§ 5.4).
- `readonly` pour tout champ assigné une seule fois ; `var` quand le type est évident.
- Membres à expression (`=>`) pour les accesseurs et fonctions d'une ligne.
- Un type public par fichier, fichier nommé comme le type.
- Pas de `async void` : `UniTask` / `UniTaskVoid` (cf. `architecture/services.md`).

## Diffs minimaux

- Ne pas reformater, réordonner ni renommer ce que le ticket ne concerne pas.
- Pas de `using` inutilisé ajouté ; pas de membre mort laissé derrière soi.
