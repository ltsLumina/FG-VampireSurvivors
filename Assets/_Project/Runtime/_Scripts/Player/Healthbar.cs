using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] Slider healthbar;
    [SerializeField] Player player;

    void Start()
    {
        healthbar = GetComponent<Slider>();
        player    = GameObject.FindWithTag("Player").GetComponent<Player>();

        healthbar.maxValue = Character.Stat.MaxHealth;
    }

    void Update() => UpdateHealthbar();

    void UpdateHealthbar()
    {
        healthbar.value = player.CurrentHealth;
    }
}
