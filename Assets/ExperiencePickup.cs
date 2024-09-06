using UnityEngine;

public class ExperiencePickup : MonoBehaviour
{
    [SerializeField] int expValue = 10;
    
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        Experience.GainExp(expValue);
        // play effect here
        Destroy(gameObject);
    }
}
