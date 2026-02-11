using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class LogbookManager : MonoBehaviour
{
    public List<GameObject> tabPanels;
    public Button buttonPrefab;
    public Transform buttonParent;

    public List<string> entriesEnemies;
    public List<string> entriesLocations;
    public List<string> entriesLore;
    public List<string> entriesItems;

    void Start()
    {
        ShowTab(0);
    }

    void GenerateButtons(List<string> currentlist)
    {
        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < entriesEnemies.Count; i++)
        {
            Button newButton = Instantiate(buttonPrefab, buttonParent);
        }
    }

    public void ShowTab(int tabIndex)
    {
        for (int i = 0; i < tabPanels.Count; i++)
        {
            tabPanels[i].SetActive(i == tabIndex);
            if (i == 0)
            {
                GenerateButtons(entriesEnemies);
            }
            else if (i == 1)
            {
                GenerateButtons(entriesLocations);
            }
            else if (i == 2)
            {
                GenerateButtons(entriesLore);
            }
            else if (i == 3)
            {
                GenerateButtons(entriesItems);
            }
        }
    }
}