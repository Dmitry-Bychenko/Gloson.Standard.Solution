using Gloson.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Games.Linguistics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Crossword cell
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class CrosswordCell {
    #region Private Data

    private readonly List<CrosswordLine> m_Lines = new List<CrosswordLine>();

    #endregion Private Data

    #region Algorithm

    internal void CoreAddLine(CrosswordLine line) {
      if (line is null)
        return;

      if (!m_Lines.Contains(line))
        m_Lines.Add(line);
    }

    #endregion Algorithm

    #region Create

    internal CrosswordCell(Crossword problem, int y, int x, char? letter) {
      Problem = problem ?? throw new ArgumentNullException(nameof(problem));
      Y = y;
      X = x;
      Letter = letter;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Crossword problem
    /// </summary>
    public Crossword Problem { get; }

    /// <summary>
    /// Lines
    /// </summary>
    public IReadOnlyList<CrosswordLine> Lines => m_Lines;

    /// <summary>
    /// X coordinate within crossword grid
    /// </summary>
    public int X { get; }

    /// <summary>
    /// Y coordinate within crossword grid
    /// </summary>
    public int Y { get; }

    /// <summary>
    /// Assigned Letter
    /// </summary>
    public char? Letter {
      get;
      set;
    }

    /// <summary>
    /// Is Completed
    /// </summary>
    public bool IsCompleted => Letter.HasValue;

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => Letter.HasValue
      ? Letter.ToString()
      : " ";

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Crossword line
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class CrosswordLine {
    #region Private Data

    private readonly List<CrosswordCell> m_Cells = new List<CrosswordCell>();

    #endregion Private Data

    #region Algorithm
    #endregion Algorithm

    #region Create

    internal CrosswordLine(IEnumerable<CrosswordCell> cells) {
      if (cells is null)
        throw new ArgumentNullException(nameof(cells));

      m_Cells = cells
        .OrderBy(item => item.Y)
        .ThenBy(item => item.X)
        .ToList();

      foreach (var cell in m_Cells)
        cell.CoreAddLine(this);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Cells
    /// </summary>
    public IReadOnlyList<CrosswordCell> Cells => m_Cells;

    /// <summary>
    /// Crossword
    /// </summary>
    public Crossword Problem => m_Cells.Count <= 0 ? null : m_Cells[0].Problem;

    /// <summary>
    /// Is Completed
    /// </summary>
    public bool IsCompleted => m_Cells.All(c => c.Letter.HasValue);

    /// <summary>
    /// Is Vertical
    /// </summary>
    public bool IsVertical => m_Cells.Count > 1 && m_Cells[0].Y == m_Cells[1].Y;

    /// <summary>
    /// Is Horizontal
    /// </summary>
    public bool IsHorizontal => m_Cells.Count > 1 && m_Cells[0].X == m_Cells[1].X;

    /// <summary>
    /// Word
    /// </summary>
    public string Word(char empty = ' ') => string.Concat(m_Cells
      .Select(c => c.Letter ?? empty));

    /// <summary>
    /// Index
    /// </summary>
    public int Index { get; internal set; }

    /// <summary>
    /// Try Apply
    /// </summary>
    public bool TryApply(string word) {
      if (null == word)
        return false;
      if (word.Length != m_Cells.Count)
        return false;

      for (int i = 0; i < m_Cells.Count; ++i)
        if (m_Cells[i].Letter.HasValue && m_Cells[i].Letter.Value != word[i])
          return false;

      for (int i = 0; i < m_Cells.Count; ++i)
        m_Cells[i].Letter = word[i];

      return true;
    }

    /// <summary>
    /// Dependent lines
    /// </summary>
    public CrosswordLine[] Dependent() =>
      m_Cells
        .SelectMany(c => c.Lines)
        .Except(new CrosswordLine[] { this })
        .Distinct()
        .ToArray();

    /// <summary>
    /// Clear
    /// </summary>
    public void Clear() {
      for (int i = 0; i < m_Cells.Count; ++i) {
        var cell = m_Cells[i];

        if (cell.Lines.Any(line => line != this && line.IsCompleted))
          continue;

        cell.Letter = null;
      }
    }

    /// <summary>
    /// Clear All
    /// </summary>
    public void ClearAll() {
      for (int i = 0; i < m_Cells.Count; ++i)
        m_Cells[i].Letter = null;
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() =>
      string.Concat(m_Cells.Select(c => c.Letter.HasValue ? c.Letter : ' '));

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Crossword itself 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class Crossword {
    #region Private Data

    private List<CrosswordCell> m_Cells;

    private readonly List<CrosswordLine> m_Lines = new List<CrosswordLine>();

    #endregion Private Data

    #region Algorithm

    private void BuildLines() {
      // Horizontal lines

      foreach (var group in m_Cells.GroupBy(cell => cell.Y).OrderBy(g => g.Key)) {
        int y = group.Key;

        var rows = group
          .OrderBy(cell => cell.X)
          .ToBatch((batch, cell) => batch.Any(c => c.X + 1 == cell.X));

        foreach (var row in rows)
          if (row.Length > 1)
            m_Lines.Add(new CrosswordLine(row));
      }

      // Vertical lines

      foreach (var group in m_Cells.GroupBy(cell => cell.X).OrderBy(g => g.Key)) {
        int y = group.Key;

        var rows = group
          .OrderBy(cell => cell.Y)
          .ToBatch((batch, cell) => batch.Any(c => c.X + 1 == cell.X));

        foreach (var row in rows)
          if (row.Length > 1)
            m_Lines.Add(new CrosswordLine(row));
      }

      var lines = m_Lines
        .OrderBy(line => line.Cells[0].Y)
        .ThenBy(line => line.Cells[0].X);

      int index = 0;

      foreach (var line in lines)
        line.Index = ++index;

      m_Lines.Sort((left, right) => left.Index.CompareTo(right.Index));
    }

    private void BuildCells(IEnumerable<(int y, int x, char? letter)> grid) {
      m_Cells = grid
        .Distinct()
        .OrderBy(item => item.y)
        .ThenBy(item => item.x)
        .Select(item => new CrosswordCell(this, item.y, item.x, item.letter))
        .ToList();

      BuildLines();
    }

    private static IEnumerable<(int y, int x, char? letter)> CoreParse(IEnumerable<string> grid, char empty) {
      if (null == grid)
        throw new ArgumentNullException(nameof(grid));

      int y = -1;

      foreach (string line in grid) {
        y += 1;

        if (string.IsNullOrEmpty(line))
          continue;

        for (int x = 0; x < line.Length; ++x)
          if (line[x] == empty)
            yield return (y, x, null);
          else if (char.IsLetter(line[x]) || line[x] == '\'')
            yield return (y, x, line[x]);
      }
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Create from cells
    /// </summary>
    public Crossword(IEnumerable<(int y, int x, char? letter)> grid) {
      if (grid is null)
        throw new ArgumentNullException(nameof(grid));

      BuildCells(grid);
    }

    /// <summary>
    /// Create from cells
    /// </summary>
    public Crossword(IEnumerable<(int y, int x)> grid) {
      if (grid is null)
        throw new ArgumentNullException(nameof(grid));

      BuildCells(grid.Select(item => (item.y, item.x, (char?)null)));
    }

    /// <summary>
    /// Create From Text
    /// </summary>
    public Crossword(IEnumerable<string> grid, char empty = '-') {
      if (grid is null)
        throw new ArgumentNullException(nameof(grid));

      BuildCells(CoreParse(grid, empty));
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Cells
    /// </summary>
    public IReadOnlyList<CrosswordCell> Cells => m_Cells;

    /// <summary>
    /// Lines
    /// </summary>
    public IReadOnlyList<CrosswordLine> Lines => m_Lines;

    /// <summary>
    /// Is Completed
    /// </summary>
    public bool IsCompleted => m_Cells.All(cell => cell.IsCompleted);

    /// <summary>
    /// To report
    /// </summary>
    public string ToReport(char empty = '.', char fill = '#') {
      if (m_Cells.Count <= 0)
        return "";

      int minY = m_Cells.Min(c => c.Y);
      int maxY = m_Cells.Max(c => c.Y);
      int minX = m_Cells.Min(c => c.X);
      int maxX = m_Cells.Max(c => c.X);

      var dict = m_Cells
        .ToDictionary(c => (y: c.Y, x: c.X), c => c);

      StringBuilder result = new StringBuilder(maxY - minY + 3);

      result.Append(new string(fill, maxX - minX + 3));

      for (int y = minY; y <= maxY; ++y) {
        result.AppendLine();

        StringBuilder sb = new StringBuilder(new string(fill, maxX - minX + 3));

        for (int x = minX; x <= maxX; ++x)
          if (dict.TryGetValue((y, x), out var cell))
            sb[x - minX + 1] = cell.IsCompleted ? cell.Letter.Value : empty;

        result.Append(sb);
      }

      result.AppendLine();
      result.Append(new string(fill, maxX - minX + 3));

      return result.ToString();
    }

    #endregion Public
  }


}
