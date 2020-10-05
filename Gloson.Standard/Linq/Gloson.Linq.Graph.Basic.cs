using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gloson.Linq {
  
  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Graph Builder
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class GraphBuilder {
    #region Public

    /// <summary>
    /// To Adjacent (list) representation
    /// </summary>
    public static Dictionary<V, Dictionary<V, E>> ToAdjacent<T, V, E>(
      this IEnumerable<T> source,
           Func<T, (V from, V to, E edge)> edgeMap,
           IEqualityComparer<V> vertexComparer = null,
           Func<(V from, V to, E oldEdge, E newEdge), E> collision = null,
           Func<(V from, V to, E edge), bool> edgeFilter = null) {

      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == edgeMap)
        throw new ArgumentNullException(nameof(edgeMap));

      vertexComparer ??= EqualityComparer<V>.Default;
      collision ??= (rec => rec.newEdge);
      edgeFilter ??= (rec => true);

      Dictionary<V, Dictionary<V, E>> result = new Dictionary<V, Dictionary<V, E>>(vertexComparer);

      foreach (T item in source) {
        var edge = edgeMap(item);

        if (!result.TryGetValue(edge.from, out var dict)) {
          dict = new Dictionary<V, E>(vertexComparer);

          result.Add(edge.from, dict);
        }

        if (edgeFilter(edge)) {
          if (dict.TryGetValue(edge.to, out E old))
            dict[edge.to] = collision((edge.from, edge.to, old, edge.edge));
          else
            dict.Add(edge.to, edge.edge);
        }

        if (!dict.ContainsKey(edge.to))
          result.Add(edge.to, new Dictionary<V, E>(vertexComparer));
      }

      return result;
    }

    #endregion Public
  }
}
