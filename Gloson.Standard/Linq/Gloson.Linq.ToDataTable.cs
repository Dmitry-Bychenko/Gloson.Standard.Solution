﻿using System;
using System.Collections.Generic;
using System.Data;

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

      if (source is null)
        throw new ArgumentNullException(nameof(source));
      else if (columns is null)
        throw new ArgumentNullException(nameof(columns));

      DataTable result = new();

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
