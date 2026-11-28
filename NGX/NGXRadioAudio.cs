using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Lumina.NGX
{
    internal sealed class NGXRadioAudio : MonoBehaviour
    {
        private static NGXRadioAudio instance;
        private AudioSource source;
        private UnityWebRequest request;
        private Coroutine loading;
        private AudioClip clip;
        private bool paused;
        private string status = "No audio loaded";

        private static NGXRadioAudio Get()
        {
            if (instance != null) return instance;
            var host = new GameObject("Lumina NGX Radio");
            DontDestroyOnLoad(host);
            instance = host.AddComponent<NGXRadioAudio>();
            instance.source = host.AddComponent<AudioSource>();
            instance.source.playOnAwake = false;
            instance.source.spatialBlend = 0f;
            instance.source.volume = 0.35f;
            return instance;
        }

        internal static string Status => instance == null ? "No audio loaded" : instance.status;
        internal static bool Playing => instance != null && instance.source != null && instance.source.isPlaying;
        internal static bool HasClip => instance != null && instance.clip != null;
        internal static float Volume => instance == null ? 0.35f : instance.source.volume;
        internal static bool Loop => instance != null && instance.source.loop;

        internal static void Load(string input)
        {
            var player = Get();
            string uri;
            string extension;
            try
            {
                var text = (input ?? "").Trim().Trim('"');
                if (Uri.TryCreate(text, UriKind.Absolute, out var remote) && remote.Scheme == Uri.UriSchemeHttps)
                {
                    uri = remote.AbsoluteUri;
                    extension = Path.GetExtension(remote.AbsolutePath);
                }
                else
                {
                    if (!Path.IsPathRooted(text) || !File.Exists(text))
                        throw new ArgumentException("Enter a local audio file path or direct HTTPS audio URL.");
                    uri = new Uri(Path.GetFullPath(text)).AbsoluteUri;
                    extension = Path.GetExtension(text);
                }
                AudioType type;
                switch (extension.ToLowerInvariant())
                {
                    case ".mp3": type = AudioType.MPEG; break;
                    case ".ogg": type = AudioType.OGGVORBIS; break;
                    case ".wav": type = AudioType.WAV; break;
                    default: throw new ArgumentException("Supported formats: MP3, OGG, WAV.");
                }
                player.Clear();
                player.status = "Loading audio...";
                player.loading = player.StartCoroutine(player.LoadClip(uri, type));
            }
            catch (Exception e) { player.status = e.Message; }
        }

        private IEnumerator LoadClip(string uri, AudioType type)
        {
            request = UnityWebRequestMultimedia.GetAudioClip(uri, type);
            request.timeout = 30;
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    clip = DownloadHandlerAudioClip.GetContent(request);
                    if (clip == null) throw new InvalidOperationException("No audio decoded.");
                    source.clip = clip;
                    paused = false;
                    source.Play();
                    status = "Audio loaded";
                }
                catch (Exception e) { status = e.Message; }
            }
            else status = "Audio could not be loaded: " + request.error;
            request.Dispose();
            request = null;
            loading = null;
        }

        internal static void Toggle()
        {
            if (!HasClip) return;
            if (Playing) { instance.source.Pause(); instance.paused = true; }
            else if (instance.paused) { instance.source.UnPause(); instance.paused = false; }
            else instance.source.Play();
        }

        internal static void Stop()
        {
            if (instance == null) return;
            instance.Clear();
            instance.status = "No audio loaded";
        }

        internal static void SetVolume(float value)
        {
            if (!float.IsNaN(value) && !float.IsInfinity(value)) Get().source.volume = Mathf.Clamp01(value);
        }

        internal static void SetLoop(bool value) => Get().source.loop = value;

        private void Clear()
        {
            if (loading != null) { StopCoroutine(loading); loading = null; }
            if (request != null) { request.Abort(); request.Dispose(); request = null; }
            if (source != null) { source.Stop(); source.clip = null; }
            if (clip != null) { Destroy(clip); clip = null; }
            paused = false;
        }

        internal static void Shutdown()
        {
            if (instance == null) return;
            instance.Clear();
            Destroy(instance.gameObject);
            instance = null;
        }

        private void OnDestroy()
        {
            Clear();
            if (instance == this) instance = null;
        }
    }
}
