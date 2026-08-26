using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Main Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject controlsPanel;

    [Header("Game Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    
    [Header("Audio Settings")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    

    private void Start()
    {
        ShowMainMenu();
    }
// Control de Botones de paneles y de regreso // 

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);

        Time.timeScale = 1f;
    }


    public void ShowControls()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        ShowMainMenu();
    }



// Hace que el boton Play Cargue la escena del juego// 

    public void PlayGame()
    {
    Time.timeScale = 1f;

    SceneManager.LoadScene("SampleScene");
    }

// Pausar y Despausar con Esc//  //

    public void PauseGame()
    {
    pausePanel.SetActive(true);

    Time.timeScale = 0f;
    }

    
    private void Update()
    {
    if (Input.GetKeyDown(KeyCode.Escape))
        {
        TogglePause();
        }
    }   

    private void TogglePause()
    {
    if (pausePanel.activeSelf)
        {
        ResumeGame();
        }
    else
        {
        PauseGame();
        }
    }

// Reanudar juego //
    public void ResumeGame() 
    {
    pausePanel.SetActive(false);

    Time.timeScale = 1f;   
    }

// Funcion para el Botón de regreso al menu anterior// 
    
    private bool settingsFromPause = false;
        
    public void ShowSettings()
    {
    settingsFromPause = false;

    mainMenuPanel.SetActive(false);
    settingsPanel.SetActive(true);
    controlsPanel.SetActive(false);
    pausePanel.SetActive(false);
    }


    public void ShowPauseSettings()
    {
    settingsFromPause = true;

    pausePanel.SetActive(false);
    settingsPanel.SetActive(true);
    }


    public void BackFromSettings()
    {
    settingsPanel.SetActive(false);

    if (settingsFromPause)
        {
        pausePanel.SetActive(true);
        }   
    else
        {
        mainMenuPanel.SetActive(true);
        }
    }

// Panel para instanciar el Game over --> Agregar esto al script con la muerte del player //
// [SerializeField] private UIManager uiManager;
// UIManager.ShowGameOver();// 

    public void ShowGameOver()
    {
    gameOverPanel.SetActive(true);

    Time.timeScale = 0f;
    }

// Reiniciar la escena desde el GameOver// 

    public void RetryGame()
    {
    Time.timeScale = 1f;

    Scene currentScene = SceneManager.GetActiveScene();

    SceneManager.LoadScene(currentScene.name);
    }

// Ir al menú desde el Game Over // 

    public void ReturnToMainMenu()
    {
    Time.timeScale = 1f;

    SceneManager.LoadScene("MainMenu");
    }

// Control de sliders// 

    public void SetMusicVolume(float value)
    {
    AudioManager.Instance.SetMusicVolume(value);
    }

public void SetSFXVolume(float value)
    {
    AudioManager.Instance.SetSFXVolume(value);
    }

}
