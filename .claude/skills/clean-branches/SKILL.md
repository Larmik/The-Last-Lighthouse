---
name: clean-branches
description: Nettoie les branches Git locales et distantes devenues inutiles (déjà mergées dans main ou dont la branche distante a disparu). Synchronise le dépôt, dresse la liste des branches supprimables en séparant local et remote, demande confirmation, puis supprime uniquement ce que l'utilisateur valide. Ne touche jamais main ni la branche courante. À utiliser après avoir mergé des PR pour faire le ménage.
disable-model-invocation: true
allowed-tools: Bash, AskUserQuestion
---

# Nettoyage des branches locales et distantes

Objectif : supprimer les branches devenues inutiles, **en local et sur `origin`**, sans rien détruire
que l'utilisateur n'ait validé. **Aucune** suppression avant l'étape 4.

## Garde-fous (non négociables)

- Ne jamais supprimer `main` ni la **branche courante**.
- Local : suppression sûre uniquement (`git branch -d`). `git branch -D` seulement sur demande
  explicite pour une branche non mergée précise.
- Distant (`git push origin --delete <b>`) : irréversible, confirmation explicite obligatoire.

## 1. Synchroniser et se placer sur main

1. `git fetch --prune origin`.
2. Noter la branche courante (`git rev-parse --abbrev-ref HEAD`). Si ce n'est pas `main` :
   `git checkout main` puis `git pull --ff-only`. Working tree sale qui bloque le checkout → s'arrêter
   et le signaler (pas de stash sans accord).

## 2. Lister les branches supprimables

```bash
# A. Locales mergées dans main
git branch --merged main | grep -vE '^\*|^\s*main$'
# B. Locales dont l'upstream a disparu (PR fusionnée, branche distante supprimée)
git branch -vv | grep ': gone]' | awk '{print $1}' | grep -vE '^\*'
# C. Distantes mergées dans origin/main
git branch -r --merged origin/main | grep -vE 'origin/(main|HEAD)'
```

Une branche `gone` fusionnée via PR en merge commit apparaît aussi en A. Aucune branche
supprimable → le dire et s'arrêter.

## 3. Présenter le diagnostic

Récapitulatif par catégorie (locales mergées / upstream disparu ; distantes mergées), avec le
dernier commit si utile (`git log -1 --format='%h %s (%cr)' <b>`). Signaler sans les proposer les
branches locales non mergées.

## 4. Confirmer puis supprimer

`AskUserQuestion` : « Local uniquement », « Local + distant », « Rien », ou une sélection. Puis,
seulement ensuite, une branche à la fois en rapportant chaque résultat :

- Local : `git branch -d <b>` (si refus et branche ciblée en connaissance de cause, proposer `-D`).
- Distant : `git push origin --delete <b>`.

## 5. Récapitulatif final

Branches supprimées (local / distant), conservées, et non mergées laissées de côté. Aucun commit,
aucune branche créée.
