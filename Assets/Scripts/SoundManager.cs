using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    Background,
    Jump,
    Crash
}

[System.Serializable]
public class SoundEntry
{
    public SoundType type;
    public AudioSource source;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private List<SoundEntry> sounds;

    private Dictionary<SoundType, AudioSource> soundDict;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        soundDict = new Dictionary<SoundType, AudioSource>();
        foreach (var s in sounds)
        {
            soundDict[s.type] = s.source;
        }
    }

    public void PlaySound(SoundType type, bool isLoop = false)
    {
        if (soundDict.TryGetValue(type, out var src))
        {
            if (!src.isPlaying) 
                src.Play();

            src.loop = isLoop;
        }
    }

    public void StopSound(SoundType type)
    {
        if (soundDict.TryGetValue(type, out var src))
        {
            if (src.isPlaying) 
                src.Stop();
        }
    }
}
