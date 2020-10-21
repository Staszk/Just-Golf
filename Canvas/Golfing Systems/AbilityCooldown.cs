using UnityEngine.UI;
using UnityEngine;

public class AbilityCooldown : MonoBehaviour
{
    private static Color ICON_ACTIVE_COLOR = Color.white;
    private static Color ICON_ON_COOLDOWN_COLOR = new Color(1f, 1f, 1f, 0.3f);
    private static Color ICON_READY_COLOR = new Color(1f, 1f, 1f, 0.6f);

    [SerializeField] private Sprite[] meterSprites = null;

    [SerializeField] private Image icon = null;
    [SerializeField] private VisualMeter cooldownMeter = null;

    private bool isCoolingDown;

    private void Start()
    {
        EndCooldown();
    }

    public void SetIcon(Sprite spr)
    {
        icon.sprite = spr;
    }

    public void UseAbility()
    {
        // Change to yellow background
        cooldownMeter.ChangeSprite(meterSprites[1]);
        // Change icon to full white
        icon.color = ICON_ACTIVE_COLOR;
    }

    private void StartCooldown()
    {
        icon.color = ICON_ON_COOLDOWN_COLOR;
        cooldownMeter.ChangeSprite(meterSprites[0]);
        isCoolingDown = true;
    }

    private void EndCooldown()
    {

        icon.color = ICON_READY_COLOR;

        isCoolingDown = false;
    }

    public void UpdateCooldown(float percent)
    {
        if (!isCoolingDown && percent != 1f)
        {
            StartCooldown();
        }

        cooldownMeter.SetFill(percent);

        // If cooldown is over
        if (percent >= 1.0f && isCoolingDown)
        {
            EndCooldown();
        }
    }
}
