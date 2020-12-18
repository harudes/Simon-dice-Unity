using UnityEngine;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine.UI;

public class LobbyPlayerListing : MonoBehaviour
{
    [SerializeField]
    private Transform content;

    [SerializeField]
    private LobbyPlayerListingManager playerListing;

    List<LobbyPlayerListingManager> playerListingList = new List<LobbyPlayerListingManager>();

    public void InitializeListing(List<Player> players)
    {
        foreach (Player player in players)
        {
            AddLobbyPlayerListing(player);
        }
    }

    private void AddLobbyPlayerListing(Player player)
    {
        Debug.Log("Añadiendo jugador " + player.NickName + " a la sala");
        LobbyPlayerListingManager listing = Instantiate(playerListing, content);
        if (listing != null)
        {
            if (listing.SetPlayerInfo(player))
            {
                listing.transform.Find("AddLivesButton").GetComponent<Button>().gameObject.SetActive(false);
            }
            playerListingList.Add(listing);
        }
    }
}
