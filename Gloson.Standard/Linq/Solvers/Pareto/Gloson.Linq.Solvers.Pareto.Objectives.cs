using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Linq.Solvers.Pareto {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Objectives Scope (items)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class ObjectivesScope<T> {
    #region Private Data

    private bool m_IsUpdated;

    private List<ObjectiveDescription<T>> m_ObjectiveDescriptions;

    private List<ObjectiveItem<T>> m_Items = new List<ObjectiveItem<T>>();

    private Dictionary<int, IReadOnlyList<ObjectiveItem<T>>> m_Frontiers = 
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

    #endregion Create

    #region Public

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
    #region Private Data

    private Dictionary<ObjectiveDescription<T>, double> m_Cached = new Dictionary<ObjectiveDescription<T>, double>();

    internal List<ObjectiveItem<T>> m_BetterThan = new List<ObjectiveItem<T>>();

    internal List<ObjectiveItem<T>> m_WorseThan = new List<ObjectiveItem<T>>();

    internal int m_FrontierLevel = -1;

    #endregion Private Data

    #region Create

    internal ObjectiveItem(ObjectivesScope<T> owner, T solution) {
      Owner = owner;
      Solution = solution;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Dominance:
    ///  +1: left dominates right
    ///   0: left and right are not comparable
    ///  -1: right dominates left
    /// </summary>
    public static int Dominance(ObjectiveItem<T> left, ObjectiveItem<T> right) {
      if (ReferenceEquals(left, right))
        return 0;
      else if (ReferenceEquals(left, null))
        return -1;
      else if (ReferenceEquals(null, right))
        return 1;
      else if (!ReferenceEquals(left.Owner, right.Owner))
        return 0;

      bool canBeBetter = true;
      bool canBeWorse = true;

      foreach (var description in left.Owner.ObjectiveDescriptions) {
        double v1 = left.ObjectiveValue(description);
        double v2 = right.ObjectiveValue(description);

        if (v1 >= v2 && description.Goal == ObjectiveGoal.Min)
          canBeBetter = false; 
        else if (v1 <= v2 && description.Goal == ObjectiveGoal.Max)
          canBeBetter = false;

        if (v2 >= v1 && description.Goal == ObjectiveGoal.Min)
          canBeWorse = false;
        else if (v2 <= v1 && description.Goal == ObjectiveGoal.Max)
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
      else if (ReferenceEquals(null, other))
        return true;
      else if (!ReferenceEquals(Owner, other.Owner))
        return false;

      foreach (var description in Owner.ObjectiveDescriptions) {
        double v1 = this.ObjectiveValue(description);
        double v2 = other.ObjectiveValue(description);

        if (v1 >= v2 && description.Goal == ObjectiveGoal.Min)
          return false;

        if (v1 <= v2 && description.Goal == ObjectiveGoal.Max)
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
      else if (ReferenceEquals(null, other))
        return false;
      else if (!ReferenceEquals(Owner, other.Owner))
        return false;

      foreach (var description in Owner.ObjectiveDescriptions) {
        double v1 = this.ObjectiveValue(description);
        double v2 = other.ObjectiveValue(description);

        if (v2 >= v1 && description.Goal == ObjectiveGoal.Min)
          return false;

        if (v2 <= v1 && description.Goal == ObjectiveGoal.Max)
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

    #endregion Public
  }
}
