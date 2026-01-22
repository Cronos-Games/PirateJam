using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    [Header("Progress")] [SerializeField] private int goalPoints = 10000;
    [SerializeField] private int startingPoints = 0;

    [Header("UI")] [SerializeField] private ProgressBar progressBar;

    [Header("Events")] 
    public UnityEvent onGameOver;
    public UnityEvent<int, int> onProgressChanged; // (current, goal)

    public int GoalPoints => goalPoints;
    public int CurrentPoints => currentPoints;
    public bool IsGameOver => gameOver;

    private int currentPoints;
    private bool gameOver;

    private readonly List<ProgressThresholdListener> listeners = new();

#if UNITY_EDITOR
    [FoldoutGroup("Editor Debug")] [Min(0)]
    public int editorAddAmount = 100;

    private bool IsPlaying => UnityEngine.Application.isPlaying;

    [FoldoutGroup("Editor Debug")]
    [Button(ButtonSizes.Medium)]
    [EnableIf("@IsPlaying")]
    private void AddProgressEditor() => AddProgress(editorAddAmount);

    [FoldoutGroup("Editor Debug")]
    [Button(ButtonSizes.Medium)]
    [EnableIf("@IsPlaying")]
    private void RemoveProgressEditor() => RemoveProgress(editorAddAmount);


    [FoldoutGroup("Editor Debug")]
    [ShowInInspector, ReadOnly]
    [ProgressBar(0, "@goalPoints", DrawValueLabel = true)]
    [LabelText("Progress")]
    private int ProgressBarRO => currentPoints;
#endif


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        currentPoints = Mathf.Clamp(startingPoints, 0, Mathf.Max(0, goalPoints));
        UpdateUI();
        NotifyProgressChanged();
        EvaluateAllThresholds(forceNotify: true);
    }

    // -------- Public API --------

    public void AddProgress(int amount) => SetProgress(currentPoints + Mathf.Abs(amount));
    public void RemoveProgress(int amount) => SetProgress(currentPoints - Mathf.Abs(amount));

    public void SetProgress(int newValue)
    {
        if (gameOver) return;

        int clamped = Mathf.Clamp(newValue, 0, Mathf.Max(0, goalPoints));
        if (clamped == currentPoints) return;

        currentPoints = clamped;

        UpdateUI();
        NotifyProgressChanged();
        EvaluateAllThresholds(forceNotify: false);

        if (goalPoints > 0 && currentPoints >= goalPoints)
        {
            gameOver = true;
            onGameOver?.Invoke();
        }
    }

    public void RegisterListener(ProgressThresholdListener listener)
    {
        if (listener == null) return;
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
            // Make sure it gets the correct state immediately
            EvaluateOne(listener, forceNotify: true);
        }
    }

    public void UnregisterListener(ProgressThresholdListener listener)
    {
        if (listener == null) return;
        listeners.Remove(listener);
    }

    // -------- Internals --------

    private void UpdateUI()
    {
        if (!progressBar) return;

        float normalized = (goalPoints <= 0) ? 0f : (float)currentPoints / goalPoints;
        progressBar.SetProgress(normalized);
    }

    private void NotifyProgressChanged()
    {
        onProgressChanged?.Invoke(currentPoints, goalPoints);
    }

    private void EvaluateAllThresholds(bool forceNotify)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            var l = listeners[i];
            if (l == null)
            {
                listeners.RemoveAt(i);
                continue;
            }

            EvaluateOne(l, forceNotify);
        }
    }

    private void EvaluateOne(ProgressThresholdListener listener, bool forceNotify)
    {
        bool nowAbove = currentPoints >= listener.TargetPoints;

        if (forceNotify)
        {
            listener.SetState(nowAbove, invokeEvent: true);
            return;
        }

        if (nowAbove != listener.IsAbove)
        {
            listener.SetState(nowAbove, invokeEvent: true);
        }
    }
}