#region
using UnityEngine;
using UnityEngine.UI;
#endregion

public class MainMenuButton : Button
{
    public void SelectCharacter() // Used on the "Start" button in unity.
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
