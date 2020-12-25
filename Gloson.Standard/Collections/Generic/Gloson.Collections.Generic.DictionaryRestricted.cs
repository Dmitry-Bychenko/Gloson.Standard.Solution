using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Restricted Dictionary (MaxCount items at most)
  /// </summary>
  /// <typeparam name="K">Key</typeparam>
  /// <typeparam name="V">Value</typeparam>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class DictionaryRestricted<K, V> : IDictionary<K, V> {
    #region Private Data

    private readonly Dictionary<K, LinkedListNode<(K key, V value)>> m_Dictionary;

    private readonly LinkedList<(K key, V value)> m_List = new LinkedList<(K key, V value)>();

    private int m_MaxLength;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public DictionaryRestricted(int maxLength, IEqualityComparer<K> comparer) {
      m_MaxLength = maxLength > 0 ? maxLength : throw new ArgumentOutOfRangeException(nameof(maxLength));
      Comparer = comparer ?? EqualityComparer<K>.Default;

      m_Dictionary = new Dictionary<K, LinkedListNode<(K key, V value)>>(comparer);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public DictionaryRestricted(int maxLength)
      : this(maxLength, null) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Comparer
    /// </summary>
    public IEqualityComparer<K> Comparer { get; }

    /// <summary>
    /// MaxLength
    /// </summary>
    public int MaxLength {
      get => m_MaxLength;
      set {
        if (value <= 0)
          throw new ArgumentOutOfRangeException(nameof(value));

        if (value >= m_MaxLength) {
          m_MaxLength = value;

          return;
        }

        m_MaxLength = value;

        while (m_List.Count > m_MaxLength) {
          m_Dictionary.Remove(m_List.First.Value.key);
          m_List.RemoveFirst();
        }
      }
    }

    /// <summary>
    /// Peek Key
    /// </summary>
    public bool PeekKey(K key) {
      if (m_Dictionary.TryGetValue(key, out var actual)) {
        m_List.Remove(actual);
        m_List.AddLast(actual);

        return true;
      }

      return false;
    }

    /// <summary>
    /// Add Or Update
    /// </summary>
    public void AddOrUpdate(K key, V value) {
      if (m_Dictionary.TryGetValue(key, out var actual)) {
        actual.Value = (key, value);

        m_List.Remove(actual);
        m_List.AddLast(actual);
      }
      else {
        LinkedListNode<(K key, V value)> node = new LinkedListNode<(K key, V value)>((key, value));

        m_Dictionary.Add(key, node);

        m_List.AddLast(node);

        while (m_List.Count > m_MaxLength) {
          node = m_List.First;

          m_Dictionary.Remove(node.Value.key);
          m_List.RemoveFirst();
        }
      }
    }

    /// <summary>
    /// Try Add
    /// </summary>
    public bool TryAdd(K key, V value) {
      if (!m_Dictionary.ContainsKey(key)) {
        LinkedListNode<(K key, V value)> node = new LinkedListNode<(K key, V value)>((key, value));

        m_Dictionary.Add(key, node);

        m_List.AddLast(node);

        while (m_List.Count > m_MaxLength) {
          node = m_List.First;

          m_Dictionary.Remove(node.Value.key);
          m_List.RemoveFirst();
        }

        return true;
      }
      else
        return false;
    }

    /// <summary>
    /// Try Update
    /// </summary>
    public bool TryUpdate(K key, V value) {
      if (m_Dictionary.TryGetValue(key, out var actual)) {
        actual.Value = (key, value);

        m_List.Remove(actual);
        m_List.AddLast(actual);

        return true;
      }

      return false;
    }

    /// <summary>
    /// Try Peek Value
    /// </summary>
    public bool TryPeekValue(K key, out V value) {
      if (m_Dictionary.TryGetValue(key, out var actual)) {
        value = actual.Value.value;

        return true;
      }

      value = default;

      return false;
    }

    /// <summary>
    /// Key Value pair in deletion order
    /// </summary>
    public KeyValuePair<K, V>[] Items => m_List
      .Select(node => new KeyValuePair<K, V>(node.key, node.value))
      .ToArray();

    #endregion Public

    #region IDictionary<K, V>

    /// <summary>
    /// Keys
    /// </summary>
    public ICollection<K> Keys => m_Dictionary.Keys;

    /// <summary>
    /// Values
    /// </summary>
    public ICollection<V> Values => m_Dictionary.Values.Select(item => item.Value.value).ToList();

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_Dictionary.Count;

    /// <summary>
    /// Is ReadOnly (always false)
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Indexer
    /// </summary>
    public V this[K key] {
      get {
        var actual = m_Dictionary[key];

        m_List.Remove(actual);
        m_List.AddLast(actual);

        return actual.Value.value;
      }
      set {
        var actual = m_Dictionary[key];

        m_List.Remove(actual);
        m_List.AddLast(actual);

        actual.Value = (key, value);
      }
    }

    /// <summary>
    /// Add
    /// </summary>
    public void Add(K key, V value) {
      LinkedListNode<(K key, V value)> node = new LinkedListNode<(K key, V value)>((key, value));

      m_Dictionary.Add(key, node);

      m_List.AddLast(node);

      if (m_List.Count > m_MaxLength) {
        node = m_List.First;

        m_Dictionary.Remove(node.Value.key);
        m_List.RemoveFirst();
      }
    }

    /// <summary>
    /// Contains Key
    /// </summary>
    public bool ContainsKey(K key) => m_Dictionary.ContainsKey(key);

    /// <summary>
    /// Remove Key
    /// </summary>
    public bool Remove(K key) {
      if (m_Dictionary.TryGetValue(key, out var actual)) {
        m_List.Remove(actual);
        m_Dictionary.Remove(key);

        return true;
      }

      return false;
    }

    /// <summary>
    /// Try Get value
    /// </summary>
    public bool TryGetValue(K key, [MaybeNullWhen(false)] out V value) {
      if (m_Dictionary.TryGetValue(key, out var actual)) {
        value = actual.Value.value;

        m_List.Remove(actual);
        m_List.AddLast(actual);

        return true;
      }

      value = default;

      return false;
    }

    /// <summary>
    /// Add Value
    /// </summary>
    public void Add(KeyValuePair<K, V> item) {
      LinkedListNode<(K key, V value)> node = new LinkedListNode<(K key, V value)>((item.Key, item.Value));

      m_Dictionary.Add(item.Key, node);

      m_List.AddLast(node);

      while (m_List.Count > m_MaxLength) {
        node = m_List.First;

        m_Dictionary.Remove(node.Value.key);
        m_List.RemoveFirst();
      }
    }

    /// <summary>
    /// Clear
    /// </summary>
    public void Clear() {
      m_Dictionary.Clear();
      m_List.Clear();
    }

    /// <summary>
    /// Contains
    /// </summary>
    public bool Contains(KeyValuePair<K, V> item) =>
      m_Dictionary.TryGetValue(item.Key, out var actual) &&
      (object.Equals(item.Value, actual));

    /// <summary>
    /// Copy To
    /// </summary>
    public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) {
      if (array is null)
        throw new ArgumentNullException(nameof(array));
      else if (arrayIndex < array.GetLowerBound(0))
        throw new ArgumentOutOfRangeException(nameof(arrayIndex), "arrayIndex is below lower bound");
      else if (arrayIndex > array.GetUpperBound(0))
        throw new ArgumentOutOfRangeException(nameof(arrayIndex), "arrayIndex is above upper bound");
      else if (arrayIndex + m_Dictionary.Count > array.GetUpperBound(0))
        throw new ArgumentException("Array is too short.", nameof(array));

      int index = arrayIndex;

      foreach (var pair in m_Dictionary)
        array[index++] = new KeyValuePair<K, V>(pair.Key, pair.Value.Value.value);
    }

    /// <summary>
    /// Remove
    /// </summary>
    public bool Remove(KeyValuePair<K, V> item) {
      if (m_Dictionary.TryGetValue(item.Key, out var actual)) {
        if (object.Equals(actual, item.Value)) {
          m_List.Remove(actual);
          m_Dictionary.Remove(item.Key);

          return true;
        }
        else
          return false;
      }
      else
        return false;
    }

    /// <summary>
    /// Typed Enumerator
    /// </summary>
    public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => m_Dictionary
      .Select(pair => new KeyValuePair<K, V>(pair.Key, pair.Value.Value.value))
      .GetEnumerator();

    /// <summary>
    /// Typeless Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion IDictionary<K, V>
  }

}
