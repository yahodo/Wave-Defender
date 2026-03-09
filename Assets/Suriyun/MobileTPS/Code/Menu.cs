using System;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private List<GameObject> menuList;
    [SerializeField] private int startMenuIndex;

    private void Start()
    {
        ChangeMenu(startMenuIndex);
    }
    public void ChangeMenu(int index)
    {
        if (index >= menuList.Count || index < 0)
        {
            Debug.LogWarning("ChangeMenu index is wrong");
            return;
        }
        DiactivateAllMenu();
        menuList[index].SetActive(true);
    }
    private void DiactivateAllMenu()
    {
        foreach (var menu in menuList)
        {
            menu.SetActive(false);
        }
    }
   
}
