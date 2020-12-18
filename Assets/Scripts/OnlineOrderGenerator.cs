using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrderProperties
{
    public int type;
    public bool isSimon;
    public string text;
    public List<string> players;
    public int color;
    public int direction;
    public string word;

    public OrderProperties()
    {

    }
}

public class OnlineOrderGenerator : MonoBehaviour
{
    int simonIsChance;
    public int difficulty = 1;
    List<string> colors = new List<string> { "rojo", "azul", "verde", "amarillo" };
    List<string> directions = new List<string> { "arriba", "abajo", "la derecha", "la izquierda" };
    List<string> wordsEasy = new List<string> { "perro", "gato", "mono", "silla", "vaso" };
    List<string> wordsMedium = new List<string> { "escoba", "mayonesa", "zapallo", "caballo", "zapato" };
    List<string> wordsHard = new List<string> { "destornillador", "hipopótamo", "experimento", "esparadrapo", "madagascar" };
    List<string> names = new List<string> { "Simón", "Samuel", "Timón" };

    void Start()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Difficulty"))
            difficulty = (int)PhotonNetwork.CurrentRoom.CustomProperties["Difficulty"] + 1;
        Debug.Log("Dificultad: " + difficulty.ToString());
        if (difficulty == 1)
        {
            simonIsChance = 90;
        }
        else if (difficulty == 2)
        {
            simonIsChance = 80;
        }
        else
        {
            simonIsChance = 70;
        }
    }

    public OrderProperties GenerateSharedOrder(List<string> playerNames)
    {
        OrderProperties order = new OrderProperties();
        order.type = Random.Range(0, 2);
        order.text = string.Empty;
        int simonIs = Random.Range(0, 100);
        if (simonIs <= simonIsChance)
        {
            order.text += names[0];
            order.isSimon = true;
        }
        else
        {
            order.text += names[Random.Range(1, names.Count)];
            order.isSimon = false;
        }

        order.text += " dice: ";
        order.players = playerNames;
        if (playerNames.Count == 1)
        {
            order.text += playerNames[0] + " ";
        }
        else if (playerNames.Count == 2)
        {
            order.text += playerNames[0] + " y " + playerNames[1] + ", ";
        }
        else
        {
            order.text += "Todos ";
        }

        //OrderType = 0 means marker type
        if (order.type == 0)
        {
            //Generate marker order
            if(playerNames.Count > 1)
            {
                order.text += "muevan";
            }            
            else
            {
                order.text += "mueve";
            }
            GenerateMarkerOrder(ref order.text, ref order.color, ref order.direction);
        }
        //OrderType = 1 means voice type
        else
        {
            //Generate voice order
            if (playerNames.Count > 1)
            {
                order.text += "digan";
            }
            else
            {
                order.text += "dí";
            }
            GenerateVoiceOrder(ref order.text, ref order.word);
        }

        return order;
    }

    public OrderProperties GenerateIndividualOrder(string playerName)
    {
        OrderProperties order = new OrderProperties();
        order.type = Random.Range(0, 2);
        order.text = string.Empty;
        order.players = new List<string>();
        order.players.Add(playerName);

        int simonIs = Random.Range(0, 100);
        if (simonIs <= simonIsChance)
        {
            order.text += names[0];
            order.isSimon = true;
        }
        else
        {
            order.text += names[Random.Range(1, names.Count)];
            order.isSimon = false;
        }

        order.text += " dice: " + playerName + " ";
        
        //OrderType = 0 means marker type
        if (order.type == 0)
        {
            order.text += "mueve";
            GenerateMarkerOrder(ref order.text, ref order.color, ref order.direction);
        }
        //OrderType = 1 means voice type
        else
        {
            order.text += "dí";
            GenerateVoiceOrder(ref order.text, ref order.word);
        }

        return order;
    }

    void GenerateVoiceOrder(ref string preOrder, ref string word)
    {
        preOrder += " la palabra ";
        if (difficulty == 1)
        {
            int wordIndex = Random.Range(0, wordsEasy.Count);
            preOrder += wordsEasy[wordIndex];
            word = wordsEasy[wordIndex];
        }
        else if (difficulty == 2)
        {
            int wordIndex = Random.Range(0, wordsMedium.Count);
            preOrder += wordsMedium[wordIndex];
            word = wordsMedium[wordIndex];
        }
        else
        {
            int wordIndex = Random.Range(0, wordsHard.Count);
            preOrder += wordsHard[wordIndex];
            word = wordsHard[wordIndex];
        }
        
        preOrder += ".";
    }


    void GenerateMarkerOrder(ref string preOrder, ref int color, ref int direction)
    {
        int colorDifficulty = difficulty;
        direction = Random.Range(0, directions.Count);
        if (difficulty == 3)
            colorDifficulty++;
        color = Random.Range(0, colorDifficulty + 1);
        preOrder += " la esfera de color " + colors[color] + " hacia " + directions[direction];
        
        preOrder += ".";
    }
}
