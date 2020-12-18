using UnityEngine.Windows.Speech;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Cambio : MonoBehaviour
{
	GameManager gameManager;

    OrderGenerator orderGenerator;
    public AudioClip audio1;
    AudioSource audioSource;

    string[] words = {"perro", "gato", "mono", "silla", "vaso","escoba", "mayonesa", "zapallo", "caballo", "zapato","destornillador", "hipopótamo", "experimento", "esparadrapo", "madagascar"};
    KeywordRecognizer recognizer;
    
	string actualWord="";
	float actualOrderTime, maxOrderTime;
    bool active;
    
    void Awake()
    {
        recognizer = new KeywordRecognizer(words);
        recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
		gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        orderGenerator = GameObject.Find("Game Manager").GetComponent<OrderGenerator>();
        audioSource = this.GetComponent<AudioSource>();
        active = false;
    }

    void Update()
    {
        actualOrderTime += Time.deltaTime;
        if (active)
        {
            if(actualOrderTime >= maxOrderTime - 0.1)
            {
                orderGenerator.UpdateLifes();
                recognizer.Stop();
                active = false;
            }
        }
    }

    void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if (actualWord == args.text)
        {           
			gameManager.IncreaseScore(100);
            audioSource.clip = audio1;
            audioSource.Play();
            recognizer.Stop();  
            active = false;
		}
    }

    void MoveToGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void CheckVoice(string word, float orderTime)
    {
        actualOrderTime = 0;
        maxOrderTime = orderTime;
		actualWord=word;
        active = true;
        recognizer.Start();  
    }    
}
