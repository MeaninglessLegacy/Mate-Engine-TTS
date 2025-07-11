using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace TTS
{
    public class PostAPI : MonoBehaviour
    {
        public AudioClip audioClip;
        private IEnumerator SendPostRequest(string text)
        {
            var bodyRaw = System.Text.Encoding.UTF8.GetBytes(text);
            using (var request = new UnityWebRequest("http://localhost:7777/tts", "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerAudioClip("http://localhost:7777/tts", AudioType.WAV);
                request.SetRequestHeader("Content-Type", "text/plain");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success) Debug.Log($"[UnityNeuroSpeech] TTS server probably is not running! Full error message: {request.error}");

                else
                {
                    audioClip = DownloadHandlerAudioClip.GetContent(request);
                }
            }
        }
    }
}
