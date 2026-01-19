using UnityEngine;
using UnityEngine.Events;

public class ProgressThresholdListener : MonoBehaviour
{
    [Header("Threshold")]
    [SerializeField] private int targetPoints = 3000;

    [Header("Events")]
    public UnityEvent onAboveOrEqual;
    public UnityEvent onBelow;

    public int TargetPoints => Mathf.Max(0, targetPoints);
    public bool IsAbove { get; private set; }

    private void OnEnable()
    {
        if (ProgressManager.Instance != null)
            ProgressManager.Instance.RegisterListener(this);
    }

    private void OnDisable()
    {
        if (ProgressManager.Instance != null)
            ProgressManager.Instance.UnregisterListener(this);
    }

    // Called by ProgressManager
    public void SetState(bool isAbove, bool invokeEvent)
    {
        IsAbove = isAbove;

        if (!invokeEvent) return;

        if (IsAbove) onAboveOrEqual?.Invoke();
        else onBelow?.Invoke();
    }
}