///-------------------------------------------------------------------------
///   Copyright Wired Visions 2019
///   Class:            PlayerHealth
///   Description:      Handles losing and gaining health for the player.
///                     Checks for death state, and on death runs logic for
///                     respawning.
///                     Implements IHaveHealth interface.
///   Author:           Parker Staszkiewicz
///   Contributor(s):   Mark Botaish
///-------------------------------------------------------------------------

using UnityEngine;

public class PlayerHealth : EntityHealth
{    
    private PlayerMovement playerMovement;
    private float lowHealthAmount = 20f;
    private float lifeTime = 0;

    private Vector3 center = Vector3.zero;

    protected override void Awake()
    {
        base.Awake();
        playerMovement = GetComponent<PlayerMovement>();
    }

    protected override void Start()
    {
        base.Start();

        if (network)
            MyID = network.GetId();
    }

    private void Update()
    {
        lifeTime += Time.deltaTime;
    }

    public override void TakeDamage(float damage, int shooterID = 0)
    {
        if (CheckShield()) { return; }

        if (!IsDead && MyID != shooterID)
        {
            lastShotID = shooterID;
            Health -= damage;
            Health = Mathf.Clamp(Health, 0, maxHealth);

            //GameplayUIController.instance.DisplayHealth((int)Health, (int)maxHealth);
            EventController.FireEvent(new PlayerHealthChangeMessage((int)Health));

            Vector3 damageOrigin = network != null ? network.GetNetworkPlayerPosition(Mathf.Abs(shooterID)) : Vector3.zero;
            //GameplayUIController.instance.ShowDamageReceived(damageOrigin);
            SoundManager.PlaySound("Take Damage");
            SoundManager.PlaySound("Thud Two");
            SoundManager.PlaySound("Light Thud");
            if (network && network.IsHost())
            {
                network.SendDamageMessage(gameObject, lastShotID);
            }

            if (Health <= 0){
                IsDead = true;
                Die();
                SoundManager.PlaySound("Grunt Impact");
                SoundManager.PlaySound("Thud");
                SoundManager.PlaySound("Wet Impact");
                SoundManager.PlaySound("Death Sound");
                EventController.FireEvent(new PersonalScoreMessage(lastShotID, PersonalScoreManager.TYPE.KILL_PLAYER));
            }

            if (Health == lowHealthAmount){
                SoundManager.PlaySound("Low Health");
            }

            if (Health > lowHealthAmount) {
                SoundManager.StopSound("Low Health");
            }
            
        }
    }

    public override void GainHealth(float amount)
    {
        base.GainHealth(amount);
        GameplayUIController.instance.DisplayHealth((int)Health, (int)maxHealth);
    }

    private void Die()
    {
        EventController.FireEvent(new TrackSuperlativeMessage(SuperlativeController.Superlative.Enduring, SuperlativeController.ConditionFlag.identity, lifeTime));
        EventController.FireEvent(new TrackSuperlativeMessage(SuperlativeController.Superlative.TheDeadGuy, SuperlativeController.ConditionFlag.additive, 1));
        EventController.FireEvent(new ClientDeathMessage(lastShotID));

        RespawnManager.instance.Death(this, respawnDelay);
       
    }

    public override void ResetPlayer(Vector3 position)
    {
        base.ResetPlayer(position);
        center.y = position.y;            
        gameObject.transform.position = position;
        playerMovement.LookAtCenter();
        //GameplayUIController.instance.DisplayHealth((int)Health, (int)maxHealth);  

        EventController.FireEvent(new PlayerHealthChangeMessage((int)Health));
        lifeTime = 0;
    }


    public override void Respawn(Vector3 location)
    {
        base.Respawn(location);
        playerMovement.LookAtCenter();
    }
}
