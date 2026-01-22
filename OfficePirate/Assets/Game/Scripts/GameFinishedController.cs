using UnityEngine;

public class GameFinishedController : MonoBehaviour
{
    [SerializeField] private GameObject gameFinishedContainer;
    [SerializeField] private int finishedAfterDay;

    private void Awake()
    {
        if (gameFinishedContainer == null)
            gameFinishedContainer = gameObject;

        gameFinishedContainer.SetActive(false);
    }

    public void checkFinished(int day)
    {
        if (day > finishedAfterDay)
        {
            gameFinishedContainer.SetActive(true);
            Time.timeScale = 0;
        }
    }
}