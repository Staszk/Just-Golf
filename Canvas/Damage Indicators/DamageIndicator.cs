using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class DamageIndicator : MonoBehaviour
{
    //[SerializeField] private Image damageCircle = null;
    [SerializeField] private Image damageOverlay = null;

    private Coroutine displayIndicator = null;

    public bool LowHealth { get; private set; } = false;

    public void MarkLowHealth(bool lowHealth)
    {
        LowHealth = lowHealth;

        if (!lowHealth)
        {
            if (displayIndicator != null)
            {
                StopCoroutine(displayIndicator);
            }

            displayIndicator = StartCoroutine(Display(false, 0.25f));
        }
    }

    public void Show()
    {
        damageOverlay.gameObject.SetActive(true);

        if (displayIndicator != null)
        {
            StopCoroutine(displayIndicator);
        }

        displayIndicator = StartCoroutine(Display());
    }

    public void Hide()
    {
        if(displayIndicator !=null )
            StopCoroutine(displayIndicator);
        damageOverlay.gameObject.SetActive(false);
    }

    private IEnumerator Display(bool displayCircle = true, float scale = 1f)
    {
        float time = 0f;
        float maxTime = 1.5f * scale;

        damageOverlay.color = Color.white;

        while (time != maxTime)
        {
            time = Mathf.Clamp(time + Time.deltaTime, 0, maxTime);

            float a = Vector2.Lerp(Vector2.right, Vector2.zero, time / maxTime).x;

            Color c = new Color(1, 1, 1, a);

            if (!LowHealth)
                damageOverlay.color = c;


            yield return null;
        }

        if (!LowHealth)
            damageOverlay.gameObject.SetActive(false);
    }
}
