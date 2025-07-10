using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

class ElevenLabs
{
    const string baseURL = "https://api.elevenlabs.io/v1/text-to-speech/"; // Base URL of HTTP request
    public string API_KEY { get; } // Eleven Labs API key
    public bool requesting = false;
    public ElevenLabs(string key) { this.API_KEY = key; }

    public AudioClip clip;

    // Requests MPEG file containing AI Voice saying the prompt and outputs the directory to said file
    public IEnumerator RequestAudio(string prompt, string voice)
    {
        string url = baseURL + voice; // Concatenate Voice ID to end of URL
        var postData = new TextToSpeechRequest
        {
            text = prompt,
            model_id = "eleven_flash_v2_5" // Cheap model
        };
        // TODO: This could be easily exposed in the Unity inspector,
        // but I had no use for it in my work demo.
        var voiceSetting = new VoiceSettings
        {
            stability = 0,
            similarity_boost = 0,
            style = 0.5f,
            use_speaker_boost = true
        };
        postData.voice_settings = voiceSetting;
        var json = JsonConvert.SerializeObject(postData);
        var uH = new UploadHandlerRaw(Encoding.ASCII.GetBytes(json));
        using (var request = UnityWebRequest.Post(url, json, "application/json"))
        {
            var downloadHandler = new DownloadHandlerAudioClip(url, AudioType.MPEG);
            request.uploadHandler = uH;
            request.downloadHandler = downloadHandler;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("xi-api-key", API_KEY);
            request.SetRequestHeader("Accept", "audio/mpeg");
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error downloading audio: " + request.error);
                yield break;
            }
            clip = downloadHandler.audioClip;
        }
    }

    [Serializable]
    public class TextToSpeechRequest
    {
        public string text;
        public string model_id; // eleven_monolingual_v1
        public VoiceSettings voice_settings;
    }

    [Serializable]
    public class VoiceSettings
    {
        public int stability; // 0
        public int similarity_boost; // 0
        public float style; // 0.5
        public bool use_speaker_boost; // true
    }
}