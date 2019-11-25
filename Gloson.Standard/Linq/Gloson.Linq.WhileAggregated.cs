using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Linq {
  
  public static partial class EnumerableExtensions {
    #region Public

    /// <summary>
    /// Take While Aggregate
    /// </summary>
    public static IEnumerable<T> TakeWhileAggregated<T, V>(
      this IEnumerable<T> source,
      V seed,
      Func<V, T, V> aggregate,
      Func<V, bool> condition) {

      if (null == source)
        throw new ArgumentNullException(nameof(source));

      if (null == aggregate)
        throw new ArgumentNullException(nameof(aggregate));

      if (null == condition)
        throw new ArgumentNullException(nameof(condition));

      V aggregatedValue = seed;

      foreach (T item in source) {
        aggregatedValue = aggregate(aggregatedValue, item);

        if (!condition(aggregatedValue))
          yield break;

        yield return item;
      }
    }

    /// <summary>
    /// Take While Aggregate
    /// </summary>
    public static IEnumerable<T> TakeWhileAggregated<T>(
      this IEnumerable<T> source,
      Func<T, T, T> aggregate,
      Func<T, bool> condition) {

      if (null == source)
        throw new ArgumentNullException(nameof(source));

      if (null == aggregate)
        throw new ArgumentNullException(nameof(aggregate));

      if (null == condition)
        throw new ArgumentNullException(nameof(condition));

      T aggregatedValue = default;
      bool first = true;

      foreach (T item in source) {
        if (first) {
          first = false;
          aggregatedValue = item;
        }
        else
          aggregatedValue = aggregate(aggregatedValue, item);

        if (!condition(aggregatedValue))
          yield break;

        yield return item;
      }
    }

    /// <summary>
    /// Skip While Aggregate
    /// </summary>
    public static IEnumerable<T> SkipWhileAggregated<T, V>(
      this IEnumerable<T> source,
      V seed,
      Func<V, T, V> aggregate,
      Func<V, bool> condition) {

      if (null == source)
        throw new ArgumentNullException(nameof(source));

      if (null == aggregate)
        throw new ArgumentNullException(nameof(aggregate));

      if (null == condition)
        throw new ArgumentNullException(nameof(condition));

      V aggregatedValue = seed;
      bool skip = true;

      foreach (T item in source) {
        if (skip) {
          aggregatedValue = aggregate(aggregatedValue, item);

          if (!condition(aggregatedValue)) {
            skip = false;

            yield return item;
          }
        }
        else
          yield return item;
      }
    }

    /// <summary>
    /// Take While Aggregate
    /// </summary>
    public static IEnumerable<T> SkipWhileAggregated<T>(
      this IEnumerable<T> source,
      Func<T, T, T> aggregate,
      Func<T, bool> condition) {

      if (null == source)
        throw new ArgumentNullException(nameof(source));

      if (null == aggregate)
        throw new ArgumentNullException(nameof(aggregate));

      if (null == condition)
        throw new ArgumentNullException(nameof(condition));

      T aggregatedValue = default;
      bool first = true;
      bool skip = true;

      foreach (T item in source) {
        if (skip) {
          if (first) {
            first = false;
            aggregatedValue = item;
          }
          else
            aggregatedValue = aggregate(aggregatedValue, item);

          if (!condition(aggregatedValue)) {
            skip = false; 

            yield return item;
          }
        }
        else
          yield return item;
      }
    }

    /// <summary>
    /// Skip While Aggregate
    /// </summary>
    public static IEnumerable<T> WhereAggregated<T, V>(
      this IEnumerable<T> source,
      V seed,
      Func<V, T, V> aggregate,
      Func<V, bool> condition) {

      if (null == source)
        throw new ArgumentNullException(nameof(source));

      if (null == aggregate)
        throw new ArgumentNullException(nameof(aggregate));

      if (null == condition)
        throw new ArgumentNullException(nameof(condition));

      V aggregatedValue = seed;

      foreach (T item in source) {
        aggregatedValue = aggregate(aggregatedValue, item);

        if (condition(aggregatedValue))
          yield return item;
      }
    }

    /// <summary>
    /// Take While Aggregate
    /// </summary>
    public static IEnumerable<T> WhereAggregated<T>(
      this IEnumerable<T> source,
      Func<T, T, T> aggregate,
      Func<T, bool> condition) {

      if (null == source)
        throw new ArgumentNullException(nameof(source));

      if (null == aggregate)
        throw new ArgumentNullException(nameof(aggregate));

      if (null == condition)
        throw new ArgumentNullException(nameof(condition));

      T aggregatedValue = default;
      bool first = true;

      foreach (T item in source) {
        if (first) {
          first = false;
          aggregatedValue = item;
        }
        else
          aggregatedValue = aggregate(aggregatedValue, item);

        if (condition(aggregatedValue))
          yield return item;
      }
    }

    #endregion Public
  }
}
