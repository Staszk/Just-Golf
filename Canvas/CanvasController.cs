using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CanvasController : EventListener
{
    //[Header("Scoreboard")]
    //[SerializeField] private GameObject scoreboardObj = null;
    //public bool ScoreboardIsActive { get; private set; }
    //public ScoreboardController Scoreboard { get { return scoreboardObj.GetComponent<ScoreboardController>(); } }

    //[Header("Gameplay UI")]
    //[SerializeField] private GameplayUIController gameUI = null;
    //public GameplayUIController GameUI { get { return gameUI; } }

    //[Header("Weapon UI")]
    //[SerializeField] private WeaponUIController weaponUI = null;
    //public WeaponUIController WeaponUI { get { return weaponUI; } }

    [Header("In-Game Settings Menu")]
    [SerializeField] private InGameSettingsController inGameSettingsMenu = null;

    //[Header("Game Over Menu")]
    //[SerializeField] private GameObject GameOverPanel = null;

    private void Start()
    {
        //gameUI.SetUp();
        //Scoreboard.SetUp(GameUI);
    }

    private void OnEnable()
    {
        EventController.AddListener(typeof(ModeChangedMessage), this);
        NetworkGameStateManager.EventEndGame += EnableEndGameUI;
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(ModeChangedMessage), this);
        NetworkGameStateManager.EventEndGame -= EnableEndGameUI;
    }

    public override void HandleEvent(EventMessage e)
    {
        if (e is ModeChangedMessage modeChange)
        {
            EnableCrosshairs(modeChange);
        }
    }

    public void ChangeGameUIVisibility(bool show)
    {
        //gameUI.ShowDynamic(show);
    }

    public void ShowScoreboard(bool show)
    {
        //scoreboardObj.SetActive(show);
        //ScoreboardIsActive = show;
    }

    public void OpenSettingsMenu(bool isOpen)
    {
        inGameSettingsMenu.Activate(isOpen);
    }

    private void EnableCrosshairs(ModeChangedMessage eventMessage)
    {
        //weaponUI.ShowElements(!eventMessage.modeChangedToGolf);
    }

    public void ChangeItemSprite(Sprite icon)
    {
        //gameUI.ShowNewItem(icon);
    }

    private void EnableEndGameUI()
    {
        //GameOverPanel.SetActive(true);
        //ShowScoreboard(true);
    }
}
