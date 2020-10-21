using UnityEngine;
using System.Collections;

public class PlayerParticleEffects : MonoBehaviour
{
    [SerializeField] private ParticleSystem confusedEffect = null;
    [SerializeField] private ParticleSystem inkEffect = null;

    private int id;

    NetworkManager network;

    public void Start()
    {
        id = GetComponent<PlayerScoreTracker>().ID;
        network = NetworkManager.instance;
    }

    private void OnEnable()
    {
        NetworkItemManager.EventClientParticleEffect += StartEffect;
        UsableItem.EventClientParticleEffect += StartEffect;
    }

    private void OnDisable()
    {
        NetworkItemManager.EventClientParticleEffect -= StartEffect;
        UsableItem.EventClientParticleEffect -= StartEffect;
    }

    private void StartEffect(ParticleEffectMessage e)
    {
        if (e.IdOfUser == id && network)
            return;

        switch (e.effectType)
        {
            case ParticleEffectMessage.Effect.Inked:
                inkEffect.Play();
                break;
            case ParticleEffectMessage.Effect.Confused:
                confusedEffect.Play();
                break;
            default:
                break;
        }
    }
}
