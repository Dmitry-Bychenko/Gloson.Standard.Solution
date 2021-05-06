using System;
using System.Collections.Generic;

namespace Gloson.Collections {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// String Trie 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class StringTrie {
    #region Inner Classes

    /// <summary>
    /// Trie Node
    /// </summary>
    public sealed class Node {
      #region Private Data

      private readonly Dictionary<char, Node> m_Items;

      #endregion Private Data

      #region Algorithm

      internal void Remove() {
        Occurrences = 0;

        if (m_Items.Count > 0) {
          List<Node> children = new(m_Items.Values);

          foreach (Node node in children)
            node.Remove();
        }

        if (IsRoot)
          return;
        else if (Parent is null)
          return;

        Parent.m_Items.Remove(Value);
        Parent = null;
        Trie = null;
      }

      #endregion Algorithm

      #region Create

      // Standard Constructor
      internal Node(StringTrie trie, Node parent, char value) {
        Trie = trie;
        Parent = parent;
        Value = value;

        m_Items = new Dictionary<char, Node>(Trie.Comparer);

        if (Parent is not null)
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
      public StringTrie Trie { get; private set; }

      /// <summary>
      /// Parent
      /// </summary>
      public Node Parent { get; private set; }

      /// <summary>
      /// Value
      /// </summary>
      public char Value { get; }

      /// <summary>
      /// Sequence
      /// </summary>
      public string Sequence {
        get {
          if (Parent is null)
            return "";

          List<char> letters = new();

          for (Node current = this; !current.IsRoot; current = current.Parent)
            letters.Add(current.Value);

          letters.Reverse();

          return new string(letters.ToArray());
        }
      }

      /// <summary>
      /// Items
      /// </summary>
      public IReadOnlyDictionary<char, Node> Items => m_Items;

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

          for (Node current = this; current.Parent is not null; current = current.Parent)
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
    public StringTrie(IEqualityComparer<char> comparer) {
      if (comparer is null)
        comparer = EqualityComparer<char>.Default;

      Comparer = comparer;
      Root = new Node(this, null, '\0');
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public StringTrie() : this(null) { }

    /// <summary>
    /// Create suffix trie
    /// </summary>
    public static StringTrie Create(string value, IEqualityComparer<char> comparer) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      StringTrie result = new(comparer);

      for (int start = 0; start < value.Length; ++start) {
        Node current = result.Root;

        current.Occurrences += 1;

        for (int i = start; i < value.Length; ++i) {
          char c = value[i];

          if (!current.Items.TryGetValue(c, out Node next))
            next = new Node(result, current, c);

          next.Occurrences += 1;
          current = next;
        }
      }

      // Empty Suffix
      result.Root.Occurrences += 1;

      return result;
    }

    /// <summary>
    /// Create suffix trie
    /// </summary>
    public static StringTrie Create(string value) => Create(value, null);

    #endregion Create

    #region Public

    /// <summary>
    /// Comparer
    /// </summary>
    public IEqualityComparer<char> Comparer { get; }

    /// <summary>
    /// Root
    /// </summary>
    public Node Root { get; }

    /// <summary>
    /// Nodes (BFS)
    /// </summary>
    public IEnumerable<Node> Nodes {
      get {
        List<Node> agenda = new() { Root };

        while (agenda.Count > 0) {
          List<Node> next = new();

          foreach (Node node in agenda) {
            yield return node;

            next.AddRange(node.Items.Values);
          }

          agenda = next;
        }
      }
    }

    /// <summary>
    /// Strings that are in trie
    /// </summary>
    public IEnumerable<string> Strings {
      get {
        foreach (Node node in Nodes) {
          int count = node.Terminations;

          if (count <= 0)
            continue;

          string result = node.Sequence;

          for (int i = 0; i < count; ++i)
            yield return result;
        }
      }
    }

    /// <summary>
    /// Add
    /// </summary>
    public void Add(string sequence) {
      if (sequence is null)
        throw new ArgumentNullException(nameof(sequence));

      Node current = Root;

      current.Occurrences += 1;

      foreach (char value in sequence) {
        if (!current.Items.TryGetValue(value, out Node next))
          next = new Node(this, current, value);

        next.Occurrences += 1;
        current = next;
      }
    }

    /// <summary>
    /// Add Range
    /// </summary>
    public void AddRange(IEnumerable<string> sequences) {
      if (sequences is null)
        throw new ArgumentNullException(nameof(sequences));

      foreach (string sequence in sequences)
        Add(sequence);
    }

    /// <summary>
    /// Remove
    /// </summary>
    public bool Remove(string sequence) {
      if (sequence is null)
        throw new ArgumentNullException(nameof(sequence));

      Node current = Root;

      List<Node> nodes = new() { current };

      foreach (char value in sequence) {
        if (!current.Items.TryGetValue(value, out var next))
          return false;

        current = next;

        nodes.Add(next);
      }

      Node leaf = nodes[^1];

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
    public int Occurred(string sequence) {
      if (sequence is null)
        throw new ArgumentNullException(nameof(sequence));

      Node current = Root;

      foreach (char value in sequence) {
        if (!current.Items.TryGetValue(value, out var next))
          return 0;

        current = next;
      }

      return current.Occurrences;
    }

    /// <summary>
    /// Starts from
    /// </summary>
    public bool StartsFrom(string sequence) {
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
