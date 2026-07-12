using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Tilemaps;

public class GroundPlusButtonSpawner : MonoBehaviour
{
    [SerializeField] private Tilemap ground;
    [SerializeField] private Tilemap path;
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

        Debug.Log("[GroundPlusButtonSpawner] No EventSystem found, created one.");
    }

    private void SpawnButtons()
    {
        if (ground == null || path == null || buttonPrefab == null)
        {
            Debug.LogWarning("[GroundPlusButtonSpawner] Missing references, skipping spawn.");
            return;
        }

        Transform parent = container != null ? container : transform;
        BoundsInt bounds = ground.cellBounds;

        Debug.Log($"[GroundPlusButtonSpawner] Scanning cellBounds {bounds} on '{ground.name}' (parent: '{parent.name}').");

        int spawned = 0;

        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);

                if (!ground.HasTile(cell) || path.HasTile(cell))
                {
                    continue;
                }

                Vector3 worldPos = ground.GetCellCenterWorld(cell);

                GroundPlusButton instance = Instantiate(buttonPrefab, parent);
                instance.transform.position = worldPos;
                instance.Initialize(cell, HandleCellClicked);
                spawned++;

                Debug.Log($"[GroundPlusButtonSpawner] Spawned button #{spawned} at cell {cell} -> world {worldPos}.");
            }
        }

        Debug.Log($"[GroundPlusButtonSpawner] Done. Spawned {spawned} buttons.");
    }

    private void HandleCellClicked(GroundPlusButton button)
    {
        if (towerPrefab != null)
        {
            Instantiate(towerPrefab, ground.GetCellCenterWorld(button.Cell), Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("[GroundPlusButtonSpawner] No towerPrefab assigned, skipping placement.");
        }

        Destroy(button.gameObject);

        Debug.Log($"[GroundPlusButtonSpawner] Tower placed on cell {button.Cell}.");
    }
}
