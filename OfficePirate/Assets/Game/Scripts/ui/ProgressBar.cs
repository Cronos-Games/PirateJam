using UnityEngine;

[ExecuteAlways]
public class ProgressBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform fillBar;

    [Header("Layout")]
    [Tooltip("Pixels of padding inside the parent bar.")]
    [SerializeField] private float leftMargin = 0f;
    [SerializeField] private float rightMargin = 0f;

    [Header("Debug")]
    [Range(0f, 1f)]
    [SerializeField] private float progress = 0.5f;

    private RectTransform parentRect;
    private float lastParentWidth = -1f;
    private float lastLeftMargin = -1f;
    private float lastRightMargin = -1f;

    private void Awake()
    {
        parentRect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        Apply(progress);
    }

    // Called by Unity when this RectTransform (or its parents) changes dimensions
    private void OnRectTransformDimensionsChange()
    {
        Apply(progress);
    }

    private void Update()
    {
        Apply(progress);
    }

    public void SetProgress(float value01)
    {
        progress = Mathf.Clamp01(value01);
        Apply(progress);
    }

    private void Apply(float value01)
    {
        if (!fillBar) return;

        if (!parentRect) parentRect = GetComponent<RectTransform>();

        float parentWidth = parentRect.rect.width;

        // Cache for change detection
        lastParentWidth = parentWidth;
        lastLeftMargin = leftMargin;
        lastRightMargin = rightMargin;

        float usableWidth = Mathf.Max(0f, parentWidth - leftMargin - rightMargin);
        float targetWidth = usableWidth * Mathf.Clamp01(value01);

        // Ensure fill grows from left to right
        // (Best if you also set anchors/pivot: AnchorMin (0,0), AnchorMax(0,1), Pivot(0,0.5))
        Vector2 size = fillBar.sizeDelta;
        size.x = targetWidth;
        fillBar.sizeDelta = size;

        // Position it inside the left margin
        Vector2 pos = fillBar.anchoredPosition;
        pos.x = leftMargin;
        fillBar.anchoredPosition = pos;
    }
}
