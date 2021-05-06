using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Games.Sudoku {

  //---------------------------------------------------------------------------
  //
  /// <summary>
  /// Sudoku solver
  /// </summary>
  //
  //---------------------------------------------------------------------------

  public class SudokuSolver : ICloneable {
    #region Private Data

    // Solving problem
    private readonly SudokuData m_Problem;
    // Data
    private List<int>[][] m_Data;

    #endregion Private Data

    #region Algorithm

    // Create Data
    private void CoreCreateData() {
      m_Data = new List<int>[9][];

      for (int i = 0; i < 9; ++i)
        m_Data[i] = new List<int>[9];

      for (int i = 0; i < 9; ++i)
        for (int j = 0; j < 9; ++j)
          m_Data[i][j] = new List<int>();

      for (int i = 0; i < 9; ++i)
        for (int j = 0; j < 9; ++j)
          if (m_Problem[i, j] != 0)
            m_Data[i][j].Add(m_Problem[i, j]);
          else
            m_Data[i][j].AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

      for (int i = 0; i < 9; ++i) {
        CoreRemoveSolved(GetLine(i));
        CoreRemoveSolved(GetColumn(i));
        CoreRemoveSolved(GetSquare(i));
      }
    }

    // Get line
    private List<int>[] GetLine(int line) {
      List<int>[] result = new List<int>[9];

      for (int i = 0; i < 9; ++i)
        result[i] = m_Data[line][i];

      return result;
    }

    // Get column
    private List<int>[] GetColumn(int column) {
      List<int>[] result = new List<int>[9];

      for (int i = 0; i < 9; ++i)
        result[i] = m_Data[i][column];

      return result;
    }

    // Get square
    private List<int>[] GetSquare(int square) {
      List<int>[] result = new List<int>[9];

      int sLine = square / 3;
      int sCol = square % 3;
      int index = 0;

      for (int i = sLine * 3; i < sLine * 3 + 3; ++i)
        for (int j = sCol * 3; j < sCol * 3 + 3; ++j) {
          result[index] = m_Data[i][j];

          index += 1;
        }

      return result;
    }

    // Remove solved
    private static void CoreRemoveSolved(List<int>[] data) {
      List<int> solved = new();

      for (int i = 0; i < data.Length; ++i)
        if (data[i].Count == 1)
          solved.Add(data[i][0]);

      for (int k = 0; k < solved.Count; ++k)
        for (int i = 0; i < data.Length; ++i)
          if (data[i].Count == 1)
            continue;
          else
            data[i].Remove(solved[k]);
    }

    // Set solved
    private static void CoreSetSolved(List<int>[] data) {
      Dictionary<int, int> dict = new();

      for (int i = 0; i < data.Length; ++i)
        for (int k = 0; k < data[i].Count; ++k) {
          int v = data[i][k];

          if (dict.ContainsKey(v))
            dict[v] += 1;
          else
            dict.Add(v, 1);
        }

      var en = dict.GetEnumerator();

      while (en.MoveNext()) {
        if (en.Current.Value != 1)
          continue;

        for (int i = 0; i < data.Length; ++i)
          if (data[i].Count != 1)
            if (data[i].Contains(en.Current.Key)) {
              data[i].Clear();
              data[i].Add(en.Current.Key);

              break;
            }
      }
    }

    // Negative step 
    private void CoreNegative() {
      for (int i = 0; i < 9; ++i) {
        CoreRemoveSolved(GetLine(i));
        CoreRemoveSolved(GetColumn(i));
        CoreRemoveSolved(GetSquare(i));
      }
    }

    // Positive
    private void CorePositive() {
      for (int i = 0; i < 9; ++i) {
        CoreSetSolved(GetLine(i));
        CoreSetSolved(GetColumn(i));
        CoreSetSolved(GetSquare(i));
      }
    }

    // Core guess
    private void CoreGuess() {
      int min = int.MaxValue;
      int minLine = 0;
      int minCol = 0;

      for (int i = 0; i < 9; ++i)
        for (int j = 0; j < 9; ++j) {
          List<int> item = m_Data[i][j];

          if (item.Count <= 1)
            continue;

          if (item.Count < min) {
            min = item.Count;
            minLine = i;
            minCol = j;
          }
        }

      SudokuSolver solver = Clone();

      solver.m_Data[minLine][minCol].Remove(m_Data[minLine][minCol][0]);

      SudokuData solution = solver.Solve();

      if (solution.IsValid) {
        CoreAssign(solver);

        return;
      }

      int v = m_Data[minLine][minCol][0];

      m_Data[minLine][minCol].Clear();
      m_Data[minLine][minCol].Add(v);
    }

    // Core solve
    private Boolean CoreSolve() {
      if (!IsCorrect)
        return true;
      else if (IsSolved)
        return false;

      while (!IsSolved) {
        if (!IsCorrect)
          return false;

        Boolean hasProgress = true;

        while (hasProgress) {
          int degreeOfFreedom = DegreeOfFreedom;

          // Simple Negative step
          CoreNegative();

          // Simple Positive step
          CorePositive();

          // Simple Negative step
          CoreNegative();

          // Simple Positive step
          CorePositive();

          hasProgress = degreeOfFreedom > DegreeOfFreedom;

          if (!IsCorrect)
            return false;
          else if (IsSolved)
            return true;
        }

        // Guess step
        CoreGuess();
      }

      return IsCorrect;
    }

    // Assign
    private void CoreAssign(SudokuSolver other) {
      if (other is null)
        return;

      for (int i = 0; i < 9; ++i)
        for (int j = 0; j < 9; ++j) {
          List<int> source = other.m_Data[i][j];
          List<int> target = m_Data[i][j];

          target.Clear();

          for (int k = 0; k < source.Count; ++k)
            target.Add(source[k]);
        }
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public SudokuSolver(SudokuData problem)
      : base() {

      m_Problem = problem ?? throw new ArgumentNullException(nameof(problem));

      CoreCreateData();
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Degree of freedom
    /// </summary>
    public int DegreeOfFreedom {
      get {
        int result = 0;

        for (int i = 0; i < 9; ++i)
          for (int j = 0; j < 9; ++j)
            if (m_Data[i][j].Count > 1)
              result += m_Data[i][j].Count - 1;

        return result;
      }
    }

    /// <summary>
    /// Is solved
    /// </summary>
    public Boolean IsSolved {
      get {
        for (int i = 0; i < 9; ++i)
          for (int j = 0; j < 9; ++j)
            if (m_Data[i][j].Count <= 0)
              return true;
            else if (m_Data[i][j].Count > 1)
              return false;

        return true;
      }
    }

    /// <summary>
    /// Is correct so far
    /// </summary>
    public Boolean IsCorrect {
      get {
        for (int i = 0; i < 9; ++i)
          for (int j = 0; j < 9; ++j)
            if (m_Data[i][j].Count <= 0)
              return false;

        return true;
      }
    }

    /// <summary>
    /// Solve
    /// </summary>
    public SudokuData Solve() {
      CoreSolve();

      if (!IsCorrect)
        return new SudokuData();

      SudokuData result = new();

      for (int i = 0; i < 9; ++i)
        for (int j = 0; j < 9; ++j)
          result[i, j] = m_Data[i][j][0];

      return result;
    }

    /// <summary>
    /// Step, no guess
    /// </summary>
    public Boolean Step() {
      int degreeOfFreedom = DegreeOfFreedom;

      // Simple Negative step
      CoreNegative();
      // Simple Positive step
      CorePositive();

      // Simple Negative step
      CoreNegative();
      // Simple Positive step
      CorePositive();

      return (DegreeOfFreedom < degreeOfFreedom);
    }

    /// <summary>
    /// To string
    /// </summary>
    public override String ToString() {
      StringBuilder sb = new();

      for (int i = 0; i < 9; ++i) {
        if (sb.Length > 0)
          sb.AppendLine();

        for (int j = 0; j < 9; ++j) {
          List<int> item = m_Data[i][j];

          if (j > 0)
            sb.Append(' ');

          if (item.Count <= 0)
            sb.Append('X');
          else {
            sb.Append('[');
            sb.Append(string.Join(", ", item));
            sb.Append(']');
          }
        }
      }

      return sb.ToString();
    }

    #endregion Public

    #region ICloneable Members

    /// <summary>
    /// Clone
    /// </summary>
    public SudokuSolver Clone() {
      SudokuSolver result = new(m_Problem);

      result.CoreAssign(this);

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
