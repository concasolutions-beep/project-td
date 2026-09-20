using System;
using UnityEngine;
using UnityEngine.UI;

public class GroundPlusButton : PoolableEntity
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
    }

    public void Dismiss()
    {
        ReleaseToPool();
    }

    protected override void OnSpawned()
    {
        if (button != null)
        {
            button.onClick.AddListener(HandleClick);
        }
    }

    protected override void OnDespawned()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(HandleClick);
        }
        onClicked = null;
    }

    private void HandleClick()
    {
        onClicked?.Invoke(this);
    }
}
