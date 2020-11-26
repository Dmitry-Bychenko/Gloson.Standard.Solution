using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Linq.Solvers.Pareto {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Objectives Scope (items)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ObjectivesScope<T> {
    #region Private Data

    private bool m_IsUpdated;

    private readonly List<ObjectiveDescription<T>> m_ObjectiveDescriptions;

    private readonly List<ObjectiveItem<T>> m_Items = new List<ObjectiveItem<T>>();

    private readonly Dictionary<int, IReadOnlyList<ObjectiveItem<T>>> m_Frontiers =
      new Dictionary<int, IReadOnlyList<ObjectiveItem<T>>>();

    #endregion Private Data

    #region Algorithm

    internal void CoreUpdate() {
      if (m_IsUpdated)
        return;

      m_IsUpdated = true;

      // dominance

      for (int i = 0; i < m_Items.Count; ++i)
        for (int j = i + 1; j < m_Items.Count; ++j) {
          ObjectiveItem<T> left = m_Items[i];
          ObjectiveItem<T> right = m_Items[j];

          int d = ObjectiveItem<T>.Dominance(left, right);

          if (d > 0) {
            left.m_BetterThan.Add(right);
            right.m_WorseThan.Add(left);
          }
          else if (d < 0) {
            right.m_BetterThan.Add(left);
            left.m_WorseThan.Add(right);
          }
        }

      // frontiers
      HashSet<ObjectiveItem<T>> agenda = new HashSet<ObjectiveItem<T>>(m_Items);
      HashSet<ObjectiveItem<T>> skips = new HashSet<ObjectiveItem<T>>();

      int level = 0;

      while (agenda.Any()) {
        level += 1;

        List<ObjectiveItem<T>> exclude = new List<ObjectiveItem<T>>(m_Items.Count);

        foreach (var item in agenda)
          if (item.WorseThan.All(x => skips.Contains(x))) {
            exclude.Add(item);

            item.m_FrontierLevel = level;
          }

        m_Frontiers.Add(level, exclude);

        agenda.ExceptWith(exclude);
        skips.UnionWith(exclude);
      }
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="solutions">Solutions</param>
    /// <param name="objectives">Objectives</param>
    public ObjectivesScope(IEnumerable<T> solutions, IEnumerable<ObjectiveDescription<T>> objectives) {
      if (null == solutions)
        throw new ArgumentNullException(nameof(solutions));
      else if (null == objectives)
        throw new ArgumentNullException(nameof(objectives));

      m_ObjectiveDescriptions = new List<ObjectiveDescription<T>>(objectives);

      foreach (var solution in solutions) {
        if (null == solution)
          continue;

        ObjectiveItem<T> item = new ObjectiveItem<T>(this, solution);

        m_Items.Add(item);
      }
    }

    /// <summary>
    /// Clone
    /// </summary>
    public ObjectivesScope<T> Clone() {
      return new ObjectivesScope<T>(Items.Select(item => item.Solution), ObjectiveDescriptions);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Eveolve
    /// </summary>
    /// <param name="generation">Generation number</param>
    /// <param name="breed">Breed function</param>
    /// <param name="comparer">Comparer (best fit), NSGA II on default</param>
    /// <returns></returns>
    public ObjectivesScope<T> Evolve(int generation,
                                     Func<T, T, T> breed,
                                     IComparer<ObjectiveItem<T>> comparer) {
      if (generation < 0)
        throw new ArgumentOutOfRangeException(nameof(generation));
      else if (null == breed)
        throw new ArgumentNullException(nameof(breed));

      if (generation == 0)
        return this;

      ParetoGeneticsSolver<T> solver = new ParetoGeneticsSolver<T>(this, breed, comparer);

      for (int i = 1; i <= generation; ++i)
        solver = solver.Next();

      return solver.Scope;
    }

    /// <summary>
    /// Eveonve
    /// </summary>
    /// <param name="generation">Generation Number</param>
    /// <param name="breed">Breed</param>
    public ObjectivesScope<T> Evolve(int generation,
                                     Func<T, T, T> breed) =>
      Evolve(generation, breed, null);

    /// <summary>
    /// Items
    /// </summary>
    public IReadOnlyList<ObjectiveItem<T>> Items => m_Items;

    /// <summary>
    /// Objective Descriptions
    /// </summary>
    public IReadOnlyList<ObjectiveDescription<T>> ObjectiveDescriptions => m_ObjectiveDescriptions;

    /// <summary>
    /// Frontier
    /// </summary>
    public IReadOnlyList<ObjectiveItem<T>> Frontier(int level) {
      CoreUpdate();

      if (m_Frontiers.TryGetValue(level, out var result))
        return result;

      return new List<ObjectiveItem<T>>();
    }

    /// <summary>
    /// Frontier
    /// </summary>
    public IReadOnlyList<ObjectiveItem<T>> Frontier() => Frontier(1);

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Objective Item
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class ObjectiveItem<T> {
    #region Inner Classes

    private class FrontierAndCroudDistanceComparerClass : IComparer<ObjectiveItem<T>> {
      public int Compare(ObjectiveItem<T> x, ObjectiveItem<T> y) {
        if (ReferenceEquals(x, y))
          return 0;
        else if (null == x)
          return -1;
        else if (null == y)
          return +1;

        if (!ReferenceEquals(x.Owner, y.Owner))
          return 0;

        int result = x.FrontierLevel.CompareTo(y.FrontierLevel);

        if (result < 0)
          return 1;
        else if (result > 0)
          return -1;

        result = x.CrowdingDistance.CompareTo(y.CrowdingDistance);

        if (result != 0)
          return result;

        return 0;
      }
    }

    #endregion Inner Classes

    #region Private Data

    private readonly Dictionary<ObjectiveDescription<T>, double> m_Cached = new Dictionary<ObjectiveDescription<T>, double>();

    internal List<ObjectiveItem<T>> m_BetterThan = new List<ObjectiveItem<T>>();

    internal List<ObjectiveItem<T>> m_WorseThan = new List<ObjectiveItem<T>>();

    internal int m_FrontierLevel = -1;
    private double m_CrowdingDistance = double.NaN;

    #endregion Private Data

    #region Create

    internal ObjectiveItem(ObjectivesScope<T> owner, T solution) {
      Owner = owner;
      Solution = solution;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Frontier And Distance Comparer
    /// </summary>
    public static IComparer<ObjectiveItem<T>> FrontierAndCroudDistanceComparer { get; } =
      new FrontierAndCroudDistanceComparerClass();

    /// <summary>
    /// Dominance:
    ///  +1: left dominates right
    ///   0: left and right are not comparable
    ///  -1: right dominates left
    /// </summary>
    public static int Dominance(ObjectiveItem<T> left, ObjectiveItem<T> right) {
      if (ReferenceEquals(left, right))
        return 0;
      else if (null == left)
        return -1;
      else if (null == right)
        return 1;
      else if (!ReferenceEquals(left.Owner, right.Owner))
        return 0;

      bool canBeBetter = true;
      bool canBeWorse = true;

      foreach (var description in left.Owner.ObjectiveDescriptions) {
        double v1 = left.ObjectiveValue(description);
        double v2 = right.ObjectiveValue(description);

        if (v1 > v2 && description.Goal == ObjectiveGoal.Min)
          canBeBetter = false;
        else if (v1 < v2 && description.Goal == ObjectiveGoal.Max)
          canBeBetter = false;

        if (v2 > v1 && description.Goal == ObjectiveGoal.Min)
          canBeWorse = false;
        else if (v2 < v1 && description.Goal == ObjectiveGoal.Max)
          canBeWorse = false;

        if (!canBeBetter && !canBeWorse)
          return 0;
      }

      if (canBeBetter)
        return 1;
      else if (canBeWorse)
        return -1;

      return 0;
    }

    /// <summary>
    /// Owner
    /// </summary>
    public ObjectivesScope<T> Owner { get; }

    /// <summary>
    /// Solution
    /// </summary>
    public T Solution { get; }

    /// <summary>
    /// Objective Value
    /// </summary>
    public double ObjectiveValue(ObjectiveDescription<T> description) {
      if (null == description)
        throw new ArgumentNullException(nameof(description));

      if (m_Cached.TryGetValue(description, out double result))
        return result;

      double value = description.ComputeObjectiveValue(Solution);

      m_Cached.Add(description, value);

      return value;
    }

    /// <summary>
    /// Dominates
    /// </summary>
    public bool Dominates(ObjectiveItem<T> other) {
      if (ReferenceEquals(this, other))
        return false;
      else if (null == other)
        return true;
      else if (!ReferenceEquals(Owner, other.Owner))
        return false;

      foreach (var description in Owner.ObjectiveDescriptions) {
        double v1 = this.ObjectiveValue(description);
        double v2 = other.ObjectiveValue(description);

        if (v1 > v2 && description.Goal == ObjectiveGoal.Min)
          return false;

        if (v1 < v2 && description.Goal == ObjectiveGoal.Max)
          return false;
      }

      return true;
    }

    /// <summary>
    /// Is Dominated
    /// </summary>
    public bool IsDominated(ObjectiveItem<T> other) {
      if (ReferenceEquals(this, other))
        return false;
      else if (null == other)
        return false;
      else if (!ReferenceEquals(Owner, other.Owner))
        return false;

      foreach (var description in Owner.ObjectiveDescriptions) {
        double v1 = this.ObjectiveValue(description);
        double v2 = other.ObjectiveValue(description);

        if (v2 > v1 && description.Goal == ObjectiveGoal.Min)
          return false;

        if (v2 < v1 && description.Goal == ObjectiveGoal.Max)
          return false;
      }

      return true;
    }

    /// <summary>
    /// Better Than
    /// </summary>
    public IReadOnlyList<ObjectiveItem<T>> BetterThan {
      get {
        Owner.CoreUpdate();

        return m_BetterThan;
      }
    }

    /// <summary>
    /// Worse Than
    /// </summary>
    public IReadOnlyList<ObjectiveItem<T>> WorseThan {
      get {
        Owner.CoreUpdate();

        return m_WorseThan;
      }
    }

    /// <summary>
    /// Frontier Level
    /// </summary>
    public int FrontierLevel {
      get {
        Owner.CoreUpdate();

        return m_FrontierLevel;
      }
    }

    /// <summary>
    /// Crowding Distance
    /// </summary>
    public double CrowdingDistance {
      get {
        if (!double.IsNaN(m_CrowdingDistance))
          return m_CrowdingDistance;

        if (Owner.ObjectiveDescriptions.Count <= 0) {
          m_CrowdingDistance = double.PositiveInfinity;

          return m_CrowdingDistance;
        }

        var description = Owner.ObjectiveDescriptions[0];

        var list = Owner
          .Frontier(FrontierLevel)
          .OrderBy(item => item.ObjectiveValue(description))
          .ToList();

        list[0].m_CrowdingDistance = double.PositiveInfinity;
        list[^1].m_CrowdingDistance = double.PositiveInfinity;

        // weights
        double[] weights = new double[Owner.ObjectiveDescriptions.Count];

        for (int i = 0; i < weights.Length; ++i) {
          weights[i] = Math.Abs(list[^1].ObjectiveValue(Owner.ObjectiveDescriptions[i]) -
                                list[0].ObjectiveValue(Owner.ObjectiveDescriptions[i]));
        }

        // distance computation
        for (int i = 1; i < list.Count - 1; ++i) {
          double distance = 0.0;

          for (int j = 0; j < Owner.ObjectiveDescriptions.Count; ++j) {
            var desc = Owner.ObjectiveDescriptions[j];

            double d1 = list[i - 1].ObjectiveValue(desc);
            double d2 = list[i + 1].ObjectiveValue(desc);

            distance += Math.Abs(d1 - d2) / weights[i];
          }

          list[i].m_CrowdingDistance = distance;
        }

        return CrowdingDistance;
      }
    }

    #endregion Public
  }
}
