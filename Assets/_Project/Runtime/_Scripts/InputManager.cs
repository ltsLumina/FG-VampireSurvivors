using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }

    public static InputManager Instance { get; private set; }

    void Awake() => Instance = this;

    void OnDestroy() => Instance = null;

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        Debug.Log("Move Vector: " + MoveInput);
    }
}
