using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Equality Comparer 
  /// </summary>
  // 
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class EqualityComparerSequence<T>
    : IEqualityComparer<IEnumerable<T>> {

    #region Algorithm

    private static int FastCount(IEnumerable<T> sequence) {
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

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="orderMatters">Order Matters</param>
    /// <param name="countMatters">Count Matters</param>
    /// <param name="comparer">Comparer</param>
    public EqualityComparerSequence(bool orderMatters,
                                    bool countMatters,
                                    IEqualityComparer<T> comparer) {
      OrderMatters = orderMatters;
      CountMatters = countMatters;

      if (comparer is null)
        comparer = EqualityComparer<T>.Default;

      ItemComparer = comparer ?? throw new ArgumentNullException(nameof(comparer), $"Type {typeof(T).Name} doesn't have default Equality Comparer");
    }

    #endregion Create 

    #region Public

    /// <summary>
    /// Order Matters
    /// </summary>
    public bool OrderMatters { get; }

    /// <summary>
    /// Count Matters
    /// </summary>
    public bool CountMatters { get; }

    /// <summary>
    /// Item Comparer 
    /// </summary>
    public IEqualityComparer<T> ItemComparer { get; }

    #endregion Public

    #region IEqualityComparer<T>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(IEnumerable<T> left, IEnumerable<T> right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (left is null || right is null)
        return false;

      if (OrderMatters) {
        if (CountMatters)
          return Enumerable.SequenceEqual(left, right, ItemComparer);

        // Ordered, not counted
        HashSet<T> leftHs = new();
        HashSet<T> rightHs = new();

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

          if (!found || ItemComparer.Equals(enLeft.Current, enRight.Current))
            return false;
        }

        while (enRight.MoveNext())
          if (rightHs.Add(enRight.Current))
            return false;

        return true;
      }
      else {
        if (CountMatters) {
          int leftCount = FastCount(left);

          if (leftCount >= 0) {
            int rightCount = FastCount(right);

            if (rightCount >= 0 && rightCount != leftCount)
              return false;
          }

          Dictionary<T, int> leftCounts = new(ItemComparer);

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
        else {
          // Not ordered, not counted
          HashSet<T> leftHs = new(left, ItemComparer);
          HashSet<T> rightHs = new(right, ItemComparer);

          if (leftHs.Count != rightHs.Count)
            return false;

          foreach (var item in leftHs)
            if (!rightHs.Contains(item))
              return false;

          return true;
        }
      }
    }

    /// <summary>
    /// Hash Code
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int GetHashCode(IEnumerable<T> obj) {
      if (obj is null)
        return 0;

      return obj.Count();
    }

    #endregion IEqualityComparer<T>
  }

}
