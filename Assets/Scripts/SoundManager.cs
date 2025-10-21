/*
 * Class for managing playing all related sounds.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum AudioSourceType
{
    Walk,
    Push,
    LightPickup,
    LightDrop,
    PlayerDeath,
    NextLevel,
    UIButtonPress,
    MainMusic,
    MenuMusic,
    IntroMusic,
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
    [SerializeField] private AudioSource introMusicSource;

    // music sources
    [SerializeField] private AudioSource mainMusicSource;
    [SerializeField] private AudioSource menuMusicSource;

    private float mainMusicCurrentTime = 0f;

    private Dictionary<string, float> originalVolumes = new Dictionary<string, float>();


    private void Awake()
    {
        Instance = this;
        BuildVolumeDictionary();
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("volume"))
        { 
            ChangeVolume(PlayerPrefs.GetFloat("volume"));
            //set master mixer to the value
        }
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
            case AudioSourceType.MenuMusic:
                return menuMusicSource;
            case AudioSourceType.IntroMusic:
                return introMusicSource;
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
     * Plays a music clip with the specified music ID using the specified audio source type.
     * @param musicID The ID of the music clip to play.
     * @param sourceType The type of audio source to use.
     */
    public void PlayMusic(int musicID, AudioSourceType sourceType)
    {
        AudioSource source = GetAudioSource(sourceType);
        if (source.isPlaying)
        {
            source.Stop();
        }
        source.clip = music[musicID];
        source.Play();
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

    /**
     * * Stops the currently playing music.
     */
    public void StopMusic(int musicID, AudioSourceType sourceType)
    {
        AudioSource source = GetAudioSource(sourceType);
        if (source.isPlaying)
        {
            source.Stop();
        }
    }
    /**
     * Stops the currently playing music.
     */
    public void StopMusicAtTime(int musicID, AudioSourceType sourceType)
    {
        AudioSource source = GetAudioSource(sourceType);
        

        if (musicID == 0)
        {
            mainMusicCurrentTime = source.time;
        }
        else
        {
            Debug.Log("BRUH CHECK SOUND MANAGER I MADE THE SCRIPT AWFULLY SPECIFIC");
        }
        if (source.isPlaying)
        {
            source.Stop();
        }
    }

    public void PlayMusicAtTime(int musicID, AudioSourceType sourceType)
    {
        AudioSource source = GetAudioSource(sourceType);
        if (source.isPlaying)
        {
            source.Stop();
        }
        source.clip = music[musicID];
        source.time = mainMusicCurrentTime;
        source.PlayScheduled(mainMusicCurrentTime);
    }

    public void PlayMusicAtTime(int musicID, AudioSourceType sourceType, float startTime)
    {
        AudioSource source = GetAudioSource(sourceType);
        if (source.isPlaying)
        {
            source.Stop();
        }
        source.clip = music[musicID];
        source.time = startTime;
        source.PlayScheduled(mainMusicCurrentTime);
    }

    private void BuildVolumeDictionary()
    {
        // Clear any previous entries
        originalVolumes.Clear();

        // Loop through all children of this SoundManager
        foreach (Transform child in transform)
        {
            AudioSource source = child.GetComponent<AudioSource>();
            if (source != null)
            {
                originalVolumes[child.name] = source.volume * 2;
            }
        }
    }

    public void ChangeVolume(float volume)
    {
        foreach (AudioSource source in GetComponentsInChildren<AudioSource>()) {
            float ogVolume = originalVolumes[source.transform.name];
            source.volume = ogVolume * volume / 10;
            if (source.volume > ogVolume) { source.volume = ogVolume; }
        }
    }
}