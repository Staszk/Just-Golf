using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrentLobbyScript : MonoBehaviour
{
    [SerializeField] private GameObject clientText;
    [SerializeField] private GameObject hostBtn;
   // [SerializeField] private TextMeshProUGUI connectText;
    [SerializeField] private GameObject myTeamPanel;
    [SerializeField] private GameObject enemyTeamPanel;

    private int myTeam = -1;
    private int id = -1;
    private NetworkStoredData data;

    public void Init(int id)
    {        
        this.id = id;
        clientText.SetActive(false);
        hostBtn.SetActive(false);
        data = NetworkStoredData.instance;
    }

    public void TurnOnButton(){
        hostBtn.SetActive(true);
    }

    public void TurnOnText()
    {
        clientText.SetActive(true);
    }

    public void SetConnectedText(List<int> num)
    {
        int enemyIndex = 0;
        int teamIndex = 1;
        myTeam = data.GetTeam(id);

        for (int i = 0; i < num.Count; i++)
        {
            if(data.GetTeam(i) == -1 || i == id) { continue; }

            if(myTeam == data.GetTeam(i))
            {
                myTeamPanel.transform.GetChild(teamIndex).gameObject.SetActive(true);
                teamIndex++;
            }
            else
            {
                enemyTeamPanel.transform.GetChild(enemyIndex).gameObject.SetActive(true);
                enemyIndex++;
            }
        }   
        
    }
}
