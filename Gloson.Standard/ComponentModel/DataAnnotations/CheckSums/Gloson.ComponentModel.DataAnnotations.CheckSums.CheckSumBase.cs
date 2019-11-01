using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.ComponentModel.DataAnnotations.CheckSums {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Abstract Check Sum
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public abstract class CheckSumBase<T> {
    #region Private Data

    private List<T> m_Items;

    #endregion Private Data

    #region Argument

    /// <summary>
    /// Perform and compute IsValid and ControlDigit
    /// </summary>
    protected abstract void Perform();

    #endregion Argument

    #region Create

    protected CheckSumBase(IEnumerable<T> sequence) {
      if (null == sequence)
        throw new ArgumentNullException(nameof(sequence));

      m_Items = sequence.ToList();

      Perform();
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Initial Items
    /// </summary>
    public IReadOnlyList<T> Sequence => m_Items;

    /// <summary>
    /// Is Valid
    /// </summary>
    public bool IsValid { get; protected set; }

    /// <summary>
    /// Control Digit
    /// </summary>
    public T ControlDigit { get; protected set; }

    /// <summary>
    /// Sequence with control digit
    /// </summary>
    public IEnumerable<T> SequenceWithControlDigit {
      get {
        foreach (var item in m_Items)
          yield return item;

        yield return ControlDigit;
      }
    }

    #endregion Public

    #region Operators

    /// <summary>
    /// To Boolean
    /// </summary>
    /// <returns>IsValid</returns>
    public bool ToBoolean() => IsValid;

    /// <summary>
    /// To Boolean (not null and IsValid)
    /// </summary>
    public static implicit operator bool (CheckSumBase<T> value) =>
      !ReferenceEquals(value, null) && value.IsValid;

    #endregion Operators
  }

}
