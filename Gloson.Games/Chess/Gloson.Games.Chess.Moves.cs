using System;
using System.Collections.Generic;

namespace Gloson.Games.Chess {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Field State 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum ChessFieldState {
    /// <summary>
    /// Invalid field
    /// </summary>
    Invalid = -1,

    /// <summary>
    /// Empty Field
    /// </summary>
    Empty = 0,

    /// <summary>
    /// White Piece
    /// </summary>
    WhitePiece = 1,

    /// <summary>
    /// Black Piece 
    /// </summary>
    BlackPiece = 2,
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Field State Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class ChessFieldStateExtensions {
    #region Public

    /// <summary>
    /// Opposite Color
    /// </summary>
    public static ChessFieldState Opposite(this ChessFieldState value) {
      if (value == ChessFieldState.BlackPiece)
        return ChessFieldState.WhitePiece;
      else if (value == ChessFieldState.WhitePiece)
        return ChessFieldState.BlackPiece;
      else
        return ChessFieldState.Invalid;
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Moves
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class ChessMoves {
    #region Public

    /// <summary>
    /// Rook Moves (except castling)
    /// </summary>
    public static IEnumerable<(int rank, int file)> Rook(
      (int rank, int file) position,
       ChessFieldState piece,
       Func<(int rank, int file), ChessFieldState> pieces) {

      if (piece != ChessFieldState.BlackPiece && piece != ChessFieldState.WhitePiece)
        throw new ArgumentOutOfRangeException(nameof(piece));

      if (pieces is null)
        throw new ArgumentNullException(nameof(pieces));

      for ((int rank, int file) at = (position.rank, position.file - 1); ; at = (at.rank, at.file - 1)) {
        ChessFieldState field = pieces(at);

        if (field == ChessFieldState.Empty)
          yield return at;
        else {
          if (field.Opposite() == piece)
            yield return at;

          break;
        }
      }

      for ((int rank, int file) at = (position.rank, position.file + 1); ; at = (at.rank, at.file + 1)) {
        ChessFieldState field = pieces(at);

        if (field == ChessFieldState.Empty)
          yield return at;
        else {
          if (field.Opposite() == piece)
            yield return at;

          break;
        }
      }

      for ((int rank, int file) at = (position.rank - 1, position.file); ; at = (at.rank - 1, at.file)) {
        ChessFieldState field = pieces(at);

        if (field == ChessFieldState.Empty)
          yield return at;
        else {
          if (field.Opposite() == piece)
            yield return at;

          break;
        }
      }

      for ((int rank, int file) at = (position.rank + 1, position.file); ; at = (at.rank + 1, at.file)) {
        ChessFieldState field = pieces(at);

        if (field == ChessFieldState.Empty)
          yield return at;
        else {
          if (field.Opposite() == piece)
            yield return at;

          break;
        }
      }
    }

    /// <summary>
    /// Bishop Moves
    /// </summary>
    public static IEnumerable<(int rank, int file)> Bishop(
      (int rank, int file) position,
       ChessFieldState piece,
       Func<(int rank, int file), ChessFieldState> pieces) {

      if (piece != ChessFieldState.BlackPiece && piece != ChessFieldState.WhitePiece)
        throw new ArgumentOutOfRangeException(nameof(piece));

      if (pieces is null)
        throw new ArgumentNullException(nameof(pieces));

      for ((int rank, int file) at = (position.rank - 1, position.file - 1); ; at = (at.rank - 1, at.file - 1)) {
        ChessFieldState field = pieces(at);

        if (field == ChessFieldState.Empty)
          yield return at;
        else {
          if (field.Opposite() == piece)
            yield return at;

          break;
        }
      }

      for ((int rank, int file) at = (position.rank - 1, position.file + 1); ; at = (at.rank - 1, at.file + 1)) {
        ChessFieldState field = pieces(at);

        if (field == ChessFieldState.Empty)
          yield return at;
        else {
          if (field.Opposite() == piece)
            yield return at;

          break;
        }
      }

      for ((int rank, int file) at = (position.rank + 1, position.file - 1); ; at = (at.rank + 1, at.file - 1)) {
        ChessFieldState field = pieces(at);

        if (field == ChessFieldState.Empty)
          yield return at;
        else {
          if (field.Opposite() == piece)
            yield return at;

          break;
        }
      }

      for ((int rank, int file) at = (position.rank + 1, position.file + 1); ; at = (at.rank + 1, at.file + 1)) {
        ChessFieldState field = pieces(at);

        if (field == ChessFieldState.Empty)
          yield return at;
        else {
          if (field.Opposite() == piece)
            yield return at;

          break;
        }
      }
    }

    /// <summary>
    /// Queen Moves
    /// </summary>
    public static IEnumerable<(int rank, int file)> Queen(
      (int rank, int file) position,
       ChessFieldState piece,
       Func<(int rank, int file), ChessFieldState> pieces) {

      if (piece != ChessFieldState.BlackPiece && piece != ChessFieldState.WhitePiece)
        throw new ArgumentOutOfRangeException(nameof(piece));

      if (pieces is null)
        throw new ArgumentNullException(nameof(pieces));

      foreach (var move in Rook(position, piece, pieces))
        yield return move;

      foreach (var move in Bishop(position, piece, pieces))
        yield return move;
    }

    /// <summary>
    /// King Moves (without Casting)
    /// </summary>
    public static IEnumerable<(int rank, int file)> King(
      (int rank, int file) position,
       ChessFieldState piece,
       Func<(int rank, int file), ChessFieldState> pieces) {

      if (piece != ChessFieldState.BlackPiece && piece != ChessFieldState.WhitePiece)
        throw new ArgumentOutOfRangeException(nameof(piece));

      if (pieces is null)
        throw new ArgumentNullException(nameof(pieces));

      for (int rank = position.rank - 1; rank <= position.rank + 1; ++rank)
        for (int file = position.file - 1; file <= position.file + 1; ++file) {
          var move = (rank, file);

          if (move != position) {
            ChessFieldState field = pieces(move);

            if (field == ChessFieldState.Empty || field.Opposite() == piece)
              yield return move;
          }
        }
    }

    /// <summary>
    /// Knight moves
    /// </summary>
    public static IEnumerable<(int rank, int file)> Knight(
      (int rank, int file) position,
       ChessFieldState piece,
       Func<(int rank, int file), ChessFieldState> pieces) {

      if (piece != ChessFieldState.BlackPiece && piece != ChessFieldState.WhitePiece)
        throw new ArgumentOutOfRangeException(nameof(piece));

      if (pieces is null)
        throw new ArgumentNullException(nameof(pieces));

      for (int y = -2; y <= 2; ++y)
        for (int x = -2; x <= 2; ++x) {
          if (x == 0 || y == 0)
            continue;

          if (Math.Abs(y % 2) + Math.Abs(x % 2) != 1)
            continue;

          var move = (position.rank + y, position.file + x);

          ChessFieldState field = pieces(move);

          if (field == ChessFieldState.Empty || field.Opposite() == piece)
            yield return move;
        }
    }

    /// <summary>
    /// Pawn moves
    /// </summary>
    public static IEnumerable<(int rank, int file)> Pawn(
      (int rank, int file) position,
       ChessFieldState piece,
       Func<(int rank, int file), ChessFieldState> pieces) {

      if (piece != ChessFieldState.BlackPiece && piece != ChessFieldState.WhitePiece)
        throw new ArgumentOutOfRangeException(nameof(piece));

      if (pieces is null)
        throw new ArgumentNullException(nameof(pieces));

      bool doubleMove = false;
      int delta = 1;

      if (piece == ChessFieldState.WhitePiece) {
        doubleMove = (pieces((position.rank - 1, position.file)) != ChessFieldState.Invalid) &&
                     (pieces((position.rank - 2, position.file)) == ChessFieldState.Invalid);

        delta = 1;
      }
      else if (piece == ChessFieldState.BlackPiece) {
        doubleMove = (pieces((position.rank + 1, position.file)) != ChessFieldState.Invalid) &&
                     (pieces((position.rank + 2, position.file)) == ChessFieldState.Invalid);

        delta = -1;
      }

      if (pieces((position.rank + delta, position.file)) == ChessFieldState.Empty) {
        yield return (position.rank + delta, position.file);

        if (doubleMove && pieces((position.rank + 2 * delta, position.file)) == ChessFieldState.Empty)
          yield return (position.rank + delta, position.file);
      }

      if (pieces((position.rank + delta, position.file - 1)) == piece.Opposite())
        yield return (position.rank + delta, position.file - 1);

      if (pieces((position.rank + delta, position.file + 1)) == piece.Opposite())
        yield return (position.rank + delta, position.file + 1);
    }

    #endregion Public
  }
}
