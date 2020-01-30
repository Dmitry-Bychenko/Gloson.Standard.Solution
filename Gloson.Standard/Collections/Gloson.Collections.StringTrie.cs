using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Collections {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Strimg Trie 
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

      private Dictionary<char, Node> m_Items;

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
      internal Node(StringTrie trie, Node parent, char value) {
        Trie = trie;
        Parent = parent;
        Value = value;

        m_Items = new Dictionary<char, Node>(Trie.Comparer);

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
      /// To String
      /// </summary>
      public override string ToString() => $"{Value} ({Occurrences} occurrences)";

      #endregion Public
    }

    #endregion Inner Classes

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public StringTrie(IEqualityComparer<char> comparer) {
      if (null == comparer)
        comparer = EqualityComparer<char>.Default;

      Comparer = comparer;
      Root = new Node(this, null, '\0');
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public StringTrie() : this(null) { }

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
    /// Add
    /// </summary>
    public void Add(string sequence) {
      if (null == sequence)
        throw new ArgumentNullException(nameof(sequence));

      Node current = Root;

      current.Occurrences += 1;

      foreach (char value in sequence) {
        Node next;

        if (!current.Items.TryGetValue(value, out next))
          next = new Node(this, current, value);

        next.Occurrences += 1;
        current = next;
      }
    }

    /// <summary>
    /// Add Range
    /// </summary>
    public void AddRange(IEnumerable<string> sequences) {
      if (null == sequences)
        throw new ArgumentNullException(nameof(sequences));

      foreach (string sequence in sequences)
        Add(sequence);
    }

    /// <summary>
    /// Remove
    /// </summary>
    public bool Remove(string sequence) {
      if (null == sequence)
        throw new ArgumentNullException(nameof(sequence));

      Node current = Root;

      List<Node> nodes = new List<Node>() { current };

      foreach (char value in sequence) {
        if (!current.Items.TryGetValue(value, out var next))
          return false;

        current = next;

        nodes.Add(next);
      }

      Node leaf = nodes[nodes.Count - 1];

      if (leaf.IsLeaf) {
        for (int i = nodes.Count - 1; i >= 0; --i) {
          nodes[i].Occurrences -= 1;

          if (nodes[i].Occurrences == 0)
            nodes[i].Remove();
        }

        return true;
      }

      int childrenCount = leaf.Items.Sum(item => item.Value.Occurrences);

      if (childrenCount >= leaf.Occurrences)
        return false;

      for (int i = nodes.Count - 1; i >= 0; --i)
        nodes[i].Occurrences -= 1;

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
      if (null == sequence)
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
