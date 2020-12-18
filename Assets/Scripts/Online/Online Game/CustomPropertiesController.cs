using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPropertiesController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LooseLife()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Lifes"))
        {
            ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable();
            newProperties.Add("Lifes", ((int)PhotonNetwork.LocalPlayer.CustomProperties["Lifes"]) - 1);
            PhotonNetwork.LocalPlayer.SetCustomProperties(newProperties);
        }
    }

    public int GetIntegerProperty(string property)
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(property))
        {
            return (int)PhotonNetwork.CurrentRoom.CustomProperties[property];
        }
        else
            return -1;
    }

    
}
