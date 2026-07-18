using System;
using UnityEngine;
using UnityEngine.UI;

public class GroundPlusButton : MonoBehaviour
{
    [SerializeField] private Button button;

    public Vector3Int Cell { get; private set; }

    private Action<GroundPlusButton> onClicked;

    void Reset()
    {
        button = GetComponentInChildren<Button>();
    }

    public void Initialize(Vector3Int cell, Action<GroundPlusButton> onClicked)
    {
        Cell = cell;
        this.onClicked = onClicked;

        if (button != null)
        {
            button.onClick.AddListener(HandleClick);
        }
    }

    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(HandleClick);
        }
    }

    private void HandleClick()
    {
        onClicked?.Invoke(this);
    }
}
