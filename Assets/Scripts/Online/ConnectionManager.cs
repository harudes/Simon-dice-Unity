using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TMP_Text playerName;

    [SerializeField]
    private GameObject onlineButton;

    [SerializeField]
    private PlayerListing playerListing;

    [SerializeField]
    private GameObject LoggedOnlineButtons;

    [SerializeField]
    private GameObject NotLoggedOnlineButtons;

    [SerializeField]
    private TMP_Text NickInput;
    // Start is called before the first frame update
    void Start()
    {
        string playerNickname = PlayerPrefs.GetString("NickName");
        if (playerNickname != "")
        {
            playerName.text = playerNickname;

            PhotonNetwork.GameVersion = "1.1";
            PhotonNetwork.NickName = playerNickname;
            PhotonNetwork.ConnectUsingSettings();

            NotLoggedOnlineButtons.SetActive(false);
        }
        else
        {
            LoggedOnlineButtons.SetActive(false);
        }
    }

    public override void OnConnectedToMaster()
    {
        print("Connected to server");
        print(PhotonNetwork.LocalPlayer.NickName);


        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.JoinLobby();
        LoggedOnlineButtons.SetActive(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnected from server, reason: " + cause.ToString());

        onlineButton.SetActive(false);
    }

    public override void OnCreatedRoom()
    {
        //base.OnCreatedRoom();
        StartCoroutine(CreateGameDB());
        print("Room successfully created");
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable { { "Stage", 1 }, { "LevelRounds", 10 }, { "MinimunScore", 1000 }, { "Difficulty", 0 } };
            Debug.Log(hash);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }
    }

    public void StablishNickname()
    {
        string playerNickname = NickInput.text;
        if(playerNickname != "")
        {
            PlayerPrefs.SetString("NickName", playerNickname);

            //LoggedOnlineButtons.SetActive(true);

            playerName.text = playerNickname;
            PhotonNetwork.GameVersion = "1.1";
            PhotonNetwork.NickName = playerNickname;
            PhotonNetwork.ConnectUsingSettings();

            NotLoggedOnlineButtons.SetActive(false);
        }
    }

    IEnumerator CreateGameDB()
    {
        string uri = "https://us-central1-simon-dice-76365.cloudfunctions.net/api/games";
        string gameId;
        
        using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, string.Empty))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                gameId = webRequest.downloadHandler.text;
                gameId = gameId.Substring(7, webRequest.downloadHandler.text.Length - 9);
                PlayerPrefs.SetString("gameId",gameId);
            }
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //base.OnCreateRoomFailed(returnCode, message);
        print("Room creation failed: " + message);
    }

    public override void OnJoinedRoom()
    {
        //base.OnJoinedRoom();
        SetCustomProperties();
        print("Successfully joined the room");
        print(PhotonNetwork.CurrentRoom.Name);
        playerListing.GetCurrentRoomPlayers();
    }

    public void StartOnlineGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //SetCustomProperties();
            SceneManager.LoadScene("OnlineScene");
        }
    }

    public void LoadLobbyScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //SetCustomProperties();
            SceneManager.LoadScene("GameLobbyScene");
        }
    }

    private void SetCustomProperties()
    {
        Debug.Log("Guardando custom properties "+ PhotonNetwork.LocalPlayer.NickName);
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable { {"Lifes",5 } };
        Debug.Log(hash);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("Cambio de propiedades");
        Debug.Log(targetPlayer.NickName);
        Debug.Log(changedProps);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
