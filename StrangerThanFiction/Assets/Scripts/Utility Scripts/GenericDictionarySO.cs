using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericDictionarySO<TKey, TValue> : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public TKey Key;
        public TValue Value;
    }

    [SerializeField]
    protected List<Entry> entries = new List<Entry>();

    public virtual void ClearEntries() => entries.Clear();

    public virtual void AddEntry(TKey key, TValue value)
    {
        entries.Add(new Entry { Key = key, Value = value });
    }

    public virtual IReadOnlyList<Entry> GetEntries() => entries;

    protected Dictionary<TKey, TValue> dictionary;

    protected virtual void OnEnable() => Initialize();

    protected void Initialize()
    {
        if (dictionary != null) return;
        dictionary = new Dictionary<TKey, TValue>();
        foreach (var entry in entries)
        {
            if (!dictionary.ContainsKey(entry.Key))
                dictionary.Add(entry.Key, entry.Value);
        }
    }

    public TValue GetByKey(TKey key) =>
        dictionary != null && dictionary.TryGetValue(key, out var value) ? value : default;
}
