# Assets/Scripts — struttura e convenzioni

## Struttura per feature

Gli script sono organizzati per **feature**, non per tipo. Ogni cartella raggruppa tutto ciò che serve a quella
feature (component, ScriptableObject di dati, UI dedicata), invece di separare `Controllers/`, `Data/`, `UI/` come
cartelle trasversali.

```
Assets/Scripts/
├── Enemy/         Nemici: movimento, spawner, dati, controller, health bar dedicata
├── Tower/         Torrette: piazzamento (GroundPlusButton), controller, dati
├── Projectile/    Proiettili: controller, dati
├── Wave/          Ondate: WaveData/WaveGroupData, WaveManager, contatore ondate UI
├── Health/         Sistema vita condiviso da Enemy/Tower: HealthController, barra vita generica
├── Economy/       UI economia (oro)
├── Pooling/       Infrastruttura di object pooling, trasversale a tutte le feature
├── Core/          Infrastruttura cross-cutting: GameManager, Singleton<T>, Utils, camera, pausa, UI vite
└── Editor/        Script Editor-only (assembly separato)
```

`Pooling/` e `Core/` **non sono feature**: sono infrastruttura riusabile da più feature. Regola per decidere dove
va un nuovo file:

- Se il file appartiene logicamente a una sola feature (es. un nuovo tipo di torretta) → cartella della feature.
- Se il file è generico, riusabile da più feature e non ha logica di dominio → `Core/`.
- Se il file riguarda il pooling degli oggetti → `Pooling/`.
- Se una feature cresce abbastanza da avere più di 4-5 file distinti per sotto-argomento, valuta una sottocartella
  (es. `Enemy/AI/`) invece di una nuova cartella di primo livello.

## Convenzioni di naming

- **Namespace = cartella**: ogni cartella feature ha il proprio namespace `ProjectTD.<Feature>` (es.
  `ProjectTD.Enemy`, `ProjectTD.Wave`). Le classi che referenziano tipi di un'altra feature usano `using
  ProjectTD.<Feature>;` esplicito — niente `using` globali o wildcard.
- **File = classe pubblica**: un file `.cs` contiene una sola classe/interfaccia/enum pubblica, con lo stesso nome
  del file (PascalCase). Tipi ausiliari privati o enum di supporto strettamente legati alla classe principale
  possono restare nello stesso file (es. `EnemyExitReason` in `EnemyController.cs`).
- **Suffissi coerenti**:
  - `*Controller` — `MonoBehaviour` con la logica principale di un'entità (`EnemyController`, `TowerController`).
  - `*Data` — `ScriptableObject` di configurazione/dati (`EnemyData`, `WaveData`).
  - `*Manager` — coordinatore/singleton di sistema (`GameManager`, `PoolManager`, `WaveManager`).
  - `*UI` — componenti di interfaccia utente (`GoldCounterUI`, `WaveCounterUI`).
  - `I*` — interfacce (`IPoolable`).
- **PascalCase** per classi, file, metodi pubblici e proprietà; `camelCase` per campi privati e parametri.
- **Test**: la struttura sotto `Assets/Tests/EditMode/` rispecchia quella di `Assets/Scripts/` per cartella
  (es. i test di `Enemy/EnemyMovement.cs` vanno in `Tests/EditMode/Enemy/EnemyMovementTests.cs`).

## Spostare o rinominare script

Ogni asset ha un file `.meta` con un GUID stabile: scene e prefab referenziano gli script tramite quel GUID, non
tramite il percorso. **Non spostare o rinominare i file `.cs` dal file system o da un editor esterno che non
gestisce i `.meta`**, altrimenti il collegamento si rompe (`Missing Script` in scena/prefab).

Modi sicuri per spostare/rinominare:

- Dall'Editor Unity (Project window): trascina o usa "Rename" — sposta `.cs` e `.meta` insieme mantenendo il GUID.
- Da un IDE con supporto Unity attivo (Rider, VS con Unity extension): la funzione Rename/Move Refactoring sposta
  anche il `.meta`.
- Da riga di comando/git: è sicuro **solo se** `.cs` e `.cs.meta` vengono spostati insieme nello stesso comando
  (es. `git mv`), con l'Editor chiuso o pronto a fare un reimport pulito. Dopo lo spostamento, verifica che `git
  status` mostri un `rename`, non un `delete` + `add` separati.

Dopo qualsiasi spostamento, apri l'Editor e controlla la Console: non devono comparire warning `Missing (Mono
Script)` né riferimenti rotti in scene o prefab.
