# The Last Lighthouse (Le Dernier Phare)

Jeu mobile Android (Unity 6, 2D URP, C#) : roguelite court (runs de 2 à 4 min) avec méta-progression idle autour d'un archipel à restaurer. Le joueur, dernier gardien, oriente d'un doigt le faisceau d'un phare pour repousser les créatures de la mer, puis rallume l'archipel îlot par îlot.

- **Source de vérité** : [docs/BRIEF.md](docs/BRIEF.md) (brief de conception et d'architecture ; `docs/Le_Dernier_Phare_Brief.pdf` en est la version d'origine archivée)
- **Guide de travail (humain et Claude Code)** : [CLAUDE.md](CLAUDE.md)
- **Suivi** : GitHub Issues et le projet « The Last Lighthouse » (jalons M0 à M6)

## Prérequis

- Unity **6000.3.25f1** (Unity 6.3 LTS) avec le module Android Build Support (SDK, NDK, OpenJDK)
- JetBrains Rider (ou Visual Studio)
- Git et **Git LFS** (`brew install git-lfs && git lfs install`) avant le premier clone

```bash
git clone https://github.com/Larmik/The-Last-Lighthouse.git
cd The-Last-Lighthouse
git lfs pull
```

Fusion des scènes et prefabs (optionnel, recommandé) avec UnityYAMLMerge :

```bash
git config merge.unityyamlmerge.name "Unity SmartMerge"
git config merge.unityyamlmerge.driver "'/Applications/Unity/Hub/Editor/6000.3.25f1/Unity.app/Contents/Tools/UnityYAMLMerge' merge -p %O %B %A %A"
```

## Structure du dépôt

```
Assets/
  _Project/
    Art/ Audio/ Prefabs/
    Scenes/        Boot, Hub, Run
    Data/          ScriptableObjects : Enemies, Upgrades, Waves, Islands, Keepers
    Scripts/
      Core/        Game.Core        C# pur, sans UnityEngine
      Data/        Game.Data        définitions ScriptableObject, UpgradeEffect, GameCatalog
      Economy/     Game.Economy     portefeuille, courbes, énergie, récompenses
      Meta/        Game.Meta        progression, îles, bâtiments, keepers
      Gameplay/    Game.Gameplay    faisceau, ennemis, vagues, RunDirector
      Services/    Game.Services    pubs, IAP, analytics, sauvegarde (+ Fakes éditeur)
      UI/          Game.UI
      Bootstrap/   Game.Bootstrap   composition root VContainer
    Tests/
      EditMode/    Game.Tests.EditMode
      PlayMode/    Game.Tests.PlayMode
Packages/          manifest des paquets Unity
ProjectSettings/   réglages du projet (versionnés)
docs/              brief et documentation
```

L'arborescence `_Project` et les assemblies sont mises en place par le ticket OPS-004.

## Fichiers versionnés

- Sérialisation **Force Text** et **Visible Meta Files** : chaque fichier `.meta` est versionné avec son asset.
- Git LFS pour les sources lourdes : images (`png`, `jpg`, `psd`, `tga`…), audio (`wav`, `ogg`, `mp3`…), vidéo, polices, modèles (voir `.gitattributes`).
- `Library/`, `Temp/`, `Logs/`, `UserSettings/`, builds et fichiers de solution générés sont ignorés.

## Flux de travail

Un ticket = une branche `feat/<CODE>` (ex. `feat/OPS-001`) = une PR vers `main`. Les commits sont préfixés par le code du ticket (`OPS-001: ...`).
