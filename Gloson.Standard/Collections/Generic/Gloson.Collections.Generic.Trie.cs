using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Trie 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class Trie<T> {
    #region Inner Classes

    /// <summary>
    /// Trie Node
    /// </summary>
    public sealed class Node {
      #region Private Data

      private readonly Dictionary<T, Node> m_Items;

      #endregion Private Data

      #region Algorithm

      internal void Remove() {
        Occurrences = 0;

        if (m_Items.Count > 0) {
          List<Node> children = new List<Node>(m_Items.Values);

          foreach (Node node in children)
            node.Remove();
        }

        if (IsRoot)
          return;
        else if (null == Parent)
          return;

        Parent.m_Items.Remove(Value);
        Parent = null;
        Trie = null;
      }

      #endregion Algorithm

      #region Create

      // Standard Constructor
      internal Node(Trie<T> trie, Node parent, T value) {
        Trie = trie;
        Parent = parent;
        Value = value;

        m_Items = new Dictionary<T, Node>(Trie.Comparer);

        if (Parent != null)
          Parent.m_Items.Add(Value, this);
      }

      #endregion Create

      #region Public

      /// <summary>
      /// Ocurrencies
      /// </summary>
      public int Occurrences { get; internal set; }

      /// <summary>
      /// Terminations (how many sequencies ends on this node)
      /// </summary>
      public int Terminations {
        get {
          if (m_Items.Count <= 0)
            return Occurrences;

          int result = Occurrences;

          foreach (var item in m_Items.Values)
            result -= item.Occurrences;

          return result;
        }
      }

      /// <summary>
      /// Trie the Node belongs to
      /// </summary>
      public Trie<T> Trie { get; private set; }

      /// <summary>
      /// Parent
      /// </summary>
      public Node Parent { get; private set; }

      /// <summary>
      /// Sequence
      /// </summary>
      public IEnumerable<T> Sequence {
        get {
          if (null == Parent)
            yield break;

          Stack<T> result = new Stack<T>();

          for (Node current = this; !current.IsRoot; current = current.Parent)
            result.Push(current.Value);

          while (result.Count > 0)
            yield return result.Pop();
        }
      }

      /// <summary>
      /// Value
      /// </summary>
      public T Value { get; }

      /// <summary>
      /// Items
      /// </summary>
      public IReadOnlyDictionary<T, Node> Items => m_Items;

      /// <summary>
      /// Is Leaf
      /// </summary>
      public bool IsLeaf => m_Items.Count <= 0;

      /// <summary>
      /// Is Root
      /// </summary>
      public bool IsRoot => ReferenceEquals(this, Trie?.Root);

      /// <summary>
      /// Level (0 for Root)
      /// </summary>
      public int Level {
        get {
          int result = 0;

          for (Node current = this; current.Parent != null; current = current.Parent)
            result += 1;

          return result;
        }
      }

      /// <summary>
      /// To String
      /// </summary>
      public override string ToString() => IsRoot
        ? $"Root ({m_Items.Count} children; {Occurrences} occurrences; {Terminations} terminations)"
        : $"{Value} ({m_Items.Count} children; {Occurrences} occurrences; {Terminations} terminations)";

      #endregion Public
    }

    #endregion Inner Classes

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public Trie(IEqualityComparer<T> comparer) {
      if (null == comparer)
        comparer = EqualityComparer<T>.Default;

      Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer), $"No default equality comparer for {typeof(T).Name}");
      Root = new Node(this, null, default);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public Trie() : this(null) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Comparer
    /// </summary>
    public IEqualityComparer<T> Comparer { get; }

    /// <summary>
    /// Root
    /// </summary>
    public Node Root { get; }

    /// <summary>
    /// Nodes (BFS)
    /// </summary>
    public IEnumerable<Node> Nodes {
      get {
        List<Node> agenda = new List<Node>() { Root };

        while (agenda.Count > 0) {
          List<Node> next = new List<Node>();

          foreach (Node node in agenda) {
            yield return node;

            next.AddRange(node.Items.Values);
          }

          agenda = next;
        }
      }
    }

    /// <summary>
    /// Sequencies that are in trie
    /// </summary>
    public IEnumerable<T[]> Sequencies {
      get {
        foreach (Node node in Nodes) {
          int count = node.Terminations;

          if (count <= 0)
            continue;

          T[] result = node.Sequence.ToArray();

          for (int i = 0; i < count; ++i)
            yield return result;
        }
      }
    }

    /// <summary>
    /// Add
    /// </summary>
    public void Add(IEnumerable<T> sequence) {
      if (null == sequence)
        throw new ArgumentNullException(nameof(sequence));

      Node current = Root;

      current.Occurrences += 1;

      foreach (T value in sequence) {
        if (!current.Items.TryGetValue(value, out Node next))
          next = new Node(this, current, value);

        next.Occurrences += 1;
        current = next;
      }
    }

    /// <summary>
    /// Add Range
    /// </summary>
    public void AddRange(IEnumerable<IEnumerable<T>> sequences) {
      if (null == sequences)
        throw new ArgumentNullException(nameof(sequences));

      foreach (IEnumerable<T> sequence in sequences)
        Add(sequence);
    }

    /// <summary>
    /// Remove
    /// </summary>
    public bool Remove(IEnumerable<T> sequence) {
      if (null == sequence)
        throw new ArgumentNullException(nameof(sequence));

      Node current = Root;

      List<Node> nodes = new List<Node>() { current };

      foreach (T value in sequence) {
        if (!current.Items.TryGetValue(value, out var next))
          return false;

        current = next;

        nodes.Add(next);
      }

      Node leaf = nodes[nodes.Count - 1];

      if (leaf.Terminations <= 0)
        return false;

      for (int i = nodes.Count - 1; i >= 0; --i) {
        nodes[i].Occurrences -= 1;

        if (nodes[i].Occurrences == 0)
          nodes[i].Remove();
      }

      return true;
    }

    /// <summary>
    /// Clear
    /// </summary>
    public void Clear() {
      Root.Remove();
    }

    /// <summary>
    /// How many times sequence starts the trie
    /// </summary>
    public int Occurred(IEnumerable<T> sequence) {
      if (null == sequence)
        throw new ArgumentNullException(nameof(sequence));

      Node current = Root;

      foreach (T value in sequence) {
        if (!current.Items.TryGetValue(value, out var next))
          return 0;

        current = next;
      }

      return current.Occurrences;
    }

    /// <summary>
    /// Starts from
    /// </summary>
    public bool StartsFrom(IEnumerable<T> sequence) {
      return Occurred(sequence) > 0;
    }

    /// <summary>
    /// Number of sequences in the trie
    /// </summary>
    public int Count {
      get {
        return Root.Occurrences;
      }
    }

    #endregion Public
  }
}
