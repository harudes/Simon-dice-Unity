using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListingManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private TMP_Text playerName;
    
    private Player player;

    public void SetPlayerInfo(Player playerInfo)
    {
        playerName.text = playerInfo.NickName;
        player = playerInfo;
    }

    public string GetPlayerName()
    {
        return player.NickName;
    }
}
