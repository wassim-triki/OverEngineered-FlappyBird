using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Audio Library")]
public class AudioLibrary : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public Sfx id;                 // e.g., Sfx.Jump
        public AudioClip[] clips;      // allow 1+ variants
        [Range(0f, 1f)] public float volume; // default volume for this SFX type
    }

    [SerializeField] private Entry[] entries;

    // Built once for fast lookups
    Dictionary<Sfx, Entry> _map;

    void OnEnable()
    {
        _map = new Dictionary<Sfx, Entry>(entries.Length);
        foreach (var e in entries)
            if (!_map.ContainsKey(e.id)) _map.Add(e.id, e);
    }

    public bool TryGetRandom(Sfx id, out AudioClip clip, out float defaultVolume)
    {
        clip = null; defaultVolume = 1f;
        if (_map == null || !_map.TryGetValue(id, out var e) || e.clips == null || e.clips.Length == 0)
            return false;

        defaultVolume = (e.volume <= 0f) ? 1f : e.volume;
        clip = e.clips[UnityEngine.Random.Range(0, e.clips.Length)];
        return clip != null;
    }
}