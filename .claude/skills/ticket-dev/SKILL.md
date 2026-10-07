---
name: ticket-dev
description: Prend un ticket (numéro/URL d'issue GitHub, code [CODE-NNN], ou texte collé), vérifie dépendances et actions humaines, crée la branche feat/<CODE>, passe la carte en In Progress, délègue l'implémentation à l'agent ticket-worker en respectant le brief et les rules, commit / push / ouvre la PR vers main après chaque passe, itère sur les retours, puis fusionne sur validation explicite. À utiliser pour traiter un ticket de bout en bout.
arguments: [numero-code-ou-url-issue-ou-texte]
disable-model-invocation: true
allowed-tools: Read, Grep, Glob, Bash, Agent, SendMessage, AskUserQuestion, Skill
---

# Traitement d'un ticket de bout en bout

Entrée fournie : **$0**

Tu es l'**orchestrateur**. Les modifications sont **déléguées** à l'agent `ticket-worker` ; toi, tu
gères l'acquisition du ticket, la branche, le board, les échanges avec l'utilisateur et **toutes**
les opérations git.

Règle d'or : **dès que `ticket-worker` a rendu la main**, tu contrôles son diff (étape 5), tu
**commits**, tu **push** et tu **crées la PR si elle n'existe pas**, **puis** tu attends les retours.
Chaque round de retours re-commit + push sur la même branche. La validation finale sert à
**fusionner** la PR. (Exception assumée à la règle « pas de git sans demande » de `CLAUDE.md`, propre
à ce flux.)

Constantes du board « The Last Lighthouse » (projet `3`, owner `Larmik`) :

| Élément | Id |
|---|---|
| Projet | `PVT_kwHOAi0L9s4BmDDL` |
| Champ Status | `PVTSSF_lAHOAi0L9s4BmDDLzhktjLY` |
| Todo / In Progress / Done | `f75ad846` / `47fc9ee4` / `98236657` |

```bash
ITEM=$(gh project item-list 3 --owner Larmik --limit 500 --format json \
  -q '.items[] | select(.content.number==<N>) | .id')
gh project item-edit --id "$ITEM" --project-id PVT_kwHOAi0L9s4BmDDL \
  --field-id PVTSSF_lAHOAi0L9s4BmDDLzhktjLY --single-select-option-id <OPTION>
```

## 1. Acquérir le ticket

- **Numéro** (`#42`, `42`) ou **URL** → `gh issue view <n> -R Larmik/The-Last-Lighthouse --json number,title,body,labels,milestone,state`.
- **Code** (`CORE-001`, `[CORE-001]`) → résoudre le numéro via
  `gh issue list -R Larmik/The-Last-Lighthouse --state all --search "[CORE-001] in:title" --json number,title`.
- **Texte collé** sans issue → proposer de créer l'issue d'abord via `/create-ticket` (le code
  `[CODE-NNN]` est nécessaire pour la branche et les commits).

Entrée vide, issue introuvable ou fermée → arrête-toi et demande. Mémorise `#N` et le code
`<CODE>-NNN`.

## 2. Vérifier les prérequis

1. **Dépendances** : pour chaque code de la section `## Dépendances`, retrouver l'issue et son état.
   Une dépendance encore ouverte → le signaler et demander via `AskUserQuestion` s'il faut continuer.
2. **Action humaine** (label `needs-human` ou ligne `**Action humaine requise**`) → l'afficher et
   demander si elle est faite, ou si l'on avance sur le reste en la laissant au développeur.
3. **[A TRANCHER]** : si le ticket ou les sections du brief citées en contiennent un qui conditionne
   l'implémentation → poser la question avant de déléguer.

## 3. Synchroniser puis créer la branche

1. Working tree propre requis (`git status --porcelain`) ; sinon, arrête-toi et signale-le (ne pas
   stash sans accord).
2. `git fetch origin`, `git checkout main`, `git pull --ff-only`. Toujours partir de `main` à jour.
3. `git checkout -b feat/<CODE>-NNN` (ex. `feat/CORE-001`). Si la branche existe déjà (reprise),
   s'y placer et la mettre à jour depuis `main` après accord.
4. Passer la carte en **In Progress** (`47fc9ee4`).
5. Annoncer la branche. Si l'éditeur Unity est ouvert (`.claude/scripts/unity-editor-open.sh`),
   rappeler qu'il réimportera les fichiers au changement de branche.

## 4. Déléguer à l'agent `ticket-worker`

Lancer l'agent (`subagent_type: "ticket-worker"`) avec :

- le **contenu intégral du ticket** (titre, corps, labels, jalon) et le **nom de la branche** ;
- les réponses de l'étape 2 (dépendances acceptées, action humaine faite ou non, décisions prises) ;
- la consigne : lire `.claude/rules-index.md` et les sections du brief citées, respecter les rules,
  **aucune opération git** (lecture `git diff`/`git status` permise), vérifier via
  `.claude/scripts/unity-run.sh`, exécuter sa relecture finale (§ 5 du worker), puis retourner son
  résumé ;
- la consigne **travail complémentaire** : s'il trouve des issues complémentaires à créer, il les
  traite directement dans le ticket en cours pour éviter de futures dépendances entre tickets ; il ne
  signale (pour `/create-ticket`) que ce qui dépend d'un ticket ouvert, exige une action humaine ou
  relève d'un autre pôle sans lien.

**Conserver l'identifiant de l'agent** : les rounds suivants continuent le même agent via
`SendMessage`.

## 5. Contrôle avant chaque commit

1. Le résumé du worker contient sa relecture (§ 5) et le résultat de la vérification ; sinon le lui
   redemander via `SendMessage`.
2. Relis toi-même `git diff main...HEAD` et `git diff`, en ciblant : commentaires ou `TODO` dans un
   `.cs`, dépendance d'asmdef à rebours, valeur d'équilibrage en dur, allocation dans `Tick`/`Update`,
   SDK hors `Game.Services`, texte d'UI en dur, secret ajouté, diff `ProjectSettings/` hors
   périmètre.
3. **`.meta`** : tout nouvel élément sous `Assets/` a son `.meta` :
   ```bash
   git status --porcelain --untracked-files=all Assets | awk '{print $2}' | grep -v '\.meta$' \
     | while read f; do [ -e "$f.meta" ] || echo "META MANQUANT: $f"; done
   ```
   Un `.meta` manquant → demander au développeur de passer sur l'éditeur Unity (import automatique),
   puis revérifier. Ne pas commiter sans.
4. Un écart → le renvoyer au worker (même agent). **Travail complémentaire** : un besoin découvert
   pendant le ticket se traite directement dans le ticket en cours (consigne à rappeler au worker,
   évite de futures dépendances entre tickets) et se mentionne dans la PR. Seul un besoin qui dépend
   d'un ticket ouvert, exige une action humaine ou relève d'un autre pôle sans lien devient une issue
   via `/create-ticket`, citée dans le résumé.

## 6. Commit / push / PR (dès la fin du worker)

1. `git add -A` (les fichiers ignorés restent exclus) puis commit :
   ```
   <CODE>-NNN: <titre du ticket sans le code>

   Co-Authored-By: Claude Opus 5.5 (1M context) <noreply@anthropic.com>
   ```
   Rounds suivants : `<CODE>-NNN: <résumé court du retour traité>`.
2. `git push -u origin feat/<CODE>-NNN`.
3. PR si absente (`gh pr view feat/<CODE>-NNN`) : base **`main`**, titre = titre du ticket, corps =
   résumé du changement + **actions humaines** restantes + `Closes #N`, terminé par :
   ```
   🤖 Generated with [Claude Code](https://claude.com/claude-code)
   ```
4. Afficher l'URL de la PR, relayer le résumé du worker (explications Unity, actions humaines,
   vérification, questions ouvertes), puis **attendre les retours**.

## 7. Boucle de retours

Tant que l'utilisateur donne des retours : continuer le **même** agent via `SendMessage` (corrections,
enrichissement des rules selon `.claude/rules-index.md`, nouvelle relecture), puis étape 5, re-commit
+ push, relayer et attendre. Les rules enrichies pendant le ticket sont commitées sur la branche du
ticket.

## 8. Validation et fusion

Avant de proposer la fusion : chaque critère d'acceptation est couvert, tests EditMode verts (ou
vérifiés dans l'éditeur par le développeur), dernier contrôle de l'étape 5 propre. Lister les écarts
restants ; tant qu'il en reste, rester en boucle de retours.

### Après validation explicite (ne jamais oublier)

1. Cocher dans l'issue les critères d'acceptation couverts (`gh issue edit <N> --body-file -`).
2. `gh pr merge <pr> --merge --delete-branch`, puis vérifier que l'issue `#N` est **fermée** ; sinon
   `gh issue close <N>`.
3. Passer la carte en **Done** (`98236657`) et vérifier le statut lu.
4. `git checkout main && git pull --ff-only`.
