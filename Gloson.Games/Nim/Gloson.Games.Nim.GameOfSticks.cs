using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Games.Nim {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Generalized 3, 5, 7 sticks game 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class GameOfSticks : IEquatable<GameOfSticks> {
    #region Internal classes

    private class SequenceComparer : IEqualityComparer<List<int>> {
      public bool Equals(List<int> x, List<int> y) => Enumerable.SequenceEqual(x, y);

      public int GetHashCode(List<int> obj) => obj.Count;
    }

    #endregion Internal classes

    #region Private Data

    private readonly Dictionary<List<int>, bool> m_Outcomes = new Dictionary<List<int>, bool>(new SequenceComparer());

    #endregion Private Data

    #region Algorithm

    private IEnumerable<List<int>> CoreNext(List<int> current) {
      HashSet<List<int>> completed = new HashSet<List<int>>(new SequenceComparer());

      HashSet<int> chunks = new HashSet<int>();

      for (int i = 0; i < current.Count; ++i) {
        int number = current[i];

        if (!chunks.Add(number))
          continue;

        for (int take = 1; take <= Math.Min(MaxTake, current.Count); ++take) {
          int upTo = number / take + (number % take == 0 ? 0 : 1);

          for (int p = 0; p <= upTo; ++p) {
            int left = p;
            int right = number - p - take;

            if (right < left)
              break;

            List<int> next = current.ToList();

            next.RemoveAt(i);

            if (left > 0)
              next.Add(left);

            if (right > 0)
              next.Add(right);

            next.Sort();

            if (completed.Add(next))
              yield return next;
          }
        }
      }
    }

    private bool CoreIsWin(List<int> current) {
      if (m_Outcomes.TryGetValue(current, out bool knownResult))
        return knownResult;

      if (current.Count <= 0)
        return !IsLastWin;

      if (current.Count == 1) {
        if (IsLastWin)
          return current[0] % (MaxTake + 1) != 0;
        else
          return current[0] % (MaxTake + 1) != 1;
      }

      foreach (var list in CoreNext(current))
        if (!CoreIsWin(list)) {
          m_Outcomes.Add(current, true);

          return true;
        }

      m_Outcomes.Add(current, false);

      return false;
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor 
    /// </summary>
    /// <param name="maxTake">Maximum Number of Sticks Can Be Taken</param>
    /// <param name="lastWins">Is Last Stick a Winning One</param>
    public GameOfSticks(int maxTake, bool lastWins) {
      MaxTake = maxTake > 0
        ? maxTake
        : throw new ArgumentOutOfRangeException(nameof(maxTake));

      IsLastWin = lastWins;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Maximum Stick can be Taken 
    /// </summary>
    public int MaxTake { get; }

    /// <summary>
    /// Is Last Stick a Winning one
    /// </summary>
    public bool IsLastWin { get; }

    /// <summary>
    /// Possible Next Positions
    /// </summary>
    /// <param name="position">Current Position</param>
    public IEnumerable<int[]> Next(IEnumerable<int> position) {
      if (null == position)
        throw new ArgumentNullException(nameof(position));

      List<int> source = new List<int>();

      foreach (int item in position) {
        if (item < 0)
          throw new ArgumentOutOfRangeException(nameof(position), "Negative numbers are not allowed");
        else if (item > 0)
          source.Add(item);
      }

      foreach (var list in CoreNext(source))
        yield return list.ToArray();
    }

    /// <summary>
    /// Is the position a winning one
    /// </summary>
    public bool IsWin(IEnumerable<int> position) {
      if (null == position)
        throw new ArgumentNullException(nameof(position));

      List<int> source = new List<int>();

      foreach (int item in position) {
        if (item < 0)
          throw new ArgumentOutOfRangeException(nameof(position), "Negative numbers are not allowed");
        else if (item > 0)
          source.Add(item);
      }

      source.Sort();

      return CoreIsWin(source);
    }

    /// <summary>
    /// Best Move
    /// </summary>
    public int[] BestMove(IEnumerable<int> position) {
      if (null == position)
        throw new ArgumentNullException(nameof(position));

      List<int> source = new List<int>();

      foreach (int item in position) {
        if (item < 0)
          throw new ArgumentOutOfRangeException(nameof(position), "Negative numbers are not allowed");
        else if (item > 0)
          source.Add(item);
      }

      source.Sort();

      foreach (var move in CoreNext(source))
        if (!CoreIsWin(move))
          return move.ToArray();

      return Array.Empty<int>();
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      return $"Up to {MaxTake} sticks to take; last stick is {(IsLastWin ? "winning" : "losing")} one";
    }

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(GameOfSticks left, GameOfSticks right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (null == left || null == right)
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(GameOfSticks left, GameOfSticks right) {
      if (ReferenceEquals(left, right))
        return false;
      else if (null == left || null == right)
        return true;

      return !left.Equals(right);
    }

    #endregion Operators

    #region IEquatable<GameOfSticks> 

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(GameOfSticks other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      return other.IsLastWin == IsLastWin &&
             other.MaxTake == MaxTake;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as GameOfSticks);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() =>
      (MaxTake ^ ((IsLastWin ? 1 : 0) << 25));

    #endregion IEquatable<GameOfSticks>
  }

}
