using System;
using System.Windows.Input;

namespace Gloson.Core.Wpf.Windows.Input {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// WaitCursor
  /// </summary>
  /// <example>
  /// <code>
  /// using (new WaitCursor()) {
  ///   Long operations here;
  /// }
  /// </code>
  /// </example>
  //
  //-------------------------------------------------------------------------------------------------------------------
  public sealed class WaitCursor : IDisposable {
    #region Private Data

    private readonly Cursor m_SavedCursor;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="waitCursor">Cursor used</param>
    public WaitCursor(Cursor waitCursor) {
      m_SavedCursor = Mouse.OverrideCursor;
      CurrentCursor = waitCursor;

      Mouse.OverrideCursor = waitCursor;
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public WaitCursor() : this(Cursors.Wait) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Current Cursor 
    /// </summary>
    public Cursor CurrentCursor { get; }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => IsDisposed
      ? $"Disposed {GetType().Name} instance"
      : $"{GetType().Name} instance with {CurrentCursor} cursor";

    #endregion Public

    #region IDisposable

    /// <summary>
    /// Is Disposed
    /// </summary>
    public bool IsDisposed { get; private set; }

    private void Dispose(bool disposing) {
      if (disposing) {
        if (!IsDisposed) {
          IsDisposed = true;

          Mouse.OverrideCursor = m_SavedCursor;
        }
      }
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose() => Dispose(true);

    #endregion IDisposable
  }

}
