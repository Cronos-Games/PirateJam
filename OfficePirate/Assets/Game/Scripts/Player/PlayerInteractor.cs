using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Input System")]
    /*
    [Tooltip("Reference to an InputAction (Button) like 'Interact' bound to E.")]
    [SerializeField] private InputActionReference interactAction;
    */

    [Header("Selection")]
    [Tooltip("Max number of candidates to consider (safety).")]
    [SerializeField] private int maxCandidates = 512;

    [Header("Events")]
    public UnityEvent<bool> onHasTargetChanged; // true when a target becomes available
    public UnityEvent onInteractPerformed;

    private readonly HashSet<IInteractable> nearby = new();
    private IInteractable currentTarget;

    /*private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += OnInteract;
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteract;
            interactAction.action.Disable();
        }
    }*/

    private void Update()
    {
        UpdateTarget();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (currentTarget == null) return;
        if (!currentTarget.CanInteract) return;
        currentTarget.Interact();
        onInteractPerformed?.Invoke();
    }

    private void UpdateTarget()
    {
        IInteractable best = null;
        float bestDistSq = float.PositiveInfinity;
        int bestPriority = int.MinValue;

        int count = 0;
        foreach (var it in nearby)
        {
            if (it == null) continue;
            if (!it.CanInteract) continue;

            // safety
            if (++count > maxCandidates) break;

            int p = it.Priority;
            float d = (it.Transform.position - transform.position).sqrMagnitude;

            // Priority first, then distance
            if (p > bestPriority || (p == bestPriority && d < bestDistSq))
            {
                best = it;
                bestPriority = p;
                bestDistSq = d;
            }
            
        }

        if (best != currentTarget)
        {
            currentTarget = best;
            onHasTargetChanged?.Invoke(currentTarget != null);
        }
    }

    // -------- Trigger detection  --------

    private void OnTriggerEnter(Collider other)
    {
        // Grab any IInteractable on this object or its parents
        var mono = other.GetComponentInParent<MonoBehaviour>();
        if (mono is IInteractable interactable)
        {
            nearby.Add(interactable);
            Debug.Log(nearby.First());           
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var mono = other.GetComponentInParent<MonoBehaviour>();
        if (mono is IInteractable interactable)
            nearby.Remove(interactable);
    }
}
