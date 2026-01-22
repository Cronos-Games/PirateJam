using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;


public class ProcessController : MonoBehaviour, IInteractable
{
    [Header("Progress")] 
    //[SerializeField] [Required] private int progressPerSecond;

    [Header("Disable Minigame")]
    [SerializeField] [Required]  private MiniGameController miniGamePrefab;
    [SerializeField] [Required]  private float disableDuration;
    [SerializeField] [Required]  private float cooldownDuration;

    [Header("Interaction")]
    [Tooltip("When multiple interactables are in range, higher priority wins.")]
    [SerializeField] private int priority = 0;

    [Header("Ai")] 
    [SerializeField] private int progressPerInteract;
    public int upperRepairTime;
    public int lowerRepairTime;
    public int upperInteractTime;
    public int lowerInteractTime;


    [Header("Map")] 
    [SerializeField] private GameObject mapShaderObject;
    
    
    private bool isDisabled;
    private float disabledTimeRemaining;
    private bool onCooldown;
    
    private Coroutine repairRoutine;
    private Coroutine progressRoutine;
    private Coroutine cooldownRoutine;
    private MiniGameController activeMiniGameInstance;
    private float tickAccumulator;

    private Outline _levelOutline;
    private Outline _mapOutline;

    // --- IInteractable ---
    public Transform Transform => transform;
    public bool CanInteract => !isDisabled && activeMiniGameInstance == null && miniGamePrefab != null && !onCooldown;
    public int Priority => priority;


    private void Start()
    {
        _levelOutline = GetComponent<Outline>();
        _mapOutline = mapShaderObject.GetComponent<Outline>();
    }

    private void Update()
    {
        if (isDisabled && disabledTimeRemaining > 0f)
            disabledTimeRemaining -= Time.deltaTime;

        //TickProgress();
    }

    /*private void TickProgress()
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
    }*/

    public void Interact()
    {
        if (!CanInteract) return;

        activeMiniGameInstance = Instantiate(miniGamePrefab);

        var mini = activeMiniGameInstance;

        if (mini == null)
        {
            Debug.LogError("MiniGame prefab must have MiniGameController on the root.");
            Destroy(activeMiniGameInstance);
            activeMiniGameInstance = null;
            return;
        }

        mini.Init(
            successCallback: OnMiniGameSuccess,
            failCallback: OnMiniGameFailed,
            cancelCallback: OnMiniGameCancelled
        );
    }

    private void OnMiniGameSuccess()
    {
        Debug.Log("MiniGame Success");
        Destroy(activeMiniGameInstance);
        activeMiniGameInstance = null;
        
        isDisabled = true;
        _levelOutline.enabled = false;
        _mapOutline.OutlineColor = Color.red; //set outline to red
    }

    private void OnMiniGameCancelled()
    {
        Debug.Log("MiniGame cancelled");
        
        Destroy(activeMiniGameInstance);
        activeMiniGameInstance = null;
        onCooldown = true;
        _mapOutline.OutlineColor = Color.yellow; //set outline to yellow
    }

    private void OnMiniGameFailed()
    {
        Debug.Log("MiniGame failed");
        Destroy(activeMiniGameInstance);
        activeMiniGameInstance = null;
        onCooldown = true;
        _mapOutline.OutlineColor = Color.yellow; //set outline to yellow
    }

    private void OnTaskReset()
    {
        isDisabled = false;
        onCooldown = false;
        _levelOutline.enabled = true;
        _mapOutline.OutlineColor = Color.green; //set outline to green
    }

    /*
    private void DisableForSeconds(float seconds)
    {
        if (disableRoutine != null)
            StopCoroutine(disableRoutine);

        disableRoutine = StartCoroutine(DisableRoutine(seconds));
    }
    */

    /*private IEnumerator DisableRoutine(float seconds)
    {
        isDisabled = true;
        disabledTimeRemaining = seconds;
        tickAccumulator = 0f;

        yield return new WaitForSeconds(seconds);

        isDisabled = false;
        disabledTimeRemaining = 0f;
        disableRoutine = null;
        
        OnTaskReset();
    }*/

    private IEnumerator RepairRoutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        OnTaskReset();
        repairRoutine = null;

    }

    private IEnumerator ProgressRoutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ProgressManager.Instance.AddProgress(progressPerInteract);
        progressRoutine = null;
    }

    private IEnumerator CooldownRoutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        OnTaskReset();
        repairRoutine = null;
    }
    
    public float AiInteract()
    {
        if (isDisabled)
        {
            float repairTime = GetRandomTime(lowerRepairTime, upperRepairTime);
            repairRoutine =  StartCoroutine(RepairRoutine(repairTime));
            return repairTime;
        }
        
        float interactTime = GetRandomTime(lowerInteractTime, upperInteractTime);
        
        if (activeMiniGameInstance != null)
        {
            activeMiniGameInstance.CompleteFail();
            cooldownRoutine = StartCoroutine(CooldownRoutine(cooldownDuration + interactTime));
        }
        

        progressRoutine = StartCoroutine(ProgressRoutine(interactTime));
        return interactTime;
    }
 
    private float GetRandomTime(float lower, float upper)
    {
        return Random.Range(lower, upper);
    }

#if UNITY_EDITOR
    private bool IsPlaying => UnityEngine.Application.isPlaying;

    [FoldoutGroup("Editor Debug")]
    [Button(ButtonSizes.Medium)]
    [EnableIf("@IsPlaying")]
    private void InteractEditor() => Interact();
#endif
}
