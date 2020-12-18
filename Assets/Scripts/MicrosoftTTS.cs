using System.Collections;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using TMPro;

public class WAV
{
    static float BytesToFloat(byte firstByte, byte secondByte)
    {
        short s = (short)((secondByte << 8) | firstByte);
        return s / 32768.0F;
    }

    static int BytesToInt(byte[] bytes, int offset = 0)
    {
        int value = 0;
        for (int i = 0; i < 4; i++)
        {
            value |= ((int)bytes[offset + i]) << (i * 8);
        }
        return value;
    }
    
    public float[] LeftChannel { get; internal set; }
    public float[] RightChannel { get; internal set; }
    public int ChannelCount { get; internal set; }
    public int SampleCount { get; internal set; }
    public int Frequency { get; internal set; }

    public WAV(byte[] wav)
    {
        ChannelCount = wav[22];

        Frequency = BytesToInt(wav, 24);
        int pos = 12;
        
        while (!(wav[pos] == 100 && wav[pos + 1] == 97 && wav[pos + 2] == 116 && wav[pos + 3] == 97))
        {
            pos += 4;
            int chunkSize = wav[pos] + wav[pos + 1] * 256 + wav[pos + 2] * 65536 + wav[pos + 3] * 16777216;
            pos += 4 + chunkSize;
        }
        pos += 8;

        SampleCount = (wav.Length - pos) / 2;
        if (ChannelCount == 2) SampleCount /= 2;

        LeftChannel = new float[SampleCount];
        if (ChannelCount == 2) RightChannel = new float[SampleCount];
        else RightChannel = null;

        int i = 0;
        while (pos < wav.Length)
        {
            LeftChannel[i] = BytesToFloat(wav[pos], wav[pos + 1]);
            pos += 2;
            if (ChannelCount == 2)
            {
                RightChannel[i] = BytesToFloat(wav[pos], wav[pos + 1]);
                pos += 2;
            }
            i++;
        }
    }
}

public class MicrosoftTTS : MonoBehaviour
{
    AudioSource ttsAudio;

    private void Awake()
    {
        ttsAudio = GetComponent<AudioSource>();
    }

    public void PlayTTS(string message)
    {
        StartCoroutine(CallAPI(message));
    }

    IEnumerator CallAPI(string message)
    {
        var config = SpeechConfig.FromSubscription("425b9030184747d7b2ee6d15cdc74904", "eastus");
        config.SpeechSynthesisVoiceName = "es-MX-DaliaNeural";
        var synthesizer = new SpeechSynthesizer(config, null);
        var result = synthesizer.SpeakTextAsync(message);
        yield return result;

        byte[] rawData = result.Result.AudioData;

        WAV wav = new WAV(rawData);
        AudioClip audioClip = AudioClip.Create("orderTTS", wav.SampleCount, 1, wav.Frequency, false);
        audioClip.SetData(wav.LeftChannel, 0);
        ttsAudio.clip = audioClip;
        ttsAudio.Play();
    }
}
