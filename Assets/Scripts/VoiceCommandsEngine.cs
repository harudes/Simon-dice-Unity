using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections;
using UnityEngine.Networking;

public class VoiceCommandsEngine : MonoBehaviour
{
    public AudioSource _audio;

    public KeywordRecognizer recognizer;

    public GameObject sphere;

    private float speed = 100.0f;

    public string[] keywords = new string[] { "up", "down", "left", "right" };
    //public string[] keywords = new string[] { "arriba", "abajo", "izquierda", "derecha" };

    public ConfidenceLevel confidence = ConfidenceLevel.Medium;

    IEnumerator DownloadAudio(string word)
    {
        Debug.Log("Llamada a audio");
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&tl=en&q=" + word, AudioType.MPEG);
        yield return www.SendWebRequest();
        _audio.clip = DownloadHandlerAudioClip.GetContent(www);
        _audio.Play();
    }

    private void Start()
    {
        recognizer = new KeywordRecognizer(keywords, confidence);
        recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
        recognizer.Start();
        
        _audio = gameObject.GetComponent<AudioSource>();
    }

    private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        
        Debug.Log(args.text);
        bool reproduce = true;
        switch (args.text)
        {
            case "right":
                sphere.transform.Translate(Vector3.right * Time.deltaTime * speed / 100);
                break;
            case "left":
                sphere.transform.Translate(-Vector3.right * Time.deltaTime * speed / 100);
                break;
            case "up":
                sphere.transform.Translate(Vector3.up * Time.deltaTime * speed / 100);
                break;
            case "down":
                sphere.transform.Translate(-Vector3.up * Time.deltaTime * speed / 100);
                break;
            default:
                reproduce = false;
                break;
        }
        if(reproduce)
            StartCoroutine(DownloadAudio(args.text));
    }

    private void OnApplicationQuit()
    {
        if (recognizer != null && recognizer.IsRunning)
        {
            recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
            recognizer.Stop();
        }
    }
}