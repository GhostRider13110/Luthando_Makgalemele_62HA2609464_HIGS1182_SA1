using System;
using UnityEngine;

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public SoundType type = SoundType.SFX;

    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    public bool loop;
    public bool playOnStart;

    [HideInInspector] public AudioSource soucre; 
}
