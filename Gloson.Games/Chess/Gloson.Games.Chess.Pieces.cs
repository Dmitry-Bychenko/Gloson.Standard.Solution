using System.Collections.Generic;

namespace Gloson.Games.Chess {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Side (to move)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum Side {
    None = 0,
    White = 1,
    Black = 2,
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Base (Abstract) Piece
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public abstract class BasePieceDescription {
    #region Public

    /// <summary>
    /// Symbol
    /// </summary>
    public abstract char Symbol(Side side);

    /// <summary>
    /// Acronym
    /// </summary>
    public abstract string Acronym { get; }

    /// <summary>
    /// Name
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Simple Moves
    /// </summary>
    public abstract IEnumerable<(int y, int x)> Moves((int y, int x) location, Position position);

    #endregion Public
  }

}
