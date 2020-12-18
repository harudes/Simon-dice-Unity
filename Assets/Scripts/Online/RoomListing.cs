using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomListing : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform content;

    [SerializeField]
    private RoomConnectionManager roomListing;

    private List<RoomConnectionManager> connectionManagers = new List<RoomConnectionManager>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //base.OnRoomListUpdate(roomList);
        print("Se actualizaron las listas");
        foreach(RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                int index = connectionManagers.FindIndex( x => x.roomInfo.Name == info.Name);
                if(index != -1)
                {
                    Destroy(connectionManagers[index].gameObject);
                    connectionManagers.RemoveAt(index);
                }
            }
            else
            {
                RoomConnectionManager listing = Instantiate(roomListing, content);
                if (listing != null)
                {
                    listing.SetRoomInfo(info);
                    connectionManagers.Add(listing);
                }
            }
            
        }
    }
}
