#region
using UnityEngine;
using UnityEngine.UI;
#endregion

//TODO: redo entire main menu
public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Starting the game...");
        SceneManagerExtended.LoadScene(1);
    }

    public void Settings()
    {
        Debug.Log("Opening the settings menu...");
        FindObjectOfType<HorizontalLayoutGroup>(true).transform.parent.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }
}
