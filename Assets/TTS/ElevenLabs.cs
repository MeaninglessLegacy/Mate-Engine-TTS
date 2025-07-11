using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

// Modified script from: https://www.davideaversa.it/blog/elevenlabs-text-to-speech-unity-script/

namespace TTS
{
    public class ElevenLabs : MonoBehaviour
    {
        [Header("Eleven Labs API")]
        public string API_URL = "https://api.elevenlabs.io/v1/text-to-speech/"; // Base URL of HTTP request
        public string API_KEY = "No api key.";

        [Header("Text to Speech Settings")]
        public string voiceID = "9BWtsMINqrJLrRacOk9x"; // Default voice
        public string modelID = "eleven_monolingual_v1"; // Default model
        public bool Streaming = false;
        public VoiceSettings voiceSettings = new VoiceSettings
        {
            stability = 0.5f,
            speed = 0.95f,
            similarity_boost = 0.75f,
            style = 0.5f,
            use_speaker_boost = true
        };

        public AudioClip audioClip; // Eleven Labs audio clip
        // Requests .wav from Eleven Labs
        public IEnumerator RequestAudio(string prompt)
        {
            string url = API_URL + voiceID; // Concatenate Voice ID to end of URL
            var postData = new TextToSpeechRequest
            {
                text = prompt,
                model_id = modelID,
            };
            postData.voice_settings = voiceSettings;
            var json = JsonConvert.SerializeObject(postData);
            var uH = new UploadHandlerRaw(Encoding.ASCII.GetBytes(json));
            using (var request = UnityWebRequest.Post(url, json, "application/json"))
            {
                var downloadHandler = new DownloadHandlerAudioClip(url, AudioType.MPEG);
                if (Streaming)
                {
                    downloadHandler.streamAudio = true;
                }
                request.uploadHandler = uH;
                request.downloadHandler = downloadHandler;
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("xi-api-key", API_KEY);
                request.SetRequestHeader("Accept", "audio/mpeg");
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error downloading audio: " + request.error);
                    audioClip = null;
                    yield break;
                }
                audioClip = downloadHandler.audioClip;
                request.Dispose();
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
            public float stability;
            public float speed;
            public float similarity_boost;
            public float style;
            public bool use_speaker_boost; // true
        }
    }
}