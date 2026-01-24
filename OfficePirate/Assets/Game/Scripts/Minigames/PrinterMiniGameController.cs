using UnityEngine;

public class PrinterMiniGameController : MiniGameController
{
    [SerializeField] private GameObject queue;
    [SerializeField] private GameObject printerQueueObject;
    [SerializeField] private GameObject verticalContainer;
    [SerializeField] private int queueCount;

    private void OnEnable()
    {
        for (int i = 0; i < queueCount; i++)
        {
            GameObject obj = Instantiate(printerQueueObject, verticalContainer.transform);
        }
    }
    
    public void Update()
    {
        if (queue.transform.childCount == 0)
        {
            CompleteSuccess();
        }
    }
}
