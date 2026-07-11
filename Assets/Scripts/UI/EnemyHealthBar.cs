using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HealthController health;
    [SerializeField] private Image fillImage;

    [Header("Placement")]
    [SerializeField] private bool followTarget = true;
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.2f, 0f);

    [Header("Animation")]
    [SerializeField] private float smoothDuration = 0.25f;

    private Transform target;
    private Camera mainCamera;
    private Coroutine animateRoutine;

    private float displayedValue = 1f;
    private float targetValue = 1f;

    void Reset()
    {
        health = GetComponentInParent<HealthController>();
        if (fillImage == null)
        {
            fillImage = GetComponentInChildren<Image>();
        }
    }

    void Awake()
    {
        if (health == null)
        {
            health = GetComponentInParent<HealthController>();
        }

        if (health != null)
        {
            target = health.transform;
        }
        else
        {
            target = transform.parent;
        }

        mainCamera = Camera.main;
    }

    void OnEnable()
    {
        if (health == null)
        {
            health = GetComponentInParent<HealthController>();
        }

        if (health == null)
        {
            return;
        }

        health.OnHealthChanged += HandleHealthChanged;
        health.OnDied += HandleDied;

        SyncImmediate();
    }

    void OnDisable()
    {
        Unsubscribe();
        StopAnimating();
    }

    void OnDestroy()
    {
        Unsubscribe();
    }

    void LateUpdate()
    {
        if (!followTarget || target == null)
        {
            return;
        }

        transform.position = target.position + worldOffset;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            transform.rotation = mainCamera.transform.rotation;
        }
    }

    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        if (maxHealth <= 0f)
        {
            SetTarget(0f);
            return;
        }

        SetTarget(currentHealth / maxHealth);
    }

    private void HandleDied()
    {
        SetTarget(0f);
        gameObject.SetActive(false);
    }

    private void SyncImmediate()
    {
        targetValue = health.NormalizedHealth;
        displayedValue = targetValue;
        UpdateVisual(displayedValue);
    }

    private void SetTarget(float value)
    {
        targetValue = Mathf.Clamp01(value);

        StopAnimating();
        animateRoutine = StartCoroutine(AnimateToTarget());
    }

    private IEnumerator AnimateToTarget()
    {
        float start = displayedValue;
        float end = targetValue;
        float elapsed = 0f;

        if (smoothDuration <= 0f)
        {
            displayedValue = end;
            UpdateVisual(displayedValue);
            animateRoutine = null;
            yield break;
        }

        while (elapsed < smoothDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / smoothDuration);
            displayedValue = Mathf.Lerp(start, end, t);
            UpdateVisual(displayedValue);
            yield return null;
        }

        displayedValue = end;
        UpdateVisual(displayedValue);
        animateRoutine = null;
    }

    private void StopAnimating()
    {
        if (animateRoutine != null)
        {
            StopCoroutine(animateRoutine);
            animateRoutine = null;
        }
    }

    private void UpdateVisual(float value)
    {
        if (fillImage == null)
        {
            return;
        }

        fillImage.fillAmount = Mathf.Clamp01(value);
    }

    private void Unsubscribe()
    {
        if (health == null)
        {
            return;
        }

        health.OnHealthChanged -= HandleHealthChanged;
        health.OnDied -= HandleDied;
    }
}
