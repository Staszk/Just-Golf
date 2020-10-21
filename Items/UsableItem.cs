using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UsableItem : MonoBehaviour, IUsableItem
{
    public static event System.Action EventItemUseStart = delegate { };
    public static event System.Action EventItemUseDone = delegate { };
    public static event System.Action<float> EventItemInUse = delegate { };
    public static event System.Action<ParticleEffectMessage> EventClientParticleEffect = delegate { };


    [SerializeField] private Sprite itemSprite = null;
    [SerializeField] private ItemBoxController.Items itemType = ItemBoxController.Items.None;

    public Sprite ItemSprite { get { return itemSprite; } }
    public ItemBoxController.Items ItemType { get { return itemType; } }

    public bool HasBeenUsed { get; protected set; }

    private ItemBoxController parent;

    public void Init(ItemBoxController controller)
    {
        parent = controller;
    }

    public void ResetUse()
    {
        HasBeenUsed = false;
    }

    public virtual void Use(PlayerController pc)
    {
        HasBeenUsed = true;
        EventItemUseStart();
        SoundManager.PlaySound("Item Use");
        Debug.Log("Used Item: " + name);
    }

    protected void InUse(float percentLeft)
    {
        EventItemInUse(percentLeft);
    }

    protected void UseDone()
    {
        ResetUse();
        parent.TakeItem(this);
        EventItemUseDone();
        Debug.Log("Done Using Item: " + name);
    }

    protected void VFXEffectEvent(ParticleEffectMessage.Effect effect)
    {
        EventClientParticleEffect(new ParticleEffectMessage(effect, 0));
    }
}
