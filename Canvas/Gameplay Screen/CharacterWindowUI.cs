using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterWindowUI : MonoBehaviour
{
    [SerializeField] private Image portrait;
    [SerializeField] private Sprite awayTeamPortrait;
    [SerializeField] private Image colorRing;
    [SerializeField] private Sprite awayTeamRing;

    public void ChangeToAwayTeam()
    {
        portrait.sprite = awayTeamPortrait;
        colorRing.sprite = awayTeamRing;
    }
}
