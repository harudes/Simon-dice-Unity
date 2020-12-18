using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Windows.Speech;

[RequireComponent(typeof(AudioSource))]
public class TutorialManager : MonoBehaviour
{
    string[] words = { "gato" };
    KeywordRecognizer recognizer;
    public TMP_Text lifes, score, order;
    public GameObject lifesFade, scoreFade, progressBar, marker, orderText, markerImage;
    public AudioClip audio1, audio2, audio3, audio4, audio5, audio6, audio7, orderTTS, finalAudio; 

    public static AudioSource audioSource;
    Vector3 prevPosition;

    bool startMarkerRecognition = false;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        recognizer = new KeywordRecognizer(words);
        recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
        StartCoroutine(StartTutorial());
    }

    void Update()
    {
        if (startMarkerRecognition == true)
        {
            if ((marker.transform.position.x - prevPosition.x) > 0.2)
            {                
                score.text = (int.Parse(score.text) + 1).ToString();
                startMarkerRecognition = false;
                markerImage.SetActive(false);                
                StartCoroutine(AfterMarkerRecognition());
            }
        }
    }

    void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if (words[0] == args.text)
        {
            score.text = (int.Parse(score.text) + 1).ToString();
        }
        recognizer.Stop();
        StartCoroutine(AfterVoiceRecognition());
    }

    IEnumerator StartTutorial()
    {   
        AudioSource audioSource = GetComponent<AudioSource>();
        yield return new WaitForSeconds(1.0f);
        audioSource.clip = audio1;
        audioSource.Play();
        yield return new WaitForSeconds(10.0f);
        orderText.SetActive(true);
        markerImage.SetActive(true);
        yield return new WaitForSeconds(audio1.length - 10.0f);

        //wait for marker to move
        prevPosition = marker.transform.position;
        startMarkerRecognition = true;
    }

    IEnumerator AfterMarkerRecognition()
    {
        audioSource.clip = audio2;
        audioSource.Play();
        yield return new WaitForSeconds(audio2.length);
        audioSource.clip = audio3;
        audioSource.Play();
        scoreFade.SetActive(true);
        yield return new WaitForSeconds(audio3.length);
        scoreFade.SetActive(false);
        order.text = "Simón dice: que digas gato";
        recognizer.Start();
    }

    IEnumerator AfterVoiceRecognition()
    {
        audioSource.clip = audio2;
        audioSource.Play();
        yield return new WaitForSeconds(audio2.length);
        audioSource.clip = audio4;
        audioSource.Play();
        yield return new WaitForSeconds(audio4.length);
        audioSource.clip = audio5;
        audioSource.Play();
        yield return new WaitForSeconds(audio5.length);
        order.text = "Escucha lo que Simón dice";
        audioSource.clip = orderTTS;
        audioSource.Play();
        yield return new WaitForSeconds(orderTTS.length);
        audioSource.clip = audio6;
        audioSource.Play();
        yield return new WaitForSeconds(audio6.length);
        audioSource.clip = audio7;
        audioSource.Play();
        progressBar.SetActive(true);
        yield return new WaitForSeconds(audio7.length / 2);
        lifesFade.SetActive(true);
        yield return new WaitForSeconds(audio7.length / 2);
        lifesFade.SetActive(false);

        audioSource.clip = finalAudio;
        audioSource.Play();
    }
}
