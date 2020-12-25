using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Linq.Graphs {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Enumerable Extensions: Graph functions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Inner classes

    /// <summary>
    /// Graph Vertex
    /// </summary>
    public sealed class Vertex<T>
      : IEquatable<Vertex<T>> {

      #region Create

      internal Vertex(T value, Vertex<T> parent, int level) {
        Value = value;
        Parent = parent;
        Level = level;
      }

      internal Vertex(T value)
        : this(value, null, 0) { }

      #endregion Create

      #region Public

      /// <summary>
      /// Value
      /// </summary>
      public T Value { get; }

      /// <summary>
      /// Parent
      /// </summary>
      public Vertex<T> Parent { get; }

      /// <summary>
      /// Level
      /// </summary>
      public int Level { get; }

      /// <summary>
      /// Is Root
      /// </summary>
      public bool IsRoot => Level <= 0;

      /// <summary>
      /// Has Parent (not root)
      /// </summary>
      public bool HasParent => Level > 0;

      /// <summary>
      /// To String
      /// </summary>
      public override string ToString() {
        return Value?.ToString();
      }

      #endregion Public

      #region Operators

      /// <summary>
      /// Value from Node
      /// </summary>
      public static implicit operator T(Vertex<T> value) {
        if (value is null)
          throw new ArgumentNullException(nameof(value));

        return value.Value;
      }

      /// <summary>
      /// Equal
      /// </summary>
      public static bool operator ==(Vertex<T> left, Vertex<T> right) {
        if (ReferenceEquals(left, right))
          return true;
        else if (left is null || right is null)
          return false;

        return left.Equals(right);
      }

      /// <summary>
      /// Not Equal
      /// </summary>
      public static bool operator !=(Vertex<T> left, Vertex<T> right) {
        if (ReferenceEquals(left, right))
          return false;
        else if (left is null || right is null)
          return true;

        return !left.Equals(right);
      }

      #endregion Operators

      #region IEquatable<Node<T>>

      /// <summary>
      /// Hash Code
      /// </summary>
      public override int GetHashCode() {
        return Value is null ? 0 : Value.GetHashCode();
      }

      /// <summary>
      /// Equals
      /// </summary>
      public override bool Equals(object obj) {
        return base.Equals(obj as Vertex<T>);
      }

      /// <summary>
      /// Equals
      /// </summary>
      public bool Equals(Vertex<T> other) {
        if (ReferenceEquals(this, other))
          return true;
        else if (other is null)
          return false;

        return object.Equals(Value, other.Value);
      }

      #endregion IEquatable<Node<T>>
    }

    #endregion Inner classes

    #region Public

    /// <summary>
    /// Breadth First Search : Flatten Graph structure arbitrary deep
    /// </summary>
    /// <param name="source">Top items (connected components representatives)</param>
    /// <param name="children">return children on given item</param>
    public static IEnumerable<Vertex<T>> BreadthFirstSearch<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> children) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      else if (children is null)
        throw new ArgumentNullException(nameof(children));

      HashSet<T> proceeded = new HashSet<T>();

      Queue<IEnumerable<Vertex<T>>> queue = new Queue<IEnumerable<Vertex<T>>>();

      queue.Enqueue(source.Select(item => new Vertex<T>(item)));

      while (queue.Count > 0) {
        IEnumerable<Vertex<T>> src = queue.Dequeue();

        if (src is null)
          continue;

        foreach (var item in src)
          if (proceeded.Add(item.Value)) {
            yield return item;

            queue.Enqueue(children(item.Value).Select(v => new Vertex<T>(v, item, item.Level + 1)));
          }
      }
    }

    /// <summary>
    /// Depth First Search : Flatten Graph structure arbitrary deep
    /// </summary>
    /// <param name="source">Top items (connected components representatives)</param>
    /// <param name="children">return children on given item</param>
    public static IEnumerable<Vertex<T>> DepthFirstSearch<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> children) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      else if (children is null)
        throw new ArgumentNullException(nameof(children));

      HashSet<T> proceeded = new HashSet<T>();

      Stack<IEnumerable<Vertex<T>>> stack = new Stack<IEnumerable<Vertex<T>>>();

      stack.Push(source.Select(item => new Vertex<T>(item)));

      while (stack.Count > 0) {
        IEnumerable<Vertex<T>> src = stack.Pop();

        if (src is null)
          continue;

        foreach (var item in src)
          if (proceeded.Add(item.Value)) {
            yield return item;

            stack.Push(children(item.Value).Select(v => new Vertex<T>(v, item, item.Level + 1)));
          }
      }
    }

    #endregion Public
  }

}
