/*
 * Class for managing playing all related sounds.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioSourceType
{
    Walk,
    Push,
    LightPickup,
    LightDrop,
    PlayerDeath,
    NextLevel,
    UIButtonPress,
    MainMusic
}

public class SoundManager : MonoBehaviour
{
    //Singleton
    public static SoundManager Instance;

    [Header("References")]
    [SerializeField] private List<AudioClip> sounds;
    [SerializeField] private List<AudioClip> music;

    // sound effect sources
    [SerializeField] private AudioSource walkSource;
    [SerializeField] private AudioSource pushSource;
    [SerializeField] private AudioSource lightPickupSource;
    [SerializeField] private AudioSource lightDropSource;
    [SerializeField] private AudioSource playerDeathSource;
    [SerializeField] private AudioSource nextLevelSource;
    [SerializeField] private AudioSource uiButtonPressSource;

    // music sources
    [SerializeField] private AudioSource mainMusicSource;

    private void Awake()
    {
        Instance = this;
    }

    /**
     * Gets the AudioSource based on the specified type.
     * @param sourceType The type of audio source to retrieve.
     * @return The corresponding AudioSource.
     */
    private AudioSource GetAudioSource(AudioSourceType sourceType)
    {
        switch (sourceType)
        {
            case AudioSourceType.Walk:
                return walkSource;
            case AudioSourceType.Push:
                return pushSource;
            case AudioSourceType.LightPickup:
                return lightPickupSource;
            case AudioSourceType.LightDrop:
                return lightDropSource;
            case AudioSourceType.PlayerDeath:
                return playerDeathSource;
            case AudioSourceType.NextLevel:
                return nextLevelSource;
            case AudioSourceType.UIButtonPress:
                return uiButtonPressSource;
            case AudioSourceType.MainMusic:
                return mainMusicSource;
            default:
                Debug.LogWarning($"Unknown audio source type: {sourceType}");
                return walkSource; // fallback
        }
    }

    /**
     * Plays a sound clip with the specified audio ID using the specified audio source type.
     * @param audioID The ID of the audio clip to play.
     * @param sourceType The type of audio source to use.
     */
    public void PlayAudio(int audioID, AudioSourceType sourceType)
    {
        AudioSource source = GetAudioSource(sourceType);
        source.PlayOneShot(sounds[audioID]);
    }
    /**
     * Plays the music clip with the specified music ID, stopping any currently playing music.
     * @param musicID The ID of the music clip to play.
     */
//     public void PlayMusic(int musicID)
//     {
//         if (musicSource.isPlaying)
//         {
//             musicSource.Stop();
//         }

//         musicSource.clip = music[musicID];
//         musicSource.Play();

//     }

//     /**
//      * Stops the currently playing music.
//      */
//     public void StopMusic()
//     {
//         if (musicSource.isPlaying)
//         {
//             musicSource.Stop();
//         }
//     }
}