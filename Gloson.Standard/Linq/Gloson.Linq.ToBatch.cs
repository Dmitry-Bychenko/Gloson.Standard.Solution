using System;
using System.Collections.Generic;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// To Batch
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Public

    /// <summary>
    /// To Batch
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="addToBatch">If current item should be added into current batch</param>
    /// <returns>Enumeration of batches</returns>
    public static IEnumerable<T[]> ToBatch<T>(
      this IEnumerable<T> source,
      Func<ICollection<T>, T, bool> addToBatch) {

      if (source is null)
        throw new ArgumentNullException(nameof(source));
      else if (addToBatch is null)
        throw new ArgumentNullException(nameof(addToBatch));

      List<T> batch = new List<T>();

      foreach (T item in source) {
        if (batch.Count > 0 && !addToBatch(batch, item)) {
          yield return batch.ToArray();

          batch.Clear();
        }

        batch.Add(item);
      }

      if (batch.Count > 0)
        yield return batch.ToArray();
    }

    #endregion Public
  }

}
