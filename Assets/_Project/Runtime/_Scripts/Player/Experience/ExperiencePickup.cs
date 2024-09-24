#region
using System;
using UnityEngine;
using VInspector;
#endregion

public class ExperiencePickup : MonoBehaviour, IPausable
{
    [Header("Settings")]
    [SerializeField] float magnetizationRadius = 2f;
    [SerializeField] float magnetizationStrength = 3f;

    [Foldout("Colour Settings")]
    [Tooltip("The threshold for the xp pickup to be blue.")]
    [SerializeField] int blueExpValue = 30;
    [Tooltip("The threshold for the xp pickup to be green.")]
    [SerializeField] int greenExpValue = 50;
    [Tooltip("The threshold for the xp pickup to be red.")]
    [SerializeField] int redExpValue = 100;

    [Header("Colours")]
    [ColorUsage(false)] 
    [SerializeField]Color blue;
    [ColorUsage(false)]
    [SerializeField] Color green;
    [ColorUsage(false)]
    [SerializeField] Color red;
    [EndFoldout]
    
    Colour colour;
    int expValue;

    void Start()
    {
        name = $"{colour} XP ({expValue} XP)";

        SetColor();
        OnCreation();

        return;
        void OnCreation()
        {
            const float force  = 5;
            const float torque = 3;

            var rb = GetComponent<Rigidbody>();
            rb.AddForce(Vector3.up   * force, ForceMode.Impulse);
            rb.AddTorque(Vector3.one * torque, ForceMode.Impulse);
        }
        
        void SetColor()
        {
            // Set the colour (enum) of the XP pickup based on the expValue.
            colour = expValue switch
            { var value when value <= blueExpValue  => Colour.Blue,
              var value when value <= greenExpValue => Colour.Green,
              var value when value <= redExpValue   => Colour.Red,
              _                                     => throw new ArgumentOutOfRangeException(nameof(expValue), expValue, "Invalid expValue.") };
            
            var material = GetComponent<MeshRenderer>().material;

            // Set the colour (Unity Color) of the XP pickup based on the Colour enum.
            Color XPColor = default;
            XPColor = colour switch
            { Colour.Blue  => blue,
              Colour.Green => green,
              Colour.Red   => red,
              _            => XPColor };

            material.color = XPColor;
        }
    }

    void Update()
    {
        Magnetize();

        return;
        void Magnetize()
        {
            Vector3 direction = Player.Position - transform.position;
            if (direction.magnitude > magnetizationRadius * Character.Stat.Magnet) return;

            // Increase the magnetization strength based on the distance to the player.
            float distance = direction.magnitude;
            float strength = magnetizationStrength * (1 - distance / (magnetizationRadius * Character.Stat.Magnet));
            
            // Move towards the player
            transform.Translate(direction.normalized * (strength * Time.deltaTime), Space.World);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, magnetizationRadius * Character.Stat.Magnet);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !Player.IsDead)
        {
            Experience.GainExp(expValue);
            
            Destroy(gameObject);
        }
    }

    public void Pause() => enabled = !enabled;

    public static ExperiencePickup Create(int xpYield, Vector3 position, Quaternion rotation)
    {
        var xp = Instantiate(Resources.Load<ExperiencePickup>("XP/XP Pickup"), position, rotation);
        xp.expValue = xpYield;
        return xp;
    }

    /// <summary>
    /// Determines how much experience the player will gain when they pick up this object.
    /// </summary>
    enum Colour
    {
        Blue,
        Green,
        Red,
    }
}
