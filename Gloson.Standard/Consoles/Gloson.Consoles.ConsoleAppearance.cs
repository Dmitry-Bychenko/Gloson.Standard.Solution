using System;

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

    private readonly ConsoleColor m_SavedBackgroundColor;
    private readonly ConsoleColor m_SavedForegroundColor;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public ConsoleAppearance() {
      m_SavedBackgroundColor = Console.BackgroundColor;
      m_SavedForegroundColor = Console.ForegroundColor;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Background Color
    /// </summary>
    public static ConsoleColor SavedBackgroundColor {
      get => Console.BackgroundColor;
      set => Console.BackgroundColor = value;
    }

    /// <summary>
    /// Foreground Color
    /// </summary>
    public static ConsoleColor ForegroundColor {
      get => Console.ForegroundColor;
      set => Console.ForegroundColor = value;
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
