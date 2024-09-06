using UnityEditor;
using UnityEngine;

public sealed class Player : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] int health = 100;
    [SerializeField] int speed = 5;

    [Space(20)]
    
    [Header("Effects")]
    [SerializeField] ParticleSystem levelUpAura;
    
    InputManager inputManager;
    CausesOfDeath.Cause causeOfDeath;
    
    public int Health
    {
        get => health;
        set
        {
            health = value;
            
            if (health <= 0) Death(causeOfDeath);
        }
    }

    public int Speed
    {
        get => speed;
        set => speed = value;
    }

    public delegate void PlayerHit(int damage);
    public static event PlayerHit OnPlayerHit;
    
    public delegate void PlayerDeath(CausesOfDeath.Cause cause);
    public static event PlayerDeath OnPlayerDeath;

    void OnEnable()
    {
        //OnPlayerHit += ;
        Experience.OnLevelUp += () => EffectPlayer.PlayEffect(levelUpAura);
    }

    void OnDisable()
    {
        //OnPlayerHit -= ;
    }

    void Awake()
    {
        Init();
        
        return;
        void Init()
        {
            inputManager = GetComponentInChildren<InputManager>();
            if (!inputManager) Logger.LogError("InputManager component not found on Player!");
        }
    }

    void FixedUpdate() => Movement(inputManager.MoveInput);

    #region Movement
    void Movement(Vector2 dir)
    {
        var moveDir = new Vector3(dir.x, 0, dir.y);
        transform.position += moveDir * (Speed * Time.deltaTime);
    }
    #endregion

    #region Health/Damage
    public void TakeDamage(int damage, CausesOfDeath.Cause cause)
    {
        if (Health <= 0) return;
        Health -= damage;

        causeOfDeath = cause; // Set the cause of death to the latest instance of damage. Works 95% of the time :)
        OnPlayerHit?.Invoke(damage);
    }
    
    void Death(CausesOfDeath.Cause cause)
    {
        Debug.Log("Player has died of " + cause);
        enabled = false;
        
        OnPlayerDeath?.Invoke(cause);
    }
    #endregion
}

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    Editor experienceBarEditor;
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        ExperienceBar experienceBar = FindObjectOfType<ExperienceBar>();

        if (experienceBar)
        {
            if (!experienceBarEditor) experienceBarEditor = CreateEditor(experienceBar);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Experience Bar", EditorStyles.boldLabel);
            experienceBarEditor.OnInspectorGUI();
        }
        else { EditorGUILayout.HelpBox("ExperienceBar component not found on Player.", MessageType.Warning); }
    }
}