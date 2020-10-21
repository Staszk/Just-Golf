using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ImageEffectsController : EventListener
{
    [SerializeField] private Shader grayscale = null;
    [SerializeField] private ParticleSystem splatter = null;

    private Cinemachine.CinemachineImpulseSource shakeSource;

    private Material[] mats;

    public bool IsGrayscale { get; set; } = true;

    private void Start()
    {
        mats = new Material[1];
        mats[0] = new Material(grayscale);

        shakeSource = GetComponent<Cinemachine.CinemachineImpulseSource>();
    }

    private void OnEnable()
    {
        EventController.AddListener(typeof(ClientDeathMessage), this);
        EventController.AddListener(typeof(ClientRespawnMessage), this);
        EventController.AddListener(typeof(GolfStrokeMessage), this);
		EventController.AddListener(typeof(GameStartMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(ClientDeathMessage), this);
        EventController.RemoveListener(typeof(ClientRespawnMessage), this);
        EventController.RemoveListener(typeof(GolfStrokeMessage), this);
		EventController.RemoveListener(typeof(GameStartMessage), this);
    }

    public override void HandleEvent(EventMessage e)
    {
        if (e is GolfStrokeMessage stroke)
        {
            Shake(stroke.power);
        }
        else if (e is ClientDeathMessage || e is ClientRespawnMessage || e is GameStartMessage)
        {
            ToggleGrayScale();
        }
    }

    private void PlaySplatter(float duration)
    {
        splatter.Play();
    }

    private void ToggleGrayScale()
    {
        IsGrayscale = !IsGrayscale;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (IsGrayscale)
            Graphics.Blit(source, destination, mats[0]);
        else
            Graphics.Blit(source, destination);
    }

    private void Shake(float power)
    {
        shakeSource.m_ImpulseDefinition.m_AmplitudeGain = Mathf.Lerp(0.05f, .5f, power / 100);
        shakeSource.GenerateImpulse();
    }
}
