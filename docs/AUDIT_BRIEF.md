# Audit du brief et des tickets

Audit de `docs/Le_Dernier_Phare_Brief.pdf` (12 pages) croisé avec les 166 issues GitHub (#1 à #166), au 7 octobre 2026.

## Comment l'utiliser

Chaque point porte un identifiant, l'impact, le premier jalon concerné et une **recommandation**. Réponds par identifiant (« C1 : OK reco », « E3 : plutôt X ») ; les décisions seront reportées dans le brief texte (`docs/BRIEF.md`, étape 2) puis dans les tickets (étapes 3 et 4).

**Statut : tous les points sont tranchés (7 octobre 2026).** « Décidé » indique un choix explicite du développeur ; sinon, la recommandation a été acceptée telle quelle. Les 14 tickets proposés (§ 9) sont acceptés.

## Synthèse

- **Graphe de tickets sain** : aucune dépendance vers un code inexistant, aucun cycle, aucune dépendance vers un jalon plus tardif, numérotation continue dans chaque pôle.
- **Couverture large** : le découpage couvre l'essentiel du brief, et ajoute des sujets que le brief ne traite pas (réglages, pause en arrière-plan, menu de debug, file de popups, mise à jour forcée, avis in-app).
- **Faiblesses principales** :
  1. Le brief laisse non chiffrées une bonne partie des valeurs nécessaires à l'implémentation : effets des pistes méta, Sparks par ennemi, passifs des Keepers, bâtiments, boss.
  2. Le modèle `StatModifier` ne peut pas exprimer la moitié des cartes.
  3. Plusieurs tickets prennent des décisions absentes du brief, sans les signaler.
  4. Les tickets sont très courts et leurs références au brief sont génériques.

---

## 1. Vérifications techniques

| Id | Constat | Recommandation | Jalon |
|---|---|---|---|
| ✅ V1 | Le brief exige **Unity 6 LTS**. Le projet avait été créé en 6000.6.4f1 (6.6, version *Supported Update*, non LTS). | **Décidé** : migration vers **6000.3.25f1 (Unity 6.3 LTS)**, faite avant OPS-002. | M0 |
| ✅ V2 | Le brief fixe minSdk 24. Les SDK prévus (AdMob, Firebase, Play Billing) ont relevé leur minimum au fil des versions, et la version de Unity impose aussi le sien. | Dans OPS-002/OPS-003, retenir le minSdk le plus haut parmi Unity et les SDK épinglés, et le noter dans CLAUDE.md. | M0 |
| ✅ V3 | La « source de temps de confiance » (brief § 4.4, SVC-015) suppose un temps serveur Firebase. Aucun des SDK Firebase listés (Analytics, Remote Config, Crashlytics, Messaging) n'en fournit simplement. | Choisir la source : en-tête HTTP `Date` d'un appel déjà fait (Remote Config), Realtime Database (`.info/serverTimeOffset`, un SDK de plus), ou pas de temps serveur (détection locale du retour d'horloge seulement). Reco : en-tête `Date`. | M6 |
| ✅ V4 | Le brief liste Firebase **Cloud Messaging** (push serveur), mais les notifications des tickets sont **locales** (SVC-012). Celles-ci exigent `com.unity.mobile.notifications`, absent de la liste des paquets. | Remplacer FCM par Mobile Notifications (locales). Ajouter FCM seulement si des push serveur (live-ops) sont voulus. | M4 |

## 2. Incohérences internes du brief

| Id | Constat | Recommandation | Jalon |
|---|---|---|---|
| ✅ C1 | `Stat` (7 valeurs) n'exprimait ni Ember Oil, Fog Shield, Tide Pulse, Chain Light, Prism, Gull Squadron, Mend, ni les pistes Lamp et Keeper's Log, ni les valeurs de base (relâché 40 %, balayage, Flare, Shadowed). | **Décidé** : `Stat` étendu à toute valeur chiffrée (`BurnDps`, `SlowRatio`, `ChainJumps`, `BeamCount`, `LightGain`, `RarityLuck`, `FlareDamage`, `FlareRadius`, `IdleDamageRatio`…) ; ce qui n'est pas une valeur (soin ponctuel, pulse périodique, alliés) devient un `UpgradeEffect` (ScriptableObject polymorphe). Une carte combine modificateurs et effets. | M0 |
| ✅ C2 | Les `%` en `Mul` se composaient (Wide Lens ×4 = +75 %). | **Décidé** : additif par stat. `valeur = (base + somme des plats) × (1 + somme des %)` ; Wide Lens ×4 = +60 %, Focused Lens s'additionne (−20 %). Remplace la formule du brief § 5.3. | M0 |
| ✅ C3 | La frappe du Lurker « après 1,5 s non éclairé » s'appliquait dès l'apparition. | **Décidé** : frappe à portée. Le Lurker avance (invisible, Shadowed) ; dans le rayon de frappe autour du phare, 1,5 s sans être éclairé → frappe (12) puis disparaît ; éclairé, le minuteur repart à zéro. Rayon à fixer dans la fiche ennemi. | M2 |
| ✅ C4 | « Fragments » désignait la monnaie et les fragments d'histoire ; « Keeper's Log » entrait en collision avec les journaux de bord. | **Décidé** : monnaie = **Fragments** ; fragments d'histoire = **Logbook entries** (journal) ; piste de chance = **Keeper's Luck**. Glossaire, `PlayerSave` et tickets à aligner. | M2 |
| ✅ C5 | Mend plafonné à 1 alors qu'il sert de choix ponctuel et de repli. | **Décidé** : Mend sans plafond, proposé seulement si l'Integrity n'est pas pleine (sinon repli « +Light »). | M2 |
| ✅ C6 | Revive par pub, mais pas de pub pendant les 3 premières runs. | **Décidé** : revive **gratuit** (une fois par run) pendant les 3 premières runs, puis par pub récompensée. | M2 |
| ✅ C7 | Sources de Fragments : « Premières victoires de boss, Elites, quêtes ». Ailleurs, les quêtes donnent des Pearls (10 par jour), et META-009 dit Pearls. | Préciser les récompenses de quêtes : Pearls seulement, ou Pearls et Fragments. | M5 |
| ✅ C8 | Le pass de saison est décrit comme « abonnement saisonnier (achat unique par saison) ». Sur Google Play, un abonnement est un type de produit distinct (renouvellement automatique). | Un produit **non consommable par saison** (`season_pass_s1`, `season_pass_s2`…) ou un consommable avec droit enregistré. Reco : non consommable par saison. Corriger le terme dans le brief. | M6 |
| ✅ C9 | « Pas de probabilités cachées » (éthique), mais aucun écran n'affiche les poids de rareté (60/30/10) ni l'effet de Keeper's Log. Et l'assistance après 3 défaites est volontairement « cachée ». | Afficher les chances de rareté dans le choix de cartes (icône « i »). Garder l'assistance discrète : elle est favorable au joueur. | M5 |

## 3. Gameplay : valeurs et règles manquantes

| Id | Manque | Recommandation | Jalon |
|---|---|---|---|
| ✅ G1 | Sparks par ennemi non définis, pas de cible de niveaux par run. | **Décidé** : cible de 8 à 10 choix de cartes par run victorieuse. Sparks pondérés par menace : Shoal 1, Drifter 2, Stormling 3, Lurker 4, Siren 4, Tentacle 8, Elite ×3 ; calibrage final par le simulateur. | M2 |
| ✅ G2 | Mode de tirage et repli non définis. | **Décidé** : par emplacement, tirer la rareté (60/30/10, modifiée par Keeper's Log), puis une carte uniforme parmi les éligibles de cette rareté, sans doublon dans l'offre. Rareté vide → rareté inférieure. Moins de 3 éligibles → Mend, puis une carte « +Light ». | M2 |
| ✅ G3 | Dégâts et rayon du Flare non chiffrés. | **Décidé** : 60 dégâts, rayon 4 (tue un Drifter jusqu'à la vague 7), révélation des Lurkers dans le rayon pendant 2 s. Valeurs en `Stat` (`FlareDamage`, `FlareRadius`). | M2 |
| ✅ G4 | Cartes non chiffrées : Chain Light (cible hors cône ? ratio de dégâts ? sens de « max 2 »), Fog Shield (rayon), Tide Pulse (force, rayon), Gull Squadron (nombre, dégâts, vitesse), Prism (orientation : GAME-017 a choisi « symétrique », le brief ne dit rien). | Fiche par carte avec toutes les valeurs, dans le brief texte. | M2 |
| ✅ G5 | Siren : la dérive de 30°/s se cumule-t-elle avec plusieurs Sirens ? Dans quel sens tourne-t-elle par rapport au balayage au relâché ? | Pas de cumul (une dérive au plus), sens opposé au balayage. | M5 |
| ✅ G6 | **Boss** : seuls les noms existent. Aucune attaque, aucun pattern par phase, aucune durée cible. | Fiche de design par boss (attaques, télégraphie, phases, durée cible) avant GAME-014, puis avant chaque ticket de boss. | M2 |
| ✅ G7 | **Elite** : taux d'apparition et contenu du coffre non chiffrés. | Elites définies dans les données de vague (pas aléatoires), coffre = N Fragments fixes. | M5 |
| ✅ G8 | Prise en compte du boss et abandon non définis. | **Décidé** : victoire = `wavesCleared` 9 (le boss compte comme une vague, plus les Fragments de première victoire). Abandon = défaite à la vague en cours (gains partiels, pas de revive proposé). | M1 |
| ✅ G9 | Place de la run tutoriel dans la progression non définie. | **Décidé** : tutoriel de 90 s = 3 vagues courtes sans boss, guidées par Wick, **sans** valeur de victoire. Premier Logbook entry et demande de notifications après la première vraie victoire (Tidecrab). Les 3 runs gratuites commencent après le tutoriel. | M5 |
| ✅ G10 | Un ennemi qui atteint le phare disparaît : donne-t-il des Sparks ? | Non (pas de récompense sans kill). | M1 |

## 4. Économie et méta-progression

| Id | Constat | Recommandation | Jalon |
|---|---|---|---|
| ✅ E1 | Niveau max 50 avec croissance 1,32 : 267 M de Light par piste, inatteignable. | **Décidé** : plafond par île. Le niveau max d'une piste monte avec la progression (ordre de grandeur : 10 sur l'île 1, 50 sur l'île 6) avec une croissance plus douce ; valeurs calées par le simulateur (ECO-004). | M3 |
| ✅ E2 | **Effets des pistes** : seul Lamp est chiffré (+6 % par niveau). Lens, Hull, Gears et Keeper's Log n'ont aucune valeur par niveau. | Fixer les valeurs (ex. Lens +5 % de dégâts, Hull +8 Integrity, Gears +4 % de rotation, Keeper's Log +0,5 point de chance Rare/Epic). | M3 |
| ✅ E3 | **Îles et bâtiments** : les 4 bâtiments sont-ils communs à tout l'archipel ou propres à chaque île ? Les conditions de déblocage de l'île suivante ne sont pas fixées (boss vaincu ? Fragments ? bâtiment restauré ?). Nombre de niveaux et production par niveau des bâtiments : non définis. | Un jeu de bâtiments **par île** (4 sur l'île 1, nouveaux ensuite), déblocage = boss vaincu + 1 bâtiment restauré. Production et niveaux à simuler. | M3 |
| ✅ E4 | **Keepers** : 6 passifs non définis, prix en Fragments non définis. | Fiche par Keeper (passif chiffré, prix, île de déblocage). | M5 |
| ✅ E5 | Minuit UTC choisi par ECO-003/META-009 sans base dans le brief. | **Décidé** : minuit **local** ; un changement de fuseau ne peut pas ramener le délai sous 20 h depuis la dernière réclamation. Jour pardonné : semaine glissante de 7 jours. | M3 |
| ✅ E6 | « Éclat cosmétique » sans système associé. | **Décidé** : le jour 7 donne un cosmétique précis (un par cycle), puis des Pearls quand la série est épuisée. | M3 |
| ✅ E7 | 10 clés Remote Config seulement, alors que tout doit être réglable. | **Décidé** : les ScriptableObjects portent les défauts ; une clé JSON par famille (`enemy_overrides`, `upgrade_overrides`, `economy_overrides`…) surcharge n'importe quel champ par `Id`. Les 10 clés du brief restent des clés simples. | M1 |
| ✅ E8 | Seuls les multiplicateurs de **PV** d'île sont dans Remote Config. Les multiplicateurs de **gain** (1 ; 1,5 ; 2,2…) n'y sont pas. | Inclus dans E7. | M4 |
| ✅ E9 | Pass de saison : XP par palier, contenu des 30 paliers, sources d'XP : non définis. | Fiche de saison avant META-010. | M6 |

## 5. Monétisation, conformité, données

| Id | Constat | Recommandation | Jalon |
|---|---|---|---|
| ✅ M1 | **Consentement analytics** : UMP est prévu pour les pubs, mais Firebase Analytics dans l'EEE doit aussi suivre le consentement (Consent Mode). Aucun ticket ne le couvre. | Nouveau ticket SVC : brancher UMP sur Firebase (Consent Mode) et Crashlytics. | M4 |
| ✅ M2 | Pearls achetées perdues au changement de téléphone tant que la sauvegarde cloud n'existe pas. | **Décidé** : SVC-013 (Play Games, sauvegarde cloud) passe en **P1** et doit être livré avant l'ouverture des achats au soft launch. | M6 |
| ✅ M3 | **Monnaies virtuelles (UE)** : les autorités de consommateurs européennes attendent que le prix d'un contenu payé en monnaie virtuelle soit lisible en argent réel, sans obliger à acheter plus que nécessaire. Le brief dit seulement « prix affichés en clair ». | Vérifier les règles en vigueur avant le soft launch (REL-002/REL-007). Packs de Pearls cohérents avec les prix des cosmétiques (pas de reste inutilisable systématique). | M6 |
| ✅ M4 | **Coffres aléatoires** : coffres d'Elite et quotidien. Si un coffre aléatoire devenait achetable, les règles sur les loot boxes (Belgique notamment) s'appliqueraient. | Ajouter au brief, section éthique : aucun contenu aléatoire n'est vendu contre de l'argent ou des Pearls. | M4 |
| ✅ M5 | **Public** : « non ciblé enfants », mais une mascotte oiseau mignonne peut faire juger le jeu « attrayant pour les enfants » par Google (politique Familles). | Direction artistique et fiche store sans codes enfantins marqués, questionnaire de public rempli avec soin (REL-003). | M6 |
| ✅ M6 | **Suppression des données** : aucune option « effacer mes données » (analytics, sauvegarde). | L'ajouter aux réglages (UI-014) : réinitialisation locale et identifiant analytics réinitialisé. | M4 |

## 6. Technique

| Id | Constat | Recommandation | Jalon |
|---|---|---|---|
| ✅ T1 | Mort du processus pendant une run non traitée. | **Décidé** : run perdue ; Lamp Oil remboursé si la run n'avait pas dépassé la vague 2 (marqueur « run en cours » dans la sauvegarde). | M2 |
| ✅ T2 | **Grands nombres** : un jeu idle affiche vite des valeurs comme 82 687 ou 1,2 M. Aucun ticket ne définit le formatage (K/M, localisation). | Ajouter un formateur partagé à UI-001. | M1 |
| ✅ T3 | **Niveaux de qualité graphique** : ART-015 parle d'un réglage de qualité, mais aucun ticket ne définit les niveaux (bas, moyen, haut) ni leur détection automatique. | Nouveau ticket QA/OPS : niveaux URP et choix automatique selon l'appareil. | M5 |
| ✅ T4 | **Appareil « milieu de gamme »** non défini pour la cible 60 fps. | Fixer un appareil de référence (ex. modèle Android de 2022, 4 Go de RAM) dans QA-003. | M1 |
| ✅ T5 | **Taille de l'app** : aucun budget. | Budget AAB (ex. < 150 Mo) dans REL-007. | M6 |
| ✅ T6 | Les définitions ScriptableObject exigent `UnityEngine` (interdit dans Core) ; Meta et Economy ne pouvaient pas les lire sans dépendre de Gameplay ; `EnemyDefinition` référençait `EnemyView` (Gameplay). Constaté en préparant la matrice de couverture. | **Décidé** : nouvelle assembly **`Game.Data`** (définitions, `UpgradeEffect`, `IRunContext`, `GameCatalog`), dépendant de Core ; Economy, Meta, Gameplay et UI en dépendent ; champs prefab typés `GameObject`. | M0 |

## 7. Décisions prises dans les tickets, absentes du brief (à confirmer)

| Id | Ticket | Décision | Recommandation |
|---|---|---|---|
| ✅ D1 | ART-001 à ART-029 (tous) | Art produit en SVG ou procédural par le pipeline, remplaçable plus tard par un artiste. | **Décidé** : approche conservée ; noms, tailles et pivots fixes ; freelance ou packs réévalués après M2. Tranche la question [A TRANCHER] « production artistique » pour M0 à M2. |
| ✅ D2 | ART-001 | Palette précise, 64 pixels par unité, tailles de sprites. | Confirmer ou laisser ART-001 décider. |
| ✅ D3 | ECO-003, META-009 | Remise à zéro à minuit UTC. | Voir E5. |
| ✅ D4 | GAME-017 | Prism orienté symétriquement (à 180°). | Voir G4. |
| ✅ D5 | OPS-007 | `versionCode` dérivé du nombre de commits. | OK, ou versionCode manuel. |
| ✅ D6 | OPS-008 | CI GameCI avec licence Unity en secrets. | **Décidé** : pas de CI pour l'instant ; tests lancés en local (`/unity-tests`) avant chaque PR. OPS-008 passe en P2 ; OPS-009 et OPS-011, qui en dépendent, suivent. |
| ✅ D7 | UI-019, SVC-014, UI-018, OPS-012 | Avis in-app, mise à jour forcée, file de popups, menu de debug. | Bons ajouts, à reporter dans le brief. |
| ✅ D8 | NARR-001 | Environ 30 fragments d'histoire, au moins 4 par île. | Confirmer. |

## 8. Qualité des tickets

| Id | Constat | Recommandation |
|---|---|---|
| ✅ Q1 | Contextes d'une ou deux phrases, 2 ou 3 critères. Les choix de conception restent implicites pour les tickets de gameplay et d'économie. | Enrichir M0 à M2 à l'étape 4 (valeurs, cas limites, tests attendus), après les décisions de cet audit. |
| ✅ Q2 | Références au brief **génériques par pôle** : tous les SVC citent « 4.7 à 4.10 et 5.6 », tous les GAME « 3 », OPS-001 « 6 et 5.1 ». | Citer la sous-section exacte (ex. GAME-012 → § 3.3 et 3.4). |
| ✅ Q3 | Les critères sont rarement vérifiables tels quels (« Lisibles », « Style cohérent », « Performants »). | Critères mesurables : valeur attendue, test vert, budget chiffré. |
| ✅ Q4 | Des tickets M0 n'ont pas de critère de test alors que le brief exige des tests EditMode « dès le début » (CORE-004, CORE-005). | Ajouter le critère « tests EditMode verts » à toute logique pure. |

## 9. Tickets manquants (proposés)

| Proposition | Pôle | Jalon | Lien |
|---|---|---|---|
| Extension de `Stat` et effets d'amélioration non-stat (`UpgradeEffect`) | CORE | M0 | C1 |
| Surcharge générique des définitions par Remote Config | CORE/SVC | M1 | E7 |
| Fiches de design : cartes, boss île 1, Flare, Sparks (valeurs de M2) | GAME | M1 | G1 à G6 |
| Fiches de design méta : pistes, bâtiments par île, déblocage d'îles, Keepers | META | M3 | E2 à E4 |
| Consentement analytics (Consent Mode, Crashlytics) | SVC | M4 | M1 |
| Écran « énergie vide » : attendre, pub `extra_oil`, 20 Pearls | UI | M3 | aucun ticket ne couvre ce flux |
| Écran pré-run : choix de l'île, Keeper équipé, coût en Lamp Oil | UI | M3 | le hub (UI-006) a seulement un bouton de lancement |
| Menu pause en run : reprendre, réglages, abandonner | UI | M2 | G8 ; le HUD (UI-002) a un bouton pause, aucun écran |
| Offre starter pack : déclencheur, affichage unique | ECO/UI | M4 | brief § 4.8 « proposé après la première session » |
| Mort du processus pendant une run (remboursement de Lamp Oil) | GAME | M2 | T1 |
| Formatage des grands nombres | UI | M1 | T2 |
| Niveaux de qualité graphique et détection d'appareil | QA | M5 | T3 |
| Affichage des chances de rareté | UI | M5 | C9 |
| Effacement des données joueur | UI/SVC | M4 | M6 |

## 10. Rappel : points [A TRANCHER] du brief

Nom final ; production artistique (voir D1) ; fin de l'histoire et nombre d'îles au lancement ; budget d'acquisition ; portage iOS ; langues après l'anglais. Seule la production artistique conditionne un jalon proche (M0, ART-001).
