using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Games.Life {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Life Generation
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class LifeGeneration
    : IEnumerable<(int y, int x)>,
      IEquatable<LifeGeneration>,
      ICloneable {

    #region Private Data

    private readonly HashSet<(int y, int x)> m_Cells = new HashSet<(int y, int x)>();

    #endregion Private Data

    #region Create

    /// <summary>
    /// Create
    /// </summary>
    public LifeGeneration() { }

    /// <summary>
    /// Create
    /// </summary>
    public LifeGeneration(IEnumerable<(int y, int x)> cells) {
      if (cells is null)
        throw new ArgumentNullException(nameof(cells));

      m_Cells = new HashSet<(int y, int x)>(cells);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Cells To Create
    /// </summary>
    public static HashSet<(int y, int x)> CellsToCreate(HashSet<(int y, int x)> cells) {
      if (cells is null)
        throw new ArgumentNullException(nameof(cells));

      if (cells.Count == 0)
        return new HashSet<(int y, int x)>();

      // Candidates to birth
      HashSet<(int y, int x)> vicinity = new HashSet<(int y, int x)>();

      foreach (var (y, x) in cells) {
        vicinity.Add((y - 1, x - 1));
        vicinity.Add((y - 1, x));
        vicinity.Add((y - 1, x + 1));

        vicinity.Add((y, x - 1));
        vicinity.Add((y, x + 1));

        vicinity.Add((y + 1, x - 1));
        vicinity.Add((y + 1, x));
        vicinity.Add((y + 1, x + 1));
      }

      HashSet<(int y, int x)> result = new HashSet<(int y, int x)>();

      foreach (var item in vicinity) {
        if (cells.Contains(item))
          continue;

        int s = 0;

        for (int y = -1; y <= 1; ++y)
          for (int x = -1; x <= 1; ++x)
            if (cells.Contains((item.y + y, item.x + x)))
              s += 1;

        if (s == 3)
          result.Add(item);
      }

      return result;
    }

    /// <summary>
    /// Cells To Kill 
    /// </summary>
    public static Queue<(int y, int x)> CellsToKill(HashSet<(int y, int x)> cells) {
      if (cells is null)
        throw new ArgumentNullException(nameof(cells));

      if (cells.Count == 0)
        return new Queue<(int y, int x)>();

      Queue<(int y, int x)> result = new Queue<(int y, int x)>();

      foreach (var item in cells) {
        int s = 0;

        for (int y = -1; y <= 1; ++y)
          for (int x = -1; x <= 1; ++x)
            if (cells.Contains((item.y + y, item.x + x)))
              s += 1;

        if (s < 3 || s > 4)
          result.Enqueue(item);
      }

      return result;
    }

    /// <summary>
    /// Cells Next Generation (cells modification)  
    /// </summary>
    public static void CellsNext(HashSet<(int y, int x)> cells) {
      if (cells is null)
        throw new ArgumentNullException(nameof(cells));

      if (cells.Count <= 0)
        return;

      HashSet<(int y, int x)> born = CellsToCreate(cells);
      Queue<(int y, int x)> dead = CellsToKill(cells);

      while (dead.Count > 0)
        cells.Remove(dead.Dequeue());

      foreach (var item in born)
        cells.Add(item);
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => $"Generation: {Generation}; Cells: {Count}";

    /// <summary>
    /// Next Generation
    /// </summary>
    public int Next() {
      Generation += 1;

      CellsNext(m_Cells);

      return Generation;
    }

    /// <summary>
    /// Generation 
    /// </summary>
    public int Generation { get; private set; }

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_Cells.Count;

    /// <summary>
    /// Contains
    /// </summary>
    public bool Contains((int y, int x) cell) => m_Cells.Contains(cell);

    /// <summary>
    /// Cells
    /// </summary>
    public IEnumerable<(int y, int x)> Cells => m_Cells;

    /// <summary>
    /// Indexer
    /// </summary>
    public bool this[int y, int x] {
      get {
        return m_Cells.Contains((y, x));
      }
      set {
        if (value)
          m_Cells.Add((y, x));
        else
          m_Cells.Remove((y, x));
      }
    }

    /// <summary>
    /// Indexer
    /// </summary>
    public bool this[(int y, int x) cell] {
      get {
        return m_Cells.Contains(cell);
      }
      set {
        if (value)
          m_Cells.Add(cell);
        else
          m_Cells.Remove(cell);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public string DrawField() {
      if (m_Cells.Count <= 0)
        return "";

      int minY = m_Cells.Min(p => p.y);
      int maxY = m_Cells.Max(p => p.y);

      int minX = m_Cells.Min(p => p.x);
      int maxX = m_Cells.Max(p => p.x);

      StringBuilder sb = new StringBuilder((maxY - minY + 1) * (maxX - minX + 3));

      for (int y = maxY; y >= minY; --y) {
        if (sb.Length > 0)
          sb.AppendLine();

        for (int x = minX; x <= maxX; ++x)
          if (m_Cells.Contains((y, x)))
            sb.Append('X');
          else
            sb.Append('.');
      }

      return sb.ToString();
    }

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(LifeGeneration left, LifeGeneration right) {
      if (ReferenceEquals(left, right))
        return true;
      else if ((left is null) || (right is null))
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(LifeGeneration left, LifeGeneration right) {
      if (ReferenceEquals(left, right))
        return false;
      else if ((left is null) || (right is null))
        return true;

      return !left.Equals(right);
    }

    #endregion Operators

    #region IEnumerator<(int y, int x)>

    /// <summary>
    /// Get Typed Enumeartor
    /// </summary>
    public IEnumerator<(int y, int x)> GetEnumerator() => m_Cells.GetEnumerator();

    /// <summary>
    /// Get Typed Enumeartor
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => m_Cells.GetEnumerator();

    #endregion IEnumerator<(int y, int x)>

    #region IEquatable<LifeGeneration>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(LifeGeneration other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (other is null)
        return false;

      if (m_Cells.Count != other.Count)
        return false;

      var deltas = m_Cells
        .OrderBy(item => item.y)
        .ThenBy(item => item.x)
        .Zip(other
           .m_Cells
           .OrderBy(item => item.y)
           .ThenBy(item => item.x), (left, right) => (dy: (long)left.y - right.y, dx: (long)left.x - right.x));

      long deltaY = 0;
      long deltaX = 0;
      bool first = true;

      foreach (var (dy, dx) in deltas) {
        if (first) {
          deltaY = dy;
          deltaX = dx;

          first = false;
        }
        else if (deltaY != dy || deltaX != dx)
          return false;
      }

      return true;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as LifeGeneration);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => m_Cells.Count;

    #endregion IEquatable<LifeGeneration>

    #region ICloneable

    /// <summary>
    /// Clone
    /// </summary>
    public LifeGeneration Clone() =>
      new LifeGeneration(m_Cells) {
        Generation = this.Generation
      };

    /// <summary>
    /// Clone
    /// </summary>
    object ICloneable.Clone() => Clone();

    #endregion ICloneable
  }
}
