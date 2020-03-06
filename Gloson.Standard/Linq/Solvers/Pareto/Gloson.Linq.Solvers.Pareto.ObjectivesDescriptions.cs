using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Linq.Solvers.Pareto {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Objective Goal (Maximization or Minimization)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum ObjectiveGoal {
    Max = 0,
    Min = 1
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class ObjectiveGoalExtensions {
    #region Public

    /// <summary>
    /// To Report
    /// </summary>
    public static string ToReport(this ObjectiveGoal value) {
      return value switch {
        ObjectiveGoal.Min => "Minimize",
        ObjectiveGoal.Max => "Maximize",
        _ => $"??? ({(int)value})",
      };
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Object Description
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ObjectiveDescription<T> 
    : IComparable<ObjectiveDescription<T>>, 
      IEquatable<ObjectiveDescription<T>> {

    #region Private Data

    private readonly Func<T, double> m_Computation;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public ObjectiveDescription(string id, ObjectiveGoal goal, Func<T, double> computation) {
      if (null == id)
        throw new ArgumentNullException(nameof(id));
      else if (null == computation)
        throw new ArgumentNullException(nameof(computation));

      Id = id?.Trim();
      Goal = goal;
      m_Computation = computation;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public ObjectiveDescription((string, ObjectiveGoal, Func<T, double>) tuple)
      : this(tuple.Item1, tuple.Item2, tuple.Item3) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Comparison
    /// </summary>
    public static int Compare(ObjectiveDescription<T> left, ObjectiveDescription<T> right) {
      if (ReferenceEquals(left, right))
        return 0;
      else if (null == right)
        return -1;
      else if (null == left)
        return 1;

      return string.Compare(left.Id, right.Id);
    }

    /// <summary>
    /// Id
    /// </summary>
    public String Id { get; }

    /// <summary>
    /// Goal
    /// </summary>
    public ObjectiveGoal Goal { get; }

    /// <summary>
    /// Compute Objective value
    /// </summary>
    public double ComputeObjectiveValue(T solution) => m_Computation(solution);

    /// <summary>
    /// Dominates
    /// </summary>
    public bool Dominates(ObjectiveDescription<T> other, T solution) {
      if (ReferenceEquals(this, other))
        return false;
      else if (null == other)
        return false;
      else if (!string.Equals(Id, other.Id))
        return false;

      double v1 = ComputeObjectiveValue(solution);
      double v2 = ComputeObjectiveValue(solution);

      if (v1 >= v2 && Goal == ObjectiveGoal.Max)
        return true;
      else if (v1 <= v2 && Goal == ObjectiveGoal.Min)
        return true;

      return false;
    }

    /// <summary>
    /// Is Dominated
    /// </summary>
    public bool IsDominated(ObjectiveDescription<T> other, T solution) {
      if (ReferenceEquals(this, other))
        return false;
      else if (null == other)
        return false;
      else if (!string.Equals(Id, other.Id))
        return false;

      double v1 = ComputeObjectiveValue(solution);
      double v2 = ComputeObjectiveValue(solution);

      if (v1 <= v2 && Goal == ObjectiveGoal.Max)
        return true;
      else if (v1 >= v2 && Goal == ObjectiveGoal.Min)
        return true;

      return false;
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => Id;

    /// <summary>
    /// To Report
    /// </summary>
    public string ToReport() {
      return $"{Goal.ToReport()} {Id}";
    }

    /// <summary>
    /// Deconstruct
    /// </summary>
    public void Deconstruct(out string id, 
                            out ObjectiveGoal goal,
                            out Func<T, double> computation) {
      id = Id;
      goal = Goal;
      computation = m_Computation;
    }

    #endregion Public

    #region Operators

    /// <summary>
    /// From tuple
    /// </summary>
    public static implicit operator ObjectiveDescription<T>((string, ObjectiveGoal, Func<T, double>) value) =>
      new ObjectiveDescription<T>(value);

    #endregion Operators

    #region IComparable<ObjectiveDescription<T>>

    /// <summary>
    /// Compare To
    /// </summary>
    public int CompareTo(ObjectiveDescription<T> other) => Compare(this, other);

    #endregion IComparable<ObjectiveDescription<T>>

    #region IEquatable<ObjectiveDescription<T>>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(ObjectiveDescription<T> other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      return string.Equals(this.Id, other.Id);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as ObjectiveDescription<T>);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => 
      null == Id ? 0 : Id.GetHashCode();

    #endregion IEquatable<ObjectiveDescription<T>>
  }
  
}
