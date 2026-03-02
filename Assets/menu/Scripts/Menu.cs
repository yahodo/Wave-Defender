using System;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private List<GameObject> menuList;
    [SerializeField] private GameObject NavButtons;
    [SerializeField] private int startMenuIndex;

    private void Start()
    {
        ChangeMenu(startMenuIndex);
    }
    public void ChangeMenu(int index)
    {
        DiactivateAllMenu();
        menuList[index].SetActive(true);

        NavButtons.SetActive(index != 0);
    }
    private void DiactivateAllMenu()
    {
        foreach (var menu in menuList)
        {
            menu.SetActive(false);
        }
    }
   
}
