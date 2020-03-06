using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Biology {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// DNA
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class Dna : 
    IReadOnlyList<DnaNuclearbase>, 
    IEquatable<Dna> {

    #region Private Data

    private List<DnaNuclearbase> m_Items;

    #endregion Private Data

    #region Create

    private Dna() { }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public Dna(IEnumerable<DnaNuclearbase> sequence) {
      if (null == sequence)
        throw new ArgumentNullException(nameof(sequence));

      m_Items = new List<DnaNuclearbase>(sequence);
    }

    /// <summary>
    /// Standrad constructor
    /// </summary>
    /// <param name="sequence"></param>
    public Dna(IEnumerable<char> sequence) {
      if (null == sequence)
        throw new ArgumentNullException(nameof(sequence));

      m_Items = sequence
        .Select(item => DnaNuclearbaseHelper.Parse(item))
        .ToList();
    }

    #endregion Create

    #region Public

    /// <summary>
    /// To Reverse Complement
    /// </summary>
    public Dna ToReverseComplement() {
      Dna result = new Dna() {
        m_Items = new List<DnaNuclearbase>(m_Items.Count)
      };

      for (int i = m_Items.Count - 1; i >= 0; --i)
        result.m_Items.Add(m_Items[i].Complement());

      return result;
    }

    /// <summary>
    /// Triplets
    /// </summary>
    public IEnumerable<DnaNuclearbase[]> Triplets(int startIndex) {
      if (startIndex < 0 || startIndex > m_Items.Count - 2)
        yield break;

      for (int i = startIndex; i < m_Items.Count - 2; i += 3) {
        DnaNuclearbase[] item = new DnaNuclearbase[3];

        item[0] = m_Items[i];
        item[1] = m_Items[i + 1];
        item[2] = m_Items[i + 2];

        yield return item;
      }
    }

    /// <summary>
    /// Items
    /// </summary>
    public IReadOnlyList<DnaNuclearbase> Items => m_Items;

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => string.Concat(m_Items);

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator == (Dna left, Dna right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (null == left || null == right)
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(Dna left, Dna right) {
      if (ReferenceEquals(left, right))
        return false;
      else if (null == left || null == right)
        return true;

      return !left.Equals(right);
    }

    /// <summary>
    /// From String
    /// </summary>
    public static implicit operator Dna(string value) => new Dna(value);

    #endregion Operators

    #region IReadOnlyList<DnaNuclearbase>

    /// <summary>
    /// Enumerator
    /// </summary>
    public IEnumerator<DnaNuclearbase> GetEnumerator() => m_Items.GetEnumerator();

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
    public DnaNuclearbase this[int index] => m_Items[index];

    #endregion IReadOnlyList<DnaNuclearbase>

    #region IEquatable<Dna>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(Dna other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      return Enumerable.SequenceEqual(m_Items, other.m_Items);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as Dna);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() {
      if (m_Items == null)
        return 0;

      unchecked {
        int n = Math.Min(16, m_Items.Count);

        int result = 0;

        for (int i = 0; i < n; ++i) 
          result ^= ((byte)m_Items[i] << (i * 8));

        return result ^ m_Items.Count;
      }
    }

    #endregion IEquatable<Dna>
  }
}
