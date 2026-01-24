using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


public class ProcessController : MonoBehaviour, IInteractable
{
    // [Header("Progress")] 
    //[SerializeField] [Required] private int progressPerSecond;

    [Header("Disable Minigame")]
    [SerializeField] [Required]  private MiniGameController miniGamePrefab;
    [SerializeField] [Required]  private float disableDuration;
    [SerializeField] [Required]  private float cooldownDuration;

    [Header("Interaction")]
    [Tooltip("When multiple interactables are in range, higher priority wins.")]
    [SerializeField] private int priority = 0;
    private Camera mainCamera;
    private MapController mapController;

    [Header("Ai")] 
    [SerializeField] private int progressPerInteract;
    public int upperRepairTime;
    public int lowerRepairTime;
    public int upperInteractTime;
    public int lowerInteractTime;
    
    


    // [Header("Map")] 
    //[SerializeField] private GameObject mapShaderObject;
    
    
    private bool isDisabled;
    private float disabledTimeRemaining;
    private bool available;
    
    private Coroutine repairRoutine;
    private Coroutine progressRoutine;
    private Coroutine cooldownRoutine;
    private float tickAccumulator;

    private Outline _levelOutline;
    //private Outline _mapOutline;

    // --- IInteractable ---
    public Transform Transform => transform;
    public bool CanInteract => !isDisabled && !miniGamePrefab.gameObject.activeInHierarchy && miniGamePrefab != null && available;

    public int Priority => priority;


    private void Start()
    {
        _levelOutline = GetComponent<Outline>();
        available = true;

        mainCamera = Camera.main;
        mapController = GameObject.FindGameObjectWithTag("Player").GetComponent<MapController>();
    }

    private void Update()
    {
        if (isDisabled && disabledTimeRemaining > 0f)
            disabledTimeRemaining -= Time.deltaTime;

    }
    public void Interact()
    {

        StartMiniGame();
    }

    private void StartMiniGame()
    {
        miniGamePrefab.gameObject.SetActive(true);
        mainCamera.enabled = false;
        mapController.Enabled = false;
        
        miniGamePrefab.Subscribe(
            successCallback: OnMiniGameSuccess,
            failCallback: OnMiniGameFailed,
            cancelCallback: OnMiniGameCancelled
        );

        Debug.Log(miniGamePrefab);
    }

    private void StopMiniGame()
    {
        Debug.Log("MiniGame Stop");
        mainCamera.enabled = true;
        mapController.Enabled = true;

        miniGamePrefab.UnSubscribe(
            successCallback: OnMiniGameSuccess,
            failCallback: OnMiniGameFailed,
            cancelCallback: OnMiniGameCancelled
        );
        miniGamePrefab.gameObject.SetActive(false);
        
    }

    private void OnMiniGameSuccess()
    {
        Debug.Log("MiniGame Success");
        StopMiniGame();
        isDisabled = true;
        _levelOutline.OutlineColor = Color.red; //set outline to red
    }

    private void OnMiniGameCancelled()
    {
        Debug.Log("MiniGame cancelled");
        
        StopMiniGame();
        available = false;
        _levelOutline.OutlineColor = Color.yellow; //set outline to yellow
    }

    private void OnMiniGameFailed()
    {
        Debug.Log("MiniGame failed");
        StopMiniGame();
        available = false;
        _levelOutline.OutlineColor = Color.yellow; //set outline to yellow
    }

    private void OnTaskReset()
    {
        isDisabled = false;
        available = true;
        _levelOutline.OutlineColor = Color.green; //set outline to green
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
        available = true;
        progressRoutine = null;
    }

    private IEnumerator CooldownRoutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        OnTaskReset();
        repairRoutine = null;
    }
    
    public float AiInteract(float timeMultiplier = 1)
    {
        if (isDisabled)
        {
            float repairTime = GetRandomTime(lowerRepairTime, upperRepairTime) * timeMultiplier;
            repairRoutine =  StartCoroutine(RepairRoutine(repairTime));
            return repairTime;
        }
        
        float interactTime = GetRandomTime(lowerInteractTime, upperInteractTime) * timeMultiplier;
        
        if (miniGamePrefab.enabled)
        {
            miniGamePrefab.CompleteFail();
            cooldownRoutine = StartCoroutine(CooldownRoutine(cooldownDuration + interactTime));
        }
        available = false;
        
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
