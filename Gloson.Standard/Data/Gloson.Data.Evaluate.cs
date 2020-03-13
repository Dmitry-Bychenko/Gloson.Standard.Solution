using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

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
      using DataTable table = new DataTable();

      return (T)(Convert.ChangeType(table.Compute(formula, null), typeof(T)));
    }

    #endregion Public
  }
}
