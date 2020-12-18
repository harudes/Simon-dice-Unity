using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    OrderGenerator orderGenerator;

    public TMP_Text scoreText;

    public Slider progressBar;

    public int score;

    public ScoresController scoreController;

    float orderTime = 0, maxOrderTime = 4, nextMaxOrderTime=4;

    bool playing = true;

    bool voiceOrder = false;

    // Start is called before the first frame update
    void Start()
    {
        orderGenerator = GameObject.Find("Game Manager").GetComponent<OrderGenerator>();
        orderGenerator.StartGame();
        score = 0;
        //setPlayerPreferences();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();
        orderTime += Time.deltaTime;
        if (orderTime >= maxOrderTime + (voiceOrder?2:0))
        {
            orderTime = 0;
            maxOrderTime = nextMaxOrderTime;
        }
        if(playing)
            progressBar.value = (maxOrderTime + (voiceOrder ? 2 : 0) - orderTime) / (maxOrderTime + (voiceOrder ? 2 : 0));
    }

    void UpdateScore()
    {
        string textScore = score.ToString();
        switch (textScore.Length)
        {
            case 1:
                textScore = "0000" + textScore;
                break;
            case 2:
                textScore = "000" + textScore;
                break;
            case 3:
                textScore = "00" + textScore;
                break;
            case 4:
                textScore = "0" + textScore;
                break;
        }
        scoreText.text = textScore;
    }

    public void IncreaseScore(int addedScore)
    {
        score += addedScore;
        if (maxOrderTime > 1.0f)
        {
            nextMaxOrderTime -= 0.4f;
            orderGenerator.UpdateOrderTime(nextMaxOrderTime);
        }

    }

    public void UpdateOrderTime(float newTime)
    {
        nextMaxOrderTime = newTime;
    }

    public void EndGame()
    {
        playing = false;
        Debug.Log("Fin del juego");
        Invoke("ShowScores", 3);
    }

    void ShowScores()
    {
        scoreController.ShowScores();
        scoreController.AddScore(score);
    }

    public void EndMatch()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void Restart()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void VoiceOrder(bool isVoiceOrder)
    {
        voiceOrder = isVoiceOrder;
    }
}
