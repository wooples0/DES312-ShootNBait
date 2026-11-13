using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    
    public AudioClip clip;
    
    [Range(0f, 1f)] public float volume;
    [Range(0f, 1f)] public float pitch;
    
    public bool loop;
    
    public bool playOnAwake;

    [HideInInspector]
    public AudioSource audioSource;
   
}
