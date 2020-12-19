using Gloson.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Algorithms.Graphs {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Minimum Spanning Tree
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class SpanningTree {
    #region Algorithm

    /// <summary>
    /// Minimum Spanning Tree (edges)
    /// </summary>
    public static IEnumerable<(T record, N fromNode, N toNode, E edge)> MininumSpanningTree<T, N, E>(
      this IEnumerable<T> source,
           Func<T, (N from, N to, E length)> graph,
           IEqualityComparer<N> nodeComparer,
           IComparer<E> edgeComparer) {

      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == graph)
        throw new ArgumentNullException(nameof(graph));

      if (nodeComparer == null)
        nodeComparer = EqualityComparer<N>.Default;

      if (null == edgeComparer) {
        edgeComparer = Comparer<E>.Default;

        if (null == edgeComparer)
          throw new ArgumentNullException(nameof(edgeComparer),
                                        $"Type {typeof(E).Name} doesn't provide default IComparer<{typeof(E).Name}>");
      }

      var edges = source
        .Select(item => (record: item, other: graph(item)))
        .Select(edge => (edge.record, edge.other.from, edge.other.to, edge.other.length))
        .Where(edge => !nodeComparer.Equals(edge.from, edge.to))
        .OrderBy(edge => edge.length, edgeComparer);

      DisjointSets<N> vertice = new DisjointSets<N>(nodeComparer);

      foreach (var edge in edges)
        if (vertice.TryAddPair(edge.from, edge.to, out int _index))
          yield return edge;
    }

    /// <summary>
    /// Minimum Spanning Tree (edges)
    /// </summary>
    public static IEnumerable<(T record, N fromNode, N toNode, E edge)> MininumSpanningTree<T, N, E>(
      this IEnumerable<T> source,
           Func<T, (N from, N to, E length)> graph,
           IEqualityComparer<N> nodeComparer) => MininumSpanningTree<T, N, E>(source, graph, nodeComparer, null);

    /// <summary>
    /// Minimum Spanning Tree (edges)
    /// </summary>
    public static IEnumerable<(T record, N fromNode, N toNode, E edge)> MininumSpanningTree<T, N, E>(
      this IEnumerable<T> source,
           Func<T, (N from, N to, E length)> graph) => MininumSpanningTree<T, N, E>(source, graph, null, null);

    #endregion Algorithm
  }

}
