using UnityEngine;
using System.Collections;
using TMPro;

public class DamagePopOff : MonoBehaviour
{
    [SerializeField] private TMP_Text damageText = null;
    [SerializeField] private Color normalColor = Color.yellow;
    [SerializeField] private Color criticalColor = Color.red;

    private Animator anim;

    public Vector3 worldPos;

    private GameplayUIController parent;

    public void Init(GameplayUIController p)
    {
        parent = p;

        anim = GetComponent<Animator>();

        gameObject.SetActive(false);
    }

    public void Use(Vector3 worldPos, string damageString, bool critical, float scale)
    {
        damageText.text = damageString;
        damageText.color = !critical ? normalColor : criticalColor;

        this.worldPos = worldPos;

        GetComponent<RectTransform>().localScale = Vector3.one * scale;

        gameObject.SetActive(true);

        int animation = Random.Range(1, 4);
        anim.SetInteger("animType", animation);
    }

    public void Done()
    {
        parent.StopPopOff(this);
        gameObject.SetActive(false);
    }
}
