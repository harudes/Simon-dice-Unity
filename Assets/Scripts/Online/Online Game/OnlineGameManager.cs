using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class OnlineGameManager : MonoBehaviourPunCallbacks
{
    public static OnlineGameManager instance;

    #region Private Fields

    [SerializeField]
    TMP_Text orderText, scoreText, startCountdownText;

    [SerializeField]
    Slider orderTimeBar;

    [SerializeField]
    GameObject startButton;

    List<string> playerNames = new List<string>();
    public string playerName;

    OnlineOrderGenerator orderGenerator;
    OnlineVoiceController voiceController;
    MarkersController markersController;
    
    float markerTime = 5.0f, voiceTime = 7.0f;
    int playersReady = 0;

    #endregion

    #region Photon Callbacks

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MenuScene");
    }


    #endregion    

    #region MonoBehaviour CallBacks

    public void Awake()
    {
        orderGenerator = GetComponent<OnlineOrderGenerator>();
        voiceController = GetComponent<OnlineVoiceController>();
        markersController = GetComponent<MarkersController>();
    }

    void Start()
    {
        Debug.Log(PlayerPrefs.GetString("gameId"));

        instance = this;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            playerNames.Add(p.NickName);
        }

        playerName = PhotonNetwork.LocalPlayer.NickName;

        if (PhotonNetwork.IsMasterClient)
        {            
            startButton.SetActive(true);
        }   
    }

    private void Update()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Lifes"))
            if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Lifes"] <= 0)
                orderText.text = "Te has quedado sin vidas, espera a que termine la ronda.";
    }

    #endregion

    #region Custom

    public void MasterStartCountdown()
    {
        startButton.SetActive(false);
        photonView.RPC("GameStartCountdown", RpcTarget.AllViaServer);
    }

    IEnumerator StartGame()
    {        
        int orderCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["LevelRounds"];
        
        while (orderCount > 0)
        {
            //shared or individual
            int targetType = Random.Range(0, 2);
            
            //case shared order
            if (targetType == 0)
            {                
                List<string> playersInOrder = new List<string>();
                for (int i = 0; i < playerNames.Count; i++)
                {
                    if(Random.Range(0, 2) == 1)
                    {
                        playersInOrder.Add(playerNames[i]);
                    }
                }

                if (playersInOrder.Count == 0)
                {
                    playersInOrder.Add(playerNames[Random.Range(0, playerNames.Count)]);
                }

                OrderProperties sharedOrder = orderGenerator.GenerateSharedOrder(playersInOrder);
                this.photonView.RPC("ReceiveSharedOrder", RpcTarget.AllViaServer, sharedOrder.type ,sharedOrder.isSimon, sharedOrder.text, sharedOrder.players.ToArray(), sharedOrder.color, sharedOrder.direction, sharedOrder.word);                    
            }
            //case individual order
            else
            {
                this.photonView.RPC("GenerateIndividualOrder", RpcTarget.AllViaServer);
            }

            orderCount--;

            yield return new WaitForSecondsRealtime(voiceTime);
            while (playersReady < PhotonNetwork.CountOfPlayersInRooms)
            {
                yield return null;
            }
            playersReady = 0;
        }

        this.photonView.RPC("SendScoreToDatabase", RpcTarget.AllViaServer);
        yield return new WaitForSecondsRealtime(5.0f);
        PhotonNetwork.LoadLevel("GameLobbyScene");
    }

    IEnumerator StartOrderCountdown(float waitTime)
    {        
        float maxTime = waitTime;
        float actualTime = waitTime;     
        orderTimeBar.value = waitTime;
        while (actualTime > 0)
        {
            actualTime -= Time.deltaTime;            
            orderTimeBar.value = actualTime / maxTime;
            yield return null;
        }

        this.photonView.RPC("PlayerReady", RpcTarget.AllViaServer);
    }

    [PunRPC]
    void PlayerReady()
    {
        playersReady++;
    }

    [PunRPC]
    IEnumerator GameStartCountdown()
    {
        float countdownTime = 5.0f;

        while (countdownTime > 0)
        {
            startCountdownText.text = countdownTime.ToString();
            yield return new WaitForSeconds(1.0f);
            countdownTime--;
        }

        startCountdownText.text = string.Empty;
        if (PhotonNetwork.IsMasterClient)
        {            
            StartCoroutine(StartGame());
        }  
    }

    public void UpdatePlayersScore(int points)
    {
        photonView.RPC("UpdateLocalScore", RpcTarget.AllViaServer, points);
    }

    [PunRPC]
    void UpdateLocalScore(int points)
    {
        int temp = int.Parse(scoreText.text);
        temp += points;
        scoreText.text = temp.ToString();
    }

    [PunRPC]
    void ReceiveSharedOrder(int type, bool isSimon, string text, string[] players, int color, int direction, string word)
    {
        if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Lifes"] > 0)
            orderText.text = text;
        List<string> orderPlayers = new List<string>(players);
        float waitTime;
        if(type == 0)
        {
            waitTime = markerTime;
        }
        else
        {
            waitTime = voiceTime;
        }

        //Set time bar
        StartCoroutine(StartOrderCountdown(waitTime));
        
        if(type == 0)
        {
            markersController.MovementOrder(color, isSimon, orderPlayers, direction, waitTime);
        } 
        else
        {            
            voiceController.CheckVoice(word, isSimon, orderPlayers, waitTime);
        }        
    }

    [PunRPC]
    void GenerateIndividualOrder()
    {        
        OrderProperties individualOrder = orderGenerator.GenerateIndividualOrder(playerName);
        if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Lifes"] > 0)
            orderText.text = individualOrder.text;

        float waitTime;
        if(individualOrder.type == 0)
        {
            waitTime = markerTime;
        }
        else
        {
            waitTime = voiceTime;
        }

        //Set time bar
        StartCoroutine(StartOrderCountdown(waitTime));

        if (individualOrder.type == 0)
        {
            markersController.MovementOrder(individualOrder.color, individualOrder.isSimon, individualOrder.players, individualOrder.direction, waitTime);
        }
        else
        {
            voiceController.CheckVoice(individualOrder.word, individualOrder.isSimon, individualOrder.players, waitTime);
        }
    }    

    [PunRPC]
    void SendScoreToDatabase()
    {
        string finalScore = scoreText.text;
        StartCoroutine(UpdateScore(finalScore));
    }

    IEnumerator UpdateScore(string score)
    {
        int level = (int)PhotonNetwork.CurrentRoom.CustomProperties["Stage"] - 1;
        string uri = "https://us-central1-simon-dice-76365.cloudfunctions.net/api/gamess/" + PlayerPrefs.GetString("gameId") + "/" + score + "/" + level.ToString();
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
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
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
            }
        }
    }
    #endregion
}
