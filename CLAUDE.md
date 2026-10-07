# CLAUDE.md

Guide de référence pour travailler sur **The Last Lighthouse** (titre de travail FR : *Le Dernier Phare*) : jeu mobile Android, roguelite court (runs de 2 à 4 min) avec méta-progression idle autour d'un archipel à restaurer.

## Source de vérité

**[docs/BRIEF.md](docs/BRIEF.md)** fait foi pour le design, l'économie et l'architecture. Le lire (au moins les sections concernées) avant de coder une fonctionnalité. `docs/Le_Dernier_Phare_Brief.pdf` est la version d'origine, conservée comme archive : ne plus s'y référer.

| Section du brief | Contenu |
|---|---|
| 1 | Décisions figées, conventions de code |
| 2 | Vision, univers, îles, direction artistique, glossaire |
| 3 | Gameplay : contrôle, combat, ennemis, run, cartes (cumul, catalogue, tirage), difficulté, boss, interruptions, FTUE, performance, pistes, îles, bâtiments, Keepers |
| 4 | Économie : monnaies, formules, énergie, quotidien et quêtes, pubs, IAP, Remote Config et surcharges, analytics, éthique |
| 5 | Architecture Unity : paquets, arborescence, assemblies, Stat/StatBlock, définitions, faisceau, services, sauvegarde, run et UI |
| 6 | Conformité et données (consentement, effacement, monnaie virtuelle) |
| 7 | Plan de réalisation (jalons M0 à M6), publication |
| 8 | Questions ouvertes |

- Un point marqué **[A TRANCHER]** dans le brief : poser la question au développeur **avant** d'implémenter.
- Les valeurs chiffrées du brief (PV, coûts, taux, délais) sont des valeurs **initiales** : elles vivent dans des ScriptableObjects, surchargeables par Remote Config, **jamais en dur** dans le code. Celles marquées *(à simuler)* sont calées par le simulateur d'économie.
- Une décision qui modifie le design se reporte dans `docs/BRIEF.md` dans la même PR.

## Le développeur

Développeur mobile expérimenté (Kotlin, Swift) mais **débutant en Unity**. Expliquer brièvement les spécificités Unity quand elles comptent (cycle de vie des MonoBehaviours, ScriptableObjects, asmdef, sérialisation, `.meta`), sans sur-expliquer le reste. Analogies utiles : VContainer ≈ Hilt, UniTask ≈ coroutines Kotlin, ScriptableObject ≈ ressource de données typée, asmdef ≈ module Gradle.

## Projet

- **Unity** : 6000.3.25f1 (Unity 6.3 LTS), 2D, URP, C#. Éditeur : `/Applications/Unity/Hub/Editor/6000.3.25f1/Unity.app/Contents/MacOS/Unity`
- **Plateforme** : Android d'abord (IL2CPP, ARM64, AAB, minSdk 25, portrait verrouillé, 60 fps, vSync désactivé, textures ASTC). iOS éventuellement plus tard.
- **minSdk 25 (Android 7.1)** : plus haute des contraintes du brief § 5.1. Plancher du brief : 24 ; minimum de Unity 6.3 : 25 (le module Android de 6000.3.25f1 ne propose pas d'API inférieure) ; SDK prévus (Google Mobile Ads + UMP, Unity IAP / Play Billing, Firebase, Mobile Notifications, Play Games Services, Play In-App Updates / Review) : 23 au plus. À revérifier à chaque épinglage ou mise à jour d'un SDK.
- **Fréquence d'images** : `vSyncCount = 0` pour tous les niveaux de qualité, `Application.targetFrameRate = 60` posé avant le chargement de la première scène par `Game.Bootstrap.FrameRateSetup` (sans cela, Android plafonne à 30 fps).
- **Langue du jeu** : anglais au lancement, via Unity Localization (français ensuite).
- **Dépôt** : `Larmik/The-Last-Lighthouse`, branche par défaut **`main`**. Tickets : GitHub Issues, préfixés par pôle (`OPS`, `CORE`, `GAME`, `ECO`, `META`, `SVC`, `UI`, `ART`, `AUD`, `NARR`, `QA`, `REL`), jalons `M0 - Setup` à `M6 - Soft launch`.

## Commandes

Le batchmode exige que l'éditeur Unity **ne soit pas ouvert** sur le projet (verrou du projet) : les scripts le détectent et sortent avec le code 3 (`EDITOR_OPEN`).

```bash
.claude/scripts/unity-run.sh Compile    # compilation seule (erreurs error CS)
.claude/scripts/unity-run.sh EditMode   # tests EditMode (logique pure), résumé TOTAL/PASSED/FAILED
.claude/scripts/unity-run.sh PlayMode   # tests PlayMode
.claude/scripts/unity-editor-open.sh    # "open" / "closed"
```

Les logs et résultats vont dans `Logs/claude-<mode>.log` et `Logs/claude-<mode>-results.xml`.

Build Android en ligne de commande (AAB et APK debug) : défini par le ticket OPS-007.

## Workflow & process

- **Pas de git sans demande explicite** : ne pas `commit` / `push` / créer de PR tant que le développeur ne l'a pas demandé (sauf flux de skill qui le prévoit explicitement).
- **Synchroniser avant de commencer** : `git fetch origin`, puis partir de `main` à jour (`git pull --ff-only`).
- **Un ticket = une branche = une PR** : branche `feat/<CODE>` (ex. `feat/OPS-001`), commits préfixés par le code du ticket (`OPS-001: ...`), PR vers `main` qui ferme l'issue (`Closes #N`).
- **Périmètre décidé par le développeur** : ne pas remettre en question le contenu d'une PR ni proposer de la scinder.
- **Travail complémentaire** : un besoin complémentaire découvert pendant un ticket (ce qui aurait donné une nouvelle issue) se traite directement dans le ticket en cours, pour éviter de futures dépendances entre tickets. Exceptions, qui deviennent une issue via `/create-ticket` : il dépend d'un ticket encore ouvert, exige une action humaine (compte, appareil, validation), ou relève d'un autre pôle sans lien avec le ticket. Une décision de design passe toujours par le développeur.
- **Rules à portée** : les consignes par couche vivent dans `.claude/rules/**` (frontmatter `paths`, chargées à l'ouverture d'un fichier correspondant) ; index dans `.claude/rules-index.md`. Un hook refuse tout commentaire dans un `.cs`.

### Skills

| Skill | Usage |
|---|---|
| `/ticket-dev <#N ou CODE>` | ticket de bout en bout : branche `feat/<CODE>`, agent `ticket-worker`, commit / push / PR, retours, fusion, board |
| `/create-ticket <description>` | nouvelle issue au format du projet (code `[CODE-NNN]`, labels, jalon, board) |
| `/edit-ticket <#N> <précisions>` | réécrire une issue existante |
| `/unity-tests [Compile\|EditMode\|PlayMode\|all]` | compiler et lancer les tests en batchmode |
| `/clean-branches` | supprimer les branches mergées (local et distant, après confirmation) |

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
| `Game.Core` | C# pur (`noEngineReferences`, seul DLL : Newtonsoft) : Wallet, StatBlock, courbes, temps, modèle `PlayerSave` et migrations | — |
| `Game.Data` | Définitions ScriptableObject, `UpgradeEffect`, `IRunContext`, `GameCatalog` | Core |
| `Game.Economy` | Portefeuille, courbes, énergie, récompenses | Core, Data |
| `Game.Meta` | Progression, îles, bâtiments, keepers, quêtes | Core, Data |
| `Game.Gameplay` | Faisceau, ennemis, vagues, `RunDirector` | Core, Data, Economy |
| `Game.Services` | Interfaces + implémentations (pubs, IAP, analytics, Remote Config, sauvegarde) | Core |
| `Game.UI` | Presenters (un par écran), sans logique métier | Core, Data, Economy, Meta, Gameplay, Services |
| `Game.Bootstrap` | Composition root VContainer (`GameLifetimeScope`, `BootFlow`) | tout |
| `Game.Editor` | Outils d'éditeur (validation du catalogue, menus), plateforme Editor uniquement | selon besoin |
| `Game.Tests.EditMode` / `Game.Tests.PlayMode` | Unity Test Framework | selon besoin |

**Jamais de dépendance inverse.** Une référence manquante entre assemblies se règle en revoyant le découpage, pas en ajoutant une dépendance à rebours.

- **Scènes** : `Boot` (services, chargement de la sauvegarde, puis Hub) → `Hub` (méta : archipel, améliorations, boutique) → `Run` (combat). Navigation par `ISceneLoader` (`Game.Data.Scenes`) ; l'ordre des Build Settings suit l'enum `GameScene` (menu `Game > Scenes > Create Missing Scenes And Build Settings`, vérifié par un test EditMode).
- **Composition root** : `GameLifetimeScope` (objet de la scène `Boot`, conservé entre scènes par `DontDestroyOnLoad`, parent des futurs scopes de Hub et Run) applique `GameInstaller` (enregistrements, vérifiés par un test EditMode) puis lance le point d'entrée `BootFlow` : chargement de la sauvegarde, puis Hub.
- **Services** : toujours une implémentation **Fake** pour l'éditeur (`#if UNITY_EDITOR`) afin de tester sans SDK.
- **RunDirector** : machine à états `Intro, Wave, UpgradeChoice, Boss, Victory, Defeat` ; possède la boucle Tick, le registre d'ennemis et le spawner poolé ; émet des événements C# écoutés par l'UI. `Time.timeScale = 0` pendant le choix de carte.
- **Sauvegarde** : JSON (Newtonsoft) dans `Application.persistentDataPath`, champ `Version` + migrations successives, écriture atomique (fichier temporaire puis remplacement), sauvegarde à chaque achat, fin de run et `OnApplicationPause`.

Paquets (brief § 5.1, versions épinglées listées dans `README.md`) : URP 2D + Light 2D, Input System, VContainer, UniTask, LitMotion, Newtonsoft JSON, Localization + TextMeshPro, Unity Test Framework ; à venir avec les tickets SVC : Google Mobile Ads + UMP, Unity IAP, Firebase (Analytics, Remote Config, Crashlytics), Mobile Notifications, Play Games Services.

## Éthique du design

Engagement par la progression et les choix, pas par la manipulation : pas de faux compte à rebours, pas de probabilités cachées, pas de pénalité brutale à l'absence, pas de pub forcée pendant l'action, pubs récompensées toujours facultatives, histoire jamais verrouillée derrière un paiement, aucune puissance de combat exclusive à l'achat, aucun contenu aléatoire vendu (les coffres ne s'achètent jamais), chances de rareté affichées.

## Tests

Unity Test Framework. Tests EditMode à écrire dès le début pour la logique pure : Wallet, EconomyCurves, StatBlock, régénération d'énergie, Light Cache hors ligne, tirage de cartes (poids et plafonds). Conventions détaillées : ticket QA-001.

## Pièges connus

- **`.meta` obligatoires** : tout asset ajouté, déplacé ou renommé doit l'être avec son `.meta` (le GUID qu'il contient relie les références). Déplacer des assets depuis l'éditeur Unity, pas depuis le Finder.
- **`JsonUtility` ne gère ni `Dictionary` ni `HashSet`** : utiliser Newtonsoft pour la sauvegarde.
- **Champs sérialisés** : renommer un champ `[SerializeField]` ou public d'un MonoBehaviour/ScriptableObject perd les valeurs saisies dans les assets (utiliser `[FormerlySerializedAs]` si un renommage est indispensable).
- **Éditeur ouvert** : les commandes batchmode échouent si le projet est ouvert dans l'éditeur.
