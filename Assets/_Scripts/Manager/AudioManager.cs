using System.Collections;
using Enums;
using UnityEngine;

namespace Manager
{
    /// <summary>
    /// Manages audio feedback for game events, playing specific sounds for level start, level stop, emote success, and emote failure.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource MusicAudioSource; // Background music audio source
        [SerializeField] private AudioSource EffectAudioSource; // Sound effects audio source
        [SerializeField] private AudioClip FailSound; // Sound to play on emote fail
        [SerializeField] private AudioClip SuccessSound; // Sound to play on emote success
        [SerializeField] private AudioClip LevelStartSound; // Sound to play when level starts
        [SerializeField] private AudioClip LevelStoppedSound; // Sound to play when level stops


        private void OnEnable()
        {
            // Subscribe to game event notifications
            EventManager.OnLevelStarted += OnLevelStartedCallback;
            EventManager.OnLevelStopped += OnLevelStoppedCallback;
            EventManager.OnEmoteFulfilled += OnEmoteFulfilledCallback;
            EventManager.OnEmoteFailed += OnEmoteFailedCallback;
        }

        private void OnDestroy()
        {
            // Unsubscribe from game event notifications
            EventManager.OnLevelStarted -= OnLevelStartedCallback;
            EventManager.OnLevelStopped -= OnLevelStoppedCallback;
            EventManager.OnEmoteFulfilled -= OnEmoteFulfilledCallback;
            EventManager.OnEmoteFailed -= OnEmoteFailedCallback;
        }

        /// <summary>
        /// Callback for when the level starts, plays the start sound and begins level music.
        /// </summary>
        private void OnLevelStartedCallback()
        {
            // Play level started sound
            PlaySoundEffect(LevelStartSound);
            
            // Start playing the background music from the beginning, with a slight delay to allow the start sound to be heard
            MusicAudioSource.volume = 1;
            MusicAudioSource.time = 0;
            MusicAudioSource.PlayDelayed(0.9f);
        }

        /// <summary>
        /// Callback for when the level stops, stops the music and plays the stop sound.
        /// </summary>
        private void OnLevelStoppedCallback()
        {
            // Stop the music when the level stops
            StartCoroutine(FadeOutMusicCoroutine(2f));
            
            // Play level stopped sound
            PlaySoundEffect(LevelStoppedSound);
        }

        /// <summary>
        /// Callback for when an emote is successfully fulfilled, plays the success sound.
        /// </summary>
        private void OnEmoteFulfilledCallback(EEmote emote, float score)
        {
            // Play success sound
            PlaySoundEffect(SuccessSound);
        }
        
        /// <summary>
        /// Callback for when an emote imitation fails, plays the fail sound.
        /// </summary>
        private void OnEmoteFailedCallback(EEmote emote)
        {
            // Play fail sound
            PlaySoundEffect(FailSound);
        }

        /// <summary>
        /// Plays a sound effect using the designated effects audio source.
        /// </summary>
        /// <param name="sound">The audio clip to play as a one-shot sound effect.</param>
        private void PlaySoundEffect(AudioClip sound)
        {
            if (sound != null)
                EffectAudioSource.PlayOneShot(sound);
        }
        
        /// <summary>
        /// Coroutine to smoothly fade out the music volume.
        /// </summary>
        /// <param name="duration">The duration over which to fade out the music.</param>
        private IEnumerator FadeOutMusicCoroutine(float duration)
        {
            float startVolume = MusicAudioSource.volume;

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                MusicAudioSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
                yield return null;
            }

            MusicAudioSource.volume = 0; // Ensure the volume is set to 0
            MusicAudioSource.Stop(); // Stop the music after fading out
        }
    }
}