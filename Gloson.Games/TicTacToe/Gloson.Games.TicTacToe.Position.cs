using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Games.TicTacToe {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Mark
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum Mark : byte {
    None = 0,
    Cross = 1,
    Nought = 2
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Mark Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class MarkHelper {
    /// <summary>
    /// To Char
    /// </summary>
    public static char ToChar(this Mark mark) {
      return mark switch
      {
        Mark.Cross => 'X',
        Mark.Nought => 'O',
        Mark.None => '.',
        _ => '?'
      };
    }
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Tic Tac Toe Position
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class TicTacToePosition : IEquatable<TicTacToePosition> {
    #region Private Data

    private readonly Mark[] m_Marks = new Mark[9];

    #endregion Private Data

    #region Create

    // Standard Constructor
    private TicTacToePosition() {
    }

    // Clone
    private TicTacToePosition Clone() {
      TicTacToePosition result = new TicTacToePosition();

      for (int i = m_Marks.Length - 1; i >= 0; --i)
        result.m_Marks[i] = m_Marks[i];

      return result;
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out TicTacToePosition result) {
      result = null;

      if (string.IsNullOrWhiteSpace(value))
        return false;

      var raw = value
        .Where(c => !char.IsWhiteSpace(c))
        .Select(c =>
             (c == 'x' || c == 'X' || c == '1' || c == '+') ? 1
           : (c == 'O' || c == 'o' || c == '0') ? 2
           : (c == '.' || c == '_' || c == '-') ? 0
           : -1)
        .Take(10)
        .ToArray();

      if (raw.Length != 9)
        return false;

      if (raw.Any(item => item < 0))
        return false;

      result = new TicTacToePosition();

      for (int i = 0; i < raw.Length; ++i)
        result.m_Marks[i] =
            raw[i] == 1 ? Mark.Cross
          : raw[i] == 2 ? Mark.Nought
          : Mark.None;

      return true;
    }

    /// <summary>
    /// Parse
    /// </summary>
    public static TicTacToePosition Parse(string value) => TryParse(value, out var result)
      ? result
      : throw new FormatException("Not a tic-tac-toe postion");

    #endregion Create

    #region Public

    /// <summary>
    /// Empty (Starting) Postion
    /// </summary>
    public static TicTacToePosition Empty { get; } = new TicTacToePosition();

    /// <summary>
    /// Entire game Tree (Breadth First Search)
    /// </summary>
    public static IEnumerable<TicTacToePosition> AllLegalPositions() {
      HashSet<TicTacToePosition> agenda = new HashSet<TicTacToePosition>() { Empty };

      while (agenda.Count > 0) {
        HashSet<TicTacToePosition> next = new HashSet<TicTacToePosition>();

        foreach (var parent in agenda) {
          yield return parent;

          foreach (var child in parent.AvailableMoves())
            next.Add(child);
        }

        agenda = next;
      }
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      return string.Join(Environment.NewLine,
        string.Concat(m_Marks.Skip(0).Take(3).Select(m => m.ToChar())),
        string.Concat(m_Marks.Skip(3).Take(3).Select(m => m.ToChar())),
        string.Concat(m_Marks.Skip(6).Take(3).Select(m => m.ToChar()))
     );
    }

    /// <summary>
    /// Who is On Move 
    /// </summary>
    public Mark WhoIsOnMove {
      get {
        if (Outcome != GameOutcome.None)
          return Mark.None;

        int c = 0;
        int n = 0;

        foreach (var mark in m_Marks)
          if (mark == Mark.Cross)
            c += 1;
          else if (mark == Mark.Nought)
            n += 1;

        if (c == n)
          return Mark.Cross;
        else if (c == n + 1 && c + n < m_Marks.Length)
          return Mark.Nought;
        else
          return Mark.None;
      }
    }

    /// <summary>
    /// Winner (if any)
    /// </summary>
    public Mark Winner {
      get {
        var outcome = Outcome;

        return
            outcome == GameOutcome.FirstWin ? Mark.Cross
          : outcome == GameOutcome.SecondWin ? Mark.Nought
          : Mark.None;
      }
    }

    /// <summary>
    /// Outcome
    /// </summary>
    public GameOutcome Outcome {
      get {
        int cs = 0;
        int ns = 0;

        foreach (var mark in m_Marks)
          if (mark == Mark.Cross)
            cs += 1;
          else if (mark == Mark.Nought)
            ns += 1;

        if (cs != ns && cs != ns + 1)
          return GameOutcome.Illegal;

        bool cWin = Lines.Any(line => line.All(m => m == Mark.Cross));
        bool nWin = Lines.Any(line => line.All(m => m == Mark.Nought));

        if (cWin && nWin)
          return GameOutcome.Illegal;
        else if (cWin)
          return GameOutcome.FirstWin;
        else if (nWin)
          return GameOutcome.SecondWin;
        else if (cs + ns == 9)
          return GameOutcome.Draw;

        return GameOutcome.None;
      }
    }

    /// <summary>
    /// Is Legal
    /// </summary>
    public bool IsLegal => Outcome != GameOutcome.Illegal;

    /// <summary>
    /// Move number (-1 for illegal position), 1-based 
    /// </summary>
    public int MoveNumber {
      get {
        if (Outcome == GameOutcome.Illegal)
          return -1;

        return m_Marks.Count(mark => mark == Mark.Cross) + 1;
      }
    }

    /// <summary>
    /// Board
    /// </summary>
    /// <param name="line">Line</param>
    /// <param name="column">Column</param>
    /// <returns>Mark</returns>
    public Mark this[int line, int column] {
      get => (line < 0 || line > 2 || column < 0 || column > 2)
        ? Mark.None
        : m_Marks[line * 3 + column];
    }

    /// <summary>
    /// Horizontals, Verticals, Diagonals
    /// </summary>
    public IEnumerable<Mark[]> Lines {
      get {
        yield return new Mark[] { m_Marks[0], m_Marks[1], m_Marks[2] };
        yield return new Mark[] { m_Marks[3], m_Marks[4], m_Marks[5] };
        yield return new Mark[] { m_Marks[6], m_Marks[7], m_Marks[8] };

        yield return new Mark[] { m_Marks[0], m_Marks[3], m_Marks[6] };
        yield return new Mark[] { m_Marks[1], m_Marks[4], m_Marks[7] };
        yield return new Mark[] { m_Marks[2], m_Marks[5], m_Marks[8] };

        yield return new Mark[] { m_Marks[0], m_Marks[4], m_Marks[8] };
        yield return new Mark[] { m_Marks[2], m_Marks[4], m_Marks[6] };
      }
    }

    /// <summary>
    /// Available Moves
    /// </summary>
    public IEnumerable<TicTacToePosition> AvailableMoves() {
      Mark mark = WhoIsOnMove;

      if (mark == Mark.None)
        yield break;

      for (int i = 0; i < m_Marks.Length; ++i)
        if (m_Marks[i] == Mark.None) {
          TicTacToePosition result = Clone();

          result.m_Marks[i] = mark;

          yield return result;
        }
    }

    /// <summary>
    /// Parent Positions
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TicTacToePosition> ParentPositions() {
      if (!IsLegal)
        yield break;

      int c = 0;
      int n = 0;

      foreach (var mark in m_Marks)
        if (mark == Mark.Cross)
          c += 1;
        else if (mark == Mark.Nought)
          n += 1;

      if (n + c == 0)
        yield break;

      Mark markToRemove = c > n ? Mark.Cross : Mark.Nought;

      for (int i = 0; i < m_Marks.Length; ++i) {
        if (m_Marks[i] == markToRemove) {
          TicTacToePosition result = Clone();

          result.m_Marks[i] = Mark.None;

          if (result.Outcome == GameOutcome.None)
            yield return result;
        }
      }
    }

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(TicTacToePosition left, TicTacToePosition right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (null == left || null == right)
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(TicTacToePosition left, TicTacToePosition right) {
      if (ReferenceEquals(left, right))
        return false;
      else if (null != left || null != right)
        return false;

      return !left.Equals(right);
    }

    #endregion Operators

    #region IEquatable<TicTacToePosition>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(TicTacToePosition other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      return Enumerable.SequenceEqual(m_Marks, other.m_Marks);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object other) => Equals(other as TicTacToePosition);

    /// <summary>
    /// Get Hash Code
    /// </summary>
    public override int GetHashCode() {
      int result = 0;

      foreach (Mark mark in m_Marks)
        result = result * 4 + (int)mark;

      return result;
    }

    #endregion IEquatable<TicTacToePosition>
  }
}
