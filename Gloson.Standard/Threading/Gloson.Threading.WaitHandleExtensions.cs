using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gloson.Threading {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// WaitHandle Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class WaitHandleExtensions {
    #region Public

    /// <summary>
    /// WaitHandle to Task
    /// </summary>
    /// <param name="handle">Handle to wrap</param>
    /// <param name="timeout">Timeout</param>
    /// <returns>Task representation of the wait handle</returns>
    public static Task AsTask(this WaitHandle handle, TimeSpan timeout) {
      if (null == handle)
        throw new ArgumentNullException(nameof(handle));

      var ts = new TaskCompletionSource<object>();

      var registration = ThreadPool.RegisterWaitForSingleObject(
        handle,
       (state, timedOut) => {
         if (timedOut)
           (state as TaskCompletionSource<object>).TrySetCanceled();
         else
           (state as TaskCompletionSource<object>).TrySetResult(null);
       },
        ts,
        timeout,
        true
      );

      ts.Task.ContinueWith(
        (task, state) => (state as RegisteredWaitHandle).Unregister(handle),
         registration,
         TaskScheduler.Default
      );

      return ts.Task;
    }

    /// <summary>
    /// WaitHandle to Task
    /// </summary>
    /// <param name="handle">Handle to wrap</param>
    public static Task AsTask(this WaitHandle handle) => AsTask(handle, Timeout.InfiniteTimeSpan);

    #endregion Public
  }

}
