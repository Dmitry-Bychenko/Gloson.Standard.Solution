using Gloson.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Topological Operations
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Algorithm

    private static List<List<T>> TopologicalList<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> required) {
      List<List<T>> result = new List<List<T>>();

      HashSet<T> completed = new HashSet<T>();

      Dictionary<T, List<T>> cache = new Dictionary<T, List<T>>();

      Queue<T> agenda = new Queue<T>(source);

      for (int level = 0; agenda.Count > 0; ++level) {
        List<T> currentLevel = new List<T>();

        Queue<T> nextAgenda = new Queue<T>();

        for (int i = agenda.Count - 1; i >=0; --i) {
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
          throw new ArgumentException("Source has loops, it can't be topologically sorted", "source");

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
    public static IEnumerable<T> TopologicalSort<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> required) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == required)
        throw new ArgumentNullException(nameof(required));

      var list = TopologicalList(source, required);

      foreach (var line in list)
        foreach (var item in line)
          yield return item;
    }

    /// <summary>
    /// Topological OrderBy (descending)
    /// </summary>
    /// <param name="source">Items to sort</param>
    /// <param name="required">Items required to be before the current item</param>
    public static IEnumerable<T> TopologicalSortDescending<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> required) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == required)
        throw new ArgumentNullException(nameof(required));

      var list = TopologicalList(source, required);

      for (int i = list.Count - 1; i >= 0; --i) {
        var line = list[i];

        foreach (var item in line)
          yield return item;
      }
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
