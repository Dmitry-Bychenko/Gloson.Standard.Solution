using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Linq random
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Public

    /// <summary>
    /// Shuffle items
    /// </summary>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random generator) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      Func<int, int> next = (arg) => RandomThreadSafe.Default.Next(arg);

      if (generator is not null)
        next = (arg) => generator.Next(arg);
      else if (generator is null)
        generator = new Random();

      T[] data = source.ToArray();

      int low = data.GetLowerBound(0);
      int high = data.GetUpperBound(0);

      for (int i = low; i < high; ++i) {
        int index = i + next(high - i + 1);

        var h = data.GetValue(i);
        data.SetValue(data.GetValue(index), i);
        data.SetValue(h, index);
      }

      return data;
    }

    /// <summary>
    /// Shuffle items
    /// </summary>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source) {
      return Shuffle(source, null);
    }

    /// <summary>
    /// Shuffle items
    /// </summary>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, int seed) {
      return Shuffle(source, new Random(seed));
    }

    /// <summary>
    /// Extract sub sequences (e.g. training, cv and test) from the main one
    /// </summary>
    /// <param name="source">source sequence</param>
    /// <param name="generator">random generator (null fro default one)</param>
    /// <param name="weights">weighs, the last one is computed</param>
    /// <returns></returns>
    public static IEnumerable<T>[] Extract<T>(this IEnumerable<T> source, Random generator, params Double[] weights) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      else if (weights is null)
        return new IEnumerable<T>[] { source };
      else if (weights.Length <= 0)
        return new IEnumerable<T>[] { source };

      if (weights.Any(x => (x < 0) || (x > 1.0)))
        throw new ArgumentOutOfRangeException(nameof(weights), "All weights must be in [0..1] range.");
      else if (weights.Sum() > 1.0)
        throw new ArgumentOutOfRangeException(nameof(weights), "Sum of weights must be less or equal to one.");

      IList<T>[] result = Enumerable
        .Range(0, weights.Length + 1)
        .Select(_ => new List<T>())
        .ToArray();

      Func<double> next = () => RandomThreadSafe.Default.NextDouble();

      if (generator is not null)
        next = () => generator.NextDouble();

      foreach (var item in source) {
        Double v = next();
        Double s = 0.0;
        int index = result.Length - 1;

        for (int i = 0; i < weights.Length; ++i) {
          s += weights[i];

          if (v < s) {
            index = i;

            break;
          }
        }

        result[index].Add(item);
      }

      return result;
    }

    /// <summary>
    /// Extract sub sequences (e.g. training, cv and test) from the main one
    /// </summary>
    /// <param name="source">source sequence</param>
    /// <param name="weights">weighs, the last one is computed</param>
    /// <returns></returns>
    public static IEnumerable<T>[] Extract<T>(this IEnumerable<T> source, params Double[] weights) {
      return Extract(source, null, weights);
    }

    #endregion Public
  }

}
