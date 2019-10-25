using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Gloson.Threading.Tasks {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Async Eager
  /// </summary>
  /// <example>
  /// <code language=c#>
  /// private LazyAsync<int> data = new LazyAsync<int>(() => ...);
  /// ...
  /// int result = await data;
  /// </code>
  /// </example>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class EagerAsync<T> {
    #region Private Data

    private readonly Lazy<Task<T>> m_Instance;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public EagerAsync(Func<T> factory, CancellationToken token) {
      m_Instance = new Lazy<Task<T>>(() => Task.Run(factory, token));

      var _value = m_Instance.Value;
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public EagerAsync(Func<T> factory)
      : this(factory, CancellationToken.None) { }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public EagerAsync(Func<Task<T>> factory, CancellationToken token) {
      m_Instance = new Lazy<Task<T>>(() => Task.Run(factory, token));

      var _value = m_Instance.Value;
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public EagerAsync(Func<Task<T>> factory)
      : this(factory, CancellationToken.None) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Awaiter
    /// </summary>
    /// <returns></returns>
    public TaskAwaiter<T> GetAwaiter() {
      return m_Instance.Value.GetAwaiter();
    }

    #endregion Public
  }

}
