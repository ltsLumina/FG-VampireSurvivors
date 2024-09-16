#region
using UnityEngine;
#endregion

public class GameManager : MonoBehaviour
{
    public static bool IsPaused => Time.timeScale == 0;

    public static void PauseGame() => Time.timeScale = 0;

    public static void ResumeGame() => Time.timeScale = 1;

    public static void TogglePause() => Time.timeScale = IsPaused ? 1 : 0;
}
