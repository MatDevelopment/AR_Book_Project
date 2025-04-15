using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        public float volume = 1f;
        public Vector2 pitchRange = new Vector2(1f, 1f); // For pitch variation
    }

    public List<Sound> sounds = new List<Sound>();

    private Dictionary<string, Sound> soundDict;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        soundDict = new Dictionary<string, Sound>();

        foreach (Sound s in sounds)
        {
            if (!soundDict.ContainsKey(s.name))
            {
                soundDict.Add(s.name, s);
            }
            else
            {
                Debug.LogWarning($"Duplicate sound name found: {s.name}");
            }
        }
    }

    public void PlaySound(string soundName)
    {
        if (soundDict.TryGetValue(soundName, out Sound s))
        {
            audioSource.pitch = Random.Range(s.pitchRange.x, s.pitchRange.y);
            audioSource.volume = s.volume;
            audioSource.loop = true;

            audioSource.clip = s.clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found on {gameObject.name}");
        }
    }

    public void PauseSound() => audioSource.Pause();

    public void StopSound() => audioSource.Stop();

    public void PlaySingleSound(string soundName)
    {
        if (soundDict.TryGetValue(soundName, out Sound s))
        {
            audioSource.pitch = Random.Range(s.pitchRange.x, s.pitchRange.y);
            audioSource.loop = false;
            audioSource.Stop();
            audioSource.PlayOneShot(s.clip, s.volume);
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found on {gameObject.name}");
        }
    }
}
