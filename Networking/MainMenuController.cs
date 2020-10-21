using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuController : EventListener
{

    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject openLobiiesPanel;
    [SerializeField] private GameObject myLobbyPanel;
    [SerializeField] private LobbyListManager lobbyList;
    [SerializeField] private CurrentLobbyScript currentLobby;
    [SerializeField] private GameObject warningScreen;
    [SerializeField] private GameObject namePanel;
    [SerializeField] private AbilitySwitchController offensePanel;
    [SerializeField] private AbilitySwitchController defensePanel;

    private TMP_InputField inputField;

    private GameObject currentPanel;

    //Change this please
    private void OnEnable()
    {
        EventController.AddListener(typeof(LobbyStateChange), this);
        EventController.AddListener(typeof(LobbyOpened), this);
        EventController.AddListener(typeof(JoinLobby), this);
        EventController.AddListener(typeof(ConnectedUsersUpdate), this);
        EventController.AddListener(typeof(CloseWarningMessage), this);
        EventController.AddListener(typeof(OpenWarningMessage), this);
        EventController.AddListener(typeof(NameMessage), this);
        EventController.AddListener(typeof(StartLobby), this);
        EventController.AddListener(typeof(IDMessage), this);
        EventController.AddListener(typeof(EnableCurrentPanel), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(LobbyStateChange), this);
        EventController.RemoveListener(typeof(LobbyOpened), this);
        EventController.RemoveListener(typeof(JoinLobby), this);
        EventController.RemoveListener(typeof(ConnectedUsersUpdate), this);
        EventController.RemoveListener(typeof(CloseWarningMessage), this);
        EventController.RemoveListener(typeof(OpenWarningMessage), this);
        EventController.RemoveListener(typeof(NameMessage), this);
        EventController.RemoveListener(typeof(StartLobby), this);
        EventController.RemoveListener(typeof(IDMessage), this);
        EventController.RemoveListener(typeof(EnableCurrentPanel), this);
    }


    // Start is called before the first frame update
    void Awake()
    {
        startPanel.SetActive(false);
        openLobiiesPanel.SetActive(false);
        myLobbyPanel.SetActive(false);
        namePanel.SetActive(false);

        InitWarning("Connecting To Online Services...");

        currentPanel = startPanel;
        inputField = namePanel.GetComponentInChildren<TMP_InputField>();
    }

    public void InitWarning(string msg)
    {
        warningScreen.SetActive(true);
        warningScreen.GetComponentInChildren<TextMeshProUGUI>().text = msg;
    }

    public void AddNameMessage(System.Action<string, GameObject> action, string title)
    {
        namePanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = title;
        inputField.onEndEdit.AddListener(delegate { action(inputField.text, namePanel); });
    }

    public override void HandleEvent(EventMessage e)
    {
        if(e is LobbyStateChange)
        {
            lobbyList.RemoveAllChildren();
        }else if(e is LobbyOpened lobby)
        {
            lobbyList.CreateButton(lobby.index, lobby.name);

        }else if(e is JoinLobby)
        {
            ShowMyLobby();
        }else if(e is StartLobby)
        {
            ShowMyLobby();
            ShowHostButton();
        }
        else if (e is ConnectedUsersUpdate users)
        {
            currentLobby.SetConnectedText(users.teams);
        }
        else if (e is OpenWarningMessage warning)
        {
            InitWarning(warning.msg);
        }
        else if (e is CloseWarningMessage )
        {
            warningScreen.SetActive(false);
        }else if(e is NameMessage name)
        {
            namePanel.SetActive(true);
            AddNameMessage(name.action, name.title);
        }else if(e is IDMessage idMessage) 
        {
            offensePanel.SetID(idMessage.id);
            defensePanel.SetID(idMessage.id);
            currentLobby.Init(idMessage.id);         
        }else if (e is EnableCurrentPanel)
        {
            EnableCurrentPanel();
        }
    }

    public void DisableCurrentPanel()
    {
        currentPanel.SetActive(false);
    }
    public void EnableCurrentPanel()
    {
        currentPanel.SetActive(true);
    }

    public void ShowMyLobby()
    {
        currentPanel.SetActive(false);
        myLobbyPanel.SetActive(true);
        currentPanel = myLobbyPanel;
    }

    public void ShowOpenLobbies()
    {
        currentPanel.SetActive(false);
        openLobiiesPanel.SetActive(true);
        currentPanel = openLobiiesPanel;
        TurnOnText();
    }

    public void ShowHostButton()
    {
        currentLobby.TurnOnButton();
    }

    public void TurnOnText()
    {
        currentLobby.TurnOnText();
    }
}
