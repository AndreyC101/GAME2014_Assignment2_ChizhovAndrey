using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum UIPanelTypes
{
    MAIN_MENU = 0,
    INSTRUCTION_PAGE_1 = 1,
    INSTRUCTION_PAGE_2 = 2,
    INSTRUCTION_PAGE_3 = 3,
    GAME_OVER = 4,
    GAME_UI = 5,
    PANEL_COUNT = 6
}

public class MenuController : MonoBehaviour
{
    [SerializeField]
    GameObject[] MenuPanels = new GameObject[(int)UIPanelTypes.PANEL_COUNT];

    [SerializeField]
    GameObject GameplayObjects;

    [SerializeField]
    TMP_Text StartButtonText;

    private bool gameInProgress = false;

    void Start()
    {
        SwitchUIState(UIPanelTypes.MAIN_MENU);
    }

    private void SwitchUIState(UIPanelTypes type)
    {
        GameplayObjects.SetActive(false);
        foreach (GameObject panel in MenuPanels)
        {
            panel.SetActive(false);
        }

        MenuPanels[(int)type].SetActive(true);

        if (type == UIPanelTypes.GAME_UI)
        {
            GameplayObjects.SetActive(true);
        }

        if (type == UIPanelTypes.MAIN_MENU)
        {
            StartButtonText.text = gameInProgress ? "Continue" : "Start";
        }
    }

    //button functions
    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }

    public void OnMainMenuButtonPressed()
    {
        SwitchUIState(UIPanelTypes.MAIN_MENU);
    }

    public void OnInstruction1ButtonPressed()
    {
        SwitchUIState(UIPanelTypes.INSTRUCTION_PAGE_1);
    }

    public void OnInstruction2ButtonPressed()
    {
        SwitchUIState(UIPanelTypes.INSTRUCTION_PAGE_2);
    }

    public void OnInstruction3ButtonPressed()
    {
        SwitchUIState(UIPanelTypes.INSTRUCTION_PAGE_3);
    }

    public void OnStartGameButtonPressed()
    {
        SwitchUIState(UIPanelTypes.GAME_UI);
        gameInProgress = true;
    }
}
