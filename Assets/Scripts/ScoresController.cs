using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoresController : MonoBehaviour
{
    // Start is called before the first frame update
    List<int> scores = new List<int> { };

    private Transform entryContainer;
    private Transform entryTemplate;

    int gameScore = 0;

    private void Awake()
    {
        LoadScores();

        entryContainer = transform.Find("Entries Container");

        entryTemplate = entryContainer.Find("Entry Template");

        entryTemplate.gameObject.SetActive(false);

        
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowScores()
    {
        Debug.Log("Mostrando Puntajes");
        this.gameObject.SetActive(true);
    }

    void LoadScores()
    {
        scores.Add(PlayerPrefs.GetInt("1st"));
        scores.Add(PlayerPrefs.GetInt("2nd"));
        scores.Add(PlayerPrefs.GetInt("3rd"));
        scores.Add(PlayerPrefs.GetInt("4th"));
        scores.Add(PlayerPrefs.GetInt("5th"));
        scores.Add(PlayerPrefs.GetInt("6th"));
        scores.Add(PlayerPrefs.GetInt("7th"));
        scores.Add(PlayerPrefs.GetInt("8th"));
        scores.Add(PlayerPrefs.GetInt("9th"));
        scores.Add(PlayerPrefs.GetInt("10th"));
    }

    public void AddScore(int newScore)
    {
        scores.Add(newScore);
        SortScores();
        SaveScores();

        gameScore = newScore;

        bool highlight = true;

        for (int i = 0; i < 5; ++i)
        {
            Transform entryTransform = Instantiate(entryTemplate, entryContainer);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -60f * i);
            entryTransform.gameObject.SetActive(true);

            entryTransform.Find("Score Rank").GetComponent<TMP_Text>().text = (i + 1).ToString();

            entryTransform.Find("Score Score").GetComponent<TMP_Text>().text = scores[i].ToString();

            if (scores[i] == gameScore && highlight)
            {
                entryTransform.Find("Score Score").GetComponent<TMP_Text>().color = new Color(0, 1, 0);
                highlight = false;
            }

            switch (i)
            {
                case 0:
                    entryTransform.Find("Golden Trophy").gameObject.SetActive(true);
                    break;
                case 1:
                    entryTransform.Find("Silver Trophy").gameObject.SetActive(true);
                    break;
                case 2:
                    entryTransform.Find("Bronze Trophy").gameObject.SetActive(true);
                    break;
            }
        }
    }

    void SortScores()
    {
        scores.Sort();
        scores.Reverse();
        while(scores.Count>10)
            scores.RemoveAt(10);
    }

    void SaveScores()
    {
        PlayerPrefs.SetInt("1st", scores[0]);
        PlayerPrefs.SetInt("2nd", scores[1]);
        PlayerPrefs.SetInt("3rd", scores[2]);
        PlayerPrefs.SetInt("4th", scores[3]);
        PlayerPrefs.SetInt("5th", scores[4]);
        PlayerPrefs.SetInt("6th", scores[5]);
        PlayerPrefs.SetInt("7th", scores[6]);
        PlayerPrefs.SetInt("8th", scores[7]);
        PlayerPrefs.SetInt("9th", scores[8]);
        PlayerPrefs.SetInt("10th", scores[9]);
    }

    void SetFirstPreferences()
    {
        PlayerPrefs.SetInt("1st", 0);
        PlayerPrefs.SetInt("2nd", 0);
        PlayerPrefs.SetInt("3rd", 0);
        PlayerPrefs.SetInt("4th", 0);
        PlayerPrefs.SetInt("5th", 0);
        PlayerPrefs.SetInt("6th", 0);
        PlayerPrefs.SetInt("7th", 0);
        PlayerPrefs.SetInt("8th", 0);
        PlayerPrefs.SetInt("9th", 0);
        PlayerPrefs.SetInt("10th", 0);
    }
}
