using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OrderGenerator : MonoBehaviour
{

    private List<string> colors = new List<string> { "rojo", "azul", "verde", "amarillo", "blanco" };
    private List<string> directions = new List<string> { "arriba", "abajo", "derecha", "izquierda" };
    private List<string> names = new List<string> { "Simón", "Samuel", "Sergio" };
    private List<string> wordsEasy = new List<string> { "perro", "gato", "mono", "silla", "vaso" };
    private List<string> wordsMedium = new List<string> { "escoba", "mayonesa", "zapallo", "caballo", "zapato" };
    private List<string> wordsHard = new List<string> { "destornillador", "hipopótamo", "experimento", "esparadrapo", "madagascar" };
    public TMP_Text text;
    public TMP_InputField diffText;

    public void ButtonOnClick()
    {
        int difficulty = int.Parse(diffText.text);
        int orderType = Random.Range(0, 100);
        int simonIs = Random.Range(0, 100);

        string order = "";
        if (simonIs <= 80)
        {
            order += names[0];
        }
        else
        {
            order += names[Random.Range(1, names.Count)];
        }

        //Si orderType es 0 entonces la orden es de tipo voice recognition
        if (orderType <= 50)
        {
            if (difficulty == 1)
                order += " dice que diga " + wordsEasy[Random.Range(0, wordsEasy.Count)];
            else if (difficulty == 2)
                order += " dice que diga " + wordsMedium[Random.Range(0, wordsMedium.Count)];
            else
                order += " dice que diga " + wordsHard[Random.Range(0, wordsHard.Count)];
        }
        else
        {
            int colorDifficulty = difficulty;
            if (difficulty == 3)
                colorDifficulty++;
            int colorPick = Random.Range(0, colorDifficulty + 1);
            order +=
            " dice mueve la esfera de color " +
            colors[colorPick] +
            " hacia " + directions[Random.Range(0, directions.Count)];
        }
        text.text = order + ".";
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
