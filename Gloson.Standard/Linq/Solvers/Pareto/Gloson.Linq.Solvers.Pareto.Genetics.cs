using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Linq.Solvers.Pareto {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Pareto Genetics Solver
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class ParetoGeneticsSolver<T> {
    #region Private Data

    // Scope
    private readonly ObjectivesScope<T> m_Scope;
    // Comparer
    private readonly IComparer<ObjectiveItem<T>> m_Comparer;
    // Breed method
    private readonly Func<T, T, T> m_Breed;

    #endregion Private Data

    #region Algorithm

    private List<T> BestFit() {
      return m_Scope
        .Items
        .OrderBy(item => item, m_Comparer)
        .Take(m_Scope.Items.Count / 2)
        .Select(item => item.Solution)
        .OrderBy(item => Guid.NewGuid())
        .ToList();
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Construcrtor
    /// </summary>
    /// <param name="scope"></param>
    /// <param name="breed"></param>
    /// <param name="comparer"></param>
    public ParetoGeneticsSolver(ObjectivesScope<T> scope,
                                Func<T, T, T> breed,
                                IComparer<ObjectiveItem<T>> comparer) {
      if (null == scope)
        throw new ArgumentNullException(nameof(scope));
      else if (null == breed)
        throw new ArgumentNullException(nameof(breed));

      if (null == comparer)
        comparer = ObjectiveItem<T>.FrontierAndCroudDistanceComparer;

      m_Comparer = comparer;
      Step = 1;

      m_Scope = scope.Clone();
      m_Breed = breed;
    }

    /// <summary>
    /// Standard Construcrtor
    /// </summary>
    /// <param name="scope"></param>
    /// <param name="breed"></param>
    public ParetoGeneticsSolver(ObjectivesScope<T> scope,
                                Func<T, T, T> breed)
      : this(scope, breed, null) { }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public ParetoGeneticsSolver(IEnumerable<T> source,
                                IEnumerable<ObjectiveDescription<T>> objectives,
                                Func<T, T, T> breed,
                                IComparer<ObjectiveItem<T>> comparer) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == objectives)
        throw new ArgumentNullException(nameof(objectives));
      else if (null == breed)
        throw new ArgumentNullException(nameof(breed));

      m_Scope = new ObjectivesScope<T>(source, objectives);
      m_Breed = breed;

      if (null == comparer)
        comparer = ObjectiveItem<T>.FrontierAndCroudDistanceComparer;

      m_Comparer = comparer;
      Step = 1;
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public ParetoGeneticsSolver(IEnumerable<T> source,
                                IEnumerable<ObjectiveDescription<T>> objectives,
                                Func<T, T, T> breed)
      : this(source, objectives, breed, null) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Next
    /// </summary>
    public ParetoGeneticsSolver<T> Next() {
      var list = BestFit();

      List<T> offsprings = new List<T>();

      offsprings.AddRange(list); // All parents

      for (int i = 1; i < list.Count; i += 2)
        offsprings.Add(m_Breed(list[i - 1], list[i]));

      while (offsprings.Count < m_Scope.Items.Count)
        offsprings.Add(m_Breed(list[0], list[list.Count - 1]));

      return new ParetoGeneticsSolver<T>(
        offsprings,
        m_Scope.ObjectiveDescriptions,
        m_Breed,
        m_Comparer) {
        Step = Step + 1
      };
    }

    /// <summary>
    /// Scope
    /// </summary>
    public ObjectivesScope<T> Scope => m_Scope;

    /// <summary>
    /// Step
    /// </summary>
    public int Step { get; private set; }

    /// <summary>
    /// Frontier (Pareto frontier)
    /// </summary>
    public T[] Frontier() {
      return m_Scope
        .Frontier()
        .Select(item => item.Solution)
        .ToArray();
    }

    /// <summary>
    /// Objectives
    /// </summary>
    public ObjectiveDescription<T>[] Objectives() {
      return m_Scope
        .ObjectiveDescriptions
        .ToArray();
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      return string.Join(Environment.NewLine,
        $"Step           : {Step,5}",
        $"Objectives ({m_Scope.ObjectiveDescriptions.Count}):",
        $"  {string.Join(Environment.NewLine + "  ", m_Scope.ObjectiveDescriptions.Select(od => od.ToReport()))}",
        $"Solutions      : {m_Scope.Items.Count,5}",
        $"Frontier       : {m_Scope.Frontier().Count,5}");
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Pareto Solver
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableParetoExtensions {
    #region Public

    /// <summary>
    /// To Pareto Solver
    /// </summary>
    public static ParetoGeneticsSolver<T> ToParetoSolver<T>(this IEnumerable<T> source,
                                                                 IEnumerable<ObjectiveDescription<T>> objectives,
                                                                 Func<T, T, T> breed,
                                                                 IComparer<ObjectiveItem<T>> comparer) =>
      new ParetoGeneticsSolver<T>(source, objectives, breed, comparer);

    /// <summary>
    /// To Pareto Solver
    /// </summary>
    public static ParetoGeneticsSolver<T> ToParetoSolver<T>(this IEnumerable<T> source,
                                                                 IEnumerable<ObjectiveDescription<T>> objectives,
                                                                 Func<T, T, T> breed) =>
      new ParetoGeneticsSolver<T>(source, objectives, breed);

    #endregion Public
  }

}
