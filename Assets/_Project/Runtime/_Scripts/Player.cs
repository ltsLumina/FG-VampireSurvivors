using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] int speed = 5;

    public int Speed
    {
        get => speed;
        set => speed = value;
    }

    public static Player Instance { get; private set; }

    // Initialize the singleton instance
    void Awake() => Instance = this;

    void OnDestroy() => Instance = null;

    void FixedUpdate() { Movement(InputManager.Instance.MoveInput); }

    void Movement(Vector2 dir) => transform.position += new Vector3(dir.x * Speed, 0, dir.y * Speed) * Time.deltaTime;
}
