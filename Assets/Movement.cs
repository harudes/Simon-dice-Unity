using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Movement : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public string direccion = string.Empty;
    // Start is called before the first frame update
    public RawImage sphere;
    //private Vector2 previousPosition = Vector2.zero;

    Vector3 actualPosition;

    public int colorIndex, direction;

    bool active, completed, hasToMove;

    float actualOrderTime, maxOrderTime;

    OnlineGameManager2 gameManager;

    OrderGenerator orderGenerator;
    public AudioClip audio1;
    AudioSource audioSource;

    List<String> orderPlayers;
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");
        //previousPosition = redSphere.rectTransform.anchoredPosition;
    }
    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("OnDrag");
        sphere.rectTransform.anchoredPosition += eventData.delta;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag");
        /*Debug.Log(previousPosition);
        Debug.Log(redSphere.rectTransform.anchoredPosition);

        double XAxis = Math.Abs(Math.Abs(previousPosition[0]) - Math.Abs(redSphere.rectTransform.anchoredPosition[0]));
        double yAxis = Math.Abs(Math.Abs(previousPosition[1]) - Math.Abs(redSphere.rectTransform.anchoredPosition[1]));

        if (XAxis > yAxis)
        {
            if (previousPosition[0] < redSphere.rectTransform.anchoredPosition[0]){
                direccion = "right";
            }
            else
            {
                direccion = "left";
            }
        }
        else
        {
            if (previousPosition[1] < redSphere.rectTransform.anchoredPosition[1])
            {
                direccion = "up";
            }
            else
            {
                direccion = "down";
            }
        }
        Debug.Log(direccion);*/

    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
    }

    void Start()
    {
        actualPosition = this.transform.position;
        gameManager = GameObject.Find("Game Manager").GetComponent<OnlineGameManager2>();
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
            if (actualOrderTime >= maxOrderTime - 0.1)
            {
                //No Cumplio

                //Cambiar
                if (hasToMove && orderPlayers.Contains(OnlineGameManager2.instance.playerName))
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
                    active = false;
                    gameManager.UpdatePlayersScore(100);
                    audioSource.clip = audio1;
                    audioSource.Play();
                    completed = true;
                    StopTracking();
                }
                //Cambiar
                active = false;
            }
            if (CalculateDirection() == direction)
            {
                //Cumplio
                if (hasToMove && orderPlayers.Contains(OnlineGameManager2.instance.playerName))
                {
                    active = false;
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
                    //Cambiar
                    active = false;
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
