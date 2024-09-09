#region
using System.Collections;
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Garlic", menuName = "Items/Garlic")]
public class Garlic : Item
{
    [Header("Effects")]
    [SerializeField] GameObject garlicEffect;
}
