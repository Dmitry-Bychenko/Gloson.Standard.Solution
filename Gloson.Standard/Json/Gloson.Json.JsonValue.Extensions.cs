using System;
using System.Collections.Generic;
using System.Json;
using System.Text;

namespace Gloson.Json {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Json extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class JsonValueExtensions {
    #region Public

    /// <summary>
    /// Value
    /// </summary>
    public static JsonValue Value(this JsonValue json, string name) {
      if (null == json)
        throw new ArgumentNullException(nameof(json));
      else if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentNullException(nameof(name));

      JsonObject obj = json as JsonObject;

      if (null == obj)
        return null;
      else if (obj.TryGetValue(name, out var result))
        return result;
      else
        return null;
    }

    /// <summary>
    /// Value
    /// </summary>
    public static JsonValue Value(this JsonValue json, int index) {
      if (null == json)
        throw new ArgumentNullException(nameof(json));

      JsonArray arr = json as JsonArray;

      if (null == arr)
        return null;
      else if (index < 0 || index >= arr.Count)
        return null;
      else
        return arr[index];
    }

    #endregion Public
  }

}
