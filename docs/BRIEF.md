# The Last Lighthouse : brief de conception et d'architecture

Titre de travail FR : *Le Dernier Phare*. Jeu mobile Android, roguelite court et méta-progression idle, Unity 6.3 LTS.

> **Source de vérité du projet.** Ce document remplace `docs/Le_Dernier_Phare_Brief.pdf` (version d'origine, conservée comme archive). Il intègre les décisions de `docs/AUDIT_BRIEF.md`.
>
> - Les valeurs chiffrées sont des **valeurs initiales**. Elles vivent dans des ScriptableObjects, surchargeables par Remote Config (§ 4.10), et **jamais en dur** dans le code.
> - Les valeurs marquées *(à simuler)* sont calées par le simulateur d'économie (ECO-004) avant de produire du contenu.
> - Un point marqué **[A TRANCHER]** se pose au développeur avant d'implémenter.
> - Le développeur est un développeur mobile expérimenté (Kotlin, Swift) qui débute en Unity : expliquer brièvement les spécificités Unity (cycle de vie, ScriptableObjects, asmdef, sérialisation) quand elles comptent.

---

## 1. Décisions figées

| Sujet | Décision |
|---|---|
| Plateforme | Android d'abord. iOS possible plus tard [A TRANCHER]. |
| Moteur et langage | Unity **6000.3.25f1 (Unity 6.3 LTS)**, 2D avec URP, C#. IDE : JetBrains Rider (ou Visual Studio). |
| Titre | *The Last Lighthouse* (titre de travail, homonymes à vérifier sur le Play Store) [A TRANCHER]. |
| Genre | Roguelite court (runs de 2 à 4 min) avec méta-progression idle autour d'un archipel à restaurer. |
| Contrôle | Tactile direct, un doigt : le faisceau du phare suit le doigt. Un bouton d'action (Flare) sous le pouce. |
| Langue | Anglais au lancement via Unity Localization. Langues suivantes [A TRANCHER] (français pressenti). |
| Monétisation | Pubs récompensées (cœur), interstitiels rares, achats in-app (suppression des pubs, pack de départ, pass de saison, Pearls, cosmétiques). |
| Public | Grand public, non ciblé enfants. Pas de violence graphique. |
| Production artistique | Art vectoriel (SVG) ou procédural produit dans le dépôt par le pipeline d'assets, avec noms, tailles et pivots fixes pour qu'un artiste puisse remplacer les fichiers. Freelance ou packs réévalués après M2. |
| Intégration continue | Pas de CI pour l'instant : tests lancés en local (`/unity-tests`) avant chaque PR. |
| Objectif produit | Rétention quotidienne et engagement sain. Valider la rétention en soft launch avant d'investir dans le contenu et l'acquisition. |

### Conventions de code

- Code C# **sans commentaires** (les noms doivent suffire) : ni blocs de commentaires, ni `TODO`.
- **Diffs minimaux** : ne pas reformater ni renommer ce qui n'est pas concerné par la tâche.
- Identifiants, noms de fichiers, clés Remote Config et événements analytics **en anglais**.
- La logique de jeu pure (économie, progression, stats) est en C# **sans `UnityEngine`**, testable en EditMode.
- **Aucune allocation dans les boucles de jeu** (pas de LINQ, pas de `new List` dans `Update`/`Tick`). **Object pooling** obligatoire pour ennemis, projectiles, alliés et effets.

---

## 2. Vision, univers et narration

### 2.1 Pitch et piliers

*Le monde a été englouti en une nuit. Il ne reste que des îlots et, sur chacun, un phare éteint. Tu es le dernier gardien : en rallumant les phares, tu repousses les ténèbres marines et tu découvres pourquoi la mer est montée.*

- **Lisible en deux secondes, jouable d'une main.** Un doigt pour viser, un bouton pour l'action.
- **La lumière est le langage du jeu.** Ce qui est éclairé est visible, vulnérable, vivant. Le rendu 2D éclairé fait l'essentiel de la valeur visuelle.
- **Chaque run offre des choix** (trois cartes d'amélioration) et une envie de recommencer.
- **Une progression visible chaque jour** : l'archipel se rallume îlot par îlot.
- **Monétisation respectueuse** : pubs facultatives, prix clairs, histoire jamais verrouillée derrière un paiement.

### 2.2 Récit (brouillon à valider)

Autrefois, un réseau de phares, la **Chain**, maintenait endormi quelque chose sous la mer, le **Deep**. Le phare mère, la **Mother Light**, s'est éteint et la mer est montée. Le joueur est le dernier gardien et progresse îlot par îlot en retrouvant des **Logbook entries** (pages de journaux de bord trouvées en run et en restaurant les bâtiments).

- **Acte I (îles 1-2)** : comprendre que les phares forment un réseau.
- **Acte II (îles 3-4)** : découvrir le passé des gardiens (les Keepers).
- **Acte III (îles 5-6)** : la vérité. Piste : la Mother Light a été éteinte volontairement par le gardien précédent, car sa lumière nourrissait le Deep. Choix final : rallumer ou laisser dormir dans le noir (deux fins, crochet pour une saison 2). [A TRANCHER]

Volume : environ 30 Logbook entries, au moins 4 par île (bible narrative : NARR-001).

### 2.3 Personnages

- **Wick** : compagnon, petit oiseau-lanterne. Il enseigne, commente, réagit. C'est la mascotte (icône, captures, pubs).
- **Keepers** (six, un par île) : débloqués avec des Fragments et la progression d'île, chacun avec un passif (§ 3.13) et une part de l'histoire. Un seul équipé par run.

### 2.4 Îles

| # | Île | Ambiance | Nouveauté de gameplay | Boss |
|---|---|---|---|---|
| 1 | Gull's Rest | Aube calme, falaises | Tutoriel, Drifters | Tidecrab |
| 2 | Saltmarrow | Marais, brume | Lurkers (visibles seulement dans le faisceau) | Mire Warden |
| 3 | The Drowned Orchard | Vergers engloutis | Shoals (essaims rapides), Tentacles (tanks) | Rootmaw |
| 4 | Hollow Reef | Corail, bioluminescence | Sirens (font dériver le faisceau) | The Pearl Choir |
| 5 | Brine Spires | Tempête, éclairs | Stormlings (trajectoires en zigzag) | Stormcaller |
| 6 | The Deep Gate | Abysses | Mélange de tout, boss final | The Sleeper |

Vertical slice : îles 1 à 3. Lancement : îles 1 à 6, ou 1 à 4 si le temps manque [A TRANCHER].

### 2.5 Direction artistique et audio

- 2D stylisé : silhouettes sombres, aplats de couleur, palette bleu nuit et ambre. Éclairage dynamique via Light 2D (Spot Light 2D à falloff doux pour le faisceau), bloom léger.
- Palette de départ (bible artistique, ART-001) : Night `#0B1426`, Deep `#14233B`, Sea `#1F3A5F`, Foam `#8FB8D8`, Amber `#F2A93B`, Ember `#D9701F`, Glow `#FFE2A8`, Alert `#C2413B`.
- Échelle : 64 pixels par unité ; ennemis 128×128, phare 256×384 (bible artistique).
- **Lisibilité mobile** : chaque ennemi se distingue par sa silhouette, pas par la couleur seule (daltonisme).
- Direction et fiche store sans codes visuels enfantins marqués (le jeu n'est pas destiné aux enfants, § 6).
- **Audio** : ambiance marine en boucle, filtre passe-bas hors faisceau, sons de kill courts, carillon d'amélioration. Haptique légère sur les événements clés (option désactivable).

### 2.6 Glossaire (termes en jeu, anglais)

| Terme | Sens |
|---|---|
| Light | Monnaie douce (runs, Light Cache, coffre quotidien) |
| Fragments | Monnaie de progression (Keepers, restauration de bâtiments) |
| Pearls | Monnaie premium |
| Lamp Oil | Énergie, une unité par run |
| Sparks | XP gagnée en run, remise à zéro à chaque run |
| Integrity | Points de vie du phare |
| Flare | Capacité active : pulse de lumière |
| Shadowed | Statut d'un ennemi hors du faisceau (plus rapide) |
| Elite | Variante renforcée d'un ennemi, lâche un coffre |
| Light Cache | Production hors ligne des bâtiments, à collecter |
| Logbook entry | Page d'histoire débloquée (journal) |
| Keeper / Wick | Personnage à passif équipé pour une run / compagnon mascotte |
| Keeper's Luck | Piste méta augmentant les chances de raretés supérieures |

---

## 3. Gameplay

### 3.1 Vue d'ensemble

Vue de dessus 2D en portrait (9:16). Le phare est fixe au centre de la zone de jeu ; les ennemis arrivent de tous les côtés depuis un anneau situé hors écran et avancent vers le phare. Le faisceau est un cône de lumière à 360 degrés de rotation. Tout ennemi dans le cône subit des dégâts continus. Le joueur gagne en survivant aux vagues, puis en battant le boss de l'île.

### 3.2 Contrôle tactile

- **Glisser n'importe où** : le faisceau pivote vers l'angle du doigt par rapport au phare, à vitesse de rotation limitée (`TurnSpeed`, 540 °/s au départ) pour donner du poids.
- **Relâcher** : le faisceau continue en balayage lent (`IdleSweepSpeed`, 25 °/s) à 40 % des dégâts (`IdleDamageRatio`). Jamais zéro, pour éviter la frustration et les arrêts de jeu.
- **Bouton Flare** (zone du pouce, bas droite) : pulse circulaire (§ 3.3). Option : inverser gauche/droite.
- Ignorer la visée quand le pointeur est sur l'UI (bouton Flare, pause). Input System (`Pointer.current`) pour que le même code marche à la souris dans l'éditeur.
- Respecter la safe area (encoches) pour l'UI.

### 3.3 Lumière et combat

| Valeur | Défaut | Stat |
|---|---|---|
| Arc du faisceau | 40° | `BeamArc` |
| Portée du faisceau | 6 unités | `BeamRange` |
| Dégâts du faisceau | 20 /s sur chaque ennemi dans le cône | `BeamDamage` |
| Dégâts au relâché | 40 % | `IdleDamageRatio` |
| Balayage au relâché | 25 °/s | `IdleSweepSpeed` |
| Flare : dégâts | 60 (une fois, sur chaque ennemi du rayon) | `FlareDamage` |
| Flare : rayon | 4 unités autour du phare | `FlareRadius` |
| Flare : recharge | 18 s | `FlareCooldown` |
| Flare : révélation des Lurkers | 2 s | `FlareRevealDuration` |
| Integrity | 100 | `MaxIntegrity` |

- Appartenance au cône par **calcul d'angle et de distance** (pas de Physics2D par ennemi), contre un registre d'ennemis actifs.
- **Shadowed** : hors lumière, les ennemis avancent **30 % plus vite** (réglage de run, pas une stat du joueur). Cela pousse à balayer au lieu de rester fixé sur un point.
- **Lurkers** : invisibles tant qu'ils ne sont pas éclairés (faisceau ou Flare). Ils avancent normalement (et Shadowed). Une fois dans leur **rayon de frappe** (2 unités autour du phare), un Lurker qui reste **1,5 s sans être éclairé** frappe le phare (dégâts de contact) puis disparaît. Éclairé, son minuteur repart à zéro.
- **Contact** : un ennemi qui atteint le phare inflige ses dégâts de contact puis disparaît, **sans** donner de Sparks.
- **Defeat** : Integrity à 0.
- **Revive** : une fois par run, 50 % de l'Integrity max rendus et Flare automatique (dégâts du Flare sur **tout l'écran**, boss compris, recharge remise à zéro). Gratuit pendant les 3 premières runs, puis par pub récompensée (§ 4.7).

### 3.4 Ennemis

| Ennemi | PV | Vitesse | Dégâts contact | Sparks | Comportement | Île |
|---|---|---|---|---|---|---|
| Drifter | 30 | 1,2 | 5 | 2 | Marche droit vers le phare | 1 |
| Lurker | 45 | 1,6 | 12 | 4 | Invisible hors faisceau, frappe à portée (§ 3.3) | 2 |
| Shoal | 10 | 2,4 | 2 | 1 | Essaim de 6 à 10, rapide | 3 |
| Tentacle | 180 | 0,7 | 20 | 8 | Tank, avance lentement | 3 |
| Siren | 40 | 0,9 | 0 | 4 | Reste à distance (portée 5) ; tant qu'au moins une Siren vit, le faisceau dérive de 30 °/s, sans cumul, dans le sens opposé au balayage au relâché | 4 |
| Stormling | 35 | 2,0 | 8 | 3 | Zigzag (amplitude et fréquence paramétrables), difficile à garder dans le cône | 5 |

- **Elite** (toutes îles) : ×2 PV, ×3 Sparks, aura visible. Placées dans les données de vague (pas aléatoires) : 1 à 2 par run sur l'île 1, 3 à 4 ensuite *(à simuler)*. Coffre : 2 Fragments.
- Sparks : cible de **8 à 10 choix de cartes par run victorieuse** sur une île à la puissance recommandée *(à simuler)*.

### 3.5 Structure d'une run

- **Tutoriel** : la toute première run est un tutoriel de 90 s (§ 3.10), hors progression.
- **Run normale** : huit vagues de 18 à 22 s, puis le boss (vague 9). Durée cible : 2 min 30 à 3 min 30 plus le boss (45 à 60 s).
- Vagues décrites par des ScriptableObjects (`WaveDefinition`) : liste de groupes {ennemi, nombre, délai de départ, intervalle, motif d'apparition (anneau, arc, rafale), Elite oui/non}.
- Les kills donnent des **Sparks**. À chaque niveau de run, le jeu se met en pause et propose **3 cartes** (1 choix). Un reroll par run via pub récompensée.
- XP pour passer du niveau n au suivant : `round(8 × 1,25^n)` (8, 10, 12, 16, 20, 24, 31, 38, 48, 60…).
- **Boss** : PV de base 1500 × multiplicateur de PV de l'île, trois phases (100, 66 et 33 %), attaques télégraphiées (§ 3.8).
- **Fin de run** :
  - Victoire : gains avec `wavesCleared = 9`, plus les Fragments de première victoire du boss (§ 4.1).
  - Défaite : gains selon les vagues terminées.
  - Abandon (menu pause) : traité comme une défaite à la vague en cours, sans revive proposé.

### 3.6 Améliorations en run

#### Règle de cumul

Chaque effet chiffré est un `StatModifier` (§ 5.3) de type **Flat** (ajout) ou **Percent** (pourcentage). Pour une stat :

`valeur = clamp((base + somme des Flat) × (1 + somme des Percent), min, max)`

Les pourcentages d'une même stat **s'additionnent** : Wide Lens ×4 donne +60 % d'arc, exactement ce qu'annonce la carte. Chaque stat a un plancher et un plafond (ex. arc minimum 10°, recharge du Flare minimum 4 s).

Ce qui n'est pas une valeur (soin ponctuel, pulse périodique, alliés) est un **effet** (`UpgradeEffect`, § 5.4). Une carte combine modificateurs et effets.

#### Catalogue

| Carte | Rareté | Max | Tags | Effet par niveau |
|---|---|---|---|---|
| Wide Lens | Common | 4 | LIGHT | `BeamArc` +15 % |
| Focused Lens | Rare | 3 | LIGHT | `BeamArc` −20 %, `BeamDamage` +35 % |
| Polished Mirror | Common | 5 | LIGHT | `BeamDamage` +12 % |
| Long Wick | Common | 4 | LIGHT | `BeamRange` +20 % |
| Quick Hands | Common | 3 | LIGHT | `TurnSpeed` +25 % |
| Lamp Reserve | Common | 3 | LIGHT | `FlareCooldown` −20 % |
| Prism | Epic | 1 | LIGHT | `BeamCount` +1 : second faisceau à 180°, à 60 % des dégâts (`SecondaryBeamRatio` 0,6) |
| Chain Light | Epic | 2 | LIGHT | `ChainJumps` +1 : l'ennemi éclairé le plus proche de l'axe relaie la lumière vers l'ennemi non éclairé le plus proche (≤ 2,5 unités), qui subit 50 % des dégâts du faisceau (`ChainRatio` 0,5) |
| Ember Oil | Rare | 3 | FIRE | `BurnDps` +4 : brûlure de 3 s, rafraîchie tant que l'ennemi est éclairé, non cumulable entre sources |
| Fog Shield | Common | 3 | TIDE | Ralentissement des ennemis à moins de 2,5 unités du phare : 25 % au niveau 1, +10 % par niveau suivant (`SlowRatio`) ; rayon +0,5 par niveau (`SlowRadius`) |
| Tide Pulse | Rare | 3 | TIDE | Effet `PeriodicPulse` : repoussement circulaire (rayon 3, recul 1,5 unité, 5 dégâts) toutes les 8 s, −1,5 s par niveau suivant (`PulseInterval`) |
| Gull Squadron | Rare | 3 | ALLY | Effet `AllySpawn` : `Allies` +1 mouette par niveau (8 dégâts par attaque, une attaque par seconde, cible l'ennemi le plus proche du phare, poolée) |
| Mend | Common | ∞ | — | Effet `Heal` : rend 25 % de l'Integrity max. Proposé seulement si l'Integrity n'est pas pleine |
| Lantern Coin | Common | ∞ | — | +20 Light ajoutés aux gains de la run. Repli uniquement (voir tirage) |

#### Tirage

1. Pour chacun des 3 emplacements : tirer la **rareté** selon les poids Common 60, Rare 30, Epic 10, modifiés par Keeper's Luck (§ 3.12).
2. Tirer une carte **uniforme** parmi les cartes éligibles de cette rareté : pas au maximum, pas déjà dans l'offre, Mend exclu à Integrity pleine, Lantern Coin exclu.
3. Rareté sans carte éligible → rareté inférieure.
4. Moins de 3 cartes éligibles au total → compléter par Mend, puis par Lantern Coin.
5. Les chances par rareté sont **affichées** dans l'écran de choix (bouton d'information).

### 3.7 Difficulté

- PV ennemi = PV de base × (1 + 0,15 × (vague − 1)) × multiplicateur de PV de l'île.
- Multiplicateurs de PV d'île : 1 ; 1,6 ; 2,5 ; 3,8 ; 5,5 ; 8.
- Nombre d'ennemis par groupe = base × (1 + 0,12 × (vague − 1)).
- **Assistance discrète** : après 3 défaites d'affilée sur une île, +10 % d'Integrity jusqu'à la prochaine victoire sur cette île (désactivable par Remote Config). Non affichée : elle est toujours favorable au joueur.
- Chaque île affiche une **puissance recommandée** (somme des niveaux des pistes du phare, § 3.12). Pas de mur invisible : le joueur sait pourquoi il bloque et quoi améliorer.

### 3.8 Boss

Cadre commun (GAME-013) : trois phases aux seuils 100, 66 et 33 % ; chaque attaque est **télégraphiée** (zone ou animation visible au moins 1 s avant) ; un Flare lancé pendant une télégraphie **annule** l'attaque et étourdit le boss 1,5 s. Le boss ne donne pas de Sparks. Barre de vie via événements.

#### Tidecrab (île 1)

| Phase | Comportement |
|---|---|
| 1 (100-66 %) | Tourne autour du phare à 5 unités (dans la portée du faisceau) à 15 °/s. **Claw Slam** toutes les 7 s : télégraphie 1,2 s, puis 10 dégâts au phare. |
| 2 (66-33 %) | Comme la phase 1, et appelle 4 Drifters toutes les 8 s. |
| 3 (< 33 %) | S'enfouit 2 s (invisible, intouchable), ressurgit à un angle aléatoire, tourne à 20 °/s ; Claw Slam toutes les 5 s. |

Durée cible : 45 à 60 s avec un build typique de l'île 1 *(à simuler)*.

#### Boss suivants (fiche détaillée à rédiger avant leur ticket)

- **Mire Warden** (île 2) : se cache dans la brume comme un Lurker, visible seulement éclairé ; appelle des Lurkers.
- **Rootmaw** (île 3) : des racines bloquent un secteur de l'écran (le faisceau n'y passe plus) ; appelle des Shoals.
- **The Pearl Choir** (île 4) : groupe de chanteuses qui font dériver le faisceau ; chaque membre a sa propre vie.
- **Stormcaller** (île 5) : éclairs télégraphiés en zigzag ; appelle des Stormlings.
- **The Sleeper** (île 6) : reprend les mécaniques des îles précédentes ; clôt le premier arc narratif.

### 3.9 Interruptions

- **Arrière-plan** : `OnApplicationPause(true)` met la run en pause ; au retour, écran de pause.
- **Mort du processus** : la run est perdue. Un marqueur « run en cours » (île, vague, horodatage) est écrit dans la sauvegarde au début de la run. S'il est trouvé au démarrage et que la run n'avait pas dépassé la vague 2, le Lamp Oil est remboursé.
- **Menu pause** : reprendre, réglages, abandonner (§ 3.5). Le bouton retour Android ouvre la pause pendant une run et ne quitte jamais l'app.

### 3.10 Premières minutes (FTUE)

- **Tutoriel** (90 s), guidé par Wick : glisser pour viser, utiliser Flare, premier niveau de run forcé avec une carte évidente. Trois vagues courtes, pas de boss, pas de coût en Lamp Oil, pas de récompense de victoire.
- Ensuite, les **trois premières runs** : aucun coût en Lamp Oil, aucune pub, revive gratuit.
- **Première victoire** (Tidecrab) : premier Logbook entry, puis demande de permission de notifications (Android 13 et plus) avec une explication claire.
- Premier achat méta guidé, puis restauration d'un premier bâtiment.
- Objectif : six à dix runs lors de la première journée.

### 3.11 Performance

60 fps sur l'appareil de référence, jusqu'à 120 ennemis à l'écran.

- Appareil de référence (milieu de gamme) : Android de 2022-2023 avec 6 Go de RAM (type Samsung Galaxy A53 ou A54).
- Appareil bas de gamme : 3 à 4 Go de RAM, 2020 ; doit rester jouable en qualité basse.
- Niveaux de qualité Low, Medium, High (Light 2D, bloom, shader de mer, particules), choisis automatiquement au premier lancement selon l'appareil et modifiables dans les réglages.

### 3.12 Méta-progression : pistes du phare

| Piste | Effet par niveau | Stat |
|---|---|---|
| Lens | +5 % de dégâts du faisceau | `BeamDamage` |
| Lamp | +6 % de Light gagnée en run | `LightGain` |
| Hull | +8 Integrity | `MaxIntegrity` |
| Gears | +4 % de vitesse de rotation | `TurnSpeed` |
| Keeper's Luck | Rareté : Common −0,5 point, Rare +0,4, Epic +0,1 | `RarityLuck` |

- **Plafond par île** : le niveau max de chaque piste dépend de l'île la plus haute débloquée : 10, 18, 26, 34, 42, 50 *(à simuler)*. Le plafond suivant est affiché.
- **Coût** : `UpgradeCost` avec base 80 et croissance 1,32 jusqu'au niveau 10 (80, 106, 139, 184, 243, 321, 423, 559, 737, 973), puis croissance 1,12 au-delà *(à simuler)*. Objectif : le plafond d'une île est atteignable sur une ou deux pistes prioritaires avant de débloquer l'île suivante.

### 3.13 Îles, bâtiments et Keepers

#### Déblocage d'une île

L'île N+1 se débloque quand le **boss de l'île N est vaincu** et qu'**au moins un bâtiment de l'île N est restauré**.

#### Bâtiments

Chaque île a son propre jeu de bâtiments. Restaurer = construire le niveau 1 (coût en Light **et** en Fragments) ; les niveaux suivants coûtent de la Light (base 150 × multiplicateur de gain de l'île, croissance 1,4). Niveau max 5.

Île 1 (Gull's Rest) :

| Bâtiment | Restauration | Effet niveau 1 | Par niveau suivant |
|---|---|---|---|
| Lantern Hut | 150 Light, 3 Fragments | Produit 20 Light/h | +10 Light/h |
| Net Loft | 300 Light, 5 Fragments | Régénération du Lamp Oil : 24 min | −1 min (minimum 20 min) |
| Archive | 300 Light, 5 Fragments | 5 % de chance de +1 Fragment par run victorieuse | +5 % |
| Dock | 500 Light, 8 Fragments | Coffre quotidien +10 % | +10 % |

Îles 2 et suivantes : un bâtiment producteur (Light/h × multiplicateur de gain de l'île), un bâtiment-phare qui débloque un Logbook entry, et un ou deux bâtiments utilitaires. Fiches détaillées à rédiger avant les tickets des îles 2-3 (vertical slice) puis 4-6.

#### Production hors ligne (Light Cache)

Stockage plafonné à 8 h (`offline_cap_hours`). À la reprise, crédit de min(temps écoulé, 8 h) à partir d'horodatages. Le joueur collecte la Light Cache ; une pub récompensée la double (3 par jour maximum).

#### Keepers

Un Keeper par île, débloqué quand l'île est atteinte, contre des Fragments. Un seul équipé par run. Skins cosmétiques achetables en Pearls.

| Keeper | Île | Prix | Passif |
|---|---|---|---|
| Orrin, le cartographe | 1 | Offert à la première victoire | `BeamRange` +10 % |
| Sela, la pêcheuse | 2 | 15 Fragments | `LightGain` +15 % |
| Iven, l'horloger | 3 | 25 Fragments | `FlareCooldown` −15 % |
| Nell, l'apicultrice | 4 | 40 Fragments | Commence chaque run avec 1 mouette (`Allies` +1) |
| Brann, le forgeron | 5 | 60 Fragments | `MaxIntegrity` +20 |
| Mira, la sonneuse de cloches | 6 | 90 Fragments | Un reroll de cartes gratuit par run (sans pub) |

---

## 4. Économie

Toutes les valeurs sont des points de départ à simuler (ECO-004) avant de produire du contenu, puis à régler en soft launch via Remote Config.

### 4.1 Monnaies

| Monnaie | Type | Sources | Utilisations |
|---|---|---|---|
| Light | Douce | Runs, Light Cache, coffre quotidien, quêtes | Pistes du phare, bâtiments |
| Fragments | Progression | Première victoire de chaque boss (10, 15, 20, 25, 30, 40 selon l'île), coffres d'Elite (2), Archive, coffre quotidien (jour 5) | Keepers, restauration de bâtiments |
| Pearls | Premium | Achats ; gratuit : boss premier clear (25), quêtes (10 par jour), piste gratuite de saison (100 au total), jalons de série | Cosmétiques, recharge de Lamp Oil, paliers premium de saison |
| Lamp Oil | Énergie | Régénération, coffre quotidien, pubs, Pearls | 1 par run |
| Sparks | XP de run | Kills | Niveaux de run (non conservés) |

Les Logbook entries ne sont pas une monnaie : elles se débloquent par la progression (victoires, bâtiments-phares) et ne s'achètent jamais.

### 4.2 Formules

```csharp
public static class EconomyCurves
{
    public static long UpgradeCost(float baseCost, float growth, int level) =>
        (long)Math.Round(baseCost * Math.Pow(growth, level));

    public static long RunPayout(int wavesCleared, int kills, float lightBonus, float islandPayoutMultiplier) =>
        (long)Math.Round((20 + 15 * wavesCleared + 0.4f * kills) * (1f + lightBonus) * islandPayoutMultiplier);

    public static float EnemyHealth(float baseHealth, int wave, float islandHealthMultiplier) =>
        baseHealth * (1f + 0.15f * (wave - 1)) * islandHealthMultiplier;

    public static int XpToNext(int level) => (int)Math.Round(8 * Math.Pow(1.25, level));
}
```

- Coût des pistes : § 3.12 (base 80, croissance 1,32 puis 1,12 au-delà du niveau 10).
- Gain de run de référence (vague 8, 100 kills, île 1, sans bonus) : 180 Light.
- Multiplicateurs de **gain** par île : 1 ; 1,5 ; 2,2 ; 3,2 ; 4,5 ; 6 (distincts des multiplicateurs de PV).
- Bâtiments : base 150, croissance 1,4 (§ 3.13).

### 4.3 Rythme de progression visé (joueur gratuit)

| Repère | Objectif |
|---|---|
| Jour 1 | 6 à 10 runs, 6 à 8 achats d'amélioration, premier bâtiment restauré |
| Jour 1-2 | Île 2 débloquée |
| Jour 3-4 | Île 3 |
| Jour 7-9 | Île 4 |
| Jour 14-18 | Île 5 |
| Jour 25-35 | Île 6 et fin du premier arc narratif |

### 4.4 Énergie (Lamp Oil)

- Maximum 5. Régénération : 1 par 25 min (réduite par le Net Loft). Une run coûte 1. Le tutoriel et les trois runs suivantes sont gratuits.
- Calculée à partir d'horodatages à la reprise de l'app (`ITimeProvider`), jamais avec des timers en mémoire.
- Recharges bonus : pub récompensée (+1, 4 par jour maximum), coffre quotidien, 20 Pearls pour un plein. Les bonus peuvent dépasser le plafond jusqu'à 8.
- Régénération arrêtée au plafond ; un bonus acquis n'est jamais perdu.
- **Énergie vide** : écran proposant d'attendre (temps restant affiché), une pub (+1) ou un plein contre 20 Pearls.
- Protection de l'horloge : un retour en arrière de l'horloge ne crédite rien et ne retire rien. Source de temps de confiance : en-tête HTTP `Date` d'une requête déjà effectuée (Remote Config), avec repli sur l'horloge locale hors ligne.

### 4.5 Production hors ligne

Voir § 3.13 (Light Cache et bâtiments).

### 4.6 Récompenses quotidiennes, série et quêtes

- **Jour de jeu** : remise à zéro à **minuit heure locale**. Un changement de fuseau ou d'horloge ne peut pas rendre une récompense disponible moins de 20 h après la précédente.
- **Cycle de 7 jours** : J1 200 Light, J2 1 Lamp Oil, J3 20 Pearls, J4 500 Light, J5 5 Fragments, J6 2 Lamp Oil, J7 un cosmétique du cycle + 50 Pearls (100 Pearls quand tous les cosmétiques du cycle sont possédés). Remove Ads double le coffre.
- **Série** : balise qui grandit chaque jour consécutif. Jalons à 3, 7, 14 et 30 jours avec Pearls. Un jour manqué par **période glissante de 7 jours** est pardonné automatiquement.
- **Quêtes quotidiennes** : trois quêtes par jour tirées d'un catalogue (ex. battre 80 ennemis, finir 3 runs, utiliser Flare 10 fois, choisir 2 cartes Rare, collecter la Light Cache, vaincre une Elite). Chaque quête donne 100 Light × multiplicateur de gain de l'île la plus haute ; les trois terminées donnent 10 Pearls.

### 4.7 Publicités

| Placement | Où | Récompense | Limite |
|---|---|---|---|
| `revive` | Écran de défaite | Reprendre à 50 % d'Integrity | 1 par run (gratuit pendant les 3 premières runs) |
| `double_rewards` | Écran de fin de run | Gains ×2 | Une fois par fin de run, sans plafond quotidien |
| `extra_lamp_oil` | Énergie vide | +1 Lamp Oil | 4 par jour |
| `light_cache_x2` | Bâtiments | Light Cache ×2 | 3 par jour |
| `reroll` | Choix de cartes | Retirage des 3 cartes | 1 par run |

- **Interstitiels** : en fin de run seulement, jamais pendant le jeu, une run sur deux au maximum, minimum 180 s depuis la dernière pub, jamais pendant le tutoriel ni les trois premières runs, jamais juste après une proposition de revive, jamais pour un joueur ayant acheté Remove Ads.
- Pas de bannière. Toute pub récompensée est facultative, présentée comme telle, avec sa récompense affichée avant le choix.
- Aucune demande de pub avant le consentement UMP (§ 6).

### 4.8 Achats in-app

| Produit (id) | Type Google Play | Prix indicatif | Contenu |
|---|---|---|---|
| `remove_ads` | Non consommable | 4,99 | Plus d'interstitiels ; coffre quotidien doublé |
| `starter_pack` | Non consommable (unique) | 1,99 | 150 Pearls, 1 000 Light, 5 Lamp Oil, skin Ember |
| `pearls_s` / `m` / `l` / `xl` / `xxl` | Consommables | 0,99 / 4,99 / 9,99 / 19,99 / 49,99 | 80 / 500 / 1 100 / 2 400 / 6 500 Pearls |
| `season_pass_s1`, `season_pass_s2`… | Non consommable, un produit par saison | 4,99 | Piste premium de la saison |

- **Starter pack** : proposé une fois, à la fin de la première session comportant une victoire, puis disponible dans la boutique tant qu'il n'est pas acheté. Aucun compte à rebours.
- **Pass de saison** (4 semaines, 30 paliers) : XP via quêtes et runs. Piste gratuite : Light, Lamp Oil, 100 Pearls au total. Piste premium : skins de phare, de faisceau et de Wick. Fiche détaillée (XP par palier, contenu) à rédiger avant META-010.
- Cosmétiques uniquement : aucune puissance de combat exclusive à l'achat. Le jeu reste gagnable sans payer.
- Prix des contenus en Pearls toujours accompagnés de leur équivalent lisible ; packs cohérents avec les prix des cosmétiques (§ 6).
- Achats restaurables (non consommables) ; achats en attente gérés ; aucun double crédit.

### 4.9 Objectifs chiffrés (à valider en soft launch)

| Indicateur | Cible indicative |
|---|---|
| Rétention J1 / J7 / J30 | 35-40 % / 12-15 % / 4-6 % |
| Sessions par jour | 3 ou plus ; 6 à 9 min par session |
| Pubs récompensées par utilisateur actif | 3 à 5 par jour |
| Conversion payeurs | 1,5 à 3 % |
| ARPDAU | 0,08 à 0,15 USD (ordre de grandeur de marché, non garanti) |

### 4.10 Remote Config et analytics

**Principe** : les ScriptableObjects portent les valeurs par défaut. Remote Config peut surcharger n'importe quel champ d'une définition par son `Id`, via une clé JSON par famille. Les surcharges s'appliquent à des copies en mémoire, jamais aux assets.

| Clé | Défaut |
|---|---|
| `lamp_oil_max` / `lamp_oil_regen_minutes` / `lamp_oil_bonus_cap` | 5 / 25 / 8 |
| `offline_cap_hours` | 8 |
| `interstitial_min_seconds` / `interstitial_every_n_runs` | 180 / 2 |
| `rewarded_extra_lamp_oil_per_day` / `rewarded_light_cache_per_day` | 4 / 3 |
| `upgrade_cost_base` / `upgrade_cost_growth` / `upgrade_cost_growth_late` | 80 / 1.32 / 1.12 |
| `island_health_multipliers` | 1, 1.6, 2.5, 3.8, 5.5, 8 |
| `island_payout_multipliers` | 1, 1.5, 2.2, 3.2, 4.5, 6 |
| `rarity_weights` | 60, 30, 10 |
| `assist_enabled` / `assist_after_losses` / `assist_integrity_bonus` | true / 3 / 0.10 |
| `free_runs_at_start` | 3 |
| `daily_min_hours_between_claims` | 20 |
| `min_app_version` | 1 |
| `enemy_overrides`, `upgrade_overrides`, `wave_overrides`, `island_overrides`, `building_overrides`, `keeper_overrides`, `economy_overrides` | `{}` |

**Événements analytics** (snake_case, au plus 25 paramètres chacun) : `tutorial_step`, `run_start`, `run_end` (résultat, vague, kills, durée, île), `run_abandon`, `revive_used`, `upgrade_picked`, `card_reroll`, `boss_phase`, `meta_upgrade_bought`, `building_restored`, `island_unlocked`, `keeper_unlocked`, `keeper_equipped`, `logbook_entry_unlocked`, `quest_completed`, `daily_claim`, `streak_milestone`, `energy_empty`, `pearls_spent`, `cosmetic_equipped`, `shop_open`, `iap_purchase`, `ad_rewarded_offered` / `started` / `completed`, `ad_interstitial_shown`, `notification_opt_in`, `consent_status`.

### Éthique du design

L'engagement passe par la progression et les choix, pas par la manipulation :

- pas de faux compte à rebours ;
- pas de probabilités cachées : les chances de rareté sont affichées ;
- pas de pénalité brutale à l'absence ;
- pas de pub forcée au milieu d'une action ;
- **aucun contenu aléatoire vendu** contre de l'argent ou des Pearls : les coffres ne s'achètent jamais.

Cela réduit aussi le risque d'avis négatifs et de retrait du store.

---

## 5. Architecture (Unity)

### 5.1 Paquets et réglages

| Besoin | Paquet / outil |
|---|---|
| Rendu 2D | URP (template Universal 2D), Light 2D, Sprite Atlas |
| Entrées | Input System (Active Input Handling : Input System Package) |
| Injection de dépendances | VContainer (équivalent d'esprit à Hilt) |
| Asynchrone | UniTask |
| Animations UI / tweens | LitMotion (MIT, distribué en UPM donc épinglable, zéro allocation, intégration UniTask) |
| Sérialisation de la sauvegarde | `com.unity.nuget.newtonsoft-json` (`JsonUtility` ne gère ni `Dictionary` ni `HashSet`) |
| Textes | Unity Localization, TextMeshPro |
| Pubs | Google Mobile Ads (AdMob), médiation AppLovin MAX ou LevelPlay (M6) ; consentement via Google UMP |
| Achats | Unity IAP (Google Play Billing) |
| Backend léger | Firebase : Analytics, Remote Config, Crashlytics (External Dependency Manager pour Android) |
| Notifications | `com.unity.mobile.notifications` (notifications locales) |
| Sauvegarde cloud | Google Play Games Services (avant l'ouverture des achats au soft launch) |
| Mises à jour | Play In-App Updates (mise à jour forcée sous `min_app_version`), Play In-App Review |
| Tests | Unity Test Framework (EditMode et PlayMode) |

- **Android** : IL2CPP, ARM64 (ARMv7 optionnel), format AAB, minSdk = le plus haut entre 24, le minimum de Unity 6.3 et ceux des SDK épinglés (valeur retenue notée dans `CLAUDE.md`), targetSdk exigé par Google Play au moment de la publication, compression ASTC, portrait verrouillé, `Application.targetFrameRate = 60`, vSync désactivé.
- **Budget de taille** : AAB < 150 Mo.
- **Git** : `.gitignore` Unity, `.meta` versionnés, Force Text, Visible Meta Files, Git LFS pour les sources lourdes.

### 5.2 Arborescence et assemblies

```
Assets/
  _Project/
    Art/ Audio/ Prefabs/
    Scenes/        Boot, Hub, Run
    Data/          Enemies, Upgrades, Waves, Islands, Buildings, Keepers (ScriptableObjects)
    Scripts/
      Core/        Game.Core        C# pur, sans UnityEngine (noEngineReferences), dont le temps (ITimeProvider)
      Data/        Game.Data        définitions ScriptableObject, UpgradeEffect, GameCatalog
      Economy/     Game.Economy     portefeuille, courbes, énergie, récompenses
      Meta/        Game.Meta        progression, îles, bâtiments, keepers, quêtes
      Gameplay/    Game.Gameplay    faisceau, ennemis, vagues, RunDirector
      Services/    Game.Services    interfaces + implémentations (pubs, IAP, analytics, sauvegarde…)
      UI/          Game.UI
      Bootstrap/   Game.Bootstrap   composition root (VContainer)
      Editor/      Game.Editor      outils d'éditeur (validation du catalogue, menus)
    Tests/
      EditMode/    Game.Tests.EditMode
      PlayMode/    Game.Tests.PlayMode
```

- **Dépendances** : Data dépend de Core ; Economy et Meta dépendent de Core et Data ; Gameplay dépend de Core, Data et Economy ; UI dépend de Core, Data, Economy, Meta, Gameplay et Services ; Services dépend de Core ; Bootstrap dépend de tout. Jamais l'inverse.
- **Game.Data** existe parce qu'un ScriptableObject exige `UnityEngine` (interdit dans Core) et que Meta et Economy doivent lire les définitions sans dépendre de Gameplay. `Scripts/Data/` contient le code des définitions ; `_Project/Data/` contient les assets.
- **Scènes** : Boot (initialise les services, charge la sauvegarde, puis charge Hub), Hub (archipel, améliorations, bâtiments, Keepers, boutique), Run (combat, une seule scène paramétrée par `IslandDefinition`).
- Les services ont toujours une implémentation **Fake** pour l'éditeur afin de tester sans SDK.

### 5.3 Noyau pur : portefeuille et stats

```csharp
public enum CurrencyType { Light, Fragments, Pearls, LampOil }

public sealed class Wallet
{
    private readonly Dictionary<CurrencyType, long> balances = new();
    public event Action<CurrencyType, long> Changed;

    public long Get(CurrencyType type) => balances.TryGetValue(type, out var value) ? value : 0;
    public bool CanAfford(CurrencyType type, long amount) => Get(type) >= amount;

    public void Add(CurrencyType type, long amount)
    {
        balances[type] = Get(type) + amount;
        Changed?.Invoke(type, balances[type]);
    }

    public bool TrySpend(CurrencyType type, long amount)
    {
        if (!CanAfford(type, amount)) return false;
        balances[type] = Get(type) - amount;
        Changed?.Invoke(type, balances[type]);
        return true;
    }
}

public enum Stat
{
    BeamDamage, BeamArc, BeamRange, TurnSpeed, IdleSweepSpeed, IdleDamageRatio,
    MaxIntegrity, FlareDamage, FlareRadius, FlareCooldown, FlareRevealDuration,
    BeamCount, SecondaryBeamRatio, ChainJumps, ChainRatio, BurnDps,
    SlowRatio, SlowRadius, PulseInterval, Allies, LightGain, RarityLuck
}

public enum ModOp { Flat, Percent }

[Serializable]
public struct StatModifier
{
    public Stat Stat;
    public ModOp Op;
    public float Value;
}

public readonly struct StatRange
{
    public readonly float Min;
    public readonly float Max;
    public StatRange(float min, float max) { Min = min; Max = max; }
}

public sealed class StatBlock
{
    private static readonly int StatCount = Enum.GetValues(typeof(Stat)).Length;
    private readonly float[] baseValues = new float[StatCount];
    private readonly float[] flatSums = new float[StatCount];
    private readonly float[] percentSums = new float[StatCount];
    private readonly StatRange[] ranges = new StatRange[StatCount];

    public StatBlock(IReadOnlyDictionary<Stat, float> bases, IReadOnlyDictionary<Stat, StatRange> limits)
    {
        for (var i = 0; i < StatCount; i++) ranges[i] = new StatRange(float.MinValue, float.MaxValue);
        foreach (var pair in bases) baseValues[(int)pair.Key] = pair.Value;
        foreach (var pair in limits) ranges[(int)pair.Key] = pair.Value;
    }

    public void Apply(StatModifier modifier)
    {
        if (modifier.Op == ModOp.Flat) flatSums[(int)modifier.Stat] += modifier.Value;
        else percentSums[(int)modifier.Stat] += modifier.Value;
    }

    public float Get(Stat stat)
    {
        var index = (int)stat;
        var value = (baseValues[index] + flatSums[index]) * (1f + percentSums[index]);
        return Math.Clamp(value, ranges[index].Min, ranges[index].Max);
    }
}
```

- `ModOp.Percent` : `Value` est une fraction (0,15 = +15 %, −0,20 = −20 %). Les pourcentages d'une même stat s'additionnent.
- Une carte prise N fois applique ses `PerStack` N fois. `Get` n'alloue pas.
- La même mécanique sert aux pistes du phare, aux Keepers et aux cartes : le `StatBlock` de départ d'une run cumule base, pistes et Keeper, puis reçoit les cartes.

### 5.4 Données : ScriptableObjects

```csharp
[CreateAssetMenu(menuName = "Phare/Enemy")]
public sealed class EnemyDefinition : ScriptableObject
{
    public string Id;
    public float MaxHealth;
    public float Speed;
    public float ContactDamage;
    public int Sparks;
    public bool HiddenInDark;
    public float StrikeRadius;
    public float StrikeDelay;
    public GameObject Prefab;
}

public enum Rarity { Common, Rare, Epic }

[CreateAssetMenu(menuName = "Phare/Upgrade")]
public sealed class UpgradeDefinition : ScriptableObject
{
    public string Id;
    public Rarity Rarity;
    public int MaxStacks = 1;
    public bool Unlimited;
    public bool FallbackOnly;
    public string[] Tags;
    public StatModifier[] PerStack;
    public UpgradeEffect[] Effects;
}

public abstract class UpgradeEffect : ScriptableObject
{
    public abstract void OnPicked(IRunContext context, int stackCount);
}

public interface IRunContext
{
    StatBlock Stats { get; }
    void Heal(float ratioOfMax);
    void AddRunLight(long amount);
    void EnablePeriodicPulse(PeriodicPulseEffect effect);
    void SetAllies(int count);
}
```

- Toutes les définitions, `UpgradeEffect`, `IRunContext` et le `GameCatalog` vivent dans `Game.Data`. `IRunContext` est implémenté par Gameplay (`RunDirector`) ; les champs prefab sont typés `GameObject` pour ne pas dépendre de Gameplay.
- Effets concrets : `HealEffect` (Mend), `PeriodicPulseEffect` (Tide Pulse), `AllySpawnEffect` (Gull Squadron), `LightBonusEffect` (Lantern Coin). Les effets lisent leurs valeurs dans le `StatBlock` quand elles en dépendent.
- Autres définitions : `WaveDefinition`, `IslandDefinition` (multiplicateurs, boss, vagues, bâtiments, Keeper, Logbook entries), `BossDefinition`, `KeeperDefinition`, `BuildingDefinition`, `QuestDefinition`, `CosmeticDefinition`, `LogbookEntryDefinition`, `RunTuning` (valeurs de base de la run : Shadowed, rayon d'apparition…).
- Un `GameCatalog` (ScriptableObject) référence toutes les définitions et est injecté par le composition root. Un validateur d'éditeur signale les `Id` dupliqués et les références manquantes.
- Les `Id` sont en `snake_case` anglais et stables : ils sont écrits dans la sauvegarde, les surcharges Remote Config et l'analytics.

### 5.5 Faisceau (contrôle tactile)

```csharp
public static class BeamGeometry
{
    public static bool Contains(Vector2 origin, float angle, float halfArc, float range, Vector2 point)
    {
        var offset = point - origin;
        if (offset.sqrMagnitude > range * range) return false;
        var pointAngle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        return Mathf.Abs(Mathf.DeltaAngle(angle, pointAngle)) <= halfArc;
    }
}

public sealed class BeamController : MonoBehaviour
{
    [SerializeField] private Transform pivot;
    private Camera cam;
    private float angle;

    public float Angle => angle;
    public bool IsAimed { get; private set; }

    private void Awake() => cam = Camera.main;

    public void Tick(float dt, float turnSpeed, float idleSweepSpeed, float driftSpeed, bool pointerOverUi)
    {
        var pointer = Pointer.current;
        IsAimed = pointer != null && pointer.press.isPressed && !pointerOverUi;
        if (IsAimed)
        {
            var screen = pointer.position.ReadValue();
            var depth = -cam.transform.position.z;
            var world = cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, depth));
            var direction = world - pivot.position;
            var target = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle = Mathf.MoveTowardsAngle(angle, target, turnSpeed * dt);
        }
        else
        {
            angle += idleSweepSpeed * dt;
        }
        angle += driftSpeed * dt;
        pivot.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}
```

- Le contrôleur est piloté par le `RunDirector` (une seule boucle `Tick` centrale). Le sprite du faisceau pointe vers le haut par défaut, d'où le décalage de 90°.
- `driftSpeed` vaut −30 °/s (sens opposé au balayage) tant qu'une Siren est vivante, 0 sinon.
- Le cône visuel (Spot Light 2D) et le cône logique partagent les mêmes valeurs (arc, portée), lues dans le `StatBlock`.

### 5.6 Services et composition root

```csharp
public interface ITimeProvider
{
    DateTime UtcNow { get; }
    TimeSpan LocalOffset { get; }
}

public interface IAdService
{
    bool IsRewardedReady { get; }
    UniTask<bool> ShowRewardedAsync(string placement, CancellationToken cancellationToken = default);
    UniTask ShowInterstitialAsync(string placement, CancellationToken cancellationToken = default);
}

public interface IPurchaseService
{
    UniTask InitializeAsync(CancellationToken cancellationToken = default);
    UniTask<bool> PurchaseAsync(string productId, CancellationToken cancellationToken = default);
    UniTask RestoreAsync(CancellationToken cancellationToken = default);
    bool Owns(string productId);
}

public interface IAnalyticsService
{
    void Log(string eventName, params (string Key, object Value)[] parameters);
}

public interface IConsentService
{
    UniTask<bool> GatherConsentAsync(CancellationToken cancellationToken = default);
    bool CanRequestAds { get; }
    void ShowPrivacyOptions();
}

public interface ISaveService
{
    PlayerSave Load();
    void Save(PlayerSave data);
}

public interface IRemoteConfigService
{
    T Get<T>(string key, T fallback);
}
```

```csharp
public sealed class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private GameCatalog catalog;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(catalog);
        builder.Register<ITimeProvider, SystemTimeProvider>(Lifetime.Singleton);
        builder.Register<ISaveService, JsonSaveService>(Lifetime.Singleton);
        builder.Register<IConsentService, UmpConsentService>(Lifetime.Singleton);
        builder.Register<IRemoteConfigService, FirebaseRemoteConfigService>(Lifetime.Singleton);
        builder.Register<IAnalyticsService, FirebaseAnalyticsService>(Lifetime.Singleton);
        builder.Register<IAdService, AdMobAdService>(Lifetime.Singleton);
        builder.Register<IPurchaseService, UnityIapPurchaseService>(Lifetime.Singleton);
        builder.Register<Wallet>(Lifetime.Singleton);
        builder.Register<EnergyService>(Lifetime.Singleton);
        builder.Register<ProgressionService>(Lifetime.Singleton);
        builder.RegisterEntryPoint<BootFlow>();
    }
}
```

En éditeur, chaque service adossé à un SDK est remplacé par son Fake (`FakeAdService`, `FakePurchaseService`, `FakeConsentService`…).

Les méthodes asynchrones des services prennent en dernier paramètre un `CancellationToken` optionnel, lié au cycle de vie de l'appelant ; une opération annulée lève `OperationCanceledException` sans effet (pas de récompense, d'achat ni de consentement enregistré).

- **Temps** : `ITimeProvider`, `SystemTimeProvider` et `FakeTimeProvider` (temps avançable, fuseau modifiable) vivent dans `Game.Core` (`Game.Core.Time`), lisibles par Economy et Meta sans dépendre de Services. En éditeur, `SystemTimeProvider` reste le choix par défaut ; `FakeTimeProvider` sert aux tests.
- **Sauvegarde** : `FakeSaveService` garde la sauvegarde en mémoire en passant par le même sérialiseur que `JsonSaveService` (tests, PlayMode sans disque).
- **Fakes** : compilés partout, choisis au composition root ; ils lèvent `ArgumentException` sur une erreur de développeur (placement inconnu, identifiant vide, nom d'événement hors snake_case). Les implémentations réelles dégradent proprement (jeu jouable sans pub ni réseau).
- **Constantes** : placements (`AdPlacements`) et produits (`ProductIds`) dans `Game.Services` ; les clés Remote Config et le catalogue d'événements analytics arrivent avec leurs services.

### 5.7 Sauvegarde

```csharp
[Serializable]
public sealed class PlayerSave
{
    public int Version = 1;
    public Dictionary<CurrencyType, long> Balances = new();
    public Dictionary<string, int> MetaLevels = new();
    public int HighestIsland = 1;
    public Dictionary<string, IslandState> Islands = new();
    public HashSet<string> UnlockedKeepers = new();
    public string EquippedKeeper;
    public HashSet<string> OwnedCosmetics = new();
    public Dictionary<string, string> EquippedCosmetics = new();
    public HashSet<string> LogbookEntries = new();
    public HashSet<string> OwnedProducts = new();
    public EnergyState Energy = new();
    public DailyState Daily = new();
    public QuestState Quests = new();
    public AdCounters Ads = new();
    public SeasonState Season = new();
    public TutorialState Tutorial = new();
    public RunInProgress ActiveRun;
    public int RunsCompleted;
    public bool StarterPackOffered;
    public long LastSeenTimestamp;
    public long LightCacheTimestamp;
    public PlayerSettings Settings = new();
}
```

- `IslandState` : boss vaincu, niveaux des bâtiments, défaites consécutives (assistance).
- `EnergyState` : quantité, horodatage de dernière régénération, bonus au-delà du plafond.
- `DailyState` : jour du cycle, horodatage de la dernière réclamation, série, jour pardonné utilisé (fenêtre glissante).
- `QuestState` : horodatage d'attribution, quêtes actives (`QuestProgress` : `Id` de la quête, progression, réclamée), bonus des trois quêtes réclamé.
- `AdCounters` : compteurs par jour par placement (`extra_lamp_oil`, `light_cache_x2`) et horodatage du jour de ces compteurs, horodatage du dernier interstitiel, runs depuis le dernier interstitiel.
- `SeasonState` : `Id` de la saison, XP, paliers gratuits et premium réclamés ; l'achat du pass se lit dans `OwnedProducts`.
- `TutorialState` : run tutoriel terminée, étapes FTUE accomplies.
- `RunInProgress` (marqueur de run en cours, § 3.9) : `Id` de l'île, vague, horodatage de début, Lamp Oil débité ou non (rien n'est remboursé pour une run gratuite).
- `PlayerSettings` : volumes musique et effets (1 par défaut), haptique (activée), gaucher (désactivé), tremblement d'écran (activé), qualité `GraphicsQuality` (`Auto`, `Low`, `Medium`, `High` ; `Auto` = choix automatique au premier lancement, § 3.11), langue (vide = langue du système).
- **Défauts neutres** : tout champ absent ou nul du JSON reprend sa valeur par défaut (collections et sous-états vides, monnaies et énergie à 0, horodatages à 0 = jamais, `HighestIsland` = 1, pas de run en cours). Le contenu de départ d'une partie (énergie pleine selon `lamp_oil_max`…) est posé à la création de la partie à partir de la configuration, jamais par le modèle.
- **Horodatages** : millisecondes Unix UTC (`long`).
- **Versionnage** : `Version` absent = version 1 ; chaque migration transforme le JSON brut de la version N vers N+1 avant désérialisation ; une sauvegarde plus récente que l'app ou illisible est refusée sans être écrasée.
- Fichier JSON dans `Application.persistentDataPath`, champ `Version` et migrations successives, écriture atomique (fichier temporaire puis remplacement) avec copie de secours, sauvegarde à chaque achat, fin de run et `OnApplicationPause(true)`.
- **Fichiers** : `player_save.json` (principal), `player_save.backup.json` (secours), `player_save.tmp` (temporaire), `player_save.unreadable.json` (principal illisible mis de côté).
- **Écriture** : le JSON produit est relu avant toute écriture (illisible = refusé, rien n'est touché), écrit dans le temporaire vidé sur disque, puis le principal lisible devient le secours et le temporaire est renommé en principal (renommages sans `File.Replace`).
- **Lecture** : aucun fichier = nouvelle partie ; principal illisible ou absent = secours ; principal et secours illisibles = chargement refusé (`SaveDataException`), sans réinitialisation implicite : le `BootFlow` affiche un message et propose une réinitialisation explicite.
- **Principal illisible lors d'une sauvegarde** : il n'est pas écrasé mais renommé en `player_save.unreadable.json` (un seul exemplaire) ; la copie de secours valide est conservée.
- Sauvegarde cloud (Play Games) avec résolution de conflit sans perte de monnaie premium.

### 5.8 Run et UI

- **RunDirector** : machine à états `Intro, Wave, UpgradeChoice, Boss, Victory, Defeat` (plus `Paused`). Il possède la boucle `Tick`, le registre d'ennemis, le spawner avec pooling, et émet des événements C# que l'UI écoute. `Time.timeScale = 0` pendant le choix de carte et la pause : les animations d'UI utilisent le temps non mis à l'échelle.
- **Bus d'événements** typé (`RunEnded`, `EnemyKilled`, `CurrencyEarned`…) pour les quêtes et l'analytics, sans couplage au gameplay.
- **UI** : un presenter par écran, qui observe `Wallet` et les services ; aucune logique métier dans les MonoBehaviours d'UI. Textes via Localization. File de popups à priorités (quotidien, offres, avis), un seul popup à la fois.
- **Grands nombres** : séparateur de milliers jusqu'à 9 999, puis notation abrégée localisée (12,3 K, 1,2 M).
- **Tests EditMode** dès le début : Wallet, EconomyCurves, StatBlock, régénération d'énergie, Light Cache hors ligne, tirage de cartes (poids, plafonds, repli), jour de jeu et série.

---

## 6. Conformité et données

- **Consentement** : formulaire Google UMP (EEE, Royaume-Uni) avant toute demande de pub. Le même consentement pilote Firebase Analytics (Consent Mode) et la collecte Crashlytics. Point d'entrée « Privacy options » dans les réglages.
- **Données du joueur** : option dans les réglages pour effacer les données locales et réinitialiser l'identifiant analytics.
- **Monnaie virtuelle** : vérifier avant le soft launch les règles européennes en vigueur sur l'affichage des prix en monnaie virtuelle (équivalent en argent réel, pas d'achat forcé de quantités inutiles).
- **Public** : non destiné aux enfants ; questionnaire de public cible rempli en cohérence avec la direction artistique et la fiche store.
- **Play Console** : politique de confidentialité en ligne, formulaire Data safety cohérent avec les SDK réellement intégrés, questionnaire de classification, déclaration de publicité.

---

## 7. Plan de réalisation

| Jalon | Contenu | Critère de fin |
|---|---|---|
| M0 - Setup | Projet Unity 6.3 LTS 2D URP, paquets, asmdef, Git, scènes Boot/Hub/Run, Wallet, StatBlock étendu, courbes, temps, sauvegarde, services Fake | Écran vide qui tourne sur un vrai téléphone, tests EditMode verts |
| M1 - Prototype | Faisceau, Drifters, spawner, Integrity, fin de run, HUD, surcharges Remote Config, formatage des nombres, fiches de design de M2 | Une run de 3 min jouable à 60 fps sur l'appareil de référence ; test de plaisir de jeu |
| M2 - Run complète | Sparks, niveaux de run, cartes et effets, Lurker, boss Tidecrab, Flare, menu pause, interruptions, effets et sons | Run complète de bout en bout, rejouable |
| M3 - Méta et économie | Pistes du phare, bâtiments et îles 1-2, Light Cache, énergie et écran « énergie vide », écran pré-run, quotidien et série, hub, simulateur d'économie | Boucle complète sur deux jours de test, tests EditMode verts |
| M4 - Services | UMP et Consent Mode, AdMob (récompensées, interstitiels), IAP et starter pack, Firebase (Analytics, Remote Config, Crashlytics), notifications locales, réglages et effacement des données | Pubs et achats de test fonctionnels, événements visibles dans la console |
| M5 - Vertical slice | FTUE, îles 1-3 et leurs boss, Keepers, quêtes, journal, localisation EN, niveaux de qualité, polish, optimisation, accessibilité | Test fermé Google Play avec de vrais joueurs |
| M6 - Soft launch | Sauvegarde cloud (avant l'ouverture des achats), médiation, îles 4-6, pass de saison, quelques pays, analyse de rétention et d'économie, réglages via Remote Config | Rétention J1 et J7 proches des cibles, sinon itérer avant tout lancement mondial |

### Publication Google Play (à vérifier au moment de publier)

- Compte développeur Play Console (frais uniques). Les comptes personnels récents peuvent devoir passer par un test fermé avec un nombre minimum de testeurs sur une durée minimale : vérifier les règles en vigueur.
- Play App Signing, clé d'upload conservée hors du dépôt.
- Consentement UMP pour l'EEE et le Royaume-Uni avant toute demande de pub.
- Fiche en anglais : icône, feature graphic, captures, courte vidéo ; vérifier l'unicité du nom.

---

## 8. Questions ouvertes

- Nom final du jeu et vérification des homonymes. [A TRANCHER]
- Fin de l'histoire (rallumer ou laisser dormir) et nombre d'îles au lancement. [A TRANCHER]
- Budget d'acquisition d'utilisateurs, ou lancement organique seul. [A TRANCHER]
- iOS : portage après validation sur Android ? [A TRANCHER]
- Langues après l'anglais (français en premier ?). [A TRANCHER]
- Production artistique au-delà de M2 (freelance, packs, ou poursuite du SVG/procédural). [A TRANCHER après M2]
