using Gloson.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Games.Sudoku {

  //---------------------------------------------------------------------------
  //
  /// <summary>
  /// Sudoku 
  /// </summary>
  //
  //---------------------------------------------------------------------------

  public class SudokuData : ICloneable {
    #region Private Data

    // Title
    private String m_Title = "";
    // Data
    private readonly int[][] m_Data;

    #endregion Private Data

    #region Algorithm
    #endregion Algorithm

    #region Create

    /// <summary>
    /// Sudoku
    /// </summary>
    public SudokuData()
      : base() {

      m_Data = new int[9][];

      for (int i = 0; i < 9; ++i)
        m_Data[i] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Title
    /// </summary>
    public String Title {
      get {
        return m_Title;
      }
      set {
        if (String.IsNullOrEmpty(value))
          value = "";

        m_Title = value;
      }
    }

    /// <summary>
    /// Line Count
    /// </summary>
    public int LineCount {
      get {
        return m_Data.Length;
      }
    }

    /// <summary>
    /// Column Count
    /// </summary>
    public int ColumnCount {
      get {
        return m_Data[0].Length;
      }
    }

    /// <summary>
    /// Items
    /// </summary>
    public int this[int line, int column] {
      get {
        if ((line < 0) || (line >= 9))
          throw new ArgumentOutOfRangeException(nameof(line));
        else if ((column < 0) || (column >= 9))
          throw new ArgumentOutOfRangeException(nameof(column));

        return m_Data[line][column];
      }
      set {
        if ((line < 0) || (line >= 9))
          throw new ArgumentOutOfRangeException(nameof(line));
        else if ((column < 0) || (column >= 9))
          throw new ArgumentOutOfRangeException(nameof(column));

        if ((value < 0) || (value > 9))
          throw new ArgumentOutOfRangeException(nameof(value));

        m_Data[line][column] = value;
      }
    }

    /// <summary>
    /// Is Correct
    /// </summary>
    public Boolean IsValid {
      get {
        HashSet<int> hs = new HashSet<int>();

        // Lines
        for (int i = 0; i < LineCount; ++i) {
          hs.Clear();

          for (int j = 0; j < ColumnCount; ++j)
            if (m_Data[i][j] == 0)
              continue;
            else if (hs.Contains(m_Data[i][j]))
              return false;
            else
              hs.Add(m_Data[i][j]);
        }

        // Columns
        for (int i = 0; i < ColumnCount; ++i) {
          hs.Clear();

          for (int j = 0; j < LineCount; ++j)
            if (m_Data[j][i] == 0)
              continue;
            else if (hs.Contains(m_Data[j][i]))
              return false;
            else
              hs.Add(m_Data[j][i]);
        }

        // Squares
        for (int i = 0; i < ColumnCount; ++i) {
          hs.Clear();

          int startX = (i / 3) * 3;
          int startY = (i % 3) * 3;

          for (int j = 0; j < LineCount; ++j) {
            int x = startX + j / 3;
            int y = startY + j % 3;

            int v = m_Data[y][x];

            if (v == 0)
              continue;
            else if (hs.Contains(v))
              return false;
            else
              hs.Add(v);
          }
        }

        // No refutations so far
        return true;
      }
    }

    /// <summary>
    /// Is Solved (correctly)
    /// </summary>
    public Boolean IsSolved {
      get {
        for (int line = 0; line < m_Data.Length; ++line) {
          int[] data = m_Data[line];

          for (int col = 0; col < data.Length; ++col)
            if (data[col] == 0)
              return false;
        }

        return IsValid;
      }
    }

    /// <summary>
    /// Can be solved (has at least one solution)
    /// </summary>
    public Boolean CanBeSolved {
      get {
        SudokuSolver solver = new SudokuSolver(this);

        solver.Solve();

        return solver.IsCorrect && solver.IsSolved;
      }
    }

    /// <summary>
    /// Solve
    /// </summary>
    public SudokuData Solve() {
      SudokuSolver solver = new SudokuSolver(this);

      return solver.Solve();
    }

    /// <summary>
    /// To string
    /// </summary>
    public override String ToString() {
      StringBuilder Sb = new StringBuilder();

      if (!String.IsNullOrEmpty(m_Title))
        Sb.Append(m_Title);

      for (int i = 0; i < m_Data.Length; ++i) {
        int[] line = m_Data[i];

        if (Sb.Length > 0)
          Sb.AppendLine();

        for (int j = 0; j < line.Length; ++j)
          if (m_Data[i][j] == 0)
            Sb.Append('.');
          else
            Sb.Append(m_Data[i][j]);
      }

      return Sb.ToString();
    }

    #endregion Public

    #region Operators

    /// <summary>
    /// To Boolean (is solved)
    /// </summary>
    public Boolean ToBoolean() {
      return IsSolved;
    }

    /// <summary>
    /// To Boolean (is solved)
    /// </summary>
    public static implicit operator Boolean(SudokuData value) {
      return value?.ToBoolean() ?? false;
    }

    /// <summary>
    /// From string
    /// </summary>
    public static SudokuData FromString(String value) {
      if (String.IsNullOrEmpty(value))
        return null;

      IList<String> lines = value.SplitToLines().ToList();

      if (lines.Count < 9)
        throw new ArgumentException("Wrong format of a Sudoku puzzle.", nameof(value));

      int count = 0;

      for (int i = lines.Count - 1; i >= 0; --i) {
        if (count >= 9)
          break;

        lines[i] = lines[i].Trim();

        if (String.IsNullOrEmpty(lines[i]))
          lines.RemoveAt(i);
        else
          count += 1;
      }

      if (lines.Count < 9)
        throw new ArgumentException("Wrong format of a Sudoku puzzle.", nameof(value));

      SudokuData result = new SudokuData();

      StringBuilder Sb = new StringBuilder();

      for (int i = 0; i < lines.Count - 9; ++i) {
        if (Sb.Length > 0)
          Sb.AppendLine();

        Sb.Append(lines[i]);
      }

      result.Title = Sb.ToString();

      for (int i = 0; i < 9; ++i) {
        String line = lines[lines.Count - 9 + i].Replace(" ", "");

        if (line.Length > 9)
          throw new ArgumentException("Wrong format of a Sudoku puzzle.", nameof(value));

        for (int j = 0; j < line.Length; ++j) {
          Char Ch = line[j];

          if ((Ch >= '0') && (Ch <= '9'))
            result.m_Data[i][j] = Ch - '0';
          else if ((Ch == '*') || (Ch == '.') || (Ch == 'x') || (Ch == '?') || (Ch == '_'))
            result.m_Data[i][j] = 0;
          else
            throw new ArgumentException("Wrong format of a Sudoku puzzle.", nameof(value));
        }
      }

      return result;
    }

    /// <summary>
    /// From String
    /// </summary>
    public static implicit operator SudokuData(String value) {
      return FromString(value);
    }

    #endregion Operators

    #region ICloneable Members

    /// <summary>
    /// Clone
    /// </summary>
    public SudokuData Clone() {
      SudokuData result = new SudokuData() {
        m_Title = this.m_Title
      };

      for (int i = 0; i < m_Data.Length; ++i)
        for (int j = 0; j < m_Data.Length; ++j)
          result.m_Data[i][j] = m_Data[i][j];

      return result;
    }

    /// <summary>
    /// Clone
    /// </summary>
    Object ICloneable.Clone() {
      return this.Clone();
    }

    #endregion ICloneable Members
  }

}
