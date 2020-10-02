using System;
using System.Collections.Generic;

namespace Gloson.Algorithms.Graphs {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Graph Shortest Paths Algorithms
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class GraphShortestPath {
    #region Public

    /// <summary>
    /// Bellman-Ford shortest path
    /// </summary>
    /// <param name="start">vertex to start from</param>
    /// <param name="neighbors">neighbors func</param>
    /// <returns>Shortest paths for all reachable vertexes from start</returns>
    public static IDictionary<V, (double length, V prior)> BellmanFord<V>(
      V start,
      Func<V, IEnumerable<(V vertex, double length)>> neighbors) {

      if (null == neighbors)
        throw new ArgumentNullException(nameof(neighbors));

      Dictionary<V, (double length, V prior)> result = new Dictionary<V, (double length, V prior)>() {
        { start, (0, default)}
      };

      // All reachable nodes, BFS
      HashSet<V> agenda = new HashSet<V>() { start };

      while (agenda.Count > 0) {
        HashSet<V> next = new HashSet<V>();

        foreach (V vertex in agenda) {
          double priorLength = result[vertex].length;

          foreach (var edge in neighbors(vertex)) {
            if (result.ContainsKey(edge.vertex))
              continue;
            else if (double.IsNaN(edge.length) || double.IsPositiveInfinity(edge.length))
              continue;

            result.Add(edge.vertex, (edge.length + priorLength, vertex));

            next.Add(vertex);
          }
        }

        agenda = next;
      }

      // Relax
      for (int step = 0; ; ++step) {
        bool relaxed = true;

        // Is negative loop found? 
        if (step > result.Count) {
          foreach (var key in result.Keys) 
            result[key] = (double.NegativeInfinity, result[key].prior);

          return result;
        }

        foreach (V vertex in result.Keys) {
          if (Equals(vertex, start))
            continue;

          double baseLength = result[vertex].length;

          foreach (var edge in neighbors(vertex)) {
            if (result.ContainsKey(edge.vertex))
              continue;
            else if (double.IsNaN(edge.length) || double.IsPositiveInfinity(edge.length))
              continue;

            if (result.ContainsKey(edge.vertex))
              if (baseLength + edge.length < result[edge.vertex].length) {
                relaxed = false;
                result[edge.vertex] = (baseLength + edge.length, edge.vertex);
              }
          }
        }

        if (relaxed)
          break;
      }

      return result;
    }

    #endregion Public
  }
}
