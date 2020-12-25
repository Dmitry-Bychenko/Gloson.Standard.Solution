using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class Enumerables {
    #region Algorithm

    private static int FastCount<T>(IEnumerable<T> sequence) {
      if (sequence is IReadOnlyList<T> r)
        return r.Count;
      else if (sequence is IList<T> l)
        return l.Count;
      else if (sequence is T[] a)
        return a.Length;
      else if (sequence is ICollection<T> c)
        return c.Count;

      return -1;
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Elaborated Sequence Equals
    /// </summary>
    public static bool SequenceEqual<T>(IEnumerable<T> left,
                                        IEnumerable<T> right,
                                        bool orderMatters,
                                        bool countMatters,
                                        IEqualityComparer<T> comparer) {
      if (ReferenceEquals(left, right))
        return true;
      else if (left is null || right is null)
        return false;

      // Ordered and Counted
      if (orderMatters && countMatters) {
        if (comparer is null)
          return Enumerable.SequenceEqual(left, right);
        else
          return Enumerable.SequenceEqual(left, right, comparer);
      }

      if (comparer is null)
        comparer = EqualityComparer<T>.Default;

      if (comparer is null)
        throw new ArgumentNullException(nameof(comparer), $"No default equality comparer for {typeof(T).Name}");

      // Ordered, not counted
      if (orderMatters) {
        HashSet<T> leftHs = new HashSet<T>();
        HashSet<T> rightHs = new HashSet<T>();

        using var enLeft = left.GetEnumerator();
        using var enRight = right.GetEnumerator();

        while (true) {
          if (!enLeft.MoveNext())
            break;
          else if (!leftHs.Add(enLeft.Current))
            continue;

          bool found = false;

          while (enRight.MoveNext())
            if (rightHs.Add(enRight.Current)) {
              found = true;

              break;
            }

          if (!found || comparer.Equals(enLeft.Current, enRight.Current))
            return false;
        }

        while (enRight.MoveNext())
          if (rightHs.Add(enRight.Current))
            return false;
      }

      // Not ordered, not counted
      if (!orderMatters && !countMatters) {
        HashSet<T> leftHs = new HashSet<T>(left, comparer);
        HashSet<T> rightHs = new HashSet<T>(right, comparer);

        if (leftHs.Count != rightHs.Count)
          return false;

        foreach (var item in leftHs)
          if (!rightHs.Contains(item))
            return false;

        return true;
      }

      // Not ordered, counted
      int leftCount = FastCount(left);

      if (leftCount >= 0) {
        int rightCount = FastCount(right);

        if (rightCount >= 0 && rightCount != leftCount)
          return false;
      }

      Dictionary<T, int> leftCounts = new Dictionary<T, int>(comparer);

      foreach (var item in left)
        if (leftCounts.TryGetValue(item, out int count))
          leftCounts[item] = count + 1;
        else
          leftCounts.Add(item, 1);

      foreach (var item in right) {
        if (leftCounts.TryGetValue(item, out int count))
          if (count == 1)
            leftCounts.Remove(item);
          else
            leftCounts[item] = count - 1;
        else
          return false;
      }

      return leftCounts.Count == 0;
    }

    #endregion Public
  }
}
