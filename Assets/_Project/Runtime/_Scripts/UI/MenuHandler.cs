using System.Collections.Generic;
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

    // ignore this lol
    public void LoadScene(int sceneIndex) => SceneManagerExtended.LoadScene(sceneIndex);
}
