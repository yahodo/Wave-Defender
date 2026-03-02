using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChooseLevelMenu : MonoBehaviour
{
    [SerializeField] private Transform grid;
    private List<Button> buttons;
    private void Start()
    {
        buttons = new List<Button>();
        for (int i = 0; i < grid.childCount; i++)
        {
            buttons.Add(grid.GetChild(i).GetChild(1).GetComponent<Button>());
        }
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].onClick.AddListener(() => StartLevel(i));
        }
    }

    public void StartLevel(int index)
    {
        SceneManager.LoadScene(index);
    }
}
