# Index des rules du projet

Ce fichier est **hors** de `.claude/rules/` : il n'est pas chargé automatiquement. Il sert au flux
`/ticket-dev` (agent `ticket-worker`) pour choisir quelles rules lire et où enrichir une rule.

## Chargement

- Un `.md` de `.claude/rules/` (sous-répertoires compris) **sans** frontmatter `paths` est chargé à
  chaque session ; **avec** `paths` (globs), il n'est chargé qu'au `Read` / `Write` / `Edit` d'un
  fichier correspondant, dans la session principale comme dans un sous-agent.
  La lecture via `cat`/`rg` dans Bash ne déclenche rien.
- `paths` est le seul champ lu ; un YAML invalide fait charger la rule sans condition.
- Toutes les rules ont des `paths`. Le transverse (workflow, conventions générales, brief) vit dans
  `CLAUDE.md`.
- Avant de **créer** un fichier (pas de `Read` préalable), lire explicitement les rules dont les
  `paths` couvrent son emplacement.

## Rules

Racine des scripts : `Assets/_Project/Scripts/` (abrégée `…/`).

| Fichier | Sujet | `paths` principaux |
|---|---|---|
| `csharp/style.md` | aucun commentaire, noms anglais, `sealed`, `[SerializeField] private`, diffs minimaux | `Assets/_Project/**/*.cs` |
| `architecture/assemblies.md` | matrice de dépendances des asmdef (dont `Game.Data`), où placer le code, namespaces | `…/**`, `Assets/_Project/Tests/**` |
| `architecture/services.md` | interface + Fake, constantes de placements/produits/clés, consentement (pubs, Analytics, Crashlytics), temps de confiance, VContainer, UniTask | `…/Services/**`, `…/Bootstrap/**` |
| `gameplay/performance.md` | boucle `Tick` unique, zéro allocation, pooling, faisceau sans Physics2D, cache | `…/Gameplay/**`, `…/UI/**` |
| `data/scriptableobjects.md` | aucune valeur d'équilibrage en dur, définitions, `Id` stable, `GameCatalog`, `FormerlySerializedAs` | `Assets/_Project/Data/**`, `…/Data/**`, `…/**/*Definition.cs`, `…/Economy/**`, `…/Meta/**`, `…/Gameplay/**` |
| `data/save-time.md` | sauvegarde versionnée et atomique, marqueur de run en cours, `ITimeProvider`, minuit local et délai de 20 h, calcul par horodatages | `…/Core/**`, `…/Economy/**`, `…/Meta/**`, `…/Services/**` |
| `ui/ui.md` | presenters sans logique métier, Localization, formateur de nombres, pause et temps non mis à l'échelle, safe area, pubs facultatives, chances affichées | `…/UI/**`, `Assets/_Project/Prefabs/UI/**` |
| `unity/assets-meta.md` | `.meta` obligatoires, pas de YAML de scène à la main, paquets épinglés, `ProjectSettings` | `Assets/**`, `Packages/**`, `ProjectSettings/**` |
| `tests/tests.md` | EditMode pour la logique pure, valeurs du brief, temps injecté, nommage | `Assets/_Project/Tests/**`, `…/Core/**`, `…/Economy/**`, `…/Meta/**` |
| `process/documentation.md` | brief intouchable, doc à jour sans historique, renvois de rules, hors périmètre → issue | `docs/**`, `.claude/**`, `CLAUDE.md`, `README.md` |

## Format d'une rule

```markdown
---
paths:
  - "Assets/_Project/Scripts/Gameplay/**"
---

# <Titre>

- Consigne prescriptive (« Faire X », « Ne pas faire Y »), une idée par puce.
- Référence courte : `#NN` (issue) ou section du brief (§ 3.3), pas de récit.
```

- Français, ton neutre, moins de 60 lignes par fichier, au plus un exemple court par consigne.
- Globs : un par ligne, entre guillemets, chemins complets depuis la racine du dépôt, **sans
  accolades**.
- Un sujet par fichier ; un sujet lié aux mêmes fichiers rejoint le fichier existant.

## Enrichir une rule (retour utilisateur)

- Le retour recoupe une rule existante → la mettre à jour.
- Préférence générale et durable non couverte, dans une catégorie existante (`csharp/`,
  `architecture/`, `gameplay/`, `data/`, `ui/`, `unity/`, `tests/`, `process/`) → l'ajouter au
  fichier le plus proche ; créer un nouveau fichier dans le répertoire seulement si le sujet ou les
  `paths` diffèrent.
- Nouvelle catégorie (nouveau répertoire) → demander confirmation à l'utilisateur avant.
- Retour propre à un seul ticket → aucune rule.
- Toute création / renommage → mettre à jour cet index.
- En cas de conflit entre une rule, le brief et un ticket, le signaler au lieu de trancher.
