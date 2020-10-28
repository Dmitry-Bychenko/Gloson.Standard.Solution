using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Aho-Corasick state machine for multiple patterns search
  /// </summary>
  /// <see cref="https://en.wikipedia.org/wiki/Aho%E2%80%93Corasick_algorithm"/>
  /// <example>
  /// <code>
  /// string source = "abcabcaba_abbabcc";
  /// 
  /// string[] patterns = new string[] {
  ///   "abc",
  ///   "a",
  ///   "bc",
  ///   "ca",
  ///   "bca",
  /// };
  /// 
  /// AhoStateMachine<char> machine = new AhoStateMachine<char>(patterns);
  /// 
  /// var result = machine
  ///   .Matches(source)
  ///   .Select(match => $"{string.Concat(match.pattern),3} at {match.position}");
  ///
  /// Console.Write(string.Join(Environment.NewLine, result));
  /// </code>
  /// </example>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class AhoStateMachine<T> {
    #region Internal Class

    private class Node {
      #region Private Data

      private readonly Dictionary<T, Node> m_Edges;

      #endregion Private Data

      #region Algorithm

      internal Node Add(T item) {
        if (m_Edges.TryGetValue(item, out var result))
          return result;

        result = new Node(m_Edges.Comparer);

        m_Edges.Add(item, result);

        return result;
      }

      #endregion Algorithm

      #region Create

      public Node(IEqualityComparer<T> comparer) {
        m_Edges = new Dictionary<T, Node>(comparer);
      }

      #endregion Create

      #region Public

      /// <summary>
      /// Is Final
      /// </summary>
      public bool IsFinal => Pattern != null;

      /// <summary>
      /// Pattern
      /// </summary>
      public IReadOnlyList<T> Pattern { get; internal set; }

      /// <summary>
      /// Final Edge
      /// </summary>
      public Node FinalEdge { get; internal set; }

      /// <summary>
      /// Suffix Edge
      /// </summary>
      public Node SuffixEdge { get; internal set; }

      /// <summary>
      /// Edge
      /// </summary>
      public IReadOnlyDictionary<T, Node> Edges => m_Edges;

      /// <summary>
      /// All Patterns
      /// </summary>
      public IEnumerable<IReadOnlyList<T>> AllPatterns() {
        if (null != Pattern)
          yield return Pattern;

        for (Node node = FinalEdge; node != null; node = node.FinalEdge)
          yield return node.Pattern;
      }

      /// <summary>
      /// Move Next 
      /// </summary>
      public (Node node, bool hasPattern) MoveNext(T value) {
        if (m_Edges.TryGetValue(value, out Node result))
          return (result, true);

        Node node = SuffixEdge;

        for (; node != null; node = node.SuffixEdge) {
          if (node.m_Edges.TryGetValue(value, out result))
            return (result, true);
        }

        return (node, false);
      }

      #endregion Public
    }

    #endregion Internal Class

    #region Private Data

    private readonly Node m_Root;

    #endregion Private Data

    #region Algorithm

    private (Dictionary<List<T>, Node> direct, Dictionary<Node, List<T>> reverse) CoreBuildDictionaries() {
      Dictionary<List<T>, Node> direct = new Dictionary<List<T>, Node>(new SequenceEqualityComparer<T>(Comparer));
      Dictionary<Node, List<T>> reverse = new Dictionary<Node, List<T>>();

      Queue<(Node node, List<T> pattern)> agenda = new Queue<(Node node, List<T> pattern)>();

      agenda.Enqueue((m_Root, new List<T>()));

      while (agenda.Count > 0) {
        var (node, pattern) = agenda.Dequeue();

        direct.Add(pattern, node);
        reverse.Add(node, pattern);

        foreach (var pair in node.Edges) {
          List<T> list = new List<T>(pattern) { pair.Key };

          agenda.Enqueue((pair.Value, list));
        }
      }

      return (direct, reverse);
    }

    private HashSet<Node> CoreBuildNodes() {
      HashSet<Node> result = new HashSet<Node>() { m_Root };

      foreach (var pattern in Patterns) {
        Node node = m_Root;

        foreach (T item in pattern) {
          node = node.Add(item);

          result.Add(node);
        }

        node.Pattern = pattern;
      }

      return result;
    }

    private void CoreBuildSuffixes(HashSet<Node> nodes) {
      Dictionary<List<T>, Node> direct;
      Dictionary<Node, List<T>> reverse;

      (direct, reverse) = CoreBuildDictionaries();

      foreach (Node node in nodes) {
        LinkedList<T> sequence = new LinkedList<T>(reverse[node]);

        while (sequence.Count > 0) {
          sequence.RemoveFirst();

          if (direct.TryGetValue(sequence.ToList(), out Node parent)) {
            node.SuffixEdge = parent;

            break;
          }
        }
      }

      m_Root.SuffixEdge = null;
    }

    private void CoreBuildFinals(HashSet<Node> nodes) {
      foreach (Node parent in nodes) {
        for (Node node = parent.SuffixEdge; node != null; node = node.SuffixEdge)
          if (node.IsFinal) {
            parent.FinalEdge = node;

            break;
          }
      }
    }

    private void CoreBuild() {
      var nodes = CoreBuildNodes();

      CoreBuildSuffixes(nodes);
      CoreBuildFinals(nodes);
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="patterns">Patterns to find</param>
    /// <param name="comparer">Comparer to use</param>
    public AhoStateMachine(IEnumerable<IEnumerable<T>> patterns, IEqualityComparer<T> comparer) {
      if (null == patterns)
        throw new ArgumentNullException(nameof(patterns));

      Comparer = comparer ?? EqualityComparer<T>.Default;

      Patterns = patterns
        .Where(pattern => pattern != null)
        .Select(pattern => pattern.ToList())
        .Where(pattern => pattern.Count > 0)
        .ToList();

      m_Root = new Node(Comparer);

      CoreBuild();
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="patterns">Patterns to find</param>
    public AhoStateMachine(IEnumerable<IEnumerable<T>> patterns)
      : this(patterns, null) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Comparer
    /// </summary>
    public IEqualityComparer<T> Comparer { get; }

    /// <summary>
    /// Patterns
    /// </summary>
    public IReadOnlyList<IReadOnlyList<T>> Patterns { get; }

    /// <summary>
    /// Matches
    /// </summary>
    public IEnumerable<(IReadOnlyList<T> pattern, int position)> Matches(IEnumerable<T> source) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));

      int position = 0;
      Node current = m_Root;

      foreach (T item in source) {
        position += 1;

        var (node, hasPattern) = current.MoveNext(item);

        current = node ?? m_Root;

        if (hasPattern)
          foreach (var pattern in current.AllPatterns())
            yield return (pattern, position - pattern.Count);
      }
    }

    #endregion Public
  }
}
