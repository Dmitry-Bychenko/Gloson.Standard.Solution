using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// IGrouping interface implementation
  /// </summary>
  /// <typeparam name="K">Key</typeparam>
  /// <typeparam name="V">Values</typeparam>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class Grouping<K, V> 
    : IGrouping<K, V>,
      IReadOnlyList<V> {

    #region Private

    private List<V> m_Items;

    #endregion Private

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="items">Items</param>
    public Grouping(K key, IEnumerable<V> items) {
      if (null == items)
        throw new ArgumentNullException(nameof(items));

      m_Items = new List<V>(items);
    }

    #endregion Create

    #region IGrouping<K, V>

    /// <summary>
    /// Key
    /// </summary>
    public K Key { get; }

    /// <summary>
    /// Enumerator 
    /// </summary>
    public IEnumerator<V> GetEnumerator() => m_Items.GetEnumerator();

    /// <summary>
    /// Enumerator
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator() => m_Items.GetEnumerator();

    #endregion IGrouping<K, V>

    #region IReadOnlyList<V>

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_Items.Count;

    /// <summary>
    /// Indexer
    /// </summary>
    public V this[int index] => m_Items[index];

    #endregion IReadOnlyList<V> 
  }

}
