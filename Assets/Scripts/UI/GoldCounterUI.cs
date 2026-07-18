using UnityEngine;
using TMPro;

public class GoldCounterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;

    void OnEnable()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.OnGoldChanged += HandleGoldChanged;
    }

    void Start()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        UpdateText(GameManager.Instance.Gold);
    }

    void OnDisable()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.OnGoldChanged -= HandleGoldChanged;
    }

    private void HandleGoldChanged(int gold)
    {
        UpdateText(gold);
    }

    private void UpdateText(int gold)
    {
        if (goldText == null)
        {
            return;
        }

        goldText.text = $"Gold: {gold}";
    }
}
