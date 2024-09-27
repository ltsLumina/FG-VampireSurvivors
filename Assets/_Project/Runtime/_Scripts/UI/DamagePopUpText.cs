using TMPro;
using UnityEngine;

public class DamagePopUpText : TextMeshProUGUI
{
    static DamagePopUpText prefab;
    new static Canvas canvas;
    
    public static DamagePopUpText Instantiate(Transform location, int damage)
    {
        prefab = Resources.Load<DamagePopUpText>("PREFABS/UI/Pop-Up Damage");
        Debug.Log(prefab);
        canvas = GameObject.FindWithTag("[Canvas] Pop-Ups Canvas").GetComponent<Canvas>();
        
        var     instance       = Instantiate(prefab, canvas.transform, false); 
        //Vector2 screenPosition = Camera.main.WorldToScreenPoint(new Vector2(location.position.x + Random.Range(-.2f, .2f), location.position.y + Random.Range(-.2f, .2f)));

        instance.transform.position = location.position;
        instance.text = damage.ToString();
        
        Destroy(instance, 5);
        return instance;
    }
}
