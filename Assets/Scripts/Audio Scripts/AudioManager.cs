
using System;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    SFX,
    Music
}


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sounds")]
    [SerializeField] private List<Sound> sounds = new List<Sound>();

    [Header("Volume")]
    [Range(0f, 1f)] [SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 1f;
    [SerializeField] private bool muted; 

    private readonly Dictionary<string, Sound> lookup = new Dictionary<string, Sound>();

    private const string MasterKey = "Audio_Master";
    private const string SFXKey = "Audio_SFX";
    private const string MusicKey = "Audio_Music";

    public float MasterVolume => masterVolume;
    public float SFXVolume => sfxVolume;
    public float MusicVolume => musicVolume;
    public bool IsMuted => muted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; 
        }

        Instance = this;   
        DontDestroyOnLoad(gameObject);

        LoadVolumes();
        BuildSources();

    }

    private void Start()
    {
        foreach (Sound s in sounds)
        {
            if (s.playOnStart) Play(s.name); 
        }
    }

    private void BuildSources()
    {
        foreach (Sound s in sounds)
        {
            if (string.IsNullOrEmpty(s.name) || s.clip == null)
            {
                Debug.LogWarning("AudioManager: a sound is missing a name or clip and was skipped.");
                continue;
            }

            if (lookup.ContainsKey(s.name))
            {
                Debug.LogWarning($"AudioManager: duplicate sound name '{s.name}' ");
                continue;
            }

            s.soucre = gameObject.AddComponent<AudioSource>();
            s.soucre.clip = s.clip;
            s.soucre.pitch = s.pitch;
            s.soucre.loop = s.loop;
            s.soucre.playOnAwake = false;

            lookup.Add(s.name, s);
            ApplyVolume(s);
        }
    }

    private bool TryGetSound(string soundName, out Sound sound)
    {
        if (lookup.TryGetValue(soundName, out sound)) return true;

        Debug.LogWarning($"AudioManager: sound '{soundName}'not found.");
        return false;
    }

    public void Play(string soundName)
    {
        if (!TryGetSound(soundName, out Sound s)) return;

        s.soucre.loop = s.loop; 
        s.soucre.Play();
    }

    public void Play(string soundName, bool loop)
    {
        if (!TryGetSound(soundName,out Sound s)) return ;

        s.soucre.loop = loop;
        s.soucre.Play();
    }

    public void PlayOneShot(string soundName)
    {
        if (!TryGetSound(soundName, out Sound s)) return; 

        s.soucre.PlayOneShot(s.clip);
    }

    public void Stop(string soundName)
    {
        if (TryGetSound(soundName,out Sound s)) s.soucre.Stop();
    }

    public void Pause(string soundName)
    {
        if(TryGetSound(soundName, out Sound s)) s.soucre.Pause();
    }

    public void Resume(string soundName)
    {
        if(TryGetSound(soundName, out Sound s)) s.soucre.UnPause();
    }

    public void StopAll()
    {
        foreach (Sound s in lookup.Values) s.soucre.Stop();
    }

    public bool IsPlaying(string soundName)
    {
        return TryGetSound(soundName, out Sound s) && s.soucre.isPlaying; 
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(MasterKey, masterVolume);
        RefreshAllVolume();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat (SFXKey, sfxVolume);
        RefreshAllVolume();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(MusicKey, musicVolume);
        RefreshAllVolume();
    }

    public void SetSounVolume(string soundName, float value)
    {
        if(!TryGetSound(soundName, out Sound s)) return;
        
        s.volume = Mathf.Clamp01(value);
        ApplyVolume(s);
    }

    public void SetMuted(bool value)
    {
        muted = value;
        RefreshAllVolume();
    }

    public void ToggleMute() => SetMuted(!muted);

    public void ApplyVolume(Sound s)
    {
        float category = s.type == SoundType.Music ? musicVolume : sfxVolume;
        s.soucre.volume = muted ? 0f : category * masterVolume; 
    }

    private void RefreshAllVolume()
    {
        foreach (Sound s in lookup.Values) ApplyVolume(s);
    }

    private void LoadVolumes()
    {
        masterVolume = PlayerPrefs.GetFloat(MasterKey, masterVolume);
        sfxVolume = PlayerPrefs.GetFloat(SFXKey, masterVolume);
        musicVolume = PlayerPrefs.GetFloat(MusicKey, masterVolume);
    }

    private void OnValidate()
    {
        if (Application.isPlaying && lookup.Count > 0) RefreshAllVolume();
    }
}
