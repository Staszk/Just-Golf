using UnityEngine;

public class AbilityCooldownController : EventListener
{
	[SerializeField] private AbilityCooldown[] playerMiniCooldowns = null;
	[SerializeField] private AbilityCooldown[] playerProfileCooldowns = null;
	[SerializeField] private AbilityCooldown[] teammateCooldowns = null;

    [SerializeField] private Sprite[] allMiniIcons = null;
    [SerializeField] private Sprite[] allProfileIcons = null;
    [SerializeField] private Sprite[] allTeammateIcons = null;

    private void Start()
    {
        if (NetworkManager.instance)
        {
            int teammateID = NetworkStoredData.instance.GetIDofTeammate(NetworkManager.instance.GetId());

            int teammateOffense = (NetworkStoredData.instance.GetOffenseAbility(teammateID) + 1);
            int teammateDefense = (NetworkStoredData.instance.GetDefenseAbility(teammateID) + 3);

            teammateCooldowns[0].SetIcon(allTeammateIcons[teammateOffense]);
            teammateCooldowns[1].SetIcon(allTeammateIcons[teammateDefense]);
        }
    }

    private void OnEnable()
	{
		EventController.AddListener(typeof(AbilityBallUseMessage), this);
		EventController.AddListener(typeof(AbilityCooldownMessage), this);
		EventController.AddListener(typeof(InitializeAbilityBall), this);
	}

	private void OnDisable()
	{
		EventController.RemoveListener(typeof(AbilityBallUseMessage), this);
		EventController.RemoveListener(typeof(AbilityCooldownMessage), this);
        EventController.RemoveListener(typeof(InitializeAbilityBall), this);
    }

    public override void HandleEvent(EventMessage e)
	{
		if (e is AbilityCooldownMessage cooldownMessage)
		{
			int index = cooldownMessage.ability;
			float percent = cooldownMessage.amount;

			if (index < playerMiniCooldowns.Length)
				playerMiniCooldowns[index].UpdateCooldown(percent);

			if (index < playerProfileCooldowns.Length)
				playerProfileCooldowns[index].UpdateCooldown(percent);
		}
		else if (e is AbilityBallUseMessage useMessage)
		{
			int index = useMessage.slotID;

			if (index < playerMiniCooldowns.Length)
				playerMiniCooldowns[index].UseAbility();

			if (index < playerProfileCooldowns.Length)
				playerProfileCooldowns[index].UseAbility();
		}
        else if (e is InitializeAbilityBall initMessage)
        {
            int index = (int)initMessage.ability;
            int slot = -1;

            if (index > 0 && index < 3)
            {
                slot = 0;
            }
            else if (index > 2 && index < 5)
            {
                slot = 1;
            }

            if (slot != -1)
            {
                playerMiniCooldowns[slot].SetIcon(allMiniIcons[index - 1]);
                playerProfileCooldowns[slot].SetIcon(allProfileIcons[index - 1]);
            }
        }
	}
}
