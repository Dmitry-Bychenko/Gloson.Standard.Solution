using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// IDictionary extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class DictionaryExtensions {
    #region Public

    /// <summary>
    /// Try Add Or Update
    /// </summary>
    /// <typeparam name="K">Key</typeparam>
    /// <typeparam name="V">Value</typeparam>
    /// <param name="dictionary">Dictionary</param>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="tie">Tie break</param>
    /// <returns>true if value added; false if value updated</returns>
    public static bool TryAddOrUpdate<K, V>(
      this IDictionary<K, V> dictionary, 
           K key,
           V value,
           Func<K, V, V> tie) {

      if (null == dictionary)
        throw new ArgumentNullException(nameof(dictionary));

      if (dictionary.TryGetValue(key, out var prior)) {
        if (tie != null)
          value = tie(key, prior);

        dictionary[key] = value;

        return false;
      }
      else
        dictionary.Add(key, value);

      return true;
    }

    /// <summary>
    /// Try Add Or Update
    /// </summary>
    /// <typeparam name="K">Key</typeparam>
    /// <typeparam name="V">Value</typeparam>
    /// <param name="dictionary">Dictionary</param>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <returns>true if value added; false if value updated</returns>
    public static bool TryAddOrUpdate<K, V>(
      this IDictionary<K, V> dictionary,
           K key,
           V value) => TryAddOrUpdate(dictionary, key, value, null);

    #endregion Public
  }

}
