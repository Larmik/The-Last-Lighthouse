---
name: edit-ticket
description: Re-rédige une issue GitHub existante de Larmik/The-Last-Lighthouse à partir de précisions (numéro d'issue + modifications), en conservant le format du projet ([CODE-NNN], Contexte / Critères d'acceptation / Dépendances / Notes) et en ajustant labels, priorité ou jalon si besoin. À utiliser pour affiner, compléter ou corriger un ticket sans le recréer.
arguments: [numero-issue, precisions-ou-modifications]
disable-model-invocation: true
allowed-tools: Read, Grep, Glob, Bash(git log *), Bash(git diff *), Bash(gh issue *), Bash(gh label *), Bash(gh api *), Agent, AskUserQuestion
---

# Édition d'une issue GitHub existante

Entrée : **$ARGUMENTS** (numéro d'issue `#N`/`N` ou code `[CODE-NNN]` + précisions).

Objectif : **re-rédiger** l'issue existante pour y intégrer les précisions, sans en créer une
nouvelle. Dépôt **public** : aucun secret dans le contenu.

## 1. Acquérir l'issue et comprendre la demande

1. Sépare l'entrée en **référence** et **précisions**. Référence ou précisions manquantes → arrête-toi
   et demande.
2. Un code `[CODE-NNN]` se résout en numéro :
   `gh issue list -R Larmik/The-Last-Lighthouse --state all --search "[CODE-NNN] in:title" --json number,title`.
3. `gh issue view <N> -R Larmik/The-Last-Lighthouse --json number,title,body,labels,milestone,state`.
   Introuvable → signale-le. Fermée → demande confirmation avant d'éditer.
4. Lis le corps en entier et identifie ce que les précisions changent : critère ajouté ou retiré,
   valeur corrigée, dépendance, périmètre, requalification (pôle, type, priorité, jalon).
5. Précision ambiguë ou contraire au brief / au corps existant → `AskUserQuestion` plutôt que de
   trancher.

## 2. Ré-enquêter si utile

Seulement si la précision touche une valeur du brief (relire la section concernée de
`docs/BRIEF.md`) ou du code existant (`chemin:ligne` à jour). Sinon, passer.

## 3. Re-rédiger le corps entier

Structure **exacte** du projet :

````markdown
## Contexte
…

## Critères d'acceptation
- [ ] …

## Dépendances
<codes ou `Aucune`>

## Notes
Référence : docs/BRIEF.md, § <x.y>. Conventions : CLAUDE.md (code sans commentaires, diffs minimaux, une PR par ticket, commits préfixés par <CODE>-NNN).
````

- **Intégrer** les précisions dans les sections concernées, comme si le ticket avait été écrit
  correctement du premier coup (pas de bloc « Mise à jour » en fin).
- Conserver ce qui reste valable ; corriger ou supprimer ce que les précisions invalident.
- **Conserver l'état des cases** `- [x]` déjà cochées si le critère reste valable.
- Garder la ligne `**Action humaine requise**` (et le label `needs-human`) tant qu'elle s'applique.

## 4. Titre, labels, jalon

Seulement si les précisions changent la nature du ticket, et **toujours le signaler** :

- **Titre** : garder le code `[CODE-NNN]`. Un changement de pôle ne renumérote pas (le code reste
  l'identifiant de branche et de commits) : changer seulement le label `area:*`.
- Labels `area:*`, `type:*` / `bug`, `P0`/`P1`/`P2`, `needs-human` ; jalon via `--milestone`.

## 5. Éditer l'issue

```bash
gh issue edit <N> -R Larmik/The-Last-Lighthouse --body-file - \
  [--title "…"] [--add-label "…"] [--remove-label "…"] [--milestone "…"] <<'BODY'
## Contexte
…
BODY
```

- N'inclure `--title`, `--add-label`, `--remove-label`, `--milestone` que s'ils changent réellement.
- Aucun fichier créé dans le dépôt.
- Afficher l'URL de l'issue et un résumé de ce qui a changé.
