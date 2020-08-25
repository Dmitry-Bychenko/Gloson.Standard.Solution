using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Geometry.Norms {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Distance
  /// </summary>
  /// <seealso cref="https://reference.wolfram.com/language/tutorial/NumericalOperationsOnData.html#11002"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface IDistance {

    /// <summary>
    /// Distance between points
    /// </summary>
    /// <param name="left">left point</param>
    /// <param name="right">right point</param>
    double Distance(IEnumerable<double> left, IEnumerable<double> right);
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Abstract Distance
  /// </summary>
  // 
  //-------------------------------------------------------------------------------------------------------------------

  public abstract class DistanceAbstract : IDistance {
    #region Algorithm

    private IEnumerable<(double x, double y)> CoreData(IEnumerable<double> left, IEnumerable<double> right) {
      using var eLeft = left.GetEnumerator();
      using var eRight = right.GetEnumerator();

      while (true) {
        if (eLeft.MoveNext())
          if (eRight.MoveNext())
            yield return (eLeft.Current, eRight.Current);
          else
            throw new ArgumentOutOfRangeException(nameof(right), "right dimension is less than left");
        else if (eRight.MoveNext())
          throw new ArgumentOutOfRangeException(nameof(right), "left dimension is less than right");
        else
          break;
      }
    }

    /// <summary>
    /// Distance
    /// </summary>
    protected abstract double CoreDistance(IEnumerable<(double x, double y)> points);

    #endregion Algorithm

    #region IDistance

    /// <summary>
    /// Distance computation
    /// </summary>
    /// <param name="left">Left point</param>
    /// <param name="right">Right Point</param>
    /// <returns>Distance</returns>
    public double Distance(IEnumerable<double> left, IEnumerable<double> right) {
      if (null == left)
        throw new ArgumentNullException(nameof(left));
      else if (null == right)
        throw new ArgumentNullException(nameof(right));

      return CoreDistance(CoreData(left, right));
    }

    #endregion IDistance
  }

}
