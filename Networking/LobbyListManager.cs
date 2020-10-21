using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListManager : MonoBehaviour
{
    public GameObject buttonPrefab;

    public void RemoveAllChildren()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void CreateButton(int index, string name)
    {
        GameObject button = Instantiate(buttonPrefab);
        Button theButton = button.GetComponent<Button>();
        LobbyButton lobbybutton = button.GetComponent<LobbyButton>();
        button.transform.SetParent(transform, false);
        button.transform.GetChild(0).gameObject.GetComponent<Text>().text = name;
        theButton.onClick.AddListener(delegate { LobbyConnect(lobbybutton); });
        lobbybutton.Index = index;
        lobbybutton.name = name;
    }

    public void LobbyConnect(LobbyButton lBut)
    {
        //print("Hell ya this works");
        EventController.FireEvent(new JoinLobby(lBut.Index));
    }
}
