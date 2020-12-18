using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OrderGenerator : MonoBehaviour
{
    public TMP_Text order;
    public TMP_Text lifesText;

    public int simonIsChance;
    public int voiceOrderChance;
    public int ttsOrderChance;
    int difficulty;
    public int lifes;

    MarkersController markersController;
    Cambio voiceController;

    //int lastLifes = 0;

    float orderTime = 4;

    private List<string> colors = new List<string> { "rojo", "azul", "verde", "amarillo" };
    private List<string> directions = new List<string> { "arriba", "abajo", "la derecha", "la izquierda" };
    private List<string> wordsEasy = new List<string> { "perro", "gato", "mono", "silla", "vaso" };
    private List<string> wordsMedium = new List<string> { "escoba", "mayonesa", "zapallo", "caballo", "zapato" };
    private List<string> wordsHard = new List<string> { "destornillador", "hipopótamo", "experimento", "esparadrapo", "madagascar" };
    private List<string> names = new List<string> { "Simón", "Samuel", "Timón" };

    GameManager gameManager;

    MicrosoftTTS tts;

    private void Start()
    {
        difficulty = PlayerPrefs.GetInt("difficulty");
        Debug.Log("Dificultad: " + difficulty.ToString());
        markersController = GameObject.Find("Game Manager").GetComponent<MarkersController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        voiceController = GameObject.Find("Game Manager").GetComponent<Cambio>();
        tts = GameObject.Find("Game Manager").GetComponent<MicrosoftTTS>();
    }

    public void StartGame()
    {
        order.text = "Preparados";
        tts.PlayTTS("Preparados");
        StartCoroutine(GenerateOrders());

    }

    IEnumerator GenerateOrders()
    {

        string tmp, finalOrder;
        int orderType, simonIs;
        yield return new WaitForSeconds(orderTime);
        while (lifes > 0)
        {

            tmp = "";
            orderType = Random.Range(0, 100);
            simonIs = Random.Range(0, 100);
            if (simonIs <= simonIsChance)
            {
                tmp += names[0];
            }
            else
            {
                tmp += names[Random.Range(1, names.Count)];
            }

            if (orderType < voiceOrderChance)
            {
                finalOrder = GenerateVoiceOrder(tmp);
                order.text = "Escuche la orden";
                tts.PlayTTS(finalOrder);
                gameManager.VoiceOrder(true);
                //Send word to voice recognition
                yield return new WaitForSeconds(orderTime + 1.5f);
            }
            else
            {
                finalOrder = GenerateMarkerOrder(tmp);
                order.text = finalOrder;
                gameManager.VoiceOrder(false);
                /*tts.PlayTTS(finalOrder);*/
                //Send color and direction to marker recognition
                yield return new WaitForSeconds(orderTime);
            }

            //expecto to upadate lifes
        }
        order.text = "Fin del juego";

        gameManager.EndGame();
    }

    string GenerateVoiceOrder(string preOrder)
    {
        string temp = "";
        preOrder += " dice que diga ";
        if (difficulty == 1)
        {
            temp = wordsEasy[Random.Range(0, wordsEasy.Count)];
            preOrder += temp;
        }
        else if (difficulty == 2)
        {
            temp = wordsMedium[Random.Range(0, wordsMedium.Count)];
            preOrder += temp;
        }
        else
        {
            temp = wordsHard[Random.Range(0, wordsHard.Count)];
            preOrder += temp;
        }

        voiceController.CheckVoice(temp, orderTime + 1.5f);

        return preOrder;
    }

    string GenerateMarkerOrder(string preOrder)
    {

        int colorDifficulty = difficulty;
        int direction = Random.Range(0, directions.Count);
        int color = Random.Range(0, colorDifficulty + 1);
        if (difficulty == 3)
            colorDifficulty++;
        preOrder += " dice mueve la esfera de color " + colors[color] + " hacia " + directions[direction];



        markersController.MovementOrder(color,true,new List<string>(), direction, orderTime);

        return preOrder;
    }

    public void UpdateOrderTime(float newTime)
    {
        orderTime = newTime;
    }

    public void UpdateLifes()
    {
        lifes -= 1;
        lifesText.text = lifes.ToString();
        gameManager.UpdateOrderTime(4.0f);
        UpdateOrderTime(4.0f);
    }
}
