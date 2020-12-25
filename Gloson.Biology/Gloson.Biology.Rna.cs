using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Biology {
  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// RNA
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class Rna :
    IReadOnlyList<RnaNuclearbase>,
    IEquatable<Rna> {

    #region Private Data

    private List<RnaNuclearbase> m_Items;

    #endregion Private Data

    #region Create

    private Rna() { }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public Rna(IEnumerable<RnaNuclearbase> sequence) {
      if (sequence is null)
        throw new ArgumentNullException(nameof(sequence));

      m_Items = new List<RnaNuclearbase>(sequence);
    }

    /// <summary>
    /// Standrad constructor
    /// </summary>
    /// <param name="sequence"></param>
    public Rna(IEnumerable<char> sequence) {
      if (sequence is null)
        throw new ArgumentNullException(nameof(sequence));

      m_Items = sequence
        .Select(item => RnaNuclearbaseHelper.Parse(item))
        .ToList();
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public static Rna FromDna(Dna source) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      Rna result = new Rna() {
        m_Items = new List<RnaNuclearbase>(source.Count)
      };

      for (int i = 0; i < source.Count; ++i)
        result.m_Items.Add(source[i].RnaComplement());

      return result;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Items
    /// </summary>
    public IReadOnlyList<RnaNuclearbase> Items => m_Items;

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => string.Concat(m_Items);

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(Rna left, Rna right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (left is null || right is null)
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(Rna left, Rna right) {
      if (ReferenceEquals(left, right))
        return false;
      else if (left is null || right is null)
        return true;

      return !left.Equals(right);
    }

    /// <summary>
    /// From String
    /// </summary>
    public static implicit operator Rna(string value) => new Rna(value);

    #endregion Operators

    #region IReadOnlyList<RnaNuclearbase>

    /// <summary>
    /// Enumerator
    /// </summary>
    public IEnumerator<RnaNuclearbase> GetEnumerator() => m_Items.GetEnumerator();

    /// <summary>
    /// Typeless Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => m_Items.GetEnumerator();

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_Items.Count;

    /// <summary>
    /// Indexer
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public RnaNuclearbase this[int index] => m_Items[index];

    #endregion IReadOnlyList<RnaNuclearbase>

    #region IEquatable<Rna>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(Rna other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (other is null)
        return false;

      return Enumerable.SequenceEqual(m_Items, other.m_Items);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as Rna);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() {
      if (m_Items is null)
        return 0;

      unchecked {
        int n = Math.Min(16, m_Items.Count);

        int result = 0;

        for (int i = 0; i < n; ++i)
          result ^= ((byte)m_Items[i] << (i * 8));

        return result ^ m_Items.Count;
      }
    }

    #endregion IEquatable<Rna>
  }
}
