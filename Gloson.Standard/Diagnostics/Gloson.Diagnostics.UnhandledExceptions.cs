using System;

namespace Gloson.Diagnostics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Unhandled Exception Trap
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface IUnhandledExceptionsTrap : IDisposable {
    /// <summary>
    /// On Unhandled Exception
    /// </summary>
    event EventHandler<UnhandledExceptionEventArgs> UnhandledException;
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Unhandled Exception Trap
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  [StartUp]
  public sealed class UnhandledExceptionTrap : IUnhandledExceptionsTrap {
    #region Algorithm

    private void CoreHandler(object sender, UnhandledExceptionEventArgs args) {
      var handler = UnhandledException;

      handler?.Invoke(sender, args);
    }

    #endregion Algorithm

    #region Create

    static UnhandledExceptionTrap() {
      Dependencies.TryRegisterService(typeof(IUnhandledExceptionsTrap), typeof(UnhandledExceptionTrap));
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public UnhandledExceptionTrap() {
      AppDomain.CurrentDomain.UnhandledException += CoreHandler;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// On Unhandled Exception
    /// </summary>
    public event EventHandler<UnhandledExceptionEventArgs> UnhandledException;

    #endregion Public

    #region IDisposable

    /// <summary>
    /// Is Disposed
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Dispose
    /// </summary>
    /// <param name="disposing"></param>
    private void Dispose(bool disposing) {
      if (disposing) {
        if (IsDisposed) {
          AppDomain.CurrentDomain.UnhandledException -= CoreHandler;

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
