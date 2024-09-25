#region
using System.Collections;
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Knife", menuName = "Items/Knife")]
public class Knife : Item
{
    [Header("Knife")]
    [SerializeField] GameObject knifePrefab;

    public override void Use()
    {
        Debug.Log($"{nameof(Knife)} used.");
        Player.Instance.SelectAttack<Knife>();
    }

    public override void Play() => Player.Instance.StartCoroutine(CardEffect());

    public void Attack(Vector3? direction = null)
    {
        var moveInput = Player.Instance.GetComponentInChildren<InputManager>().MoveInput;
        var shootDir  = direction ?? new Vector3(moveInput.x, 0, moveInput.y).normalized;
        var offset    = shootDir * 2;

        if (moveInput == Vector2.zero && direction == null) return; // TODO: Should keep shooting in the last direction

        GameObject knife = Instantiate(knifePrefab, Player.Instance.transform.position + offset, Quaternion.identity);
        knife.transform.rotation = Quaternion.LookRotation(shootDir);
        knife.GetComponent<Rigidbody>().AddForce(shootDir * 10, ForceMode.Impulse);
        
        Destroy(knife, 3f);
    }

    public IEnumerator KnifeCooldown()
    {
        var cooldown = GetBaseStat<float>(Levels.StatTypes.Speed) * Character.Stat.Dexterity;
        
        while (true)
        {
            Attack();
            yield return new WaitForSeconds(cooldown);
        }
    }

    IEnumerator CardEffect()
    {
        int   numShots  = 25;
        float angleStep = 360f / numShots;
        float angle     = 0f;

        for (int i = 0; i < numShots; i++)
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
