# CLAUDE.md

Guide de référence pour travailler sur **The Last Lighthouse** (titre de travail FR : *Le Dernier Phare*) : jeu mobile Android, roguelite court (runs de 2 à 4 min) avec méta-progression idle autour d'un archipel à restaurer.

## Source de vérité

**[docs/Le_Dernier_Phare_Brief.pdf](docs/Le_Dernier_Phare_Brief.pdf)** fait foi pour le design, l'économie et l'architecture. Le lire (au moins les sections concernées) avant de coder une fonctionnalité.

| Section du brief | Contenu |
|---|---|
| 1 | Décisions figées, conventions de code |
| 2 | Vision, univers, îles, direction artistique, glossaire |
| 3 | Gameplay : contrôle, combat, ennemis, run, cartes, difficulté, FTUE, méta |
| 4 | Économie : monnaies, formules, énergie, hors ligne, pubs, IAP, Remote Config, analytics |
| 5 | Architecture Unity : paquets, arborescence, assemblies, code de référence |
| 6 | Plan de réalisation (jalons M0 à M6), publication |
| 7 | Questions ouvertes |

- Un point marqué **[A TRANCHER]** dans le brief : poser la question au développeur **avant** d'implémenter.
- Les valeurs chiffrées du brief (PV, coûts, taux, délais) sont des valeurs **initiales** : elles vivent dans des ScriptableObjects et/ou Remote Config, **jamais en dur** dans le code.

## Le développeur

Développeur mobile expérimenté (Kotlin, Swift) mais **débutant en Unity**. Expliquer brièvement les spécificités Unity quand elles comptent (cycle de vie des MonoBehaviours, ScriptableObjects, asmdef, sérialisation, `.meta`), sans sur-expliquer le reste. Analogies utiles : VContainer ≈ Hilt, UniTask ≈ coroutines Kotlin, ScriptableObject ≈ ressource de données typée, asmdef ≈ module Gradle.

## Projet

- **Unity** : 6000.6.4f1 (Unity 6), 2D, URP, C#. Éditeur : `/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity`
- **Plateforme** : Android d'abord (IL2CPP, ARM64, AAB, minSdk 24, portrait verrouillé, 60 fps, vSync désactivé, textures ASTC). iOS éventuellement plus tard.
- **Langue du jeu** : anglais au lancement, via Unity Localization (français ensuite).
- **Dépôt** : `Larmik/The-Last-Lighthouse`, branche par défaut **`main`**. Tickets : GitHub Issues, préfixés par pôle (`OPS`, `CORE`, `GAME`, `ECO`, `META`, `SVC`, `UI`, `ART`, `AUD`, `NARR`, `QA`, `REL`), jalons `M0 - Setup` à `M6 - Soft launch`.

## Commandes

Les commandes en ligne de commande exigent que l'éditeur Unity **ne soit pas ouvert** sur le projet (verrou du projet).

```bash
UNITY="/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity"

# Tests EditMode (logique pure) / PlayMode
"$UNITY" -batchmode -nographics -projectPath . -runTests -testPlatform EditMode -testResults Logs/editmode-results.xml -logFile -
"$UNITY" -batchmode -nographics -projectPath . -runTests -testPlatform PlayMode -testResults Logs/playmode-results.xml -logFile -

# Compilation seule (vérifier que le projet compile)
"$UNITY" -batchmode -nographics -projectPath . -quit -logFile -
```

Build Android en ligne de commande (AAB et APK debug) : défini par le ticket OPS-007.

## Workflow & process

- **Pas de git sans demande explicite** : ne pas `commit` / `push` / créer de PR tant que le développeur ne l'a pas demandé (sauf flux de skill qui le prévoit explicitement).
- **Synchroniser avant de commencer** : `git fetch origin`, puis partir de `main` à jour (`git pull --ff-only`).
- **Un ticket = une branche = une PR** : branche `feat/<CODE>` (ex. `feat/OPS-001`), commits préfixés par le code du ticket (`OPS-001: ...`), PR vers `main` qui ferme l'issue (`Closes #N`).
- **Périmètre décidé par le développeur** : ne pas remettre en question le contenu d'une PR ni proposer de la scinder.
- **Hors périmètre** : un problème découvert en dehors du ticket se signale (ou devient un ticket), il ne se corrige pas en passant.

## Conventions de code

- **Aucun commentaire dans le code C#** : pas de blocs de commentaires, pas de `TODO`. Les noms doivent suffire.
- **Diffs minimaux** : ne pas reformater ni renommer ce qui n'est pas concerné par la tâche.
- **Anglais** pour les identifiants, noms de fichiers, clés Remote Config et événements analytics. La documentation et les échanges restent en français.
- **Logique de jeu pure** (économie, progression, stats) en C# **sans `UnityEngine`**, testable en EditMode.
- **Aucune allocation dans les boucles de jeu** : pas de LINQ, pas de `new List` dans `Update`/`Tick`. **Object pooling obligatoire** pour ennemis, projectiles et effets.
- **Une seule boucle centrale** : le `RunDirector` appelle des `Tick(dt)` plutôt qu'un `Update` par objet.
- **Pas de Physics2D par ennemi** pour le faisceau : test d'appartenance au cône par angle + distance, sur un registre d'ennemis actifs.
- **Données dans des ScriptableObjects** (`EnemyDefinition`, `UpgradeDefinition`, `WaveDefinition`, `IslandDefinition`, `KeeperDefinition`, `BuildingDefinition`), référencées par un `GameCatalog` injecté par le composition root.
- **Effets de cartes en `StatModifier`** : pas de logique spécifique par carte quand un modificateur de stat suffit.
- **Temps** : énergie, production hors ligne et récompenses se calculent à partir d'horodatages UTC (temps injectable), jamais de timers en mémoire.
- **Entrées** : Input System (`Pointer.current`) pour que le même code marche à la souris dans l'éditeur ; ignorer la visée quand le pointeur est sur l'UI ; respecter la safe area.

## Architecture

Assemblies sous `Assets/_Project/Scripts/` (une asmdef par dossier) :

| Assembly | Rôle | Dépend de |
|---|---|---|
| `Game.Core` | C# pur (`noEngineReferences`) : Wallet, StatBlock, courbes, temps | — |
| `Game.Economy` | Portefeuille, courbes, énergie, récompenses | Core |
| `Game.Meta` | Progression, îles, bâtiments, keepers | Core |
| `Game.Gameplay` | Faisceau, ennemis, vagues, `RunDirector` | Core, Economy |
| `Game.Services` | Interfaces + implémentations (pubs, IAP, analytics, Remote Config, sauvegarde) | Core |
| `Game.UI` | Presenters (un par écran), sans logique métier | Core, Economy, Meta, Gameplay |
| `Game.Bootstrap` | Composition root VContainer (`GameLifetimeScope`, `BootFlow`) | tout |
| `Game.Tests.EditMode` / `Game.Tests.PlayMode` | Unity Test Framework | selon besoin |

**Jamais de dépendance inverse.** Une référence manquante entre assemblies se règle en revoyant le découpage, pas en ajoutant une dépendance à rebours.

- **Scènes** : `Boot` (services, chargement de la sauvegarde, puis Hub) → `Hub` (méta : archipel, améliorations, boutique) → `Run` (combat).
- **Services** : toujours une implémentation **Fake** pour l'éditeur (`#if UNITY_EDITOR`) afin de tester sans SDK.
- **RunDirector** : machine à états `Intro, Wave, UpgradeChoice, Boss, Victory, Defeat` ; possède la boucle Tick, le registre d'ennemis et le spawner poolé ; émet des événements C# écoutés par l'UI. `Time.timeScale = 0` pendant le choix de carte.
- **Sauvegarde** : JSON (Newtonsoft) dans `Application.persistentDataPath`, champ `Version` + migrations successives, écriture atomique (fichier temporaire puis remplacement), sauvegarde à chaque achat, fin de run et `OnApplicationPause`.

Paquets prévus (brief § 5.1) : URP 2D + Light 2D, Input System, VContainer, UniTask, DOTween ou LitMotion, Newtonsoft JSON, Localization + TextMeshPro, Google Mobile Ads + UMP, Unity IAP, Firebase (Analytics, Remote Config, Crashlytics, Messaging), Unity Test Framework.

## Éthique du design

Engagement par la progression et les choix, pas par la manipulation : pas de faux compte à rebours, pas de probabilités cachées, pas de pénalité brutale à l'absence, pas de pub forcée pendant l'action, pubs récompensées toujours facultatives, histoire jamais verrouillée derrière un paiement, aucune puissance de combat exclusive à l'achat.

## Tests

Unity Test Framework. Tests EditMode à écrire dès le début pour la logique pure : Wallet, EconomyCurves, StatBlock, régénération d'énergie, Light Cache hors ligne, tirage de cartes (poids et plafonds). Conventions détaillées : ticket QA-001.

## Pièges connus

- **`.meta` obligatoires** : tout asset ajouté, déplacé ou renommé doit l'être avec son `.meta` (le GUID qu'il contient relie les références). Déplacer des assets depuis l'éditeur Unity, pas depuis le Finder.
- **`JsonUtility` ne gère ni `Dictionary` ni `HashSet`** : utiliser Newtonsoft pour la sauvegarde.
- **Champs sérialisés** : renommer un champ `[SerializeField]` ou public d'un MonoBehaviour/ScriptableObject perd les valeurs saisies dans les assets (utiliser `[FormerlySerializedAs]` si un renommage est indispensable).
- **Éditeur ouvert** : les commandes batchmode échouent si le projet est ouvert dans l'éditeur.
