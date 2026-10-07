---
paths:
  - "Assets/_Project/Tests/**"
  - "Assets/_Project/Scripts/Core/**"
  - "Assets/_Project/Scripts/Economy/**"
  - "Assets/_Project/Scripts/Meta/**"
---

# Tests

- Logique pure (Core, Economy, Meta) : tests EditMode (Unity Test Framework, NUnit) livrés dans le
  même ticket que le code, quand le ticket les demande ou que la logique est un calcul du brief.
- Priorités du brief (§ 5.6) : Wallet, EconomyCurves, StatBlock, régénération d'énergie, Light Cache
  hors ligne, tirage de cartes (poids et plafonds).
- Valeurs attendues tirées du brief quand il les donne (ex. coûts de piste 80, 106, 139… ; XP
  `round(8 × 1,25^n)`), avec tolérance explicite pour les flottants.
- Le temps est injecté (`ITimeProvider` factice) : aucun test ne dépend de l'horloge réelle.
- Classe de test `<TypeTesté>Tests`, méthode `Method_Condition_Expected`.
- Tests PlayMode réservés à ce qui exige le moteur (scènes, pooling, boucle de run).
- Un ticket n'est pas terminé si les tests EditMode sont rouges.
