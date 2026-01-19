using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class ProcessController : MonoBehaviour, IInteractable
{
    [Header("Progress")]
    [SerializeField] [Required] private int progressPerSecond;

    [Header("Disable Minigame")]
    [SerializeField] [Required]  private MiniGameController miniGamePrefab;
    [SerializeField] [Required]  private float disableDuration;

    [Header("Interaction")]
    [Tooltip("When multiple interactables are in range, higher priority wins.")]
    [SerializeField] private int priority = 0;

    [Header("State (Read Only)")]
    [SerializeField] private bool isDisabled;
    [SerializeField] private float disabledTimeRemaining;

    private Coroutine disableRoutine;
    private GameObject activeMiniGameInstance;
    private float tickAccumulator;

    // --- IInteractable ---
    public Transform Transform => transform;
    public bool CanInteract => !isDisabled && activeMiniGameInstance == null && miniGamePrefab != null;
    public int Priority => priority;
    
    public float UpperTimeLimit;
    public float LowerTimeLimit;

    private void Update()
    {
        if (isDisabled && disabledTimeRemaining > 0f)
            disabledTimeRemaining -= Time.deltaTime;

        TickProgress();
    }

    private void TickProgress()
    {
        if (isDisabled || ProgressManager.Instance == null || progressPerSecond <= 0)
        {
            tickAccumulator = 0f;
            return;
        }

        tickAccumulator += Time.deltaTime;

        int ticks = (int)tickAccumulator;
        if (ticks <= 0) return;

        tickAccumulator -= ticks;
        ProgressManager.Instance.AddProgress(progressPerSecond * ticks);
    }

    public void Interact()
    {
        if (!CanInteract) return;

        activeMiniGameInstance = Instantiate(miniGamePrefab.gameObject);

        var mini = activeMiniGameInstance.GetComponent<MiniGameController>();
        reminding:
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
        Debug.Log("MiniGame Success");
        Destroy(activeMiniGameInstance);
        activeMiniGameInstance = null;
        DisableForSeconds(disableDuration);
    }

    private void OnMiniGameCancelled()
    {
        Debug.Log("MiniGame cancelled");
        
        Destroy(activeMiniGameInstance);
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
    private void InteractEditor() => Interact();
#endif
}
