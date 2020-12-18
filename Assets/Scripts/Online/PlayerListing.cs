using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListing : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform content;

    [SerializeField]
    private PlayerListingManager playerListing;

    List<PlayerListingManager> playerListingList = new List<PlayerListingManager>();

    private void Awake()
    {
        //
        //GetCurrentRoomPlayers();
    }

    public void GetCurrentRoomPlayers()
    {
        print("Enlistando jugadores");
        foreach (KeyValuePair<int,Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            AddPlayerListing(playerInfo.Value);
        }
    }

    private void AddPlayerListing(Player player)
    {
        PlayerListingManager listing = Instantiate(playerListing, content);
        if (listing != null)
        {
            listing.SetPlayerInfo(player);
            playerListingList.Add(listing);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print("Se actualizaron las listas");
        AddPlayerListing(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print("Se fue un jugador");
        for(int i=0; i < playerListingList.Count; ++i)
        {
            if(otherPlayer.NickName == playerListingList[i].GetPlayerName())
            {
                print("Jugador encontrado");
                Destroy(playerListingList[i].gameObject);
                playerListingList.Remove(playerListingList[i]);
                return;
            }
        }
    }
}
