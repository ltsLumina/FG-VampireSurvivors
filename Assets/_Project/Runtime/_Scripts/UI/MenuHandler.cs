using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] List<Canvas> menus;
    
    public void ShowMenu(Canvas menu)
    {
        foreach (Canvas m in menus)
        {
            m.gameObject.SetActive(m == menu);
        }
    }
}
