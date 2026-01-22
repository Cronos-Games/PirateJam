using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverContainer;

    private void Awake()
    {
        if (gameOverContainer == null)
            gameOverContainer = gameObject;

        // Make sure it's hidden at start
        gameOverContainer.SetActive(false);
    }

    // Called by ProgressManager when game is over
    public void ShowGameOver()
    {
        gameOverContainer.SetActive(true);
        Time.timeScale = 0;
    }
}