using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Tilemaps;
using ProjectTD.Core;
using ProjectTD.Pooling;

namespace ProjectTD.Tower
{
    public class GroundPlusButtonSpawner : MonoBehaviour
    {
        [SerializeField] private Tilemap ground;
        [SerializeField] private Tilemap path;
        [SerializeField] private Tilemap obstacles;
        [SerializeField] private GroundPlusButton buttonPrefab;
        [SerializeField] private GameObject towerPrefab;
        [SerializeField] private Transform container;

        void Start()
        {
            EnsureEventSystem();
            SpawnButtons();
        }

        private void EnsureEventSystem()
        {
            if (EventSystem.current != null)
            {
                return;
            }

            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<InputSystemUIInputModule>();
        }

        private void SpawnButtons()
        {
            if (ground == null || path == null || buttonPrefab == null)
            {
                Utils.WarningLog("[GroundPlusButtonSpawner] Missing references, skipping spawn.");
                return;
            }

            Transform parent = container != null ? container : transform;
            BoundsInt bounds = ground.cellBounds;


            int spawned = 0;

            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                for (int x = bounds.xMin; x < bounds.xMax; x++)
                {
                    Vector3Int cell = new Vector3Int(x, y, 0);

                    bool blocked = path.HasTile(cell) || (obstacles != null && obstacles.HasTile(cell));

                    if (!ground.HasTile(cell) || blocked)
                    {
                        continue;
                    }

                    Vector3 worldPos = ground.GetCellCenterWorld(cell);

                    GameObject buttonGO = PoolManager.Instance.GetPool(buttonPrefab.gameObject).Get();
                    buttonGO.transform.SetParent(parent);
                    buttonGO.transform.position = worldPos;

                    GroundPlusButton instance = buttonGO.GetComponent<GroundPlusButton>();
                    instance.Initialize(cell, HandleCellClicked);
                    spawned++;

                }
            }

            Utils.DebugLog($"[GroundPlusButtonSpawner] Done. Spawned {spawned} buttons.");
        }

        private void HandleCellClicked(GroundPlusButton button)
        {
            if (towerPrefab == null)
            {
                Utils.WarningLog("[GroundPlusButtonSpawner] No towerPrefab assigned, skipping placement.");
                return;
            }

            TowerController towerController = towerPrefab.GetComponent<TowerController>();
            int cost = towerController != null && towerController.data != null ? towerController.data.cost : 0;

            if (GameManager.Instance != null && !GameManager.Instance.TrySpendGold(cost))
            {
                Utils.DebugLog("[GroundPlusButtonSpawner] Gold insufficiente per piazzare la torre.");
                return;
            }

            GameObject towerGO = PoolManager.Instance.GetPool(towerPrefab).Get();
            towerGO.transform.SetPositionAndRotation(ground.GetCellCenterWorld(button.Cell), Quaternion.identity);
            button.Dismiss();

            Utils.DebugLog($"[GroundPlusButtonSpawner] Tower placed on cell {button.Cell}.");
        }
    }
}
