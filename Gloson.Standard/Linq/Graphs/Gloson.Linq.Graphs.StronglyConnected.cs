using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Linq.Graphs {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Enumerable Extensions (Strongly Connected Components)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Algorithm

    private static HashSet<T> WeakComponent<T>(
      T start,
      Func<T, HashSet<T>> next,
      HashSet<T> exclude) {

      HashSet<T> result = new() { };
      Queue<T> agenda = new();

      agenda.Enqueue(start);

      while (agenda.Count > 0) {
        T node = agenda.Dequeue();

        if (exclude.Contains(node))
          continue;

        if (!result.Add(node))
          continue;

        foreach (var child in next(node))
          agenda.Enqueue(child);
      }

      return result;
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Strongly Connected Components
    /// </summary>
    /// <param name="source">Nodes of the graph</param>
    /// <param name="children">Children for a given node</param>
    /// <returns>Strongly Connected Components</returns>
    public static IEnumerable<T[]> StronglyConnectedComponents<T>(
      this IEnumerable<T> source,
           Func<T, IEnumerable<T>> children) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      else if (children is null)
        throw new ArgumentNullException(nameof(children));

      Dictionary<T, (HashSet<T> to, HashSet<T> from)> graph = source
        .ToDictionary(item => item, item => (new HashSet<T>(), new HashSet<T>()));

      foreach (var pair in graph) {
        foreach (var edge in children(pair.Key)) {
          pair.Value.to.Add(edge);
          graph[edge].from.Add(pair.Key);
        }
      }

      HashSet<T> completed = new();

      foreach (T node in graph.Keys) {
        if (completed.Contains(node))
          continue;

        var direct = WeakComponent(node, n => graph[n].to, completed);
        var reverse = WeakComponent(node, n => graph[n].from, completed);

        direct.IntersectWith(reverse);

        T[] component = direct.ToArray();

        foreach (T cn in component)
          completed.Add(cn);

        yield return component;
      }
    }

    /// <summary>
    /// Strongly Connected Components
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="vertex">Vertex from source item</param>
    /// <param name="children">Children from source item</param>
    /// <returns>Strongly Connected Components</returns>
    public static IEnumerable<N[]> StronglyConnectedComponents<T, N>(
      this IEnumerable<T> source,
           Func<T, N> vertex,
           Func<T, IEnumerable<N>> children) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      else if (vertex is null)
        throw new ArgumentNullException(nameof(vertex));
      else if (children is null)
        throw new ArgumentNullException(nameof(children));

      Dictionary<N, (HashSet<N> to, HashSet<N> from)> graph = new();

      foreach (T record in source) {
        N node = vertex(record);
        IEnumerable<N> tos = children(record).ToList();

        if (!graph.ContainsKey(node))
          graph.Add(node, (new HashSet<N>(), new HashSet<N>()));

        foreach (var item in tos) {
          graph[node].to.Add(item);

          if (!graph.ContainsKey(item))
            graph.Add(item, (new HashSet<N>(), new HashSet<N>()));

          graph[item].from.Add(node);
        }
      }

      HashSet<N> completed = new();

      foreach (N node in graph.Keys) {
        if (completed.Contains(node))
          continue;

        var direct = WeakComponent(node, n => graph[n].to, completed);
        var reverse = WeakComponent(node, n => graph[n].from, completed);

        direct.IntersectWith(reverse);

        N[] component = direct.ToArray();

        foreach (N cn in component)
          completed.Add(cn);

        yield return component;
      }
    }

    #endregion Public
  }
}
