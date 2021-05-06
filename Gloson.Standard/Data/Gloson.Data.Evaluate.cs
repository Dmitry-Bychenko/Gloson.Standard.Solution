using System;
using System.Data;

namespace Gloson.Data {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Evaluate
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class Evaluate {
    #region Public

    /// <summary>
    /// Run 
    /// </summary>
    public static T Run<T>(string formula) {
      if (formula is null)
        throw new ArgumentNullException(nameof(formula));

      using DataTable table = new();

      return (T)(Convert.ChangeType(table.Compute(formula, null), typeof(T)));
    }

    /// <summary>
    /// Run With Variables
    /// </summary>
    public static T RunWithVariables<T>(string formula,
                                        params (string name, object value)[] variables) {
      using DataTable table = new();

      foreach (var (n, v) in variables)
        table.Columns.Add(n, v is null ? typeof(object) : v.GetType());

      table.Rows.Add();

      foreach (var (n, v) in variables)
        table.Rows[0][n] = v;

      table.Columns.Add("__Result", typeof(double)).Expression = formula ?? throw new ArgumentNullException(nameof(formula)); ;

      return (T)(Convert.ChangeType(table.Compute($"Min(__Result)", null), typeof(T)));
    }

    #endregion Public
  }
}
