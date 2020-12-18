using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;


public class RoomConnectionManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private TMP_Text roomName;

    [SerializeField]
    private TMP_Text roomPlayers;

    public RoomInfo roomInfo;


    public void SetRoomInfo(RoomInfo info)
    {
        roomInfo = info;
        roomName.text = info.Name;
        roomPlayers.text = info.PlayerCount.ToString() + '/' + info.MaxPlayers.ToString();
    }

    public void ButtonPress()
    {
        Debug.Log("Uniendo a sala "+roomName.text);
        PhotonNetwork.JoinRoom(roomName.text);

        GameManager.FindObjectOfType<Canvas>().transform.Find("RoomLobby").gameObject.SetActive(true);

        GameManager.FindObjectOfType<Canvas>().transform.Find("ListingRooms").gameObject.SetActive(false);
    }
}
