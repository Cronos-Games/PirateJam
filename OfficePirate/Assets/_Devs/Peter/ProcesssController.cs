using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class ProcessController : MonoBehaviour
{
    [Header("Progress")]
    [Tooltip("How many progress points this process adds each second.")]
    [SerializeField] private int progressPerSecond = 5;

    [Header("Disable Minigame")]
    [Tooltip("Prefab that spawns UI/minigame. Must have MiniGameController on the root.")]
    [SerializeField] private MiniGameController miniGamePrefab;

    [Tooltip("How long this process stops producing progress after successful minigame.")]
    [SerializeField] private float disableDuration = 5f;

    [Header("State (Read Only)")]
    [SerializeField] private bool isDisabled;
    [SerializeField] private float disabledTimeRemaining;

    private Coroutine disableRoutine;
    private GameObject activeMiniGameInstance;

    // Accumulates time until we can apply whole-second ticks.
    private float tickAccumulator;

    private void Update()
    {
        // Inspector-friendly countdown (optional)
        if (isDisabled && disabledTimeRemaining > 0f)
            disabledTimeRemaining -= Time.deltaTime;

        TickProgress();
    }

    private void TickProgress()
    {
        // If we're not actively producing, don't accumulate time.
        if (isDisabled || ProgressManager.Instance == null || progressPerSecond <= 0)
        {
            tickAccumulator = 0f;
            return;
        }

        // Add the frame time; on a hitch, this could jump by e.g. 0.2s or 2.0s.
        tickAccumulator += Time.deltaTime;

        // How many full 1-second ticks have elapsed?
        int ticks = (int)tickAccumulator; // same as FloorToInt for non-negative
        if (ticks <= 0) return;

        // Remove the processed seconds, keep remainder (0.. <1 sec)
        tickAccumulator -= ticks;

        // Apply discrete integer progress
        ProgressManager.Instance.AddProgress(progressPerSecond * ticks);
    }

    /// <summary>
    /// Call this when the player interacts with the process.
    /// For now it's public so you can trigger it from anywhere.
    /// </summary>
    public void Interact()
    {
        if (isDisabled) return;
        if (activeMiniGameInstance != null) return;
        if (miniGamePrefab == null) return;

        activeMiniGameInstance = Instantiate(miniGamePrefab.gameObject);

        var mini = activeMiniGameInstance.GetComponent<MiniGameController>();
        if (mini == null)
        {
            Debug.LogError("MiniGame prefab must have MiniGameController on the root.");
            Destroy(activeMiniGameInstance);
            activeMiniGameInstance = null;
            return;
        }

        mini.Init(
            successCallback: OnMiniGameSuccess,
            cancelCallback: OnMiniGameCancelled
        );

    }

    private void OnMiniGameSuccess()
    {
        activeMiniGameInstance = null;
        DisableForSeconds(disableDuration);
    }

    private void OnMiniGameCancelled()
    {
        activeMiniGameInstance = null;
    }

    private void DisableForSeconds(float seconds)
    {
        if (disableRoutine != null)
            StopCoroutine(disableRoutine);

        disableRoutine = StartCoroutine(DisableRoutine(seconds));
    }

    private IEnumerator DisableRoutine(float seconds)
    {
        isDisabled = true;
        disabledTimeRemaining = seconds;

        // Important: no banking time while disabled
        tickAccumulator = 0f;

        yield return new WaitForSeconds(seconds);

        isDisabled = false;
        disabledTimeRemaining = 0f;
        disableRoutine = null;
    }
    
#if UNITY_EDITOR
    private bool IsPlaying => UnityEngine.Application.isPlaying;

    [FoldoutGroup("Editor Debug")]
    [Button(ButtonSizes.Medium)]
    [EnableIf("@IsPlaying")]
    private void InteractEditor()
    {
        Interact();
    }
#endif
}
