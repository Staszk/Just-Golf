using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class ScoreboardEntity
{
    public GameObject border;
    public Image gunIcon;
    public TMP_Text currentStrokesText, personalBestText;
    public TMP_Text killsText, deathsText;
}

public class ScoreboardController : MonoBehaviour
{
    #region Singleton
    public static ScoreboardController instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Release()
    {
        instance = null;
    }
    #endregion

    public class DataHolder
    {
        public int id, current, kills, deaths;
        public int? best = null;

        public DataHolder(int id)
        {
            this.id = id;
            current = kills = deaths = 0;
            best = null;
        }
    }

    class Comparison : System.IComparable<Comparison>
    {
        public int id;
        public int? best;

        public Comparison(int id, int? best)
        {
            this.id = id;
            this.best = best;
        }

        public int CompareTo(Comparison other)
        {
            if (other.best == null && best == null) // both null
                return 0;
            else if (other.best == null && best != null) // other is null, this is not
                return -1;
            else if (other.best != null && best == null) // other is not, this is null
                return 1;
            else if (other.best == best) // same
                return 0;
            else if (other.best > best)
                return -1;
            else
                return 1;
        }
    }

    private GameplayUIController gameScreen;
    int networkID = 0;
    int IDofWinner = -1;

    [SerializeField] private ScoreboardEntity[] entities = null;
    [SerializeField] private Sprite[] gunIcons = null;
    private DataHolder[] data = null;
    private int[] mappings = null;

    public void SetUp(GameplayUIController gameUI)
    {
        gameScreen = gameUI;

        data = new DataHolder[4];

        for (int i = 0; i < data.Length; i++)
        {
            data[i] = new DataHolder(i);
        }

        mappings = new int[4] { 0, 1, 2, 3 };

        if (NetworkManager.instance)
        {
            networkID = NetworkManager.instance.GetId();
            entities[networkID].border.SetActive(true);

            //NetworkManager.instance.GetPlayerOfID(networkID).GetComponent<Winner>().BecomeWinner(true); //remove crown for now since it breaks networking 
        }
        else
        {
            entities[0].border.SetActive(true);
        }

        CheckOrder();
    }


    public void UpdateEntity(int playerIndex, int currentStrokesVal, int? personalBestVal, int killsVal, int deathsVal)
    {
        UpdatePoints(playerIndex, currentStrokesVal, personalBestVal);
        UpdateKillsDeaths(playerIndex, killsVal, deathsVal);
        CheckOrder();
    }

    private void UpdatePoints(int playerIndex, int currentStrokesVal, int? personalBestVal)
    {
        data[playerIndex].current = currentStrokesVal;
        data[playerIndex].best = personalBestVal;
        CheckOrder();
    }

    private void UpdateKillsDeaths(int playerIndex, int kills, int deaths)
    {
        data[playerIndex].kills = kills;
        data[playerIndex].deaths = deaths;
    }

    public DataHolder GetScoreInfo(int id)
    {
        return data[id];
    }

    private void CheckOrder()
    {
        Comparison[] temp = new Comparison[4]
        {
            new Comparison(data[0].id, data[0].best),
            new Comparison(data[1].id, data[1].best),
            new Comparison(data[2].id, data[2].best),
            new Comparison(data[3].id, data[3].best)
        };

        System.Array.Sort(temp, delegate (Comparison a, Comparison b) { return a.CompareTo(b); });

        mappings[0] = temp[0].id;
        mappings[1] = temp[1].id;
        mappings[2] = temp[2].id;
        mappings[3] = temp[3].id;

        int rank = 0;

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i].id == networkID)
            {
                rank = i;
                break;
            }
        }

        UpdateGraphics(rank);
    }

    private void UpdateGraphics(int rank)
    {
        for (int i = 0; i < entities.Length; i++)
        {
            entities[i].currentStrokesText.text = data[mappings[i]].current.ToString();
            entities[i].personalBestText.text = data[mappings[i]].best != null ? data[mappings[i]].best.ToString() : "X";
            entities[i].killsText.text = data[mappings[i]].kills.ToString();
            entities[i].deathsText.text = data[mappings[i]].deaths.ToString();

            entities[i].gunIcon.sprite = gunIcons[data[mappings[i]].id];
            entities[i].border.SetActive(data[mappings[i]].id == networkID);
        }

        gameScreen.ShowRank(rank);

        if (data[mappings[0]].id != IDofWinner && IDofWinner >= 0)
        {
            NetworkManager.instance.GetPlayerOfID(IDofWinner).GetComponent<Winner>().BecomeWinner(false);
            IDofWinner = data[mappings[0]].id;
            NetworkManager.instance.GetPlayerOfID(IDofWinner).GetComponent<Winner>().BecomeWinner(true);
        }

        if(IDofWinner < 0 && NetworkManager.instance)
        {
            IDofWinner = networkID;
            NetworkManager.instance.GetPlayerOfID(networkID).GetComponent<Winner>().BecomeWinner(true);            
        }
    }
}
