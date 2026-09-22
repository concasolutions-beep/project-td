using UnityEngine;
using TMPro;

namespace ProjectTD.Core
{
    public class LifeCounterUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text livesText;
        private int startLife;


        void OnEnable()
        {
            if (GameManager.Instance == null)
            {
                return;
            }

            GameManager.Instance.OnLivesChanged += HandleLivesChanged;

        }

        void Start()
        {
            if (GameManager.Instance == null)
            {
                return;
            }
            startLife = GameManager.Instance.Lives;
            UpdateText(GameManager.Instance.Lives);
        }

        void OnDisable()
        {
            if (GameManager.Instance == null)
            {
                return;
            }

            GameManager.Instance.OnLivesChanged -= HandleLivesChanged;
        }

        private void HandleLivesChanged(int lives)
        {
            UpdateText(lives);
        }

        private void UpdateText(int lives)
        {
            if (livesText == null)
            {
                return;
            }

            livesText.text = $"Vita: {lives}/{startLife}";
        }
    }
}
