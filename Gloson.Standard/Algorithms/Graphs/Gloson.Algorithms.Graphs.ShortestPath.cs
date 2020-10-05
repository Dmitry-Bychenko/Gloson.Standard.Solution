using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Algorithms.Graphs {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Graph Shortest Paths Algorithms
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class GraphShortestPath {
    #region Algorithm

    private static Dictionary<V, Dictionary<V, double>> BuildGraph<T, V>(
      IEnumerable<T> source,
      Func<T, (V from, V to, double length)> edgeMap,
      bool biDirect,
      IEqualityComparer<V> comparer = null,
      Func<(V from, V to, double oldLength, double length), double> collision = null) {

      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == edgeMap)
        throw new ArgumentNullException(nameof(edgeMap));

      if (null == comparer)
        comparer = EqualityComparer<V>.Default;

      if (collision == null)
        collision = r => Math.Min(r.oldLength, r.length);

      Dictionary<V, Dictionary<V, double>> result =
        new Dictionary<V, Dictionary<V, double>>(comparer);

      void AddToGraph((V from, V to, double length) edge) {
        if (double.IsNaN(edge.length) || double.IsPositiveInfinity(edge.length))
          return;

        if (comparer.Equals(edge.from, edge.to))
          return;

        if (!result.TryGetValue(edge.from, out var inner)) {
          inner = new Dictionary<V, double>(comparer);

          result.Add(edge.from, inner);
        }

        if (inner.TryGetValue(edge.to, out var old))
          inner[edge.to] = collision((edge.from, edge.to, old, edge.length));
        else
          inner.Add(edge.to, edge.length);
      }

      foreach (T record in source) {
        var edge = edgeMap(record);

        AddToGraph(edge);

        if (biDirect)
          AddToGraph((edge.to, edge.from, edge.length));
      }

      return result;
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Bellman-Ford shortest paths
    /// </summary>
    /// <param name="source">Edges</param>
    /// <param name="startVertex">Starting vertex</param>
    /// <param name="edgeMap">Edges to (vertex, vertex, length) mapping</param>
    /// <param name="biDirect">is graph bi direct one</param>
    /// <param name="comparer">vertex comparer</param>
    /// <param name="collision">collision function (for parallel edges)</param>
    /// <returns></returns>
    public static IDictionary<V, (double length, V prior, bool hasPrior)> BellmanFord<T, V>(
      this IEnumerable<T> source,
      V startVertex,
      Func<T, (V from, V to, double length)> edgeMap,
      bool biDirect = false,
      IEqualityComparer<V> comparer = null,
      Func<(V from, V to, double oldLength, double length), double> collision = null) {

      if (null == comparer)
        comparer = EqualityComparer<V>.Default;

      Dictionary<V, Dictionary<V, double>> graph = BuildGraph(source, edgeMap, biDirect, comparer, collision);

      Dictionary<V, (double length, V prior, bool hasPrior)> result =
        new Dictionary<V, (double length, V prior, bool hasPrior)>(comparer);

      Queue<V> agenda = new Queue<V>();
      agenda.Enqueue(startVertex);

      while (agenda.Count > 0) {
        V vertex = agenda.Dequeue();

        if (result.ContainsKey(vertex))
          continue;

        result.Add(vertex, (double.PositiveInfinity, default(V), false));

        if (graph.TryGetValue(vertex, out var edges)) {
          foreach (var edge in edges) {
            if (result.ContainsKey(edge.Key))
              continue;

            agenda.Enqueue(edge.Key);
          }
        }
      }

      result[startVertex] = (0, default(V), false);

      // Relax
      bool relaxed = true;

      var keys = result.Keys.ToList();

      for (int iteration = 0; relaxed; ++iteration) {
        relaxed = false;

        // Negative loop
        if (iteration > result.Count) {
          foreach (var key in result.Keys)
            result[key] = (double.NegativeInfinity, result[key].prior, result[key].hasPrior);

          return result;
        }

        foreach (V v in keys) {
          foreach (var e in graph[v]) {
            V u = e.Key;
            double d = e.Value;

            if (result[u].length > result[v].length + d) {
              result[u] = (result[v].length + d, u, true);

              relaxed = true;
            }
          }
        }
      }

      return result;
    }

    #endregion Public
  }
}
