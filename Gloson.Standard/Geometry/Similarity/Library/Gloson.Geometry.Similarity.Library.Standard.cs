using System.Collections.Generic;

namespace Gloson.Geometry.Similarity.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Matching Dissimilarity
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class MatchingDissimilarity : BaseDissimilarity {
    /// <summary>
    /// Dissimilarity Computation 
    /// </summary>
    protected override double CoreDissimilarity(List<bool> left, List<bool> right, int n00, int n01, int n10, int n11) =>
      (n10 + n01) / (double)left.Count;
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Jaccard Dissimilarity
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class JaccardDissimilarity : BaseDissimilarity {
    /// <summary>
    /// Dissimilarity Computation 
    /// </summary>
    protected override double CoreDissimilarity(List<bool> left, List<bool> right, int n00, int n01, int n10, int n11) =>
      (n01 + n10) / (double)(n01 + n10 + n11);
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Russell And Rao Dissimilarity
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class RussellRaoDissimilarity : BaseDissimilarity {
    /// <summary>
    /// Dissimilarity Computation 
    /// </summary>
    protected override double CoreDissimilarity(List<bool> left, List<bool> right, int n00, int n01, int n10, int n11) =>
      (n00 + n10 + n01) / (double)left.Count;
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Sokal And Sneath Dissimilarity
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class SokalSneathDissimilarity : BaseDissimilarity {
    /// <summary>
    /// Dissimilarity Computation 
    /// </summary>
    protected override double CoreDissimilarity(List<bool> left, List<bool> right, int n00, int n01, int n10, int n11) =>
      2.0 * (n01 + n10) / (n11 + 2.0 * (n01 + n10));
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Rogers and Tanimoto Dissimilarity
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class RogersTanimotoDissimilarity : BaseDissimilarity {
    /// <summary>
    /// Dissimilarity Computation 
    /// </summary>
    protected override double CoreDissimilarity(List<bool> left, List<bool> right, int n00, int n01, int n10, int n11) =>
      2.0 * (n10 + n01) / (n00 + n11 + 2.0 * (n10 + n01));
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Dice Dissimilarity
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class DiceDissimilarity : BaseDissimilarity {
    /// <summary>
    /// Dissimilarity Computation 
    /// </summary>
    protected override double CoreDissimilarity(List<bool> left, List<bool> right, int n00, int n01, int n10, int n11) =>
      (n10 + n01) / (2.0 * n11 + n10 + n01);
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Yules Dissimilarity
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class YuleDissimilarity : BaseDissimilarity {
    /// <summary>
    /// Dissimilarity Computation 
    /// </summary>
    protected override double CoreDissimilarity(List<bool> left, List<bool> right, int n00, int n01, int n10, int n11) =>
      2.0 * n10 * n01 / (n11 * n00 + n10 * n01);
  }

}
