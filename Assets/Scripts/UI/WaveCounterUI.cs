using UnityEngine;
using TMPro;

public class WaveCounterUI : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private TMP_Text waveText;

    void OnEnable()
    {
        if (waveManager == null)
        {
            return;
        }

        waveManager.OnWaveStarted += HandleWaveStarted;
        UpdateText(waveManager.CurrentWaveIndex);
    }

    void OnDisable()
    {
        if (waveManager == null)
        {
            return;
        }

        waveManager.OnWaveStarted -= HandleWaveStarted;
    }

    private void HandleWaveStarted(int waveIndex)
    {
        UpdateText(waveIndex);
    }

    private void UpdateText(int waveIndex)
    {
        if (waveText == null || waveManager == null)
        {
            return;
        }

        int current = Mathf.Max(0, waveIndex) + 1;
        waveText.text = $"Ondate {current} / {waveManager.TotalWaves}";
    }
}
