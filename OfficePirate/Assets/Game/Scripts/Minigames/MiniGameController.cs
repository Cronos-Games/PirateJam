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

    /// <summary>
    /// Called by the ProcessController right after spawning the minigame.
    /// We ADD callbacks instead of clearing anything.
    /// </summary>
    public void Init(UnityAction successCallback, UnityAction cancelCallback = null)
    {
        if (successCallback != null)
            onSuccess.AddListener(successCallback);

        if (cancelCallback != null)
            onCancel.AddListener(cancelCallback);

        // Fire start event AFTER wiring is done
        onStart?.Invoke();
    }

    public void CompleteSuccess()
    {
        onSuccess?.Invoke();
        Destroy(gameObject);
    }

    public void CompleteFail()
    {
        onFail?.Invoke();
        Destroy(gameObject);
    }

    public void CompleteCancel()
    {
        onCancel?.Invoke();
        Destroy(gameObject);
    }
}