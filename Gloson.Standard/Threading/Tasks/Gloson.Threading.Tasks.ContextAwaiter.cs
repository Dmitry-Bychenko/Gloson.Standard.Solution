using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Gloson.Threading.Tasks {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Await Current Context (UI Context)
  /// </summary>
  /// <example>
  /// <code>
  /// var uiContext = new ContextAwaiter();
  /// ...
  /// // Note .ConfigureAwait(false);
  /// await Task.Delay(100).ConfigureAwait(false);
  /// ...
  /// await uiContext;
  /// myTextBox.Text = "awaited";
  /// </code>
  /// </example>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ContextAwaiter : INotifyCompletion {
    #region Private Data

    private static readonly SendOrPostCallback m_PostCallback = state => {
      if (state is Action action)
        action();
      else
        throw new InvalidOperationException($"{nameof(state)} must be an Action");
    };

    #endregion Private Data

    #region Create

    /// <summary>
    /// Constructor from Context
    /// </summary>
    public ContextAwaiter(SynchronizationContext context) {
      Context = context;
    }

    /// <summary>
    /// Constructor from Current context
    /// </summary>
    public ContextAwaiter() : this(SynchronizationContext.Current) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Synchronization Context
    /// </summary>
    public SynchronizationContext Context { get; }

    /// <summary>
    /// When considered completed
    /// </summary>
    public bool IsCompleted => Context == SynchronizationContext.Current;

    /// <summary>
    /// Operation to do when not completed 
    /// </summary>
    public void OnCompleted(Action continuation) => Context.Post(m_PostCallback, continuation);

    /// <summary>
    /// Result (if any) after completion
    /// </summary>
//#pragma warning disable CA1822 // Can't be static for to be a correct Awaiter
    public void GetResult() { }
    //#pragma warning restore CA1822 // Can't be static for to be a correct Awaiter

    /// <summary>
    /// Awaiter
    /// </summary>
    public ContextAwaiter GetAwaiter() => this;

    #endregion Public

    #region Operators

    /// <summary>
    /// From Context
    /// </summary>
    /// <param name="context"></param>
    public static implicit operator ContextAwaiter(SynchronizationContext context) => new(context);

    #endregion Operators
  }

}
