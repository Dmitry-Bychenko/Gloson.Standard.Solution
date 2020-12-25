using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Aggregator
  /// </summary>
  /// <typeparam name="S"></typeparam>
  /// <typeparam name="V"></typeparam>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class Aggregator<S, V> {
    #region Private Data

    private readonly Func<V, S, int, V> m_UpdateWithIndex;

    private readonly Func<V, S, V> m_Update;

    private readonly bool m_Initialized;

    private V m_Current;

    #endregion Private Data

    #region Algorithm

    protected internal virtual void Next(S item, int index) {
      if (m_Initialized)
        throw new InvalidOperationException("Aggregator is not initialized");

      Index = index;

      if (m_UpdateWithIndex is not null)
        m_Current = m_UpdateWithIndex(Current, item, index);
      else if (m_Update is not null)
        m_Current = m_Update(Current, item);
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Default Constructor
    /// </summary>
    protected Aggregator() { }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public Aggregator(V seed, Func<V, S, int, V> update) {
      Index = -1;
      m_Current = seed;
      m_UpdateWithIndex = update ?? throw new ArgumentNullException(nameof(update));
      m_Initialized = true;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public Aggregator(V seed, Func<V, S, V> update) {
      Index = -1;
      m_Current = seed;
      m_Update = update ?? throw new ArgumentNullException(nameof(update));
      m_Initialized = true;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Current
    /// </summary>
    public V Current {
      get {
        if (m_Initialized)
          return m_Current;

        throw new InvalidOperationException($"{nameof(Current)} is not initialized.");
      }
      protected set {
        m_Current = value;
      }
    }

    /// <summary>
    /// Is Initialized
    /// </summary>
    public bool IsInitialized {
      get;
      protected set;
    }

    /// <summary>
    /// Index
    /// </summary>
    public int Index { get; private set; }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Aggregator
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class Aggregator<V> : Aggregator<V, V> {
    #region Algorithm

    protected internal override void Next(V item, int index) {
      if (!IsInitialized) {
        IsInitialized = true;

        Current = item;
      }
      else
        base.Next(item, index);
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public Aggregator(V seed, Func<V, V, int, V> update)
      : base(seed, update) {
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public Aggregator(V seed, Func<V, V, V> update)
      : base(seed, update) {
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public Aggregator(Func<V, V, int, V> update)
      : base(default, update) {

      IsInitialized = false;
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public Aggregator(Func<V, V, V> update)
      : base(default, update) {

      IsInitialized = false;
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Many Aggregations in one go
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Public

    /// <summary>
    /// Compute several aggregators
    /// </summary>
    public static Aggregator<T, V>[] AggregateMany<T, V>(
      this IEnumerable<T> source,
      params Aggregator<T, V>[] aggregators) {

      if (source is null)
        throw new ArgumentNullException(nameof(source));
      else if (aggregators is null)
        throw new ArgumentNullException(nameof(aggregators));
      else if (aggregators.Any(item => item is null))
        throw new ArgumentNullException(nameof(aggregators), $"{nameof(aggregators)}'s items must not be null.");

      int index = 0;

      foreach (T item in source) {
        foreach (var agg in aggregators)
          agg.Next(item, index);

        index += 1;
      }

      return aggregators;
    }

    /// <summary>
    /// Compute several aggregators
    /// </summary>
    public static Aggregator<T>[] AggregateMany<T>(
      this IEnumerable<T> source,
      params Aggregator<T>[] aggregators) {

      if (source is null)
        throw new ArgumentNullException(nameof(source));
      else if (aggregators is null)
        throw new ArgumentNullException(nameof(aggregators));
      else if (aggregators.Any(item => item is null))
        throw new ArgumentNullException(nameof(aggregators), $"{nameof(aggregators)}'s items must not be null.");

      int index = 0;

      foreach (T item in source) {
        foreach (var agg in aggregators)
          agg.Next(item, index);

        index += 1;
      }

      return aggregators;
    }

    #endregion Public
  }
}
