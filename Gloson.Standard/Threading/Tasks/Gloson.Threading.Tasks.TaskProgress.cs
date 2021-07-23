using System;
using System.Diagnostics;

namespace Gloson.Threading.Tasks {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Task Progress
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class TaskProgress<T> : IProgress<T> {
    #region Private Data

    private readonly Action<TaskProgress<T>> m_Action;

    private readonly Stopwatch m_Stopwatch = new ();

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public TaskProgress(long count, Action<TaskProgress<T>> action) {
      Count = count < 0 ? -1 : count;
      m_Action = action;
      StartAt = DateTime.Now;

      m_Stopwatch.Start();
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public TaskProgress(Action<TaskProgress<T>> action) {
      m_Action = action;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public TaskProgress(long count) {
      Count = count < 0 ? -1 : count;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public TaskProgress() {
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Current completed transaction 
    /// </summary>
    public T Current { get; private set; }

    /// <summary>
    /// Current completed index
    /// </summary>
    public long CurrentIndex { get; private set; } = 0;

    /// <summary>
    /// Total transactions to complete
    /// </summary>
    public long Count { get; private set; } = -1;

    /// <summary>
    /// Predictable
    /// </summary>
    public bool IsPredictable => Count > 0 && CurrentIndex > 0;

    /// <summary>
    /// Start At
    /// </summary>
    public DateTime StartAt { get; }

    /// <summary>
    /// Completed Fraction, in [0..1] range
    /// </summary>
    public double CompletedFraction => Count <= 0 ? 0 : (double)CurrentIndex / Count;

    /// <summary>
    /// Expected Stop At
    /// </summary>
    public DateTime ExpectedStopAt {
      get {
        if (Count <= 0 || CurrentIndex <= 0)
          return DateTime.MaxValue;

        long ticks = (long)(m_Stopwatch.ElapsedTicks / CompletedFraction + 0.5);

        return StartAt.AddTicks(ticks);
      }
    }

    /// <summary>
    /// Duration
    /// </summary>
    public TimeSpan Duration => m_Stopwatch.Elapsed;

    /// <summary>
    /// Time To Complete
    /// </summary>
    public TimeSpan TimeToComplete {
      get {
        if (Count <= 0 || CurrentIndex <= 0)
          return TimeSpan.MaxValue;

        long ticks = (long)(m_Stopwatch.ElapsedTicks / CompletedFraction + 0.5);

        return new TimeSpan(ticks);
      }
    }

    /// <summary>
    /// Speed in transactions per second
    /// </summary>
    public double Speed {
      get {
        if (CurrentIndex <= 0)
          return 0.0;

        return CurrentIndex / m_Stopwatch.ElapsedMilliseconds * 1000.0;
      }
    }

    /// <summary>
    /// Is Completed
    /// </summary>
    public bool IsCompleted => Count > 0 && CurrentIndex == Count;

    /// <summary>
    /// Progressed
    /// </summary>
    public event EventHandler Progressed;

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      if (Count <= 0) {
        if (CurrentIndex <= 0)
          return "Started";
        else
          return $"{CurrentIndex} ({Current})";
      }
      else {
        if (CurrentIndex <= 0)
          return $"Started {Count} tasks";
        else
          return $"{CurrentIndex} from {Count} ({Current})";
      }
    }

    #endregion Public

    #region IProgress<T>

    /// <summary>
    /// Report about progress
    /// </summary>
    void IProgress<T>.Report(T value) {
      Current = value;

      CurrentIndex += 1;

      if (CurrentIndex > Count && Count < 0)
        Count = CurrentIndex;

      EventHandler progressed = Progressed;

      if (m_Action is not null)
        m_Action(this);

      if (progressed is not null)
        progressed.Invoke(this, EventArgs.Empty);
    }

    #endregion IProgress<T>
  }

}
