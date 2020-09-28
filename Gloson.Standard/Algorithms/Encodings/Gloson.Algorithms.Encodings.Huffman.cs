using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gloson.Collections.Generic;

namespace Gloson.Algorithms.Encodings {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Huffman encoding
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class HuffmanCodes {
    #region Internal classes

    internal sealed class HuffmanNode<T> : IComparable<HuffmanNode<T>> {
      #region Private Data

      internal StringBuilder Code { get; } = new StringBuilder();

      #endregion Private Data

      #region Create

      public HuffmanNode(T value, double weight, int index) {
        Value = value;
        Weight = weight;
        Index = index;
      }

      public HuffmanNode(HuffmanNode<T> left, HuffmanNode<T> right) {
        Value = default;
        Weight = left.Weight + right.Weight;

        Left = left;
        Right = right;
      }

      #endregion Create

      #region Public

      public T Value { get; }

      public int Index { get; }

      public double Weight { get; }

      public HuffmanNode<T> Left { get; }

      public HuffmanNode<T> Right { get; }

      #endregion Public

      #region IComparable<HuffmanNode<T>>

      public int CompareTo(HuffmanNode<T> other) {
        if (null == other)
          return 1;

        return Weight.CompareTo(other.Weight);
      }

      #endregion IComparable<HuffmanNode<T>>
    }

    #endregion Internal classes

    #region Algorithm

    private static HuffmanNode<T> BuildHuffmanTree<T>(IEnumerable<T> source, Func<T, double> weight) {
      MinHeap<HuffmanNode<T>> heap = new MinHeap<HuffmanNode<T>>();

      int index = 0;

      foreach (T item in source)
        heap.Add(new HuffmanNode<T>(item, weight(item), index++));

      if (heap.Count == 0)
        return null;

      while (heap.Count > 1) {
        HuffmanNode<T> min1 = heap.Pop();
        HuffmanNode<T> min2 = heap.Pop();

        HuffmanNode<T> combined = new HuffmanNode<T>(min1, min2);

        heap.Add(combined);
      }

      return heap.Pop();
    }

    private static IEnumerable<(T value, string code, int index)> CoreBuildCodes<T>(HuffmanNode<T> root) {
      if (null == root)
        yield break;

      if (root.Left == null && root.Right == null) {
        yield return (root.Value, "0", 0);

        yield break;
      }

      Queue<HuffmanNode<T>> agenda = new Queue<HuffmanNode<T>>();

      agenda.Enqueue(root);

      while (agenda.Count > 0) {
        HuffmanNode<T> node = agenda.Dequeue();

        if (node.Left == null && node.Right == null)
          yield return (node.Value, node.Code.ToString(), node.Index);
        else {
          if (node.Left != null) {

            node.Left.Code.Append(node.Code.ToString());
            node.Left.Code.Append('1');
            agenda.Enqueue(node.Left);
          }

          if (node.Right != null) {
            node.Right.Code.Append(node.Code.ToString());
            node.Right.Code.Append('0');
            agenda.Enqueue(node.Right);
          }
        }
      }
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Huffman codes 
    /// </summary>
    public static IEnumerable<(T value, string code)> EncodeHuffman<T>(this IEnumerable<T> source, Func<T, double> weight) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == weight)
        throw new ArgumentNullException(nameof(weight));

      var root = BuildHuffmanTree(source, weight);

      foreach (var (value, code, index) in CoreBuildCodes(root).OrderBy(x => x.index))
        yield return (value, code);
    }

    #endregion Public
  }

}
