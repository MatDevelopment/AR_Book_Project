using UnityEngine;
using System;
using UnityEngine.Audio;
public class AudioManager : MonoBehaviour
{
    public Sound[] sounds; //Skaber et array kaldet sounds som indeholder alle sounds
    
    // Start is called before the first frame update
    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();     //Tager fat i Audio Source komponenten p� et gameObject
            s.source.clip = s.clip;         //S�tter audio klippet fra audio source component til s.clip

            s.source.volume = s.volume;     //S�tter volume fra audio source component til s.volume
            s.source.pitch = s.pitch;       //S�tter pitch fra audio source til s.pitch
            s.source.loop = s.loop;
        }
    }

    public void Play (string name)       //Tilf�jer en m�de at spille en specifik sound ud fra den string name
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);     //Finder den sound i 'sounds' array som har det samme navn som string name (i denne void) og opbevarer den fundne sound i variablen 's'
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }

    public void StopSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        
        s.source.Stop();
    }
}
