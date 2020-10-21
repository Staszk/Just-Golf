using UnityEngine;
using System.Collections;

public class WorldSpaceVFX : EventListener
{
    public enum WorldVFXType { bomb, ice, shield, impact };

    private const int MAX_VFX = 6;
    private const int MAX_ICE_VFX = MAX_VFX * 3;
    private const int MAX_IMPACT_VFX = MAX_VFX * 6;

    [SerializeField] private ParticleSystem bombParticle = null;
    [SerializeField] private ParticleSystem iceParticle = null;
    [SerializeField] private ParticleSystem ballImpactParticle = null;

    private ParticleSystem[] bombVFX;
    private int bombIndex;
    private ParticleSystem[] iceVFX;
    private int iceIndex;
    private ParticleSystem[] impactVFX;
    private int impactIndex;

    private void Start()
    {
        PrepareVFX();
    }

    private void OnEnable()
    {
        // Subscribe to events
        EventController.AddListener(typeof(WorldVFXMessage), this);
    }

    private void OnDisable()
    {
        // Unsubscribe to events
        EventController.RemoveListener(typeof(WorldVFXMessage), this);
    }

    public override void HandleEvent(EventMessage e)
    {
        if (e is WorldVFXMessage vfx)
        {
            switch (vfx.type)
            {
                case WorldVFXType.bomb:
                    bombVFX[bombIndex].transform.position = vfx.worldPosition;
                    bombVFX[bombIndex].Play();
                    bombIndex = (bombIndex + 1) % MAX_VFX;
                    break;
                case WorldVFXType.ice:
                    iceVFX[iceIndex].transform.SetParent(transform);
                    iceVFX[iceIndex].transform.position = vfx.worldPosition;
                    iceVFX[iceIndex].transform.SetParent(vfx.parent);
                    iceVFX[iceIndex].Play();
                    iceIndex = (iceIndex + 1) % MAX_ICE_VFX;
                    break;
                case WorldVFXType.impact:
                    impactVFX[impactIndex].transform.position = vfx.worldPosition;
                    impactVFX[impactIndex].Play();
                    impactIndex = (impactIndex + 1) % MAX_IMPACT_VFX;
                    break;
                default:
                    break;
            }
        }
    }

    private void PrepareVFX()
    {
        bombVFX = new ParticleSystem[MAX_VFX];
        bombIndex = 0;

        iceVFX = new ParticleSystem[MAX_ICE_VFX];
        iceIndex = 0;

        impactVFX = new ParticleSystem[MAX_IMPACT_VFX];
        impactIndex = 0;

        for (int i = 0; i < MAX_VFX; i++)
        {
            bombVFX[i] = Instantiate(bombParticle, transform);
        }

        for (int i = 0; i < MAX_ICE_VFX; i++)
        {
            iceVFX[i] = Instantiate(iceParticle, transform);
        }

        for (int i = 0; i < MAX_IMPACT_VFX; i++)
        {
            impactVFX[i] = Instantiate(ballImpactParticle, transform);
        }
    }
}
