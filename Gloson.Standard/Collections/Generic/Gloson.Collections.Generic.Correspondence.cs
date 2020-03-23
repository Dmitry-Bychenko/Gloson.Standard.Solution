using System;
using System.Collections;
using System.Collections.Generic;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Read Only Correspondence (bi-direct dictionary)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface IReadOnlyCorrespondence<K, V> : IReadOnlyDictionary<K, V> {
    bool ContainsValue(V value);

    bool TryGetKey(V value, out K key);
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// One to one Key to Key correspondence (bi-direct dictionary)
  /// </summary>
  /// <typeparam name="K">Key (1st key)</typeparam>
  /// <typeparam name="V">Value (2nd key)</typeparam>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class Correspondence<K, V>
    : IDictionary<K, V>,
      IReadOnlyCorrespondence<K, V> {

    #region Private Data

    private readonly Dictionary<K, V> m_Direct;

    private readonly Dictionary<V, K> m_Reverse;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="capacity">Capacity</param>
    /// <param name="directComparer">Direct Comparer</param>
    /// <param name="reversedComparer">reversedComparer</param>
    public Correspondence(int capacity,
                          IEqualityComparer<K> directComparer,
                          IEqualityComparer<V> reversedComparer) {
      if (capacity <= 0)
        capacity = 0;

      if (null == directComparer)
        directComparer = EqualityComparer<K>.Default;

      if (null == reversedComparer)
        reversedComparer = EqualityComparer<V>.Default;

      m_Direct = new Dictionary<K, V>(capacity, directComparer);
      m_Reverse = new Dictionary<V, K>(capacity, reversedComparer);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public Correspondence(int capacity)
      : this(capacity, null, null) { }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public Correspondence(IEqualityComparer<K> directComparer,
                          IEqualityComparer<V> reversedComparer)
      : this(0, directComparer, reversedComparer) { }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public Correspondence()
      : this(0, null, null) { }

    #endregion Create

    #region IDictionary<K, V>

    /// <summary>
    /// Direct Indexer
    /// </summary>
    public V this[K key] {
      get {
        return m_Direct[key];
      }
      set {
        bool hasOld = (m_Direct.TryGetValue(key, out V oldValue));

        m_Direct.Add(key, value);

        try {
          m_Reverse.Add(value, key);
        }
        catch {
          if (hasOld)
            m_Direct[key] = oldValue;

          throw;
        }
      }
    }

    /// <summary>
    /// Keys
    /// </summary>
    public ICollection<K> Keys => m_Direct.Keys;

    /// <summary>
    /// Values
    /// </summary>
    public ICollection<V> Values => m_Direct.Values;

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_Direct.Count;

    /// <summary>
    /// Is Read Only
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Add
    /// </summary>
    public void Add(K key, V value) {
      m_Direct.Add(key, value);

      try {
        m_Reverse.Add(value, key);
      }
      catch {
        m_Direct.Remove(key);

        throw;
      }
    }

    /// <summary>
    /// Add
    /// </summary>
    public void Add(KeyValuePair<K, V> item) => Add(item.Key, item.Value);

    /// <summary>
    /// Clear
    /// </summary>
    public void Clear() {
      m_Direct.Clear();
      m_Reverse.Clear();
    }

    /// <summary>
    /// Contains Key
    /// </summary>
    public bool ContainsKey(K key) => m_Direct.ContainsKey(key);

    /// <summary>
    /// Contains Value
    /// </summary>
    public bool ContainsValue(V value) => m_Reverse.ContainsKey(value);

    /// <summary>
    /// Copy To
    /// </summary>
    public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) =>
      (m_Direct as IDictionary<K, V>).CopyTo(array, arrayIndex);

    /// <summary>
    /// Remove
    /// </summary>
    public bool Remove(K key) {
      if (m_Direct.TryGetValue(key, out V v)) {
        m_Reverse.Remove(v);
        m_Direct.Remove(key);

        return true;
      }

      return false;
    }

    /// <summary>
    /// Remove
    /// </summary>
    public bool RemoveValue(V value) {
      if (m_Reverse.TryGetValue(value, out K key)) {
        m_Reverse.Remove(value);
        m_Direct.Remove(key);

        return true;
      }

      return false;
    }

    /// <summary>
    /// Remove
    /// </summary>
    public bool Remove(KeyValuePair<K, V> item) {
      if (m_Direct.ContainsKey(item.Key)) {
        if (m_Reverse.ContainsKey(item.Value)) {
          m_Reverse.Remove(item.Value);
          m_Direct.Remove(item.Key);

          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Try Get Value
    /// </summary>
    public bool TryGetValue(K key, out V value) => m_Direct.TryGetValue(key, out value);

    /// <summary>
    /// Try Get Key
    /// </summary>
    public bool TryGetKey(V value, out K key) => m_Reverse.TryGetValue(value, out key);

    /// <summary>
    /// Enumerator
    /// </summary>
    public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => m_Direct.GetEnumerator();

    /// <summary>
    /// Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => m_Direct.GetEnumerator();

    /// <summary>
    /// Contains 
    /// </summary>
    public bool Contains(KeyValuePair<K, V> item) => ContainsKey(item.Key) && ContainsValue(item.Value);

    #endregion IDictionary<K, V>

    #region IReadOnlyCorrespondence<K, V>

    /// <summary>
    /// Read only Keys
    /// </summary>
    IEnumerable<K> IReadOnlyDictionary<K, V>.Keys => m_Direct.Keys;

    /// <summary>
    /// Read only Values
    /// </summary>
    IEnumerable<V> IReadOnlyDictionary<K, V>.Values => m_Reverse.Keys;

    #endregion #region IReadOnlyCorrespondence<K, V>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Enumerable Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Public

    /// <summary>
    /// To Correspondence
    /// </summary>
    public static Correspondence<K, V> ToCorrespondence<T, K, V>(this IEnumerable<T> source,
                                                                      Func<T, K> key,
                                                                      Func<T, V> value,
                                                                      IEqualityComparer<K> keyComparer,
                                                                      IEqualityComparer<V> valueComparer) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == key)
        throw new ArgumentNullException(nameof(key));
      else if (null == value)
        throw new ArgumentNullException(nameof(value));

      Correspondence<K, V> result = new Correspondence<K, V>(keyComparer, valueComparer);

      foreach (T item in source)
        result.Add(key(item), value(item));

      return result;
    }

    /// <summary>
    /// To Correspondence
    /// </summary>
    public static Correspondence<K, V> ToCorrespondence<T, K, V>(this IEnumerable<T> source,
                                                                      Func<T, K> key,
                                                                      Func<T, V> value) =>
      ToCorrespondence(source, key, value, null, null);

    #endregion Public
  }

}
