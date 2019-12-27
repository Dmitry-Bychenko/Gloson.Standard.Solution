using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Diagnostics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Validation Event Args
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class ValidationEventArgs<T> 
    : EventArgs,
      IEnumerable<string> {

    #region Private Data

    // Cached Errors
    private List<string> m_Errors;

    // Errors returned
    private IEnumerable<string> m_ErrorsFound;

    #endregion  Private Data

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public ValidationEventArgs(T valueUnderTest) 
      : base() {

      if (null == valueUnderTest)
        throw new ArgumentNullException(nameof(valueUnderTest));

      ValueUnderTest = valueUnderTest;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Object Under Test
    /// </summary>
    public T ValueUnderTest { get; }

    /// <summary>
    /// Errors
    /// </summary>
    public void ApplyErrors(IEnumerable<string> errors) {
      m_Errors = null;

      m_ErrorsFound = errors;
    }

    /// <summary>
    /// Errors
    /// </summary>
    public IReadOnlyList<string> Errors { 
      get {
        if (null != m_Errors)
          return m_Errors;

        if (m_ErrorsFound == null)
          m_Errors = new List<String>();
        else
          m_Errors = m_ErrorsFound
            .Where(error => !string.IsNullOrWhiteSpace(error))
            .ToList();

        return m_Errors;
      }
    }

    /// <summary>
    /// Is Valid
    /// </summary>
    public bool IsValid => !Errors.Any();

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      return string.Join(Environment.NewLine, Errors);
    }

    #endregion Public

    #region IEnumerable<string>

    /// <summary>
    /// Enumerator
    /// </summary>
    public IEnumerator<string> GetEnumerator() => Errors.GetEnumerator();

    /// <summary>
    /// Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    #endregion IEnumerable<string>
  }

}
