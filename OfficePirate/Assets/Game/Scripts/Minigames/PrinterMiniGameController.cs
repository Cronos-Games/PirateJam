using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PrinterMiniGameController : MiniGameController
{
    [SerializeField] private GameObject queue;
    public void Update()
    {
        if (queue.transform.childCount == 0)
        {
            CompleteSuccess();
        }
    }
}
