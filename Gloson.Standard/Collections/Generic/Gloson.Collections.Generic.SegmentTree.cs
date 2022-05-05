using System;
using System.Linq;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Segment Tree
  /// </summary>
  /// <typeparam name="T">Item type</typeparam>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class SegmentTree<T> {
    #region Private Data

    private readonly T[] m_Array;

    #endregion Private Data

    #region Algorithm

    private static int GetNextPowerOfTwo(int n) {
      int m = n,
      cnt = 0;

      if (n > 0 && (n & (n - 1)) == 0)
        return n;

      while (m != 0) {
        m >>= 1;
        cnt++;
      }

      return 1 << cnt;
    }

    private T[] CoreBuildSegmentTree(T[] segmentTree, T[] input, int l, int r, int pos) {
      if (l == r)
        segmentTree[pos] = input[l];
      else {
        int mid = l + (r - l) / 2;

        CoreBuildSegmentTree(segmentTree, input, l, mid, pos * 2 + 1);
        CoreBuildSegmentTree(segmentTree, input, mid + 1, r, pos * 2 + 2);

        segmentTree[pos] = Operation(segmentTree[pos * 2 + 1], segmentTree[pos * 2 + 2]);
      }

      return segmentTree;
    }

    private T[] CoreUpdateSegmentTree(T[] segmentTree, int i, T val, int l, int r, int pos) {
      if (i < l || r < i)
        return segmentTree;
      else if (l == r)
        segmentTree[pos] = val;
      else {
        int mid = l + (r - l) / 2;

        CoreUpdateSegmentTree(segmentTree, i, val, l, mid, pos * 2 + 1);
        CoreUpdateSegmentTree(segmentTree, i, val, mid + 1, r, pos * 2 + 2);

        segmentTree[pos] = Operation(segmentTree[pos * 2 + 1], segmentTree[pos * 2 + 2]);
      }

      return segmentTree;
    }

    private T CoreQuery(int l, int r, int queryL, int queryR, int pos) {
      if (queryL <= l && r <= queryR)
        return m_Array[pos];
      else if (queryL > r || queryR < l)
        return None;

      int mid = l + (r - l) / 2;

      return Operation(CoreQuery(l, mid, queryL, queryR, pos * 2 + 1), CoreQuery(mid + 1, r, queryL, queryR, pos * 2 + 2));
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="items">Items</param>
    /// <param name="operation">Operation to use</param>
    /// <param name="none">None (default) item</param>
    /// <exception cref="ArgumentNullException">When either items or operation are null</exception>
    public SegmentTree(T[] items, Func<T, T, T> operation, T none = default!) {
      if (items is null)
        throw new ArgumentNullException(nameof(items));

      Operation = operation ?? throw new ArgumentNullException(nameof(operation));

      T[] array = items.ToArray();

      Count = array.Length;
      None = none;

      var empty = Enumerable
        .Repeat(None, GetNextPowerOfTwo(Count) * 2 - 1)
        .ToArray();

      m_Array = CoreBuildSegmentTree(empty, array, 0, Count - 1, 0);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Empty element
    /// </summary>
    public T None { get; }

    /// <summary>
    /// Number of items in the initial collection
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Operation
    /// </summary>
    public Func<T, T, T> Operation { get; }

    /// <summary>
    /// Update value at index
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <exception cref="ArgumentOutOfRangeException">when index is out of [0..Count - 1] range</exception>
    public void Update(int index, T value) {
      if (index < 0 || index >= Count)
        throw new ArgumentOutOfRangeException(nameof(index));

      CoreUpdateSegmentTree(m_Array, index, value, 0, Count - 1, 0);
    }

    /// <summary>
    /// Query [left..right] interval
    /// </summary>
    /// <param name="left">Left Border</param>
    /// <param name="right">Right Border</param>
    /// <returns>Function on the interval</returns>
    /// <exception cref="ArgumentOutOfRangeException">when either left or right are out of [0..Count - 1] range</exception>
    public T Query(int left, int right) {
      if (left < 0 || left >= Count)
        throw new ArgumentOutOfRangeException(nameof(left));
      if (right < 0 || right >= Count)
        throw new ArgumentOutOfRangeException(nameof(right));

      if (left > right)
        (left, right) = (right, left);

      return CoreQuery(0, Count - 1, left, right, 0);
    }

    #endregion Public
  }

}
