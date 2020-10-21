using UnityEngine;

public class GameCanvasController : EventListener
{
    // Game Timer
    // Game Score
    // Cooldown Meters
    [SerializeField] private AbilityCooldownController abilityCooldownController = null;
    // Charge Meter
    // Health
    [SerializeField] private PlayerHealthUIController playerHealthUI = null;
    // Damage Indicator
    [SerializeField] private DamageIndicator damageIndicator = null;
    // Character Windows
    [SerializeField] private CharacterWindowUI[] characterWindows = null;
    // Location References
    [SerializeField] private LocationReferenceUIController locationReferenceUI = null;

    private void Start()
    {
        if (NetworkManager.instance?.GetMyTeam() == 1)
        {
            // Change to Away Team
            playerHealthUI.ChangeToAwayTeam();
            characterWindows[0].ChangeToAwayTeam();
            characterWindows[1].ChangeToAwayTeam();
        }
    }

    private void OnEnable()
    {
        EventController.AddListener(typeof(ObjectBallsSpawnedMessage), this);
        EventController.AddListener(typeof(SetTeamUIMessage), this);
        EventController.AddListener(typeof(PlayerHealthChangeMessage), this);
        EventController.AddListener(typeof(EndGameMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(ObjectBallsSpawnedMessage), this);
        EventController.RemoveListener(typeof(SetTeamUIMessage), this);
        EventController.RemoveListener(typeof(PlayerHealthChangeMessage), this);
        EventController.RemoveListener(typeof(EndGameMessage), this);

    }

    private void Update()
    {
        locationReferenceUI.HandleUpdate();    
    }

    public override void HandleEvent(EventMessage e)
    {
        if (e is ObjectBallsSpawnedMessage ballsMessage)
        {
            locationReferenceUI.Init();
            locationReferenceUI.PrepareBallTransforms(ballsMessage.ballsArray);
        }
        else if(e is SetTeamUIMessage teamMessage)
        {
            locationReferenceUI.PrepareTeammateTransform(teamMessage.teammate);
        }
        else if (e is PlayerHealthChangeMessage)
        {
            damageIndicator.Show();
        }
        else if (e is EndGameMessage)
        {
            GetComponent<TemporaryUI>().CheckScoreAndEndGame();
            TurnOffAllUI(transform);
        }
    }

    private void TurnOffAllUI(Transform obj)
    {
        for(int i = 0; i < obj.transform.childCount; i++)
        {
            if (obj.GetChild(i).GetComponent<Canvas>())
                TurnOffAllUI(obj.GetChild(i));
            else if(obj.GetChild(i).tag != "End Game UI")
                obj.GetChild(i).gameObject.SetActive(false);
        }
    }
}
