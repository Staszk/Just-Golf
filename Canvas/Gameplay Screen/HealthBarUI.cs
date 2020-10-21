using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
	[SerializeField] private Sprite[] awayTeamColors = null;
	
	[SerializeField] private Image[] teamColorSegments = null;
	[SerializeField] private GameObject[] shieldSegments = null;

	public void ChangeToAwayTeam()
	{
		for (int i = 0; i < teamColorSegments.Length; i++)
		{
			teamColorSegments[i].sprite = awayTeamColors[i];
		}
	}

	public void DisplayHealth(int health)
	{
		int visibleIndex = Mathf.RoundToInt(health / 25) - 1;

		for (int i = 3; i >= 0; i--)
		{
			bool show = (i <= visibleIndex);

			teamColorSegments[i].gameObject.SetActive(show);
			shieldSegments[i].SetActive(show);
		}
	}

	public void ShowShield(bool shieldActive)
	{
        for (int i = 3; i >= 0; i--)
        {
            shieldSegments[i].SetActive(shieldActive);
        }
    }
}
