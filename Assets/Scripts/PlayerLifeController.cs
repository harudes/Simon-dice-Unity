using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerLifeController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    TMP_Text playerNickName;

    [SerializeField]
    TMP_Text playerLifes;

    [SerializeField]
    int playerId;

    Player player;

    bool infoUpdated = false;

    [SerializeField]
    GameObject lifesIcon;

    // Start is called before the first frame update
    void Start()
    {
        foreach(KeyValuePair<int,Player> info in PhotonNetwork.CurrentRoom.Players)
        {
            if (info.Key == playerId)
            {
                lifesIcon.SetActive(true);
                infoUpdated = true;
                player = info.Value;
                playerNickName.text = info.Value.NickName;
                if (info.Value.CustomProperties.ContainsKey("Lifes"))
                    playerLifes.text = ((int)info.Value.CustomProperties["Lifes"]).ToString();
                if (info.Value.NickName == PhotonNetwork.LocalPlayer.NickName)
                    playerNickName.color = new Color(0.0f, 1.0f, 0.0f);
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (infoUpdated)
        {
            if (targetPlayer.NickName == player.NickName)
            {
                if (changedProps.ContainsKey("Lifes"))
                    playerLifes.text = ((int)changedProps["Lifes"]).ToString();
            }
        }
    }
}
