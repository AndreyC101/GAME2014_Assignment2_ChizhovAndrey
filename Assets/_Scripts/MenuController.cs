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
    GAME_FINISH = 5,
    GAME_UI = 6,
    PANEL_COUNT = 6
}

public class MenuController : MonoBehaviour
{
    public bool DEV_MODE;

    [SerializeField]
    GameplayManager gm;

    [SerializeField]
    GameObject[] MenuPanels = new GameObject[(int)UIPanelTypes.PANEL_COUNT];

    [SerializeField]
    GameObject GameplayObjects;

    [SerializeField]
    TMP_Text StartButtonText;

    private bool gameInProgress = false;

    void Start()
    {
        GameplayObjects.SetActive(true);
        gm.ResetLevel();
        if (DEV_MODE)
        {
            SwitchUIState(UIPanelTypes.GAME_UI);
            SoundManager.Instance.PlayAudio(SoundID.Gameplay_Music);
            return;
        }
        SwitchUIState(UIPanelTypes.MAIN_MENU);
        GameplayObjects.SetActive(false);
        SoundManager.Instance.PlayAudio(SoundID.Menu_Music);
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

    public void OnGameFinished()
    {
        GameplayObjects.SetActive(false);
        SwitchUIState(UIPanelTypes.GAME_FINISH);
        SoundManager.Instance.StopAudio(SoundID.Gameplay_Music);
    }

    public void OnGameOver()
    {
        GameplayObjects.SetActive(false);
        SwitchUIState(UIPanelTypes.GAME_OVER);
        SoundManager.Instance.StopAudio(SoundID.Gameplay_Music);
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
        SoundManager.Instance.StopAudio(SoundID.Menu_Music);
        SoundManager.Instance.PlayAudio(SoundID.Gameplay_Music);
        SwitchUIState(UIPanelTypes.GAME_UI);
        gameInProgress = true;
        gm.ResetLevel();
    }

    public void OnFinishGameButtonPressed()
    {
        SwitchUIState(UIPanelTypes.MAIN_MENU);
        gameInProgress = false;
        SoundManager.Instance.PlayAudio(SoundID.Menu_Music);
    }
}
