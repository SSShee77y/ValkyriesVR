using UnityEngine.Audio;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour {

    [Serializable]
    public class Sound
    {
        public string name;

        public AudioClip clip;

        [Range(0f, 2f)] public float volume = 1f;
        [Range(.1f, 3f)] public float pitch = 1f;
        public bool loop;

        [HideInInspector]
        public AudioSource source;
    }

    public Sound[] Sounds;

    public static AudioManager Instance;

    void Awake() {
        foreach (Sound s in Sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.spatialBlend = 1f; // 0.0 for 2D, 1.0 for 3D
            s.source.dopplerLevel = 0; // 0 to prevent audio bugs when moving position fast
            s.source.loop = s.loop;
        }
    }

    protected void DontDestroyOnLoad()
    {
        if (Instance == null)
            Instance = this;
        else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Play(string name) {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        if (s.source.isPlaying == false) s.source.Play();
    }

    // Play repeatedly plays audio that bypasses the check to see if the audio is already playing.
    // Basically allows multiple of the same audio playing at once.
    public void PlayRepeatedly(string name) {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }

    public void Play(string name, float specificTime) {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        if (specificTime == -1f) s.source.time = Random.Range(0f, (float)s.source.clip.length);
        else s.source.time = specificTime;
        if (s.source.isPlaying == false) s.source.Play();
    }

    public void Stop(string name) {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        if (s.source.isPlaying == true) s.source.Stop();
    }

    public void Pause(string name) {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        if (s.source.isPlaying == true) s.source.Pause();
    }

    public void UnPause(string name) {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        if (s.source.isPlaying == false) s.source.UnPause();
    }

    public void SetVolume(string name, float f) {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.volume = f;
    }

    public bool IsPlaying(string name) {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return false;
        }
        return s.source.isPlaying;
    }
}
