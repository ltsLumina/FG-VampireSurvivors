#region
using UnityEngine;
using UnityEngine.UI;
#endregion

public class MainMenuButton : Button
{
    public void Settings() // TODO: this is horrendous
    {
        Debug.Log("Opening the settings menu...");
        var menu = GameObject.Find("Settings Menu");
        menu.transform.GetChild(0).gameObject.SetActive(!menu.transform.GetChild(0).gameObject.activeSelf);
        menu.transform.GetChild(1).gameObject.SetActive(!menu.transform.GetChild(1).gameObject.activeSelf);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }
}
