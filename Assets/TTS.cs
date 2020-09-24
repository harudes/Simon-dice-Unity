using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;



public class TTS : MonoBehaviour
{

    public AudioSource _audio;
    // Start is called before the first frame update

    public string word;

    IEnumerator DownloadAudio()
    {
        word = "Lavadora";
        Debug.Log("Llamada a audio");
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&tl=en&q=" + word, AudioType.MPEG);
        yield return www.SendWebRequest();
        _audio.clip = DownloadHandlerAudioClip.GetContent(www);
        _audio.Play();
    }

    void Start()
    {
        _audio = gameObject.GetComponent<AudioSource>();
        StartCoroutine(DownloadAudio());

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
