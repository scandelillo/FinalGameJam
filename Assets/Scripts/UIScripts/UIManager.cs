using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [System.Serializable]
    public class Screen
    {
        public string id;          // nombre libre, ej "MainMenu", "Settings"
        public CanvasGroup group;
    }

    [Header("Todas las pantallas que este manager controla")]
    [SerializeField] private List<Screen> screens;

    [Header("Pantalla con la que arranca escenaconui")]
    [SerializeField] private string initialScreenId = "MainMenu";

    private readonly Stack<CanvasGroup> history = new Stack<CanvasGroup>();
    private CanvasGroup current;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        HideAll();
        Show(initialScreenId, remember: false);
    }

    public void Show(string id, bool remember = true)
    {
        EventSystem.current?.SetSelectedGameObject(null);

        var target = screens.Find(s => s.id == id)?.group;
        if (target == null)
        {
            Debug.LogWarning($"UIManager: no existe una pantalla con id '{id}'");
            return;
        }

        if (remember && current != null)
            history.Push(current);

        foreach (var s in screens)
            SetVisible(s.group, s.group == target);

        current = target;
    }

    public void Back()
    {
        if (history.Count > 0)
            Show(history.Pop(), remember: false);
        else
            HideAll();
    }

    private void Show(CanvasGroup target, bool remember)
    {
        EventSystem.current?.SetSelectedGameObject(null);
        if (remember && current != null) history.Push(current);
        foreach (var s in screens) SetVisible(s.group, s.group == target);
        current = target;
    }

    public void HideAll()
    {
        EventSystem.current?.SetSelectedGameObject(null);
        foreach (var s in screens) SetVisible(s.group, false);
        current = null;
        history.Clear();
    }

    private void SetVisible(CanvasGroup cg, bool visible)
    {
        cg.alpha = visible ? 1f : 0f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }

    // ---- Acciones concretas para conectar a botones ----

    public void PlayGame(string sceneName)
    {
        HideAll();
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    public void PauseGame()
    {
        Show("Pause");
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        HideAll();
        Time.timeScale = 1;
    }

    public void GoToMainMenuScene()
    {
        Time.timeScale = 1;
        HideAll();
        SceneManager.LoadScene("EscenaconUI");
        Show("MainMenu", remember: false);
    }

    public void TriggerGameOver()
    {
        Show("GameOver");
        Time.timeScale = 0;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        HideAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowScreen(string id)
    {
        Show(id);
    }
}