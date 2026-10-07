---
name: unity-tests
description: Compile le projet Unity et lance les tests EditMode et/ou PlayMode en batchmode, puis résume le résultat (total, réussis, échecs, erreurs de compilation). À utiliser pour vérifier que le projet compile ou que les tests passent, avant une PR ou après une modification de code C#.
arguments: [Compile|EditMode|PlayMode|all]
allowed-tools: Bash(.claude/scripts/unity-run.sh *), Bash(.claude/scripts/unity-editor-open.sh), Read, Grep
---

# Compilation et tests Unity

Mode demandé : **$0** (par défaut `EditMode`).

1. Lancer `.claude/scripts/unity-run.sh <mode>` (`all` = `EditMode` puis `PlayMode`). Timeout long :
   un premier lancement peut prendre plusieurs minutes (import du projet).
2. Code de sortie **3** (`EDITOR_OPEN`) : l'éditeur Unity est ouvert sur le projet, le batchmode est
   impossible. Proposer au développeur de fermer l'éditeur et relancer, ou de vérifier dans l'éditeur
   (*Window > General > Test Runner*, onglet EditMode, *Run All* ; Console sans erreur rouge).
3. Résumer :
   - erreurs de compilation `error CS…` avec `fichier(ligne)` ;
   - `TOTAL / PASSED / FAILED` et la liste des tests en échec ;
   - pour un test en échec, lire le message dans `Logs/claude-<mode>-results.xml` (balise `<failure>`
     du `test-case`) et l'expliquer en une phrase.
4. Ne rien corriger dans ce skill : rapporter seulement.
