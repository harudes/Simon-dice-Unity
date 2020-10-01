using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.IO;

[System.Serializable]
public class GoogleTextToSpeech
{
    public string audioContent;
}

public class WAV
{

    // convert two bytes to one float in the range -1 to 1
    static float bytesToFloat(byte firstByte, byte secondByte)
    {
        // convert two bytes to one short (little endian)
        short s = (short)((secondByte << 8) | firstByte);
        // convert to range from -1 to (just below) 1
        return s / 32768.0F;
    }

    static int bytesToInt(byte[] bytes, int offset = 0)
    {
        int value = 0;
        for (int i = 0; i < 4; i++)
        {
            value |= ((int)bytes[offset + i]) << (i * 8);
        }
        return value;
    }

    private static byte[] GetBytes(string filename)
    {
        return File.ReadAllBytes(filename);
    }
    // properties
    public float[] LeftChannel { get; internal set; }
    public float[] RightChannel { get; internal set; }
    public int ChannelCount { get; internal set; }
    public int SampleCount { get; internal set; }
    public int Frequency { get; internal set; }

    // Returns left and right double arrays. 'right' will be null if sound is mono.
    public WAV(string filename) :
        this(GetBytes(filename))
    { }

    public WAV(byte[] wav)
    {

        // Determine if mono or stereo
        ChannelCount = wav[22];     // Forget byte 23 as 99.999% of WAVs are 1 or 2 channels

        // Get the frequency
        Frequency = bytesToInt(wav, 24);

        // Get past all the other sub chunks to get to the data subchunk:
        int pos = 12;   // First Subchunk ID from 12 to 16

        // Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
        while (!(wav[pos] == 100 && wav[pos + 1] == 97 && wav[pos + 2] == 116 && wav[pos + 3] == 97))
        {
            pos += 4;
            int chunkSize = wav[pos] + wav[pos + 1] * 256 + wav[pos + 2] * 65536 + wav[pos + 3] * 16777216;
            pos += 4 + chunkSize;
        }
        pos += 8;

        // Pos is now positioned to start of actual sound data.
        SampleCount = (wav.Length - pos) / 2;     // 2 bytes per sample (16 bit sound mono)
        if (ChannelCount == 2) SampleCount /= 2;        // 4 bytes per sample (16 bit stereo)

        // Allocate memory (right will be null if only mono sound)
        LeftChannel = new float[SampleCount];
        if (ChannelCount == 2) RightChannel = new float[SampleCount];
        else RightChannel = null;

        // Write to double array/s:
        int i = 0;
        while (pos < wav.Length)
        {
            LeftChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
            pos += 2;
            if (ChannelCount == 2)
            {
                RightChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
                pos += 2;
            }
            i++;
        }
    }

    public override string ToString()
    {
        return string.Format("[WAV: LeftChannel={0}, RightChannel={1}, ChannelCount={2}, SampleCount={3}, Frequency={4}]", LeftChannel, RightChannel, ChannelCount, SampleCount, Frequency);
    }
}

public class TTS : MonoBehaviour
{
    public TMP_Text text;
    public AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        audio = gameObject.GetComponent<AudioSource>();
    }

    IEnumerator CallAPI()
    {
        string bodyAsJson = "{\r\n  \"input\":{\r\n    \"text\":\"" + text.text + "\"\r\n  },\r\n  \"voice\":{\r\n    \"languageCode\":\"es-ES\",\r\n    \"name\":\"es-ES-Standard-A\",\r\n    \"ssmlGender\":\"FEMALE\"\r\n  },\r\n  \"audioConfig\":{\r\n    \"audioEncoding\":\"LINEAR16\"\r\n  }\r\n}";
        UnityWebRequest request = UnityWebRequest.Post("https://texttospeech.googleapis.com/v1/text:synthesize", "");
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(bodyAsJson));
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Goog-Api-Key", "AIzaSyAKqtHv4b8vFV0B5YbcK6R49y6u4heftTw");

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log("Error While Sending: " + request.error);
        }
        else
        {
            Debug.Log("Text: " + text.text);
            Debug.Log(request.downloadHandler.text);

            GoogleTextToSpeech response = JsonUtility.FromJson<GoogleTextToSpeech>(request.downloadHandler.text);
            Debug.Log("Audio: " + response.audioContent);
            byte[] rawData = System.Convert.FromBase64String(response.audioContent);

            WAV wav = new WAV(rawData);
            AudioClip audioClip = AudioClip.Create("testSound", wav.SampleCount, 1, wav.Frequency, false, false);
            audioClip.SetData(wav.LeftChannel, 0);
            audio.clip = audioClip;
            audio.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onClick()
    {
        StartCoroutine(CallAPI());
    }
}
