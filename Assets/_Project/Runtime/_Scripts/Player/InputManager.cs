#region
using UnityEngine;
using UnityEngine.InputSystem;
#endregion

public class InputManager : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }

    // -- Player Input Actions --

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();

        //Logger.Log("Move Vector: " + MoveInput);
    }

    // -- UI Input Actions --

    /// <summary>
    ///     Toggles the pause state of the game.
    ///     <remarks> Although the name is "OnPause", it is used to toggle the pause state of the game. </remarks>
    /// </summary>
    /// <param name="context"></param>
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed) GameManager.TogglePause();
    }
}
