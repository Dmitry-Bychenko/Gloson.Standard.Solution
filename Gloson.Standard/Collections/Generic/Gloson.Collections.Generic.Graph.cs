using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Graph Options
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  [Flags]
  public enum GraphOptions {
    /// <summary>
    /// 
    /// </summary>
    None = 0,
    /// <summary>
    /// 
    /// </summary>
    Undirected = 1,
    /// <summary>
    /// 
    /// </summary>
    Loops = 2,
    /// <summary>
    /// 
    /// </summary>
    MultiEdges = 4,
    /// <summary>
    /// 
    /// </summary>
    Acyclic = 8,
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Graph
  /// </summary>
  /// <typeparam name="V">Value Type associated with Vertex</typeparam>
  /// <typeparam name="E">Value Type associated with Edge</typeparam>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed partial class Graph<V, E> {
    #region Inner classes

    /// <summary>
    /// Vertex
    /// </summary>
    public sealed class Vertex {
      #region Private Data

      private readonly HashSet<Edge> m_InEdges = new HashSet<Edge>();

      private readonly HashSet<Edge> m_OutEdges = new HashSet<Edge>();

      #endregion Private Data

      #region Create

      /// <summary>
      /// Standard constructor
      /// </summary>
      /// <param name="graph">Parent Graph</param>
      /// <param name="value">Value associated with the Vertex</param>
      public Vertex(Graph<V, E> graph, V value) {
        Graph = graph ?? throw new ArgumentNullException(nameof(graph));
        Value = value;

        if (!Graph.CoreAddVertex(this)) {
          Graph = null;

          throw new ArgumentException("The vertex can't be added to the graph", nameof(graph));
        }
      }

      /// <summary>
      /// Standard constructor
      /// </summary>
      /// <param name="graph">Parent Graph</param>
      public Vertex(Graph<V, E> graph) : this(graph, default) { }

      #endregion Create

      #region Public

      /// <summary>
      /// Connect To
      /// </summary>
      public Edge ConnectTo(Vertex to, E value) => new Edge(this, to, value);

      /// <summary>
      /// Connect To
      /// </summary>
      public Edge ConnectTo(Vertex to) => new Edge(this, to);

      /// <summary>
      /// Connect From
      /// </summary>
      public Edge ConnectFrom(Vertex from, E value) => new Edge(from, this, value);

      /// <summary>
      /// Connect From
      /// </summary>
      public Edge ConnectFrom(Vertex from) => new Edge(from, this);

      /// <summary>
      /// Value
      /// </summary>
      public V Value { get; set; }

      /// <summary>
      /// Graph
      /// </summary>
      public Graph<V, E> Graph { get; internal set; }

      /// <summary>
      /// In Edges
      /// </summary>
      public IEnumerable<Edge> InEdges => m_InEdges;

      /// <summary>
      /// In Count
      /// </summary>
      public int InCount => m_InEdges.Count;

      /// <summary>
      /// Out Edges
      /// </summary>
      public IEnumerable<Edge> OutEdges => m_OutEdges;

      /// <summary>
      /// Out Count
      /// </summary>
      public int OutCount => m_OutEdges.Count;

      #endregion Public
    }

    /// <summary>
    /// Edge
    /// </summary>
    public sealed class Edge {
      #region Private Data

      private E m_Value;

      private Edge m_Twin;

      #endregion Private Data

      #region Algorithm
      #endregion Algorithm

      #region Create

      /*
      private Edge(Vertex from, Vertex to, E value, bool check) { //, bool check
        To = to;
        From = from;
        Value = value;
      }
      */

      /// <summary>
      /// Standard Constructor
      /// </summary>
      /// <param name="from">From</param>
      /// <param name="to">To</param>
      /// <param name="value">Associated value</param>
      public Edge(Vertex from, Vertex to, E value) {
        if (null == from)
          throw new ArgumentNullException(nameof(from));
        else if (null == to)
          throw new ArgumentNullException(nameof(to));
        else if (from.Graph != to.Graph)
          throw new ArgumentException("From and To vertexes belong to different graphs.", nameof(to));

        if (!from.Graph.IsEdgeAllowed(from, to))
          throw new ArgumentException("Edge is not allowed", nameof(to));

        To = to;
        From = from;
        Value = value;

        // Correct; create create twin
        if (Graph.Options.HasFlag(GraphOptions.Undirected)) {
          m_Twin = new Edge(to, from, value) {
            m_Twin = this
          };
        }
      }

      /// <summary>
      /// Standard Constructor
      /// </summary>
      /// <param name="from">From</param>
      /// <param name="to">To</param>
      public Edge(Vertex from, Vertex to) : this(from, to, default) { }

      #endregion Create

      #region Public

      /// <summary>
      /// From Vertex
      /// </summary>
      public Vertex From { get; }

      /// <summary>
      /// To Vertex
      /// </summary>
      public Vertex To { get; }

      /// <summary>
      /// Graph
      /// </summary>
      public Graph<V, E> Graph => null != From
        ? From.Graph
        : To?.Graph;

      /// <summary>
      /// Value
      /// </summary>
      public E Value {
        get {
          return m_Value;
        }
        set {
          m_Value = value;

          if (null != m_Twin)
            m_Twin.m_Value = value;
        }
      }

      #endregion Public
    }

    #endregion Inner classes

    #region Private Data

    private readonly HashSet<Vertex> m_Vertexes = new HashSet<Vertex>();

    #endregion Private Data

    #region Algorithm

    // Add Vertex (always possible)
    private bool CoreAddVertex(Vertex value) {
      return null == value
        ? false
        : m_Vertexes.Add(value);
    }

    // If Edge allowed
    private bool IsEdgeAllowed(Vertex from, Vertex to) {
      if (null == from)
        return false;
      else if (null == to)
        return false;
      else if (from.Graph != to.Graph)
        return false;

      if (from == to && !Options.HasFlag(GraphOptions.Loops))
        return false;

      if (!Options.HasFlag(GraphOptions.MultiEdges))
        if (from.OutEdges.Any(edge => edge.To == to) ||
            to.InEdges.Any(edge => edge.From == from))
          return false;

      //TODO: Implement me! Acyclic test (for both directed and undirected cases)
      if (Options.HasFlag(GraphOptions.Acyclic)) {
        if (ReferenceEquals(to, from))
          return false;

        HashSet<Vertex> used = new HashSet<Vertex>();
        HashSet<Vertex> agenda = new HashSet<Vertex>() { from, to };

        while (agenda.Any()) {
          List<Vertex> nodes = agenda.ToList();
          agenda.Clear();

          foreach (var node in nodes) {
            if (used.Contains(node))
              continue;

            if (ReferenceEquals(from, node))
              return false;

            foreach (var edge in node.OutEdges)
              agenda.Add(edge.To);

            used.Add(node);
          }
        }
      }

      return true;
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public Graph(GraphOptions options)
      : base() {

      Options = options;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public Graph() : this(GraphOptions.None) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Options
    /// </summary>
    public GraphOptions Options { get; }

    /// <summary>
    /// Vertexes
    /// </summary>
    public IEnumerable<Vertex> Vertexes => m_Vertexes;

    /// <summary>
    /// Edges
    /// </summary>
    public IEnumerable<Edge> Edges => m_Vertexes
      .SelectMany(vertex => vertex.OutEdges);

    /// <summary>
    /// Add Vertex
    /// </summary>
    /// <param name="value">Associated value</param>
    /// <returns>Vertex</returns>
    public Vertex Add(V value) => new Vertex(this, value);

    /// <summary>
    /// Add Vertex
    /// </summary>
    /// <returns>Vertex with default associated value</returns>
    public Vertex Add() => new Vertex(this);

    #endregion Public
  }
}
