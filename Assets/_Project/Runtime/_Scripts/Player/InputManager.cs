#region
using UnityEngine;
using UnityEngine.InputSystem;
#endregion

public class InputManager : MonoBehaviour
{
    PlayerInput playerInput;
    
    public Vector2 MoveInput { get; private set; }

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (!playerInput) Logger.LogError("PlayerInput component not found on InputManager!");
    }

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
        //TODO: Clean this up. This is terrible.
        if (LevelUpManager.Instance.transform.GetChild(0).gameObject.activeSelf) return; // Prevent pausing/un-pausing the game when the level up menu is active
        if (context.performed) GameManager.Instance.TogglePause();
    }
}
