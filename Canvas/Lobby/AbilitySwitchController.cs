using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySwitchController : MonoBehaviour
{
    #region Enums/Structs
    public enum AbilityType
    {
        Offense = 0, 
        Defense
    }
    #endregion

    public AbilityType type;
    public Sprite[] icons;

    private int currentIcon = 0;
    private Image iconViewer;
    private int ID = -1;
    // Start is called before the first frame update

      

    void Start()
    {
        iconViewer = transform.GetChild(0).gameObject.GetComponent<Image>();
    }


    public void SwitchForward()
    {
        currentIcon = (currentIcon + 1) % icons.Length;
        UpdateAbility();
    }

    public void SwitchBackward()
    {
        currentIcon--;
        if (currentIcon < 0) { currentIcon = icons.Length - 1; }
        UpdateAbility();
    }

    public void UpdateAbility()
    {
        iconViewer.sprite = icons[currentIcon];
        EventController.FireEvent(new SendAbilityBallUpdate((int)type, ID, currentIcon));
    }

    public void SetID(int id) { ID = id; }

}
