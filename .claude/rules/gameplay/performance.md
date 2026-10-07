---
paths:
  - "Assets/_Project/Scripts/Gameplay/**"
  - "Assets/_Project/Scripts/UI/**"
---

# Performance en jeu (60 fps, 120 ennemis)

## Boucle centrale

- Une seule boucle : le `RunDirector` appelle `Tick(dt)` sur les systèmes. Pas d'`Update` par
  ennemi, projectile ou effet.
- `dt` vient du `RunDirector` (respecte `Time.timeScale = 0` pendant le choix de carte).

## Zéro allocation par frame

- Dans `Tick`, `Update`, `LateUpdate` et tout ce qu'ils appellent : pas de LINQ, pas de `new` de
  collection, pas de lambda capturante, pas de `foreach` sur une interface (`IEnumerable`), pas de
  concaténation de chaînes, pas de boxing.
- Collections réutilisées (champs pré-alloués, `Clear()`), itération par index sur `List<T>` / tableau.
- Texte UI mis à jour seulement quand la valeur change, via `TMP_Text.SetText` avec arguments
  numériques.

## Pooling

- Ennemis, projectiles, effets et textes flottants passent par le pool générique ; jamais
  d'`Instantiate` / `Destroy` pendant une run.
- Un objet rendu au pool réinitialise tout son état (PV, statuts, abonnements).

## Requêtes

- Appartenance au faisceau : `BeamGeometry.Contains` (angle + distance au carré) sur le registre
  d'ennemis actifs, pas de `Physics2D` par ennemi.
- Pas de `GetComponent`, `Find*`, `Camera.main` ni `Resources.Load` dans la boucle : résoudre une fois
  (`Awake`, injection) et mettre en cache.
- Comparer des distances au carré (`sqrMagnitude`), pas `Vector2.Distance`.
