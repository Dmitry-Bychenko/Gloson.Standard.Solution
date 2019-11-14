using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Recursion generator
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class Recursion {
    #region Public

    /// <summary>
    /// Generate
    /// </summary>
    public static IEnumerable<T> Generate<T>(Func<T> next) {
      if (null == next)
        throw new ArgumentNullException(nameof(next));

      while (true) 
        yield return next();
    }

    /// <summary>
    /// Generate
    /// </summary>
    public static IEnumerable<T> Generate<T>(T first, Func<T, T> next) {
      if (null == next)
        throw new ArgumentNullException(nameof(next));

      yield return first;

      while (true) {
        first = next(first);

        yield return first;
      }
    }

    /// <summary>
    /// Generate
    /// </summary>
    public static IEnumerable<T> Generate<T>(T first, T second, Func<T, T, T> next) {
      if (null == next)
        throw new ArgumentNullException(nameof(next));

      yield return first;
      yield return second;

      while (true) {
        T result = next(first, second);

        yield return result;

        first  = second;
        second = result;
      }
    }

    /// <summary>
    /// Generate
    /// </summary>
    public static IEnumerable<T> Generate<T>(T first, T second, T third, Func<T, T, T, T> next) {
      if (null == next)
        throw new ArgumentNullException(nameof(next));

      yield return first;
      yield return second;
      yield return third;

      while (true) {
        T result = next(first, second, third);

        yield return result;

        first  = second;
        second = third;
        third  = result;
      }
    }

    /// <summary>
    /// Generate
    /// </summary>
    public static IEnumerable<T> Generate<T>(T first, T second, T third, T fourth, Func<T, T, T, T, T> next) {
      if (null == next)
        throw new ArgumentNullException(nameof(next));

      yield return first;
      yield return second;
      yield return third;
      yield return fourth;

      while (true) {
        T result = next(first, second, third, fourth);

        yield return result;

        first  = second;
        second = third;
        third  = fourth;
        fourth = result;
      }
    }

    /// <summary>
    /// Generate
    /// </summary>
    public static IEnumerable<T> Generate<T>(Func<T[], T> next, params T[] seeds) {
      if (null == next)
        throw new ArgumentNullException(nameof(next));
      else if (null == seeds)
        throw new ArgumentNullException(nameof(seeds));
      else if (seeds.Length <= 0)
        throw new ArgumentException("Seends can't be empty", nameof(seeds));

      Queue<T> queue = new Queue<T>();

      foreach (T item in seeds) {
        queue.Enqueue(item);

        yield return item;
      }

      while (true) {
        T current = next(queue.ToArray());

        yield return current;

        queue.Enqueue(queue.Dequeue());
      }
    }

    #endregion Public
  }

}
