using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   [Header ("Menús")]

    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject controlsMenu;


   public void OpenMenu (GameObject menu)
   
   {
    mainMenu.SetActive(false);
    settingsMenu.SetActive(false);
    controlsMenu.SetActive(false);
   
    menu.SetActive(true);
   }

    public void switchScenes (string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
