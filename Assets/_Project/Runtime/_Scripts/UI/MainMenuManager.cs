#region
using Lumina.Essentials.Sequencer;
using UnityEngine;
#endregion

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Starting the game...");
        SceneManagerExtended.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting the game...");
        var sequence = new Sequence(this);
        sequence.WaitThenExecute(1.5f, Application.Quit);
    }
}
