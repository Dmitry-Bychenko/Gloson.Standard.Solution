using Gloson.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Topological Operations
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Internal classes

    private class TopologicallyOrdered<T> : IOrderedEnumerable<T>, IEnumerable<T> {
      readonly List<List<T>> m_List;
      internal TopologicallyOrdered(List<List<T>> list) {
        m_List = list;
      }

      public IOrderedEnumerable<T> CreateOrderedEnumerable<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer, bool descending) {
        return descending
          ? m_List
             .Select(list => list.OrderByDescending(item => item))
             .SelectMany(item => item)
             .OrderBy(item => 1)
          : m_List
             .Select(list => list.OrderBy(item => item))
             .SelectMany(item => item)
             .OrderBy(item => 1);
      }

      public IEnumerator<T> GetEnumerator() => m_List.SelectMany(item => item).GetEnumerator();

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    #endregion Internal classes

    #region Algorithm
    private static List<List<T>> TopologicalList<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> required) {
      List<List<T>> result = new List<List<T>>();

      HashSet<T> completed = new HashSet<T>();

      Dictionary<T, List<T>> cache = new Dictionary<T, List<T>>();

      Queue<T> agenda = new Queue<T>(source);

      for (int level = 0; agenda.Count > 0; ++level) {
        List<T> currentLevel = new List<T>();

        Queue<T> nextAgenda = new Queue<T>();

        for (int i = agenda.Count - 1; i >= 0; --i) {
          T item = agenda.Dequeue();

          if (!cache.TryGetValue(item, out List<T> req))
            req = required(item)?.ToList() ?? new List<T>();

          if (req.All(r => completed.Contains(r)))
            currentLevel.Add(item);
          else
            nextAgenda.Enqueue(item);
        }

        if (currentLevel.Count > 0)
          result.Add(currentLevel);
        else
          throw new ArgumentException("Source has loops, it can't be topologically sorted", nameof(source));

        agenda = nextAgenda;

        foreach (T item in currentLevel)
          completed.Add(item);
      }

      return result;
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Topological OrderBy
    /// </summary>
    /// <param name="source">Items to sort</param>
    /// <param name="required">Items required to be before the current item</param>
    public static IOrderedEnumerable<T> TopologicalOrderBy<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> required) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == required)
        throw new ArgumentNullException(nameof(required));

      List<List<T>> list = TopologicalList(source, required);

      return new TopologicallyOrdered<T>(list);
    }

    /// <summary>
    /// Topological OrderBy (descending)
    /// </summary>
    /// <param name="source">Items to sort</param>
    /// <param name="required">Items required to be before the current item</param>
    public static IOrderedEnumerable<T> TopologicalOrderByDescending<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> required) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == required)
        throw new ArgumentNullException(nameof(required));

      List<List<T>> list = TopologicalList(source, required);

      list.Reverse();

      return new TopologicallyOrdered<T>(list);
    }

    /// <summary>
    /// Topological GroupBy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="required"></param>
    /// <returns></returns>
    public static IEnumerable<IGrouping<int, T>> TopologicalGroupBy<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> required) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == required)
        throw new ArgumentNullException(nameof(required));

      var list = TopologicalList(source, required);

      for (int i = 0; i < list.Count; ++i)
        yield return new Grouping<int, T>(i, list[i]);
    }

    #endregion Public
  }
}
