using System;
using System.Collections.Generic;

namespace Gloson.Numerics {

  #region Abstract 

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Data Smoothing
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface IDataSequenceSmoother {
    /// <summary>
    /// Smoothing Data
    /// </summary>
    public IEnumerable<double> Smooth(IEnumerable<double> source);
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Data Smoothing (abstract class)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public abstract class BaseDataSequenceSmoother : IDataSequenceSmoother {
    #region Algorithm

    protected abstract IEnumerable<double> CoreSmooth(IEnumerable<double> source);

    #endregion Algorithm

    #region IDataSmoothing

    /// <summary>
    /// Smooth
    /// </summary>
    public IEnumerable<double> Smooth(IEnumerable<double> source) =>
      source is not null ? CoreSmooth(source) : throw new ArgumentNullException(nameof(source));

    #endregion IDataSmoothing
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Data Sequence Smoothing 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class DataSequenceSmoothing {
    #region Public

    /// <summary>
    /// Smooth
    /// </summary>
    public static IEnumerable<double> Smooth(this IEnumerable<double> source, IDataSequenceSmoother smoother) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      if (smoother is null)
        smoother = new SmootherNone();

      foreach (var item in smoother.Smooth(source))
        yield return item;
    }

    #endregion Public
  }

  #endregion Abstract

  #region Library

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Smoother None
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class SmootherNone
    : BaseDataSequenceSmoother,
      IEquatable<SmootherNone> {

    #region Algorithm

    /// <summary>
    /// Core Smooth
    /// </summary>
    protected override IEnumerable<double> CoreSmooth(IEnumerable<double> source) => source;

    #endregion Algorithm

    #region Public

    /// <summary>
    /// To String 
    /// </summary>
    public override string ToString() => $"Do nothing smoothing";

    #endregion Public

    #region IEquatable<SmootherNone>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(SmootherNone other) => other is not null;

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as SmootherNone);

    /// <summary>
    /// Get Hash Code
    /// </summary>
    public override int GetHashCode() => 0;

    #endregion IEquatable<SmootherNone>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Simple Moving Average
  /// </summary>
  /// <see cref="https://en.wikipedia.org/wiki/Moving_average"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class SmootherSimpleMovingAverage
    : BaseDataSequenceSmoother,
      IEquatable<SmootherSimpleMovingAverage> {

    #region Algorithm

    /// <summary>
    /// Core Smooth
    /// </summary>
    protected override IEnumerable<double> CoreSmooth(IEnumerable<double> source) {
      Queue<double> queue = new Queue<double>();

      double sum = 0.0;

      foreach (double x in source) {
        sum += x;
        queue.Enqueue(x);

        if (queue.Count == Window) {
          yield return sum / Window;

          sum -= queue.Dequeue();
        }
      }
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="window">Window width</param>
    public SmootherSimpleMovingAverage(int window) {
      Window = (window > 0)
        ? window
        : throw new ArgumentOutOfRangeException(nameof(window));
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Window
    /// </summary>
    public int Window { get; }

    /// <summary>
    /// Moving Average
    /// </summary>
    public override string ToString() => $"Moving Average Smoothing with {Window} window";

    #endregion Public

    #region IEquatable<SmootherSimpleMovingAverage>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(SmootherSimpleMovingAverage other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (other is null)
        return false;

      return Window == other.Window;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as SmootherSimpleMovingAverage);

    /// <summary>
    /// Get Hash Code
    /// </summary>
    public override int GetHashCode() => Window;

    #endregion IEquatable<SmootherSimpleMovingAverage>
  }

  #endregion Library

}
