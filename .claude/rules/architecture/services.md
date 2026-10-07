---
paths:
  - "Assets/_Project/Scripts/Services/**"
  - "Assets/_Project/Scripts/Bootstrap/**"
---

# Services, injection et asynchrone

## Services

- Tout service externe (pubs, IAP, analytics, Remote Config, sauvegarde, temps, notifications) est
  une interface (`IAdService`, `IPurchaseService`…) avec une implémentation réelle **et** une
  implémentation `Fake` utilisable dans l'éditeur sans SDK.
- Le choix réel / Fake se fait au composition root (`#if UNITY_EDITOR` ou réglage), jamais dans le
  code appelant.
- Les noms de placements pub (`revive`, `double_rewards`…), produits IAP (`remove_ads`…), clés Remote
  Config et événements analytics sont en `snake_case` anglais, exactement ceux du brief (§ 4.7, 4.8,
  4.10), regroupés dans des constantes uniques (pas de littéral recopié).
- Toute lecture Remote Config passe par `IRemoteConfigService.Get(key, fallback)` avec la valeur du
  brief comme repli.
- Aucune demande de pub avant le consentement UMP ; aucune pub pendant l'action (brief § 4.7).
- Le consentement UMP pilote aussi Firebase Analytics (Consent Mode) et Crashlytics ; aucun
  événement analytics ne porte de donnée personnelle (brief § 6).
- Temps de confiance : en-tête HTTP `Date` d'une requête déjà faite, repli local hors ligne ; jamais
  d'appel réseau dédié bloquant le démarrage (brief § 4.4).

## Injection (VContainer)

- Dépendances par constructeur (classes C#) ou `[Inject]` sur méthode (MonoBehaviours) ; pas de
  singleton statique, pas de `FindObjectOfType`, pas de `ServiceLocator`.
- Enregistrement dans le `LifetimeScope` concerné (`GameLifetimeScope` pour les singletons de jeu) ;
  points d'entrée via `RegisterEntryPoint`.

## Asynchrone (UniTask)

- `UniTask` / `UniTask<T>` pour toute opération asynchrone ; pas de `Task`, pas de coroutine pour de
  la logique de service, pas de `async void`.
- Passer un `CancellationToken` lié au cycle de vie (`destroyCancellationToken` côté MonoBehaviour).
- Une opération asynchrone qui échoue (SDK absent, réseau) dégrade proprement : le jeu reste jouable
  hors ligne et sans pub.
