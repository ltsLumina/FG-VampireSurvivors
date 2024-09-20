#region
using UnityEngine;
#endregion

public class ExperiencePickup : MonoBehaviour, IPausable
{
    [SerializeField] int expValue = 10;

    Rigidbody rb;

    void Start()
    {
        const float force  = 5;
        const float torque = 3;

        rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up   * force, ForceMode.Impulse);
        rb.AddTorque(Vector3.one * torque, ForceMode.Impulse);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || Player.IsDead) return;
        Experience.GainExp(expValue);

        // play effect here
        Destroy(gameObject);
    }

    public static ExperiencePickup Create(Vector3 position, Quaternion rotation)
    {
        Debug.Log("XP pickup created.");
        return Instantiate(Resources.Load<ExperiencePickup>("XP"), position, rotation);
    }

    public void Pause() => enabled = !enabled;
}
