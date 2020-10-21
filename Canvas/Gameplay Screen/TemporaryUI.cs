using UnityEngine;
using System.Collections;
using System;

public class TemporaryUI : EventListener
{

	[SerializeField] private TMPro.TMP_Text gameTimer = null;

	[SerializeField] private GameObject[] clubObjects = null;

    [SerializeField] private VisualMeter chargeMeter = null;
    //[SerializeField] private VisualMeter[] abilityMeters = null;

    [SerializeField] private TMPro.TextMeshProUGUI teamScoreUI = null;
    [SerializeField] private TMPro.TextMeshProUGUI enemyScoreUI = null;

    private int index = 0;

	private void Start()
	{
        Cursor.lockState = CursorLockMode.Locked;

		gameTimer.color = Color.yellow;
	}

    private void OnEnable()
    {
        EventController.AddListener(typeof(ClubChangedMessage), this);
        EventController.AddListener(typeof(GolfChargeMessage), this);
        //EventController.AddListener(typeof(AbilityCooldownMessage), this);
		EventController.AddListener(typeof(GameTimeMessage), this);
		EventController.AddListener(typeof(GameStartMessage), this);
        //EventController.AddListener(typeof(TakeDamageMessage), this);
        EventController.AddListener(typeof(TeamScoreMessage), this);
        EventController.AddListener(typeof(EnemyScoreMessage), this);
        EventController.AddListener(typeof(ClientRespawnMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(ClubChangedMessage), this);
        EventController.RemoveListener(typeof(GolfChargeMessage), this);
        EventController.RemoveListener(typeof(AbilityCooldownMessage), this);
		EventController.RemoveListener(typeof(GameTimeMessage), this);
		EventController.RemoveListener(typeof(GameStartMessage), this);
        //EventController.RemoveListener(typeof(TakeDamageMessage), this);
        EventController.RemoveListener(typeof(TeamScoreMessage), this);
        EventController.RemoveListener(typeof(EnemyScoreMessage), this);
        EventController.RemoveListener(typeof(ClientRespawnMessage), this);
    }

    public override void HandleEvent(EventMessage e)
    {
		if (e is GameTimeMessage timeMessage)
		{
			DisplayTime(timeMessage.timeToDisplay);
		}
        else if (e is GolfChargeMessage chargeMessage)
        {
            ChangeVisualMeter(chargeMeter, chargeMessage.amount);
        }
        else if (e is AbilityCooldownMessage cooldownMessage)
        {
            //Debug.Log(cooldownMessage.ability);
            //ChangeVisualMeter(abilityMeters[cooldownMessage.ability], cooldownMessage.amount);
        }
        else if (e is ClubChangedMessage clubChange)
        {
            ChangeActiveClub(clubChange.index);
        }
		else if (e is GameStartMessage)
		{
			gameTimer.color = Color.white;
		}
        else if (e is TeamScoreMessage)
        {
            IncreaseTeamScore();
        }
        else if (e is EnemyScoreMessage)
        {
            IncreaseEnemyScore();
        }
    }

    public void CheckScoreAndEndGame()
    {
        VictoryDefeatMessage message;
        if ((int.Parse(teamScoreUI.text)) > (int.Parse(enemyScoreUI.text)))
            message = new VictoryDefeatMessage(true);
        else
            message = new VictoryDefeatMessage(false);

        EventController.FireEvent(message);
    }

    private void IncreaseTeamScore()
    {
        teamScoreUI.text = (int.Parse(teamScoreUI.text) + 1).ToString();
    }

    private void IncreaseEnemyScore()
    {
        enemyScoreUI.text = (int.Parse(enemyScoreUI.text) + 1).ToString();
    }

	private void DisplayTime(float timeToDisplay)
	{
		float min = Mathf.Floor(timeToDisplay / 60.0f);
		float sec = Mathf.Floor(timeToDisplay % 60.0f);

		if (min > 0)
			gameTimer.text = string.Format("{0:}:{1:}", min.ToString("00"), sec.ToString("00"));
		else
			gameTimer.text = string.Format("{0:0.00}", timeToDisplay) + "s";

    }

	private void ChangeActiveClub(int nextIndex)
    {
		clubObjects[index].SetActive(false);
		clubObjects[nextIndex].SetActive(true);

        index = nextIndex;
    }

    private void ChangeVisualMeter(VisualMeter meter, float amount)
    {
        meter.SetFill(amount);
    }
}
