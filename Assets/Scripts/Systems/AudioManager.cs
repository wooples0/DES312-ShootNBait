using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] sounds;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;

        foreach (Sound sound in sounds)
        {
            var source = gameObject.AddComponent<AudioSource>();
            sound.audioSource = source;
            sound.audioSource.clip = sound.clip;
            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch = sound.pitch;
            sound.audioSource.loop = sound.loop;
            sound.audioSource.playOnAwake = sound.playOnAwake;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound(string name)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);

        if(sound == null)
        {
            Debug.Log($"Sound {name} not found");
            return;
        }

        sound.audioSource.Play();

    }

    public void PlayRandom(string[] names)
    {
        var r = UnityEngine.Random.Range(0, names.Length);

        Sound sound = Array.Find(sounds, sound => sound.name == names[r]);

        if (sound == null)
        {
            Debug.Log($"Sound {name} not found");
            return;
        }

        sound.audioSource.Play();
    }
    public void PlayRandomOneShot(string[] names)
    {
        
        var r = UnityEngine.Random.Range(0, names.Length);
        if (sounds[r].name == "none")
        {
            return;
        }
        Sound sound = Array.Find(sounds, sound => sound.name == names[r]);

        if (sound == null)
        {
            Debug.Log($"Sound {name} not found");
            return;
        }
        Debug.Log(sound.name);
        sound.audioSource.PlayOneShot(sound.audioSource.clip);
    }

    public void StopSound(string name)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);

        if (sound == null)
        {
            Debug.Log($"Sound {name} not found");
            return;
        }

        sound.audioSource.Stop();

    }
}
