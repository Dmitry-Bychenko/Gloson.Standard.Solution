using System;
using System.Linq;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Bloom Filter
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class BloomFilter<T> : IEquatable<BloomFilter<T>> {
    #region Private Data

    private readonly ulong[] m_Bits;

    private readonly Func<T, int>[] m_Hashes;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard constructor 
    /// </summary>
    /// <param name="size">Bloom Filter Size</param>
    /// <param name="hashes">Hash functions to use in the filter</param>
    public BloomFilter(int size, params Func<T, int>[] hashes) {
      if (size <= 0)
        throw new ArgumentOutOfRangeException(nameof(size));
      if (hashes is null)
        throw new ArgumentNullException(nameof(hashes));

      m_Hashes = hashes
        .Where(hash => hash is not null)
        .Distinct()
        .ToArray();

      if (m_Hashes.Length <= 0)
        throw new ArgumentException("No valid hash function provided.", nameof(hashes));

      m_Bits = new ulong[size / 64 + (size % 64 == 0 ? 0 : 1)];
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Recommended number of hash functions
    /// </summary>
    /// <param name="size">Bloom filter size</param>
    /// <param name="expectedCount">Expected number of items</param>
    public static int RecommendedHashesCount(int size, int expectedCount) {
      if (size <= 0)
        throw new ArgumentOutOfRangeException(nameof(size));
      if (expectedCount < 0)
        throw new ArgumentOutOfRangeException(nameof(expectedCount));

      if (expectedCount <= 0)
        expectedCount = 1;

      return (int)((Math.Log(2) * size) / expectedCount + 0.5);
    }

    /// <summary>
    /// Bloom Filter Size
    /// </summary>
    public int Size => m_Bits.Length * sizeof(ulong);

    /// <summary>
    /// Number of hash functions used
    /// </summary>
    public int HashCount => m_Hashes.Length;

    /// <summary>
    /// Number of items set
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Add New Value 
    /// </summary>
    public void Add(T value) {
      Count += 1;

      foreach (Func<T, int> func in m_Hashes) {
        int index = ((func(value) % Size) + Size) % Size;

        m_Bits[index / 64] |= (1ul << (index % 64));
      }
    }

    /// <summary>
    /// Contains (with possible false positive)
    /// </summary>
    /// <param name="value">value to test</param>
    /// <returns>true, if, probably contains; false if doesn't contain</returns>
    public bool Contains(T value) {
      foreach (Func<T, int> func in m_Hashes) {
        int index = ((func(value) % Size) + Size) % Size;

        if ((m_Bits[index / 64] & (1ul << (index % 64))) == 0)
          return false;
      }

      return true;
    }

    /// <summary>
    /// False Positive rate for actual Count
    /// </summary>
    public double FalsePositiveRate =>
      Math.Pow(1 - Math.Exp(-((double)m_Hashes.Length) * Count / Size), m_Hashes.Length);

    /// <summary>
    /// Union 
    /// </summary>
    public BloomFilter<T> Union(BloomFilter<T> other) {
      if (other is null)
        throw new ArgumentNullException(nameof(other));

      if (!StructuralEquals(other))
        throw new ArgumentException("Filters are not structural equal to one another", nameof(other));

      BloomFilter<T> result = new BloomFilter<T>(Size, m_Hashes);

      for (int i = 0; i < result.m_Bits.Length; ++i)
        result.m_Bits[i] = m_Bits[i] | other.m_Bits[i];

      return result;
    }

    /// <summary>
    /// Intersect
    /// </summary>
    public BloomFilter<T> Intersect(BloomFilter<T> other) {
      if (other is null)
        throw new ArgumentNullException(nameof(other));

      if (!StructuralEquals(other))
        throw new ArgumentException("Filters are not structural equal to one another", nameof(other));

      BloomFilter<T> result = new BloomFilter<T>(Size, m_Hashes);

      for (int i = 0; i < result.m_Bits.Length; ++i)
        result.m_Bits[i] = m_Bits[i] & other.m_Bits[i];

      return result;
    }

    #endregion Public

    #region IEquatable<BloomFilter<T>>

    /// <summary>
    /// Structural Equals
    /// </summary>
    public bool StructuralEquals(BloomFilter<T> other) {
      if (other is null)
        return false;

      return (Size == other.Size) && m_Hashes.SequenceEqual(other.m_Hashes);
    }

    /// <summary>
    /// Equals 
    /// </summary>
    public bool Equals(BloomFilter<T> other) {
      if (other is null)
        return false;

      return Size == other.Size &&
             m_Bits.SequenceEqual(other.m_Bits) &&
             m_Hashes.SequenceEqual(other.m_Hashes);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => obj is BloomFilter<T> other && Equals(other);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => unchecked((Size << 6) ^ m_Hashes.Length ^ m_Hashes[0].GetHashCode());

    #endregion IEquatable<BloomFilter<T>>
  }

}
