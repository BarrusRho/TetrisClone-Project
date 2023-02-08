using System;
using TetrisClone.Utility;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TetrisClone.Management
{
    public class AudioManager : MonoBehaviour
    {
        public bool isMusicEnabled = true;
        public bool isSFXEnabled = true;
        
        [Range(0, 1)]
        public float musicVolume = 1.0f;
        [Range(0, 1)]
        public float sFXVolume = 1.0f;

        public AudioClip clearRowSound;
        public AudioClip moveSound;
        public AudioClip dropSound;
        public AudioClip errorSound;
        public AudioClip holdSound;
        public AudioClip gameOverSound;
        public AudioClip gameOverVocal;
        public AudioClip levelUpVocalClip;
        
        public AudioSource audioSource;

        [SerializeField] private AudioClip[] _backgroundMusicAudioClips;
        private AudioClip _randomBackgroundMusicAudioClip;

        public AudioClip[] vocalAudioClips;

        public IconToggle musicIconToggle;
        public IconToggle sfxIconToggle;
        
        private void Start()
        {
            _randomBackgroundMusicAudioClip = GetRandomAudioClip(_backgroundMusicAudioClips);
            PlayBackgroundMusic(_randomBackgroundMusicAudioClip);
        }
        
        public void PlaySound(AudioClip audioClip, float volumeMultiplier = 1.0f)
        {
            if (isSFXEnabled && audioClip)
            {
                AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position,
                    Mathf.Clamp(sFXVolume * volumeMultiplier, 0.05f, 1f));
            }
        }
        
        public void PlayBackgroundMusic(AudioClip audioClip)
        {
            if (!isMusicEnabled || !audioClip || !audioSource)
            {
                return;
            }
            
            audioSource.Stop();
            audioSource.clip = audioClip;
            audioSource.volume = musicVolume;
            audioSource.loop = true;
            audioSource.Play();
        }

        private void UpdateMusic()
        {
            if (audioSource.isPlaying != isMusicEnabled)
            {
                if (isMusicEnabled)
                {
                    _randomBackgroundMusicAudioClip = GetRandomAudioClip(_backgroundMusicAudioClips);
                    PlayBackgroundMusic(_randomBackgroundMusicAudioClip);
                }
                else
                {
                    audioSource.Stop();
                }
            }
        }

        public void ToggleMusic()
        {
            isMusicEnabled = !isMusicEnabled;
            UpdateMusic();

            if (musicIconToggle)
            {
                musicIconToggle.ToggleIcon(isMusicEnabled);
            }
        }

        public AudioClip GetRandomAudioClip(AudioClip[] audioClips)
        {
            var randomAudioClip = audioClips[Random.Range(0, audioClips.Length)];
            return randomAudioClip;
        }

        public void ToggleSFX()
        {
            isSFXEnabled = !isSFXEnabled;

            if (sfxIconToggle)
            {
                sfxIconToggle.ToggleIcon(isSFXEnabled);
            }
        }
    }
}