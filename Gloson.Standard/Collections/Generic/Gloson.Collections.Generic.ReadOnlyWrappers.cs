using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// IReadOnlyHashSet
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface IReadOnlyHashSet<T> : IEnumerable<T> {
    /// <summary>
    /// Count
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Contains
    /// </summary>
    public bool Contains(T value);
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// ReadOnly HashSet Wrapper
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ReadOnlyHashSetWrapper<T> 
    : IReadOnlyHashSet<T> {

    #region Private Data

    private readonly HashSet<T> m_HashSet;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public ReadOnlyHashSetWrapper(HashSet<T> value) {
      m_HashSet = value ?? throw new ArgumentNullException(nameof(value));
    }

    #endregion Create

    #region IReadOnlyHashSet<T>

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_HashSet.Count;

    /// <summary>
    /// Contains
    /// </summary>
    public bool Contains(T value) => m_HashSet.Contains(value);

    /// <summary>
    /// Get Enumerator
    /// </summary>
    public IEnumerator<T> GetEnumerator() => m_HashSet.GetEnumerator();

    /// <summary>
    /// Get Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => m_HashSet.GetEnumerator();

    #endregion IReadOnlyHashSet<T>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// ReadOnlyArray Wrapper
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ReadOnlyArrayWrapper<T> 
    : IReadOnlyList<T> {

    #region Private Data

    private readonly T[] m_Array;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard collection
    /// </summary>
    public ReadOnlyArrayWrapper(T[] value) {
      m_Array = value ?? throw new ArgumentNullException(nameof(value));
    }

    #endregion Create

    #region IReadOnlyList<T>

    /// <summary>
    /// Indexer
    /// </summary>
    public T this[int index] => m_Array[index];

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_Array.Length;

    /// <summary>
    /// Get Enumerator
    /// </summary>
    public IEnumerator<T> GetEnumerator() => (m_Array as IEnumerable<T>).GetEnumerator();

    /// <summary>
    /// Get Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => m_Array.GetEnumerator();

    #endregion IReadOnlyList<T>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// ReadOnly Correspondence (bi-direct dictionary)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ReadOnlyCorrespondenceWrapper<K, V> : IReadOnlyCorrespondence<K, V> {
    #region Private Data

    private readonly Correspondence<K, V> m_Correspondence;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public ReadOnlyCorrespondenceWrapper(Correspondence<K, V> value) {
      m_Correspondence = value ?? throw new ArgumentNullException(nameof(value));
    }

    #endregion Create

    #region IReadOnlyCorrespondence<K, V>

    /// <summary>
    /// Indexer
    /// </summary>
    public V this[K key] => m_Correspondence[key];

    /// <summary>
    /// Keys
    /// </summary>
    public IEnumerable<K> Keys => m_Correspondence.Keys;

    /// <summary>
    /// Values
    /// </summary>
    public IEnumerable<V> Values => m_Correspondence.Values;

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_Correspondence.Count;

    /// <summary>
    /// Contains Key
    /// </summary>
    public bool ContainsKey(K key) => m_Correspondence.ContainsKey(key);

    /// <summary>
    /// Contains Value
    /// </summary>
    public bool ContainsValue(V value) => m_Correspondence.ContainsValue(value);
    
    /// <summary>
    /// Try Get Key
    /// </summary>
    public bool TryGetKey(V value, out K key) => m_Correspondence.TryGetKey(value, out key);

    /// <summary>
    /// Try Get Value
    /// </summary>
    public bool TryGetValue(K key, out V value) => m_Correspondence.TryGetValue(key, out value);

    /// <summary>
    /// Enumeartor
    /// </summary>
    public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => m_Correspondence.GetEnumerator();

    /// <summary>
    /// Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => m_Correspondence.GetEnumerator();

    #endregion IReadOnlyCorrespondence<K, V>
  }
}
