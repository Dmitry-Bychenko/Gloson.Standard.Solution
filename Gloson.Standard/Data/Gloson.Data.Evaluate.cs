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
      if (null == formula)
        throw new ArgumentNullException(nameof(formula));

      using DataTable table = new DataTable();

      return (T)(Convert.ChangeType(table.Compute(formula, null), typeof(T)));
    }

    /// <summary>
    /// Run With Variables
    /// </summary>
    public static T RunWithVariables<T>(string formula, 
                                        params (string name, object value)[] variables) {
      if (null == formula)
        throw new ArgumentNullException(nameof(formula));

      using DataTable table = new DataTable();

      foreach (var v in variables)
        table.Columns.Add(v.name, v.value == null ? typeof(object) : v.value.GetType());

      table.Rows.Add();

      foreach (var v in variables)
        table.Rows[0][v.name] = v.value;

      table.Columns.Add("__Result", typeof(double)).Expression = formula;

      return (T) (Convert.ChangeType(table.Compute($"Min(__Result)", null), typeof(T)));
    }

    #endregion Public
  }
}
