using UnityEngine;
using UnityEngine.UI;

public class VisualMeter : MonoBehaviour
{
    public Image meter = null;

    public void SetFill(float percent)
    {
        meter.fillAmount = percent;
    }

    public void ChangeSprite(Sprite newSprite)
    {
        meter.sprite = newSprite;
    }
}
