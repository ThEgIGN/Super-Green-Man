using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager Instance;
    
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(string name, float volume = 0.35f) {
        Sound music = Array.Find(musicSounds, s => s.soundName == name);
        if (music != null) {
            musicSource.clip = music.audioCLip;
            musicSource.volume = volume;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name, float volume = 0.35f) {
        Sound sfx = Array.Find(sfxSounds, s => s.soundName == name);
        if (sfx != null) {
            sfxSource.PlayOneShot(sfx.audioCLip, volume);
        }
    }

}
