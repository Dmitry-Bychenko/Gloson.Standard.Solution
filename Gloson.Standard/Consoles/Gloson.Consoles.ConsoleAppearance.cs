using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Consoles {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Console Appearance
  /// </summary>
  /// <example>
  /// <code>
  /// using (new ConsoleAppearance() {ForegroundColor = ConsoleColor.Red}) {
  ///   Console.WriteLine("Warning (RED alert)!");
  /// }
  /// </code>
  /// </example>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ConsoleAppearance : IDisposable {
    #region Private Data

    readonly private ConsoleColor m_SavedBackgroundColor;
    readonly private ConsoleColor m_SavedForegroundColor;
    readonly private int m_SavedCursorSize;
    readonly private bool m_SavedCursorVisible;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standrad constructor
    /// </summary>
    public ConsoleAppearance() {
      m_SavedBackgroundColor = Console.BackgroundColor;
      m_SavedForegroundColor = Console.ForegroundColor;
      m_SavedCursorSize = Console.CursorSize;
      m_SavedCursorVisible = Console.CursorVisible;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Background Color
    /// </summary>
    public ConsoleColor SavedBackgroundColor {
      get => Console.BackgroundColor;
      set => Console.BackgroundColor = value;
    }

    /// <summary>
    /// Foreground Color
    /// </summary>
    public ConsoleColor ForegroundColor {
      get => Console.ForegroundColor;
      set => Console.ForegroundColor = value;
    }

    /// <summary>
    /// Cursor Size
    /// </summary>
    public int CursorSize {
      get => Console.CursorSize;
      set => Console.CursorSize = value;
    }

    /// <summary>
    /// Is Cursor Visible
    /// </summary>
    public bool CursorVisible {
      get => Console.CursorVisible;
      set => Console.CursorVisible = value;
    }

    #endregion Public

    #region IDisposable

    /// <summary>
    /// Is instance disposed
    /// </summary>
    public bool IsDisposed { get; private set; } = false;

    private void Dispose(bool disposing) {
      if (IsDisposed)
        return;

      IsDisposed = true;

      if (disposing) {
        Console.BackgroundColor = m_SavedBackgroundColor;
        Console.ForegroundColor = m_SavedForegroundColor;
        Console.CursorSize = m_SavedCursorSize;
        Console.CursorVisible = m_SavedCursorVisible;
      }
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose() {
      Dispose(true);
    }

    #endregion IDisposable
  }
}
