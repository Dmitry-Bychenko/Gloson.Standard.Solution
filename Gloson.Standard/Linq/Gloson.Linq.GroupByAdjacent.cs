using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Gloson.Collections.Generic;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Group By Adjacent 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Public

    /// <summary>
    /// Group By adjacent items only
    /// </summary>
    /// <typeparam name="S">Source</typeparam>
    /// <typeparam name="K">Key</typeparam>
    /// <typeparam name="V">Value</typeparam>
    /// <param name="source">Source</param>
    /// <param name="key">Key from source</param>
    /// <param name="value">Value from source</param>
    /// <param name="comparer">Keys comparer</param>
    /// <returns>Groups of adjacent items</returns>
    public static IEnumerable<IGrouping<K, V>> GroupByAdjacent<S, K, V>
      (this IEnumerable<S> source, 
            Func<S, K> key, 
            Func<S, V> value, 
            IEqualityComparer<K> comparer) {

      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == key)
        throw new ArgumentNullException(nameof(key));
      else if (null == value)
        throw new ArgumentNullException(nameof(value));

      if (null == comparer)
        comparer = EqualityComparer<K>.Default;

      if (null == comparer)
        throw new ArgumentNullException(
            nameof(comparer), 
          $"No default equality comparer found for {typeof(K).Name}");

      List<V> group = new List<V>();
      K expectedKey = default;

      foreach (S s in source) {
        if (group.Count <= 0) {
          expectedKey = key(s);

          group.Add(value(s));
        }
        else {
          K currentKey = key(s);

          if (comparer.Equals(currentKey, expectedKey))
            group.Add(value(s));
          else {
            yield return new Grouping<K, V>(expectedKey, group);

            expectedKey = currentKey;
            group.Clear();
            group.Add(value(s));
          }
        }
      }

      if (group.Count > 0)
        yield return new Grouping<K, V>(expectedKey, group);
    }

    /// <summary>
    /// Group By adjacent items only
    /// </summary>
    /// <typeparam name="S">Source</typeparam>
    /// <typeparam name="K">Key</typeparam>
    /// <typeparam name="V">Value</typeparam>
    /// <param name="source">Source</param>
    /// <param name="key">Key from source</param>
    /// <param name="value">Value from source</param>
    /// <returns>Groups of adjacent items</returns>
    public static IEnumerable<IGrouping<K, V>> GroupByAdjacent<S, K, V>
      (this IEnumerable<S> source,
            Func<S, K> key,
            Func<S, V> value) => GroupByAdjacent(source, key, value, null);

    /// <summary>
    /// Group By adjacent items only
    /// </summary>
    /// <typeparam name="S">Source</typeparam>
    /// <typeparam name="K">Key</typeparam>
    /// <param name="source">Source</param>
    /// <param name="key">Key from source</param>
    /// <param name="comparer">Keys comparer</param>
    /// <returns>Groups of adjacent items</returns>
    public static IEnumerable<IGrouping<K, S>> GroupByAdjacent<S, K>
     (this IEnumerable<S> source,
           Func<S, K> key,
           IEqualityComparer<K> comparer) => GroupByAdjacent<S, K, S>(source, key, item => item, comparer);

    /// <summary>
    /// Group By adjacent items only
    /// </summary>
    /// <typeparam name="S">Source</typeparam>
    /// <typeparam name="K">Key</typeparam>
    /// <param name="source">Source</param>
    /// <param name="key">Key from source</param>
    /// <returns>Groups of adjacent items</returns>
    public static IEnumerable<IGrouping<K, S>> GroupByAdjacent<S, K>
     (this IEnumerable<S> source,
           Func<S, K> key) => GroupByAdjacent<S, K, S>(source, key, item => item, null);

    #endregion Public
  }

}
