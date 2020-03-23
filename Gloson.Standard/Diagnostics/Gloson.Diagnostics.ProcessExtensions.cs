using System;
using System.Diagnostics;

using System.Threading;
using System.Threading.Tasks;

namespace Gloson.Diagnostics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Process Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class ProcessExtensions {
    #region Public

    /// <summary>
    /// WaitForExit Async version
    /// </summary>
    /// <param name="process">Process to wait</param>
    /// <param name="token">Cancellation token</param>
    /// <param name="killOnCancel">Kill on cancellation</param>
    /// <returns>Exit code</returns>
    public static Task<int> WaitForExitAsync(this Process process, CancellationToken token, bool killOnCancel) {
      if (null == process)
        throw new ArgumentNullException(nameof(process));

      TaskCompletionSource<int> source = new TaskCompletionSource<int>();

      process.EnableRaisingEvents = true;

      process.Exited += (sender, e) => {
        if (token.IsCancellationRequested)
          source.TrySetCanceled(token);
        else
          source.TrySetResult(process.ExitCode);
      };

      if (process.HasExited)
        source.TrySetResult(process.ExitCode);

      if (token != CancellationToken.None)
        token.Register(() => {
          try {
            if (killOnCancel && !process.HasExited)
              process.Kill();
          }
          catch (InvalidOperationException) {
            ;
          }

          source.TrySetCanceled(token);
        });

      return source.Task;
    }

    /// <summary>
    /// WaitForExit Async version
    /// </summary>
    /// <param name="process">Process to wait</param>
    /// <returns>Exit code</returns>
    public static Task<int> WaitForExitAsync(this Process process) {
      return WaitForExitAsync(process, CancellationToken.None, true);
    }

    #endregion Public
  }

}
