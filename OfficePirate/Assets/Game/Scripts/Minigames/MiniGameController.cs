using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class MiniGameController : MonoBehaviour
{
    [Header("Lifecycle Events")]

    [Tooltip("Fired when the minigame is initialized and shown.")]
    public UnityEvent onStart;

    [Tooltip("Fired when the player completes the minigame successfully.")]
    public UnityEvent onSuccess;

    [Tooltip("Fired when the player fails the minigame (wrong answer, timeout, etc.).")]
    public UnityEvent onFail;

    [Tooltip("Fired when the player exits/cancels the minigame voluntarily.")]
    public UnityEvent onCancel;

    private void OnEnable()
    {
        onStart.Invoke();
    }

    /// <summary>
    /// Called by the ProcessController right after spawning the minigame.
    /// We ADD callbacks instead of clearing anything.
    /// </summary>
    public void Subscribe(UnityAction successCallback, UnityAction failCallback, UnityAction cancelCallback)
    {
        if (successCallback != null)
            onSuccess.AddListener(successCallback);

        if (cancelCallback != null)
            onCancel.AddListener(cancelCallback);

        if (failCallback != null)
            onFail.AddListener(failCallback);
        
    }
    
    public void UnSubscribe(UnityAction successCallback, UnityAction failCallback, UnityAction cancelCallback)
    {
        if (successCallback != null)
            onSuccess.RemoveListener(successCallback);

        if (cancelCallback != null)
            onCancel.RemoveListener(cancelCallback);

        if (failCallback != null)
            onFail.RemoveListener(failCallback);
        
    }

    public void CompleteSuccess()
    {
        onSuccess?.Invoke();
    }

    public void CompleteFail()
    {
        onFail?.Invoke();
    }

    public void CompleteCancel()
    {
        onCancel?.Invoke();
    }
}