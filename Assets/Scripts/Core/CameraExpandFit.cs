using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraExpandFit : MonoBehaviour
{
    [SerializeField] private float referenceOrthographicSize = 5f;
    [SerializeField] private float referenceAspect = 1.8f;
    [SerializeField, Range(0f, 0.5f)] private float uiTopReservedFraction = 0.101171f;

    private Camera targetCamera;
    private int lastScreenWidth;
    private int lastScreenHeight;

    void Awake()
    {
        targetCamera = GetComponent<Camera>();
        ApplyFit();
    }

    void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            ApplyFit();
        }
    }

    void ApplyFit()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        targetCamera.rect = new Rect(0f, 0f, 1f, 1f - uiTopReservedFraction);

        float screenAspect = (float)Screen.width / Screen.height;
        float visibleAspect = screenAspect / (1f - uiTopReservedFraction);

        targetCamera.orthographicSize = visibleAspect >= referenceAspect
            ? referenceOrthographicSize
            : referenceOrthographicSize * (referenceAspect / visibleAspect);

        Debug.Log($"[CameraExpandFit] screen={lastScreenWidth}x{lastScreenHeight} screenAspect={screenAspect:F3} visibleAspect={visibleAspect:F3} orthoSize={targetCamera.orthographicSize:F3} rect={targetCamera.rect}");
    }
}
