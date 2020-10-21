using System.Collections.Generic;
using UnityEngine;

public class HoleManager : MonoBehaviour
{
    [SerializeField]
    private List<Hole> holeList = new List<Hole>();

    [SerializeField]
    private List<GameObject> arrowList = new List<GameObject>();

    private int currentHoleIndex;
    private NetworkManager network;

    // Start is called before the first frame update
    void Start()
    {
        currentHoleIndex = 0;
        //holeList[0].ActivateHole();
    }

    public void ChangeHole(bool shouldFireEvent = true)
    {
        if (network && !network.IsHost()) { return; }

        holeList[currentHoleIndex].DeactivateHole();
        arrowList[currentHoleIndex].SetActive(false);
        currentHoleIndex++;

        if (currentHoleIndex >= holeList.Count)
        {
            currentHoleIndex = 0;
        }

        holeList[currentHoleIndex].ActivateHole();
        arrowList[currentHoleIndex].SetActive(true);
        if (shouldFireEvent) { EventController.FireEvent(new ChangeHoleEvent()); }
        
    }
}
