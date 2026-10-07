---
name: create-ticket
description: Crée une issue GitHub au format du projet ([CODE-NNN] titre, sections Contexte / Critères d'acceptation / Dépendances / Notes, labels area/type/priorité, jalon, board « The Last Lighthouse ») sur Larmik/The-Last-Lighthouse à partir d'une description de feature, bug ou tâche. À utiliser pour transformer une idée, un bug ou un problème découvert hors périmètre en ticket actionnable.
arguments: [description-feature-bug-ou-tache]
allowed-tools: Read, Grep, Glob, Bash(git log *), Bash(git diff *), Bash(gh issue *), Bash(gh label *), Bash(gh api *), Bash(gh project *), Agent, AskUserQuestion
---

# Création d'une issue GitHub

Description fournie en entrée : **$0**

Objectif : créer **une issue** propre et actionnable sur `Larmik/The-Last-Lighthouse`, au **même
format** que les tickets existants.

> Le dépôt est **public** : aucun secret (clé, identifiant AdMob réel, keystore, token) dans l'issue.

## 1. Comprendre la demande

1. Si `$0` est vide, **arrête-toi** et demande la description.
2. Détermine :
   - la **nature** : feature, bug, tâche technique (chore), test, contenu, art, audio, release ;
   - le **pôle** (code et label) :

     | Code | Label | Domaine |
     |---|---|---|
     | `OPS` | `area:ops` | projet, build, CI, outillage |
     | `CORE` | `area:core` | logique pure, données, sauvegarde |
     | `GAME` | `area:game` | gameplay de run |
     | `ECO` | `area:eco` | économie, énergie, récompenses, pubs côté règles |
     | `META` | `area:meta` | progression hors run, îles, bâtiments, keepers |
     | `SVC` | `area:svc` | SDK et services (pubs, IAP, Firebase, Play Games) |
     | `UI` | `area:ui` | écrans, HUD, presenters |
     | `ART` | `area:art` | visuels |
     | `AUD` | `area:aud` | audio |
     | `NARR` | `area:narr` | histoire, textes, localisation |
     | `QA` | `area:qa` | tests, qualité, accessibilité |
     | `REL` | `area:rel` | publication, store, live-ops |

   - la **priorité** (`P0` bloquant pour le jalon, `P1` important, `P2` confort) et le **jalon**
     (`M0 - Setup` … `M6 - Soft launch`, `Post-lancement`). En cas de doute, propose ton choix via
     `AskUserQuestion`.
3. **Ancre dans le brief** : repère la ou les sections de `docs/BRIEF.md` concernées
   pour reprendre les valeurs et contraintes exactes.
4. **Enquête dans le code** si le sujet touche de l'existant (`Assets/_Project/`) : fichiers en
   `chemin:ligne`, cause probable d'un bug. Pour une investigation large, délègue à un agent `Explore`.
   Ne sur-investigue pas.
5. **Doublons** : `gh issue list -R Larmik/The-Last-Lighthouse --state all --search "<mots-clés>"`.
   Un ticket existant couvre déjà le sujet → le signaler et proposer `/edit-ticket` plutôt que créer.

## 2. Code du ticket

Prochain numéro du pôle = max existant + 1, sur 3 chiffres :

```bash
gh issue list -R Larmik/The-Last-Lighthouse --state all --limit 500 --json title \
  --jq '.[].title' | grep -oE '^\[<CODE>-[0-9]+\]' | grep -oE '[0-9]+' | sort -n | tail -1
```

Titre : `[<CODE>-NNN] <titre court en français>` (ex. `[GAME-031] Coffre lâché par les Elites`).

## 3. Rédiger le corps

Structure **exacte** (c'est le corps de l'issue, sans titre H1), ton concis et factuel, en français :

````markdown
## Contexte
<1 à 4 phrases : quoi, où dans le jeu ou le projet, pourquoi. Valeurs du brief reprises telles
quelles. Pour un BUG : comportement observé vs attendu, étapes de reproduction, fréquence.>

## Critères d'acceptation
- [ ] <critère vérifiable>
- [ ] <critère vérifiable>

## Dépendances
<Codes des tickets prérequis séparés par des virgules (ex. `CORE-001, OPS-004`), ou `Aucune`.>

## Notes
Référence : docs/BRIEF.md, § <x.y>. Conventions : CLAUDE.md (code sans commentaires, diffs minimaux, une PR par ticket, commits préfixés par <CODE>-NNN).
````

- Critères **vérifiables** (test vert, comportement observable, valeur attendue), 2 à 5 cases.
- Pour un bug : ajouter un critère de non-régression (test EditMode quand la logique est pure).
- Pistes techniques utiles (`chemin:ligne`, solution recommandée) : en fin de **Notes**, en une ou
  deux puces, conformes aux rules (`.claude/rules-index.md`).
- Action dans l'éditeur Unity, compte externe ou fichier fourni par le développeur : ajouter en fin
  de Notes `**Action humaine requise** : <quoi>` et le label `needs-human`.
- Point [A TRANCHER] du brief concerné : le mentionner dans les Notes.
- Le ticket introduit une règle ou une valeur absente de `docs/BRIEF.md` : la signaler à l'utilisateur
  et ajouter le critère « `docs/BRIEF.md` mis à jour (§ x.y) » ; ne jamais trancher seul une
  décision de design dans un ticket.

## 4. Labels

- `area:<pôle>` + priorité `P0`/`P1`/`P2` (toujours).
- Type : `type:feature`, `type:chore`, `type:test`, `type:content`, `type:art`, `type:audio`,
  `type:release` ; pour un bug, label `bug` à la place du `type:*`.
- `needs-human` si une action humaine est requise.

## 5. Créer l'issue

```bash
gh issue create -R Larmik/The-Last-Lighthouse \
  --title "[GAME-031] Titre court" \
  --label "area:game,type:feature,P1" \
  --milestone "M2 - Run complète" \
  --project "The Last Lighthouse" \
  --body-file - <<'BODY'
## Contexte
…
BODY
```

- `--project "The Last Lighthouse"` est **systématique** ; placer ensuite la carte en `Todo` :
  ```bash
  ITEM=$(gh project item-list 3 --owner Larmik --limit 500 --format json \
    -q '.items[] | select(.content.number==<N>) | .id')
  gh project item-edit --id "$ITEM" --project-id PVT_kwHOAi0L9s4BmDDL \
    --field-id PVTSSF_lAHOAi0L9s4BmDDLzhktjLY --single-select-option-id f75ad846
  ```
- Afficher l'URL et le numéro `#N` renvoyés.
- **Ne crée aucun fichier** dans le dépôt : le ticket vit dans l'issue.
