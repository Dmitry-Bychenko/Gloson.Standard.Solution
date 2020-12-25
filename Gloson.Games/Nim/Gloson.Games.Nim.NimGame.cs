using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gloson.Games.Nim {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Nim Game
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class NimGame : IEquatable<NimGame> {
    #region Private Data

    private readonly List<long> m_Heaps = new List<long>();

    private readonly long m_Max;
    private readonly int m_ArgMax;

    private long m_WinningCount;
    private int m_WinningHeap;

    #endregion Private Data

    #region Algorithm

    private long Adjust(int i) {
      int factor = 0;

      long current = m_Heaps[i];
      long saved = current;
      long restNim = NimSum ^ current;

      for (long nim = NimSum; nim > 0 && current >= 0; ++factor) {
        long rem = (nim >> factor) % 2;

        if (rem == 0)
          continue;

        long v = rem << factor;

        current -= v;
        nim = restNim ^ current;
      }

      if (current < 0)
        return -1;
      else
        return saved - current;
    }

    private void WinningMove() {
      for (int i = 0; i < m_Heaps.Count; ++i) {
        long result = Adjust(i);

        if (result >= 0) {
          m_WinningHeap = i;
          m_WinningCount = result;

          break;
        }
      }
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="heaps">Heaps</param>
    public NimGame(IEnumerable<long> heaps) {
      if (heaps is null)
        throw new ArgumentNullException(nameof(heaps));

      int argMax = 0;
      long max = 0;
      long nim = 0;

      foreach (long item in heaps) {
        if (item < 0)
          throw new ArgumentOutOfRangeException(nameof(heaps), $"Item {nameof(heaps)}[{item}] < 0");

        if (item >= max) {
          argMax = m_Heaps.Count;
          max = item;
        }

        nim ^= item;

        m_Heaps.Add(item);
      }

      NimSum = nim;
      m_Max = max;
      m_ArgMax = argMax;

      WinningMove();
    }

    /// <summary>
    /// Try parse; any sequences of non-nengative long numbers like "123 456 78" or "123;456;78" are valid  
    /// </summary>
    public static bool TryParse(string value, out NimGame result) {
      result = null;

      if (string.IsNullOrEmpty(value))
        return false;

      var lines = Regex
        .Matches(value, "-?[0-9]+")
        .OfType<Match>()
        .Select(match => match.Value);

      List<long> list = new List<long>();

      foreach (string line in lines) {
        if (!long.TryParse(line, out long v))
          return false;

        if (v < 0)
          return false;

        list.Add(v);
      }

      result = new NimGame(list);

      return true;
    }

    /// <summary>
    /// Parse; any sequences of non-nengative long numbers like "123 456 78" or "123;456;78" are valid  
    /// </summary>
    public static NimGame Parse(string value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      return TryParse(value, out var result)
        ? result
        : throw new FormatException($"Failed to Parse into {typeof(NimGame).Name}");
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Advice (best move)
    /// </summary>
    public static (int heap, long count, bool win) Advice(IEnumerable<long> heaps) {
      NimGame nim = new NimGame(heaps);

      return (nim.BestMove.heap, nim.BestMove.count, nim.IsWin);
    }

    /// <summary>
    /// Is Win
    /// </summary>
    public bool IsWin => NimSum != 0;

    /// <summary>
    /// Is Completed
    /// </summary>
    public bool IsComplted => m_Heaps.All(item => item == 0);

    /// <summary>
    /// Best Move
    /// </summary>
    public (int heap, long count) BestMove => IsWin
      ? (m_WinningHeap, m_WinningCount)
      : (m_ArgMax, m_Max);

    /// <summary>
    /// Nim Sum
    /// </summary>
    public long NimSum { get; }

    /// <summary>
    /// Heaps
    /// </summary>
    public IReadOnlyList<long> Heaps => m_Heaps;

    /// <summary>
    /// Move
    /// </summary>
    public NimGame Move(int heap, long count) {
      if (heap < 0 || heap >= m_Heaps.Count)
        throw new ArgumentOutOfRangeException(nameof(heap));
      else if (count <= 0)
        throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive");
      else if (count > m_Heaps[heap])
        throw new ArgumentOutOfRangeException(nameof(count), $"Heap #{heap} has {m_Heaps[heap]} items only; {count} can't be taken");

      return new NimGame(m_Heaps
        .Select((v, i) => i == heap ? v - count : v));
    }

    /// <summary>
    /// Move
    /// </summary>
    public NimGame Move((int heap, long count) move) => Move(move.heap, move.count);

    #endregion Public

    #region Operators

    /// <summary>
    /// ToString
    /// </summary>
    public override string ToString() => string.Join(" ", m_Heaps);

    /// <summary>
    /// Boolean - IsWinning
    /// </summary>
    public static implicit operator bool(NimGame value) {
      if (value is null)
        return false;

      return value.IsWin;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(NimGame left, NimGame right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (left is null || right is null)
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(NimGame left, NimGame right) {
      if (ReferenceEquals(left, right))
        return false;
      else if (left is null || right is null)
        return true;

      return !left.Equals(right);
    }

    #endregion Operators

    #region IEquatable<NimPosition>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(NimGame other) {
      if (other is null)
        return false;

      return m_Heaps
        .Where(item => item > 0)
        .OrderBy(item => item)
        .SequenceEqual(other
           .m_Heaps
           .Where(item => item > 0)
           .OrderBy(item => item));
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as NimGame);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() {
      return unchecked((int)(m_Heaps.Count ^ (m_Max << 4)));
    }

    #endregion IEquatable<NimPosition>
  }

}
