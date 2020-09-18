using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections;

public class VoiceCommandsEngine : MonoBehaviour
{
    public KeywordRecognizer recognizer;

    public GameObject sphere;

    private float speed = 100.0f;

    public string[] keywords = new string[] { "up", "down", "left", "right" };
    //public string[] keywords = new string[] { "arriba", "abajo", "izquierda", "derecha" };

    public ConfidenceLevel confidence = ConfidenceLevel.Medium;

    protected string word = "right";

    private void Start()
    {
        recognizer = new KeywordRecognizer(keywords, confidence);
        recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
        recognizer.Start();

        
    }

    private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        
        Debug.Log(args.text);
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
                break;
        }
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