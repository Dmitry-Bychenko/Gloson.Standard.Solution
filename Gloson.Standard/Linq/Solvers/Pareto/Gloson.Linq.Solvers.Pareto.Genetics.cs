using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Gloson.Linq.Solvers.Pareto {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Pareto Genetics Solver
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public abstract class ParetoGeneticsSolver<T> {
    #region Private Data

    // Scope
    private ObjectivesScope<T> m_Scope;
    // Comparer
    private IComparer<ObjectiveItem<T>> m_Comparer;

    #endregion Private Data

    #region Algorithm

    private List<ObjectiveItem<T>> BestFit() {
      return m_Scope
        .Items
        .OrderBy(item => item, m_Comparer)
        .Take(m_Scope.Items.Count / 2)
        .ToList();
    }

    /// <summary>
    /// Breed
    /// </summary>
    protected abstract IEnumerable<T> Breed(List<ObjectiveItem<T>> list);

    /// <summary>
    /// Mutate
    /// </summary>
    protected abstract IEnumerable<T> Mutate(IEnumerable<T> list);

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public ParetoGeneticsSolver(IEnumerable<T> source, 
                                IEnumerable<ObjectiveDescription<T>> objectives,
                                IComparer<ObjectiveItem<T>> comparer) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == objectives)
        throw new ArgumentNullException(nameof(objectives));

      m_Scope = new ObjectivesScope<T>(source, objectives);

      if (null == comparer)
        comparer = ObjectiveItem<T>.FrontierAndCroudDistanceComparer;

      m_Comparer = comparer;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Next;
    /// </summary>
    public ParetoGeneticsSolver<T> Next() {
      var list = BestFit();

      var data = Mutate(Breed(list));

      return Activator.CreateInstance(GetType(), data, m_Scope.ObjectiveDescriptions, m_Comparer) as ParetoGeneticsSolver<T>;
    }

    #endregion Public
  }
}
