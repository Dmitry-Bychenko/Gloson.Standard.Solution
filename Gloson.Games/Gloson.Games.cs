namespace Gloson.Games {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Game outcome
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum GameOutcome {
    None = 0,
    FirstWin = 1,
    SecondWin = 2,
    Draw = 3,
    Illegal = 4,
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Game Outcome Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class GameOutcomeExtensions {
    #region Public

    /// <summary>
    /// Choose the best for the 1st 
    /// </summary>
    public static GameOutcome BestForFirst(this GameOutcome left, GameOutcome right) {
      if (left == right)
        return left;

      if (left == GameOutcome.FirstWin)
        return left;
      else if (right == GameOutcome.FirstWin)
        return right;
      else if (left == GameOutcome.Draw)
        return left;
      else if (right == GameOutcome.Draw)
        return right;
      else if (left == GameOutcome.None)
        return left;
      else if (right == GameOutcome.None)
        return right;

      return left;
    }

    /// <summary>
    /// Choose the best for the 2nd
    /// </summary>
    public static GameOutcome BestForSecond(this GameOutcome left, GameOutcome right) {
      if (left == right)
        return left;

      if (left == GameOutcome.SecondWin)
        return left;
      else if (right == GameOutcome.SecondWin)
        return right;
      else if (left == GameOutcome.Draw)
        return left;
      else if (right == GameOutcome.Draw)
        return right;
      else if (left == GameOutcome.None)
        return left;
      else if (right == GameOutcome.None)
        return right;

      return left;
    }

    /// <summary>
    /// Reverse
    /// </summary>
    public static GameOutcome Reverse(this GameOutcome value) =>
        value == GameOutcome.FirstWin ? GameOutcome.SecondWin
      : value == GameOutcome.SecondWin ? GameOutcome.FirstWin
      : value;

    #endregion Public
  }
}
