using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    
    [SerializeField] GameObject gameOverMenu;

    void Start()
    {
        gameOverMenu.SetActive(false);
    }

    
    public void TriggerGameOver()
    {
        gameOverMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Restart()   
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 0f;
    }

        
}
