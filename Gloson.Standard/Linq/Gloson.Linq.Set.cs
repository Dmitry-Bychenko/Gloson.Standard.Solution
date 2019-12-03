using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Linq {

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
    /// Symmetric Except
    /// </summary>
    public static IEnumerable<T> SymmetricExcept<T>(this IEnumerable<T> source, 
                                                         IEnumerable<T> other, 
                                                         IEqualityComparer<T> comparer) {
      if (ReferenceEquals(null, source))
        throw new ArgumentNullException(nameof(source));
      else if (ReferenceEquals(null, other))
        throw new ArgumentNullException(nameof(other));

      if (ReferenceEquals(null, comparer))
        comparer = EqualityComparer<T>.Default;

      if (ReferenceEquals(null, comparer))
        throw new ArgumentNullException(nameof(comparer), 
          $"{typeof(T).Name} doesn't provide default IEqualityComparer<{typeof(T).Name}>");

      HashSet<T> hs = new HashSet<T>(source, comparer);

      hs.SymmetricExceptWith(other);

      foreach (T item in hs)
        yield return item;
    }

    /// <summary>
    /// Symmetric Except
    /// </summary>
    public static IEnumerable<T> SymmetricExcept<T>(this IEnumerable<T> source, IEnumerable<T> other) =>
      SymmetricExcept(source, other, null);

    /// <summary>
    /// Intersect with duplicates
    /// </summary>
    public static IEnumerable<T> IntersectWithDuplicates<T>(this IEnumerable<T> source,
                                                                 IEnumerable<T> other,
                                                                 IEqualityComparer<T> comparer) {
      if (ReferenceEquals(null, source))
        throw new ArgumentNullException(nameof(source));
      else if (ReferenceEquals(null, other))
        throw new ArgumentNullException(nameof(other));

      if (ReferenceEquals(null, comparer))
        comparer = EqualityComparer<T>.Default;

      if (ReferenceEquals(null, comparer))
        throw new ArgumentNullException(nameof(comparer),
          $"{typeof(T).Name} doesn't provide default IEqualityComparer<{typeof(T).Name}>");

      Dictionary<T, int> dict1 = source
        .GroupBy(item => item, comparer)
        .ToDictionary(group => group.Key, group => group.Count());

      Dictionary<T, int> dict2 = other
        .GroupBy(item => item, comparer)
        .ToDictionary(group => group.Key, group => group.Count());

      foreach (var pair in dict1) {
        if (dict2.TryGetValue(pair.Key, out int v2)) {
          int v = Math.Min(pair.Value, v2);

          for (int i = 0; i < v; ++i)
            yield return pair.Key;
        }
      }
    }

    /// <summary>
    /// Intersect with duplicates
    /// </summary>
    public static IEnumerable<T> IntersectWithDuplicates<T>(this IEnumerable<T> source, IEnumerable<T> other) =>
      IntersectWithDuplicates(source, other, null);

    /// <summary>
    /// Except with duplicates
    /// </summary>
    public static IEnumerable<T> ExceptWithDuplicates<T>(this IEnumerable<T> source,
                                                              IEnumerable<T> other,
                                                              IEqualityComparer<T> comparer) {
      if (ReferenceEquals(null, source))
        throw new ArgumentNullException(nameof(source));
      else if (ReferenceEquals(null, other))
        throw new ArgumentNullException(nameof(other));

      if (ReferenceEquals(null, comparer))
        comparer = EqualityComparer<T>.Default;

      if (ReferenceEquals(null, comparer))
        throw new ArgumentNullException(nameof(comparer),
          $"{typeof(T).Name} doesn't provide default IEqualityComparer<{typeof(T).Name}>");

      Dictionary<T, int> dict = source
        .GroupBy(item => item, comparer)
        .ToDictionary(group => group.Key, group => group.Count());

      foreach (var item in other) {
        if (dict.TryGetValue(item, out int v)) {
          v -= 1;

          if (v == 0)
            dict.Remove(item);
          else
            dict[item] = v; 
        }
      }

      foreach (var pair in dict) 
        for (int i = 0; i < pair.Value; ++i)
          yield return pair.Key;
    }

    /// <summary>
    /// Except with duplicates
    /// </summary>
    public static IEnumerable<T> ExceptWithDuplicates<T>(this IEnumerable<T> source, IEnumerable<T> other) =>
      ExceptWithDuplicates(source, other, null);

    /// <summary>
    /// Union with duplicates
    /// </summary>
    public static IEnumerable<T> UnionWithDuplicates<T>(this IEnumerable<T> source,
                                                             IEnumerable<T> other,
                                                             IEqualityComparer<T> comparer) {
      if (ReferenceEquals(null, source))
        throw new ArgumentNullException(nameof(source));
      else if (ReferenceEquals(null, other))
        throw new ArgumentNullException(nameof(other));

      if (ReferenceEquals(null, comparer))
        comparer = EqualityComparer<T>.Default;

      if (ReferenceEquals(null, comparer))
        throw new ArgumentNullException(nameof(comparer),
          $"{typeof(T).Name} doesn't provide default IEqualityComparer<{typeof(T).Name}>");

      Dictionary<T, int> dict1 = source
        .GroupBy(item => item, comparer)
        .ToDictionary(group => group.Key, group => group.Count());

      Dictionary<T, int> dict2 = other
        .GroupBy(item => item, comparer)
        .ToDictionary(group => group.Key, group => group.Count());

      foreach (var pair in dict1) {
        int v = pair.Value;

        if (dict2.TryGetValue(pair.Key, out int v2))
          v = Math.Max(v, v2);

        for (int i = 0; i < v; ++i)
          yield return pair.Key;
      }

      foreach (var pair in dict2) {
        if (!dict1.ContainsKey(pair.Key))
          for (int i = 0; i < pair.Value; ++i)
            yield return pair.Key;
      }
    }

    /// <summary>
    /// Union with duplicates
    /// </summary>
    public static IEnumerable<T> UnionWithDuplicates<T>(this IEnumerable<T> source, IEnumerable<T> other) =>
      UnionWithDuplicates(source, other, null);

    /// <summary>
    /// Symmetric Except with duplicates
    /// </summary>
    public static IEnumerable<T> SymmetricExceptWithDuplicates<T>(this IEnumerable<T> source,
                                                             IEnumerable<T> other,
                                                             IEqualityComparer<T> comparer) {
      if (ReferenceEquals(null, source))
        throw new ArgumentNullException(nameof(source));
      else if (ReferenceEquals(null, other))
        throw new ArgumentNullException(nameof(other));

      if (ReferenceEquals(null, comparer))
        comparer = EqualityComparer<T>.Default;

      if (ReferenceEquals(null, comparer))
        throw new ArgumentNullException(nameof(comparer),
          $"{typeof(T).Name} doesn't provide default IEqualityComparer<{typeof(T).Name}>");

      Dictionary<T, int> dict1 = source
        .GroupBy(item => item, comparer)
        .ToDictionary(group => group.Key, group => group.Count());

      Dictionary<T, int> dict2 = other
        .GroupBy(item => item, comparer)
        .ToDictionary(group => group.Key, group => group.Count());

      foreach (var pair in dict1) {
        int v = pair.Value;

        if (dict2.TryGetValue(pair.Key, out int v2)) 
          v = Math.Min(v, v2);

        for (int i = 0; i < v; ++i)
          yield return pair.Key;
      }

      foreach (var pair in dict2) {
        if (!dict1.ContainsKey(pair.Key))
          for (int i = 0; i < pair.Value; ++i)
            yield return pair.Key;
      }
    }

    /// <summary>
    /// Symmetric Except with duplicates
    /// </summary>
    public static IEnumerable<T> SymmetricExceptWithDuplicates<T>(this IEnumerable<T> source, IEnumerable<T> other) =>
      SymmetricExceptWithDuplicates(source, other, null);

    #endregion Public
  }
}
