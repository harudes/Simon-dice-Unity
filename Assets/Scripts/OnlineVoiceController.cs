using UnityEngine.Windows.Speech;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections.Generic;

public class OnlineVoiceController : MonoBehaviour
{

    string[] words = { "perro", "gato", "mono", "silla", "vaso", "escoba", "mayonesa", "zapallo", "caballo", "zapato", "destornillador", "hipopótamo", "experimento", "esparadrapo", "madagascar" };
    KeywordRecognizer recognizer;

    string orderWord;
    bool orderIsSimon;
    List<string> orderPlayers;
    float actualOrderTime, maxOrderTime;
    bool recognizerIsActive;

    void Awake()
    {
        recognizer = new KeywordRecognizer(words);
        recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
        recognizerIsActive = false;
    }

    void Update()
    {
        actualOrderTime += Time.deltaTime;
        if (recognizerIsActive)
        {
            if (actualOrderTime >= maxOrderTime - 0.1)
            {
                if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Lifes"]>0)
                {
                    if (orderIsSimon && orderPlayers.Contains(OnlineGameManager.instance.playerName))
                    {
                        recognizer.Stop();
                        
                        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Lifes"))
                        {
                            ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable();
                            newProperties.Add("Lifes", ((int)PhotonNetwork.LocalPlayer.CustomProperties["Lifes"]) - 1);
                            PhotonNetwork.LocalPlayer.SetCustomProperties(newProperties);
                        }
                    }
                    else
                    {
                        OnlineGameManager.instance.UpdatePlayersScore(100);
                    }
                    recognizerIsActive = false;
                }
            }
        }
    }

    void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if (orderWord == args.text)
        {
            if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Lifes"] > 0)
            {
                if (orderIsSimon && orderPlayers.Contains(OnlineGameManager.instance.playerName))
                {
                    OnlineGameManager.instance.UpdatePlayersScore(100);
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
           
            recognizer.Stop();
            recognizerIsActive = false;
        }
    }

    public void CheckVoice(string word, bool isSimon, List<string> players, float orderTime)
    {
        actualOrderTime = 0.0f;
        maxOrderTime = orderTime;
        orderWord = word;
        orderPlayers = players;
        orderIsSimon = isSimon;
        recognizerIsActive = true;
        recognizer.Start();
    }
}
