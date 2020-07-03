using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Edit Operation Kind
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  [Flags]
  public enum EditOperationKind {
    /// <summary>
    /// None (Keep current item)
    /// </summary>
    None = 0,
    /// <summary>
    /// Insert
    /// </summary>
    Insert = 1,
    /// <summary>
    /// Delete
    /// </summary>
    Delete = 2,
    /// <summary>
    /// Edit (Or Insert + Delete)
    /// </summary>
    Edit = Insert | Delete
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Edit Operation
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class EditOperation<T>
    : IEquatable<EditOperation<T>> {

    #region Algorithm

    internal string ToReport() {
      return Kind switch
      {
        EditOperationKind.None => $"Keep   '{Before}', ({Cost})",
        EditOperationKind.Insert => $"Insert '{After}', ({Cost})",
        EditOperationKind.Delete => $"Delete '{Before}', ({Cost})",
        EditOperationKind.Edit => $"Edit   '{Before}' into '{After}', ({Cost})",
        _ => $"???    '{Before}' into '{After}', ({Cost})",
      };
    }

    #endregion Algorithm

    #region Create

    // Standard constructor
    internal EditOperation(EditOperationKind kind, T before, T after, double cost)
      : base() {

      Kind = kind;
      Before = before;
      After = after;
      Cost = cost;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Kind
    /// </summary>
    public EditOperationKind Kind {
      get;
    }

    /// <summary>
    /// Before
    /// </summary>
    public T Before {
      get;
    }

    /// <summary>
    /// After
    /// </summary>
    public T After {
      get;
    }

    /// <summary>
    /// Cost
    /// </summary>
    public double Cost {
      get;
      private set;
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      return Kind switch
      {
        EditOperationKind.None => $"Keep '{Before}'",
        EditOperationKind.Insert => $"Insert '{After}'",
        EditOperationKind.Delete => $"Delete '{Before}'",
        EditOperationKind.Edit => $"Edit '{Before}' into '{After}'",
        _ => $"Unknown '{Before}' into '{After}'",
      };
    }

    #endregion Public

    #region Operators

    /// <summary>
    /// Equal
    /// </summary>
    public static bool operator ==(EditOperation<T> left, EditOperation<T> right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (null == left || null == right)
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Not Equal
    /// </summary>
    public static bool operator !=(EditOperation<T> left, EditOperation<T> right) {
      if (ReferenceEquals(left, right))
        return false;
      else if (null == left || null == right)
        return true;

      return !left.Equals(right);
    }

    #endregion Operators

    #region IEquatable<EditOperation<T>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(EditOperation<T> other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      return object.Equals(this.Before, other.Before) &&
             object.Equals(this.After, other.After) &&
             this.Kind == other.Kind;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) {
      return Equals(obj as EditOperation<T>);
    }

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() {
      return (Before == null ? 0 : Before.GetHashCode()) ^
             (After == null ? 0 : After.GetHashCode()) ^
             (int)Kind;
    }

    #endregion IEquatable<EditOperation<T>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Edit Cost (prices for insertion, deletion, edit)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface IEditCost<T> {
    /// <summary>
    /// Insertion Price (positive) 
    /// </summary>
    double InsertionPrice(T value);
    /// <summary>
    /// Deletion price (positive) 
    /// </summary>
    double DeletionPrice(T value);
    /// <summary>
    /// Edit price (positive) 
    /// </summary>
    double EditPrice(T from, T to);
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Some standard edit costs
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class EditCosts<T> {
    #region Inner classes

    // Uniform
    private class UniformEditCost : IEditCost<T> {
      public double DeletionPrice(T value) => 1.0;
        
      public double EditPrice(T from, T to) => object.Equals(from, to) ? 0 : 1.0;

      public double InsertionPrice(T value) => 1.0;
    }

    // Double cost for edit
    private class InsertDeleteEditCost : IEditCost<T> {
      public double DeletionPrice(T value) => 1.0;
      
      public double EditPrice(T from, T to) => object.Equals(from, to) ? 0 : 2.0;

      public double InsertionPrice(T value) => 1.0;
    }

    #endregion Inner classes

    #region Public

    /// <summary>
    /// Uniform (deletion == insertion == edit == 1)
    /// </summary>
    public static IEditCost<T> Uniform {
      get;
    } = new UniformEditCost();

    /// <summary>
    /// Standard (edit = 2; deletion == insertion == 1)
    /// </summary>
    public static IEditCost<T> Standard {
      get;
    } = new InsertDeleteEditCost();

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Edit Procedure
  /// </summary>
  // 
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class EditProcedure<T> : IReadOnlyList<EditOperation<T>> {
    #region Private Data

    private List<EditOperation<T>> m_Sequence;

    #endregion Private Data

    #region Algorithm

    private void CorePerform(T[] source,
                             T[] target,
                             Func<T, double> insertCost,
                             Func<T, double> deleteCost,
                             Func<T, T, double> editCost) {
      // Best operation (among insert, update, delete) to perform 
      EditOperationKind[][] M = Enumerable
        .Range(0, source.Length + 1)
        .Select(line => new EditOperationKind[target.Length + 1])
        .ToArray();

      // Minimum cost so far
      double[][] D = Enumerable
        .Range(0, source.Length + 1)
        .Select(line => new double[target.Length + 1])
        .ToArray();

      // Edge: all removes
      double sum = 0.0;

      for (int i = 1; i <= source.Length; ++i) {
        M[i][0] = EditOperationKind.Delete;
        D[i][0] = (sum += deleteCost(source[i - 1]));
      }

      // Edge: all inserts
      sum = 0.0;

      for (int i = 1; i <= target.Length; ++i) {
        M[0][i] = EditOperationKind.Insert;
        D[0][i] = (sum += insertCost(target[i - 1]));
      }

      // Having fit N - 1, K - 1 characters let's fit N, K
      for (int i = 1; i <= source.Length; ++i)
        for (int j = 1; j <= target.Length; ++j) {
          // here we choose the operation with the least cost
          double insert = D[i][j - 1] + insertCost(target[j - 1]);
          double delete = D[i - 1][j] + deleteCost(source[i - 1]);
          double edit = D[i - 1][j - 1] + editCost(source[i - 1], target[j - 1]);

          double min = Math.Min(Math.Min(insert, delete), edit);

          if (min == insert)
            M[i][j] = EditOperationKind.Insert;
          else if (min == delete)
            M[i][j] = EditOperationKind.Delete;
          else if (min == edit)
            M[i][j] = object.Equals(source[i - 1], target[j - 1])
              ? EditOperationKind.None
              : EditOperationKind.Edit;

          D[i][j] = min;
        }

      EditDistance = D[source.Length][target.Length];

      // Backward: knowing scores (D) and actions (M) let's building edit sequence
      m_Sequence =
        new List<EditOperation<T>>(source.Length + target.Length);

      for (int x = target.Length, y = source.Length; (x > 0) || (y > 0);) {
        EditOperationKind op = M[y][x];

        if (op == EditOperationKind.Insert) {
          x -= 1;
          m_Sequence.Add(new EditOperation<T>(op, default, target[x], D[y][x + 1] - D[y][x]));
        }
        else if (op == EditOperationKind.Delete) {
          y -= 1;
          m_Sequence.Add(new EditOperation<T>(op, source[y], default, D[y + 1][x] - D[y][x]));
        }
        else if (op == EditOperationKind.Edit || op == EditOperationKind.None) {
          x -= 1;
          y -= 1;
          m_Sequence.Add(new EditOperation<T>(op, source[y], target[x], D[y + 1][x + 1] - D[y][x]));
        }
        else // Start of the matching (EditOperationKind.None)
          break;
      }

      m_Sequence.Reverse();
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="source">Source sequence</param>
    /// <param name="target">Target Sequence</param>
    /// <param name="insertCost">Insertion cost</param>
    /// <param name="deleteCost">Delete cost</param>
    /// <param name="editCost">Edit cost</param>
    public EditProcedure(IEnumerable<T> source,
                         IEnumerable<T> target,
                         Func<T, double> insertCost,
                         Func<T, double> deleteCost,
                         Func<T, T, double> editCost) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == target)
        throw new ArgumentNullException(nameof(target));
      else if (null == insertCost)
        throw new ArgumentNullException(nameof(insertCost));
      else if (null == deleteCost)
        throw new ArgumentNullException(nameof(deleteCost));
      else if (null == editCost)
        throw new ArgumentNullException(nameof(editCost));

      CorePerform(source.ToArray(),
                  target.ToArray(),
                  insertCost,
                  deleteCost,
                  editCost);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="source">Source sequence</param>
    /// <param name="target">Target Sequence</param>
    /// <param name="price">Prices (insert, delete, edit)</param>
    public EditProcedure(IEnumerable<T> source,
                         IEnumerable<T> target,
                         IEditCost<T> price) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == target)
        throw new ArgumentNullException(nameof(target));
      else if (null == price)
        throw new ArgumentNullException(nameof(price));

      Func<T, double> insertCost = price.InsertionPrice;
      Func<T, double> deleteCost = price.DeletionPrice;
      Func<T, T, double> editCost = price.EditPrice;

      CorePerform(source.ToArray(),
                  target.ToArray(),
                  insertCost,
                  deleteCost,
                  editCost);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Edit Distance
    /// </summary>
    public double EditDistance {
      get;
      private set;
    }

    /// <summary>
    /// Edit Sequence
    /// </summary>
    public IReadOnlyList<EditOperation<T>> EditSequence => m_Sequence;
    
    /// <summary>
    /// To String 
    /// </summary>
    /// <returns></returns>
    public override string ToString() {
      return string.Join(Environment.NewLine,
        $"Distance: {EditDistance}",
        $"Sequence ({m_Sequence.Count} steps):",
          string.Join(Environment.NewLine, m_Sequence
            .Select(item => $"  {item.ToReport()}")));
    }

    #endregion Public

    #region IReadOnlyList<EditOperation<T>>

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_Sequence.Count;

    /// <summary>
    /// Item ad index
    /// </summary>
    public EditOperation<T> this[int index] => m_Sequence[index];

    /// <summary>
    /// Get Enumerator
    /// </summary>
    public IEnumerator<EditOperation<T>> GetEnumerator() => m_Sequence.GetEnumerator();

    /// <summary>
    /// Get Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => m_Sequence.GetEnumerator();

    #endregion IReadOnlyList<EditOperation<T>>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Enumerable Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Public

    /// <summary>
    /// To Edit procedure
    /// </summary>
    /// <param name="source">Source sequence</param>
    /// <param name="target">Target sequence</param>
    /// <param name="insertCost">Insertion cost</param>
    /// <param name="deleteCost">Deletion cost</param>
    /// <param name="editCost">Edit cost</param>
    /// <returns>Edit procedure (edit distance and edit sequence)</returns>
    public static EditProcedure<T> ToEditProcedure<T>(this IEnumerable<T> source,
                                                           IEnumerable<T> target,
                                                           Func<T, double> insertCost,
                                                           Func<T, double> deleteCost,
                                                           Func<T, T, double> editCost) {
      return new EditProcedure<T>(source, target, insertCost, deleteCost, editCost);
    }

    /// <summary>
    /// To Edit procedure (insert, delete have cost 1)
    /// </summary>
    /// <param name="source">Source sequence</param>
    /// <param name="target">Target sequence</param>
    /// <returns>Edit procedure (edit distance and edit sequence)</returns>
    public static EditProcedure<T> ToEditProcedure<T>(this IEnumerable<T> source,
                                                           IEnumerable<T> target,
                                                           double editCost = 1) {
      return new EditProcedure<T>(source,
                                  target,
                                 (x) => 1,
                                 (x) => 1,
                                 (x, y) => object.Equals(x, y) ? 0 : editCost);
    }

    /// <summary>
    /// To Edit procedure 
    /// </summary>
    /// <param name="source">Source sequence</param>
    /// <param name="target">Target sequence</param>
    /// <param name="prices">Prices</param>
    /// <returns>Edit procedure (edit distance and edit sequence)</returns>
    public static EditProcedure<T> ToEditProcedure<T>(this IEnumerable<T> source,
                                                           IEnumerable<T> target,
                                                           IEditCost<T> prices) {
      return new EditProcedure<T>(source, target, prices);
    }

    #endregion Public
  }

}
