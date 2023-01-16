using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TetrisClone.Managers
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
        public AudioClip gameOverSound;
        public AudioClip gameOverVocal;
        public AudioClip errorSound;

        public AudioSource audioSource;

        [SerializeField] private AudioClip[] _backgroundMusicAudioClips;
        private AudioClip _randomBackgroundMusicAudioClip;

        public AudioClip[] vocalAudioClips;
        
        private void Start()
        {
            _randomBackgroundMusicAudioClip = GetRandomAudioClip(_backgroundMusicAudioClips);
            PlayBackgroundMusic(_randomBackgroundMusicAudioClip);
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
        }

        public AudioClip GetRandomAudioClip(AudioClip[] audioClips)
        {
            var randomAudioClip = audioClips[Random.Range(0, audioClips.Length)];
            return randomAudioClip;
        }

        public void ToggleSFX()
        {
            isSFXEnabled = !isSFXEnabled;
        }
    }
}