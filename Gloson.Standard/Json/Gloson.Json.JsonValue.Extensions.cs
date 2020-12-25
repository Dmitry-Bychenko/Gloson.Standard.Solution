using System;
using System.Json;

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
      if (json is null)
        throw new ArgumentNullException(nameof(json));
      else if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentNullException(nameof(name));

      if (json is JsonObject obj && obj.TryGetValue(name, out var result))
        return result;
      else
        return null;
    }

    /// <summary>
    /// Value
    /// </summary>
    public static JsonValue Value(this JsonValue json, int index) {
      if (json is null)
        throw new ArgumentNullException(nameof(json));

      if (json is JsonArray arr && (index >= 0 || index < arr.Count))
        return arr[index];
      else
        return null;
    }

    #endregion Public
  }

}
