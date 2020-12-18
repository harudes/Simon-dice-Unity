using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameLobbyController : MonoBehaviourPunCallbacks
{
    public static GameLobbyController Instance = null;

    List<Player> players = new List<Player>();
    List<int> playerLifes = new List<int> { 5, 5, 5 };

    [SerializeField]
    LobbyPlayerListing playerListing;

    [SerializeField]
    TMP_Text gameDataText;

    bool receivedGameData = false;

    private GameData gameData;

    private PhotonView PV;

    bool isGameOver = false;

    //private int continueButtonSituation = -1;

    public void Awake()
    {
        PV = GetComponent<PhotonView>();
        Instance = this;
    }

    [SerializeField]
    List<StageSummaryController> summaryControllers;

    void Start()
    {
        Debug.Log("Imprimiendo vidas del jugador");
        Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties);

        Debug.Log("Imprimiendo propiedades de la sala");
        Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties);

        List<KeyValuePair<int, Player>> playersList = new List<KeyValuePair<int, Player>>();
        foreach (KeyValuePair<int, Player> info in PhotonNetwork.CurrentRoom.Players)
        {
            playersList.Add(info);
        }
        playersList.Sort((x, y) => x.Key.CompareTo(y.Key));
        foreach (KeyValuePair<int, Player> info in playersList)
        {
            Debug.Log("Imprimiendo jugador " + info.Key.ToString());
            Debug.Log(info);
            Debug.Log(info.Value.CustomProperties);
            players.Add(info.Value);
        }

        ListPlayers();

        StartCoroutine(GetGameData());
    }

    void ListPlayers()
    {
        playerListing.InitializeListing(players);
    }

    void Update()
    {
        if (gameDataText.text == "No tienes vidas suficientes" && (int)PhotonNetwork.LocalPlayer.CustomProperties["Lifes"] > 0)
        {
            gameDataText.text = "";
        }
    }

    IEnumerator GetGameData()
    {
        string uri = "https://us-central1-simon-dice-76365.cloudfunctions.net/api/games/" + PlayerPrefs.GetString("gameId");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                receivedGameData = true;
                gameData = JsonUtility.FromJson<GameData>(webRequest.downloadHandler.text);
                GameLevel actualLevel = gameData.GetActualLevel();
                Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);

                PlayerPrefs.SetInt("Difficulty", actualLevel.difficulty);
                PlayerPrefs.SetInt("Rounds", actualLevel.rounds);
                PlayerPrefs.SetInt("MinimunScore", actualLevel.minimunScore);

                isGameOver = TestGameOver();


                TestGamePossibility();
                
                foreach (StageSummaryController summaryController in summaryControllers)
                {
                    summaryController.UpdateButtonColor();
                }
            }
        }
    }

    private bool TestGameOver()
    {
        int LastStage = (int)PhotonNetwork.CurrentRoom.CustomProperties["Stage"] - 1;
        if (GetStageMinimunScore(LastStage) <= GetStageLastScore(LastStage))
            return false;
        else
            return true;
    }

    public void TestGamePossibility()
    {
        bool isPosible = true;
        int playersWithLifes = 0;
        int totalLifes = 0;
        foreach (Player info in players)
        {
            if ((int)info.CustomProperties["Lifes"] <= 0)
            {
                isPosible = false;
            }
            else
            {
                playersWithLifes += 1;
                totalLifes += (int)info.CustomProperties["Lifes"];
            }
        }
        if (!isPosible)
        {
            if (!isGameOver)
            {
                if (playersWithLifes * 1000 >= gameData.GetActualLevel().minimunScore)
                {
                    gameDataText.text = "Pueden continuar.";
                    isGameOver = false;
                    if (totalLifes > 2)
                    {
                        gameDataText.text += " Se recomienda que todos los jugadores tengan al menos una vida.";
                        isGameOver = false;
                    }
                }
                else
                {
                    gameDataText.text = "No pueden alcanzar el puntaje mínimo necesario.";

                    isGameOver = true;
                }
            }
            else
            {
                gameDataText.text = "No alcanzaron el puntaje mínimo. Fin del juego.";
            }
        }
        else
        {
            //continueButtonSituation = 0;
            gameDataText.text = "Todo listo para el siguiente nivel.";
        }
        if ((int)PhotonNetwork.CurrentRoom.CustomProperties["Stage"] == 3 && !isGameOver) {
            gameDataText.text = "Han completado todos los niveles. Felicitaciones.";
        }
    }

    public void GiveLifeToPlayer(Player player)
    {
        if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Lifes"] > 0)
        {
            Debug.Log("Dando vida al jugador " + player.NickName);
            ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable();
            newProperties.Add("Lifes", ((int)PhotonNetwork.LocalPlayer.CustomProperties["Lifes"]) - 1);
            PhotonNetwork.LocalPlayer.SetCustomProperties(newProperties);

            PV.RPC("IncreaseLifes", RpcTarget.AllViaServer, player);
        }
        else
        {
            gameDataText.text = "No tienes vidas suficientes";
        }

    }

    public void ContinueButtonOnClick()
    {
        if (receivedGameData)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (isGameOver)
                {
                    PV.RPC("LeaveRoom", RpcTarget.AllViaServer);
                }
                else
                {
                    int currentStage = (int)PhotonNetwork.CurrentRoom.CustomProperties["Stage"];
                    if (currentStage < 3)
                    {
                        ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable();
                        newProperties.Add("Stage", currentStage + 1);
                        newProperties.Add("LevelRounds", GetStageRounds(currentStage));
                        newProperties.Add("MinimunScore", GetStageMinimunScore(currentStage));
                        newProperties.Add("Difficulty", GetStageDifficulty(currentStage));
                        PhotonNetwork.CurrentRoom.SetCustomProperties(newProperties);
                        SceneManager.LoadScene("OnlineScene");
                    }
                    else
                    {
                        PV.RPC("LeaveRoom", RpcTarget.AllViaServer);
                    }
                }
            }
        }
        
    }

    public int GetStageDifficulty(int stage)
    {
        if (gameData != null)
        {
            switch (stage)
            {
                case 0:
                    return gameData.levels.level0.difficulty;
                case 1:
                    return gameData.levels.level1.difficulty;
                case 2:
                    return gameData.levels.level2.difficulty;
                default:
                    return -1;
            }
        }
        else
            return -1;

    }

    public int GetStageMinimunScore(int stage)
    {
        if (gameData != null)
        {
            switch (stage)
            {
                case 0:
                    return gameData.levels.level0.minimunScore;
                case 1:
                    return gameData.levels.level1.minimunScore;
                case 2:
                    return gameData.levels.level2.minimunScore;
                default:
                    return -1;
            }
        }
        else
            return -1;


    }

    public int GetStageRounds(int stage)
    {
        if (gameData != null)
        {
            switch (stage)
            {
                case 0:
                    return gameData.levels.level0.rounds;
                case 1:
                    return gameData.levels.level1.rounds;
                case 2:
                    return gameData.levels.level2.rounds;
                default:
                    return -1;
            }
        }
        else
            return -1;


    }

    public int GetStageLastScore(int stage)
    {
        if (gameData != null)
        {
            switch (stage)
            {
                case 0:
                    return gameData.levels.level0.lastScore;
                case 1:
                    return gameData.levels.level1.lastScore;
                case 2:
                    return gameData.levels.level2.lastScore;
                default:
                    return -1;
            }
        }
        else
            return -1;


    }

    [PunRPC]
    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MenuScene");
    }

    [PunRPC]
    private void IncreaseLifes(Player player)
    {
        if(PhotonNetwork.LocalPlayer.NickName == player.NickName)
        {
            ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable();
            newProperties.Add("Lifes", ((int)PhotonNetwork.LocalPlayer.CustomProperties["Lifes"]) + 1);
            PhotonNetwork.LocalPlayer.SetCustomProperties(newProperties);
        }
    }

    [System.Serializable]
    public class GameData
    {
        public GameLevels levels;

        public GameLevel GetActualLevel()
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Stage"))
                switch ((int)PhotonNetwork.CurrentRoom.CustomProperties["Stage"])
                {
                    case 0:
                        return levels.level0;
                    case 1:
                        return levels.level1;
                    case 2:
                        return levels.level2;
                    default:
                        return null;
                }
            else
                return null;
        }
    }

    [System.Serializable]
    public class GameLevels
    {
        public GameLevel level0;
        public GameLevel level1;
        public GameLevel level2;
    }

    [System.Serializable]
    public class GameLevel
    {
        public int difficulty;
        public int rounds;
        public int minimunScore;
        public int lastScore;
    }
}
