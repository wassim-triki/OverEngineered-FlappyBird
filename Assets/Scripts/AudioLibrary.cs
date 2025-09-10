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
        public Sfx id;
        public AudioClip[] clips;
        [Range(0f, 1f)] public float volume;
    }

    [SerializeField] private Entry[] entries;

    Dictionary<Sfx, int> _indexMap;

    void OnEnable() => RebuildIndex();

    void RebuildIndex()
    {
        if (entries == null) { _indexMap = null; return; }
        _indexMap = new Dictionary<Sfx, int>(entries.Length);
        for (int i = 0; i < entries.Length; i++)
        {
            var id = entries[i].id;
            if (!_indexMap.ContainsKey(id)) _indexMap.Add(id, i);
        }
    }

    public bool TryGetRandom(Sfx id, out AudioClip clip, out float defaultVolume)
    {
        clip = null;
        defaultVolume = 1f;

        if (_indexMap == null || !_indexMap.TryGetValue(id, out int idx)) return false;
        if (idx < 0 || idx >= entries.Length) return false;

        ref Entry e = ref entries[idx];
        if (e.clips == null || e.clips.Length == 0) return false;

        defaultVolume = Mathf.Clamp01(e.volume); // allow 0..1
        clip = e.clips[UnityEngine.Random.Range(0, e.clips.Length)];
        return clip != null;
    }
}