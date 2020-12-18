using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using System;

public class MoveDetection : MonoBehaviour
{
    Vector3 actualPosition;

    public int colorIndex, direction;

    bool active, completed, hasToMove;

    float actualOrderTime, maxOrderTime;

    OnlineGameManager gameManager;

    OrderGenerator orderGenerator;
    public AudioClip audio1;
    AudioSource audioSource;

    List<String> orderPlayers;

    void Start()
    {
        actualPosition = this.transform.position;
        gameManager = GameObject.Find("Game Manager").GetComponent<OnlineGameManager>();
        orderGenerator = GameObject.Find("Game Manager").GetComponent<OrderGenerator>();
        audioSource = this.GetComponent<AudioSource>();
        active = false;
        completed = false;
    }

    // Update is called once per frame
    void Update()
    {
        actualOrderTime += Time.deltaTime;
        if (active)
        {
            if(actualOrderTime >= maxOrderTime - 0.1)
            {
                //No Cumplio

                //Cambiar
                if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Lifes"] > 0)
                {
                    if (hasToMove && orderPlayers.Contains(OnlineGameManager.instance.playerName))
                    {
                        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Lifes"))
                        {
                            ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable();
                            newProperties.Add("Lifes", ((int)PhotonNetwork.LocalPlayer.CustomProperties["Lifes"]) - 1);
                            PhotonNetwork.LocalPlayer.SetCustomProperties(newProperties);
                        }
                    }
                    else
                    {
                        gameManager.UpdatePlayersScore(100);
                        audioSource.clip = audio1;
                        audioSource.Play();
                        completed = true;
                        StopTracking();
                    }
                }
                
                //Cambiar
                active = false;
            }
            if (CalculateDirection() == direction)
            {
                //Cumplio
                active = false;
                if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Lifes"] > 0)
                {
                    if (hasToMove && orderPlayers.Contains(OnlineGameManager.instance.playerName))
                    {
                        gameManager.UpdatePlayersScore(100);
                        audioSource.clip = audio1;
                        audioSource.Play();
                        completed = true;
                        StopTracking();
                    }
                    else
                    {
                        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Lifes"))
                        {
                            ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable();
                            newProperties.Add("Lifes", ((int)PhotonNetwork.LocalPlayer.CustomProperties["Lifes"]) - 1);
                            PhotonNetwork.LocalPlayer.SetCustomProperties(newProperties);
                        }
                    }
                }
            }
        }
    }

    int CalculateDirection()
    {
        Vector3 movement = this.transform.position - actualPosition;
        int movementDirection = 0;
        int movementIndex = 1;
        if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
        {
            movementIndex = 0;
            movementDirection = 2;
        }
        if (Mathf.Abs(movement[movementIndex]) / 2 > Mathf.Abs(movement[1 - movementIndex]) &&
            Mathf.Abs(movement[movementIndex]) > 0.03)
        {
            if (movement[movementIndex] < 0)
                movementDirection += 1;
        }
        else
        {
            //Debug.Log(Mathf.Abs(movement[movementDirection]));
            movementDirection = -1;
        }
        return movementDirection;
    }

    public void StartTracking(int dir, bool isSimon, List<string> players, float orderTime)
    {
        actualOrderTime = 0;
        maxOrderTime = orderTime;
        actualPosition = this.transform.position;
        active = true;
        completed = false;
        direction = dir;
        orderPlayers = players;

        hasToMove = isSimon;
    }

    public void StopTracking()
    {
        active = false;
    }

    public bool AskState()
    {
        return completed;
    }
}
