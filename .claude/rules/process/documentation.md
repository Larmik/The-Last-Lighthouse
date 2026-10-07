---
paths:
  - "docs/**"
  - ".claude/**"
  - "CLAUDE.md"
  - "README.md"
---

# Documentation et configuration Claude

- `docs/Le_Dernier_Phare_Brief.pdf` est la source de vérité : ne jamais le modifier. Une décision qui
  s'en écarte (point [A TRANCHER] tranché, valeur rééquilibrée) se consigne dans `CLAUDE.md` ou dans
  l'issue concernée, et se signale.
- `CLAUDE.md`, `README.md` et `.claude/` décrivent l'état **actuel** du projet : pas de récit
  historique (git le garde), pas de mention d'élément supprimé.
- Un changement d'architecture, de commande, de paquet ou de convention met à jour la section
  impactée de `CLAUDE.md` / `README.md` dans la même PR.
- Une rule se désigne par son chemin relatif à `.claude/rules/` (ex. `csharp/style.md`) ; le code
  n'en cite aucune.
- Renommer ou supprimer une rule → `rg "<chemin>" .claude CLAUDE.md README.md`, corriger chaque
  renvoi, puis mettre à jour `.claude/rules-index.md`.
- Un problème découvert hors périmètre d'un ticket devient une issue via `/create-ticket` au lieu
  d'être corrigé en passant.
