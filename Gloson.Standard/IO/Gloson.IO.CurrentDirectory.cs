using System;

namespace Gloson.IO {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Current Directory (temporary)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class CurrentDirectory : IDisposable {
    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="currentDirectory"></param>
    public CurrentDirectory(string currentDirectory) {
      PreviousDefaultDirectory = Environment.CurrentDirectory;

      try {
        CurrentDefaultDirectory = currentDirectory;
        Environment.CurrentDirectory = CurrentDefaultDirectory;
      }
      catch {
        CurrentDefaultDirectory = PreviousDefaultDirectory;

        throw;
      }
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Previous Current Directory
    /// </summary>
    public string PreviousDefaultDirectory { get; }

    /// <summary>
    /// Current Directory
    /// </summary>
    public string CurrentDefaultDirectory { get; }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() =>
      $"Current: {CurrentDefaultDirectory} Saved: ({PreviousDefaultDirectory})";

    #endregion Public

    #region IDisposable

    /// <summary>
    /// Is Disposed
    /// </summary>
    public bool IsDisposed { get; private set; }

    // Dispose
    private void Dispose(bool disposing) {
      if (disposing) {
        if (IsDisposed)
          return;

        if (string.Equals(Environment.CurrentDirectory, CurrentDefaultDirectory, StringComparison.OrdinalIgnoreCase)) {
          Environment.CurrentDirectory = PreviousDefaultDirectory;

          IsDisposed = true;
        }
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
