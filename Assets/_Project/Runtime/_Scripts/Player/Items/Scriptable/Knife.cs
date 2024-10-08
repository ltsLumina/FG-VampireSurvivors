#region
using System.Collections;
using UnityEngine;
#endregion

public class Knife : WeaponItem
{
    [Header("Knife")]
    [SerializeField] GameObject knifePrefab;
    [SerializeField] float velocity = 10f;

    [Header("Card Effect")]
    [SerializeField] int numShots = 30;

    public override void Use()
    {
        Debug.Log($"{nameof(Knife)} used.");
        Player.Instance.SelectAttack<Knife>();
    }

    public override void Play() => Player.Instance.StartCoroutine(CardEffect());

    void Attack(Vector3? direction = null)
    {
        Vector2 moveInput        = Player.Instance.GetComponentInChildren<InputManager>().MoveInput;
        Vector3 shootDir         = direction ?? new Vector3(moveInput.x, 0, moveInput.y).normalized;
        Vector3 offsetFromPlayer = shootDir * 2;

        if (shootDir == Vector3.zero) return;

        for (int i = 0; i < GetItemSpecificStat(ItemSpecificStats.Stats.Knives) + Character.Stat.Amount; i++)
        {
            GameObject knife = Instantiate(knifePrefab, Player.Instance.transform.position + offsetFromPlayer, Quaternion.identity);

            var knifeOffset = new Vector3(Random.Range(-1.5f, 1.5f), 0, Random.Range(-0.5f, 0.5f));
            knife.transform.position += knifeOffset;
            knife.transform.rotation =  Quaternion.LookRotation(shootDir);
            knife.GetComponent<Rigidbody>().AddForce(shootDir * (velocity * Character.Stat.Dexterity), ForceMode.Impulse);

            Destroy(knife, 3f * Character.Stat.Intelligence);
        }
    }

    public IEnumerator KnifeCooldown()
    {
        while (true)
        {
            Attack();
            yield return new WaitForSeconds(Cooldown);
        }
    }

    IEnumerator CardEffect()
    {
        float angleStep = 360f / numShots;
        float angle     = 0f;

        for (int i = 0; i < numShots * (Character.Stat.Amount > 0 ? Character.Stat.Amount : 1); i++)
        {
            float   directionX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float   directionZ = Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector3 direction  = new Vector3(directionX, 0, directionZ).normalized;

            Attack(direction);

            angle += angleStep;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
