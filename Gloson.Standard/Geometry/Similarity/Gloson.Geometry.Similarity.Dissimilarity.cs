using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Geometry.Similarity {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Dissimilarity
  /// </summary>
  /// <see cref="https://reference.wolfram.com/language/tutorial/NumericalOperationsOnData.html#11002"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface IDissimilarity {
    /// <summary>
    /// Dissimilarity
    /// </summary>
    double Dissimilarity(IEnumerable<bool> left, IEnumerable<bool> right);
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Dissimilarity Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class DissimilarityExtensions {
    /// <summary>
    /// Dissimilarity
    /// </summary>
    public static double Dissimilarity(this IDissimilarity rules, IEnumerable<int> left, IEnumerable<int> right) {
      if (rules is null)
        throw new ArgumentNullException(nameof(rules));
      else if (left is null)
        throw new ArgumentNullException(nameof(left));
      else if (right is null)
        throw new ArgumentNullException(nameof(right));

      return rules.Dissimilarity(left.Select(x => x != 0), right.Select(x => x != 0));
    }
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Base Dissimilarity
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public abstract class BaseDissimilarity : IDissimilarity {
    #region Algorithm

    /// <summary>
    /// Dissimlarity Computation 
    /// </summary>
    protected abstract double CoreDissimilarity(List<bool> left, List<bool> right, int n00, int n01, int n10, int n11);

    #endregion Algorithm

    #region IDissimilarity

    /// <summary>
    /// Dissimilarity
    /// </summary>
    public double Dissimilarity(IEnumerable<bool> left, IEnumerable<bool> right) {
      if (left is null)
        throw new ArgumentNullException(nameof(left));
      else if (left is null)
        throw new ArgumentNullException(nameof(left));

      List<bool> listLeft = new List<bool>();
      List<bool> listRight = new List<bool>();

      using var enLeft = left.GetEnumerator();
      using var enRight = right.GetEnumerator();

      int n00 = 0;
      int n01 = 0;
      int n10 = 0;
      int n11 = 0;

      while (true) {
        if (!enLeft.MoveNext()) {
          if (!enRight.MoveNext())
            break;

          throw new ArgumentException("left is too long", nameof(left));
        }
        else if (!enRight.MoveNext())
          throw new ArgumentException("right is too long", nameof(right));

        listLeft.Add(enLeft.Current);
        listRight.Add(enRight.Current);

        if (enLeft.Current)
          if (enRight.Current)
            n11 += 1;
          else
            n10 += 1;
        else if (enRight.Current)
          n01 += 1;
        else
          n00 += 1;
      }

      return CoreDissimilarity(listLeft, listRight, n00, n01, n10, n11);
    }

    #endregion IDissimilarity
  }

}
