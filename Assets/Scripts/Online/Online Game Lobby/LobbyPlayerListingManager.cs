using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class LobbyPlayerListingManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    TMP_Text playerName;
    [SerializeField]
    TMP_Text playerLifes;

    [SerializeField]
    GameObject addLifeButton;

    Player player;
    int playerId;

    private void Start()
    {
        if ((int)PhotonNetwork.CurrentRoom.CustomProperties["Stage"] == 3)
        {
            addLifeButton.SetActive(false);
        }
    }

    public bool SetPlayerInfo(Player playerInfo)
    {
        playerName.text = playerInfo.NickName;
        player = playerInfo;
        if (playerInfo.CustomProperties.ContainsKey("Lifes"))
        {
            playerLifes.text = ((int)playerInfo.CustomProperties["Lifes"]).ToString();
        }
        
        return playerInfo.NickName == PhotonNetwork.NickName;
    }

    public string GetPlayerName()
    {
        return player.NickName;
    }

    public void AddLifeButtonOnClick()
    {
        GameManager.FindObjectOfType<GameLobbyController>().GiveLifeToPlayer(player);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("Cambio de propiedades");
        Debug.Log(targetPlayer.NickName);
        Debug.Log(changedProps);

        if (targetPlayer.NickName == player.NickName)
        {
            if(changedProps.ContainsKey("Lifes"))
                playerLifes.text = ((int)changedProps["Lifes"]).ToString();
        }

        GameManager.FindObjectOfType<GameLobbyController>().TestGamePossibility();
    }
}
