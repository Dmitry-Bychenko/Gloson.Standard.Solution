using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gloson.Threading.Tasks {
  
  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Task Execution
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class TaskExecution {
    #region Public

    /// <summary>
    /// Run All Task in some sequence
    /// </summary>
    public static async IAsyncEnumerable<Task<T>> RunAll<T>(IEnumerable<Task<T>> source) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      for (var hs = new HashSet<Task<T>>(source); hs.Count > 0;) {
        var completed = await Task.WhenAny(hs).ConfigureAwait(false);

        hs.Remove(completed);

        yield return completed;
      }
    }

    #endregion Public
  }

}
