using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// To Data Table
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Public

    /// <summary>
    /// To Data Table
    /// </summary>
    public static DataTable ToDataTable<T>(
      this IEnumerable<T> source, 
      params (string, Type, Func<T, object>)[] columns) {

      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == columns)
        throw new ArgumentNullException(nameof(columns));

      DataTable result = new DataTable();

      for (int i = 0; i < columns.Length; ++i) 
        result.Columns.Add(columns[i].Item1 ?? $"Column #{i + 1}", columns[i].Item2 ?? typeof(object));

      foreach (var item in source) {
        DataRow row = result.NewRow();

        for (int i = 0; i < columns.Length; ++i)
          row[i] = columns[i].Item3(item);
      }

      return result;
    }

    #endregion Public
  }
}
