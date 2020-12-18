using UnityEngine.Windows.Speech;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CalibradorManager : MonoBehaviour
{
    public GameObject redMarkerImage, blueMarkerImage, greenMarkerImage, yellowMarkerImage, ARCameraVideo, calbradorWord1, calbradorWord2, calbradorWord3;
    public TMP_Text word1, word2, word3;

    string[] words = {"calibrador", "funciona", "bien"};
    KeywordRecognizer recognizer;
    int actualWord = 0;

    bool redMarker = false, blueMarker = true, greenMarker = true, yellowMarker = true;

    void Start()
    {
        recognizer = new KeywordRecognizer(words);
        recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
    }

    void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if (words[actualWord] == args.text)
        {           
            actualWord++;
            if(actualWord == 1)
            {
                word1.color = Color.green;
            }
            if (actualWord == 2)
            {
                word2.color = Color.green;
            }
            if (actualWord == 3)
            {
                word3.color = Color.green;
            }
        }
        if(actualWord > 2)
        {
            recognizer.Stop();            
            this.GetComponent<AudioSource>().Play();
            SceneManager.LoadScene("GameScene");   
        }          
    }

    public void CheckMarker(int marker)
    {
        if (marker == 1 && redMarker == false)
        {
            redMarkerImage.SetActive(false);
            blueMarkerImage.SetActive(true);
            redMarker = true;
            blueMarker = false;
        }
            
        if (marker == 2 && blueMarker == false)
        {
            blueMarkerImage.SetActive(false);
            greenMarkerImage.SetActive(true);
            blueMarker = true;
            greenMarker = false;
        }

        if (marker == 3 && greenMarker == false)
        {
            greenMarkerImage.SetActive(false);
            yellowMarkerImage.SetActive(true);
            greenMarker = true;
            yellowMarker = false;
        }

        if (marker == 4 && yellowMarker == false)
        {
            yellowMarkerImage.SetActive(false);
            ARCameraVideo.SetActive(false);
            calbradorWord1.SetActive(true);
            calbradorWord2.SetActive(true);
            calbradorWord3.SetActive(true);
            recognizer.Start();
            yellowMarker = true;
        }
    }    
}
