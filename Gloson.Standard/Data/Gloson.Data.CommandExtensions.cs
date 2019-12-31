using System;
using System.Collections.Generic;
using System.Data;

namespace Gloson.Data {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Command Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class CommandExtensions {
    #region Public

    /// <summary>
    /// As Enumerable
    /// </summary>
    public static IEnumerable<IDataRecord> AsEnumerable(
      this IDbCommand command) {

      if (null == command)
        throw new ArgumentNullException(nameof(command));

      using (IDataReader reader = command.ExecuteReader()) {
        while (reader.Read())
          yield return reader;
      }
    }

    #endregion Public 
  }

}
