using UnityEngine;
using System.Collections;

public class StrokeChangeUI : MonoBehaviour
{
    private StrokeChangeController parent;
    private TMPro.TMP_Text text;

    public void SetUp(StrokeChangeController c)
    {
        parent = c;
        text = GetComponent<TMPro.TMP_Text>();
        Done();
    }

    public void Set(string s, Color c)
    {
        text.text = s;
        text.color = c;
        gameObject.SetActive(true);
    }

    public void Done()
    {
        parent.Add(this);
        gameObject.SetActive(false);
    }

}
