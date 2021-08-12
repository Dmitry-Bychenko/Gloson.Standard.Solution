using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Gloson.Json {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Abstract JSON element 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public abstract class JElement {
    #region Private Data

    private static readonly Dictionary<char, string> s_Escape = new() {
      { '\\', "\\\\" },
      { '"', "\\\"" },
      { '\n', "\\n" },
      { '\r', "\\r" },
      { '\t', "\\t" },
      { '\f', "\\f" },
      { '\b', "\\b" },
    };

    #endregion Private Data

    #region Algorithm

    /// <summary>
    /// String to JSON
    /// </summary>
    protected static string StringToJson(string value) {
      if (value is null)
        return "null";

      StringBuilder sb = new(2 * value.Length + 2);

      sb.Append('"');

      foreach (char c in value)
        if (s_Escape.TryGetValue(c, out var st))
          sb.Append(st);
        else if (c < ' ')
          sb.Append($"\\u{((int)c):x4}");
        else
          sb.Append(c);

      sb.Append('"');

      return sb.ToString();
    }

    /// <summary>
    /// Object To Json
    /// </summary>
    protected static string ObjectToJson(object value) {
      if (value is null)
        return "null";

      if (value is JElement element)
        return element.Build();

      if (value is sbyte sint8)
        return sint8.ToString();
      if (value is Int16 sint16)
        return sint16.ToString();
      if (value is Int32 sint32)
        return sint32.ToString();
      if (value is Int64 sint64)
        return sint64.ToString();
      if (value is BigInteger intInf)
        return intInf.ToString();

      if (value is byte uint8)
        return uint8.ToString();
      if (value is UInt16 uint16)
        return uint16.ToString();
      if (value is UInt32 uint32)
        return uint32.ToString();
      if (value is UInt64 uint64)
        return uint64.ToString();

      if (value is float float32)
        return float32.ToString(CultureInfo.InvariantCulture);
      if (value is double float64)
        return float64.ToString(CultureInfo.InvariantCulture);
      if (value is decimal float128)
        return float128.ToString(CultureInfo.InvariantCulture);

      if (value is bool logic)
        return logic ? "true" : "false";

      if (value is DateTime date)
        return $"\"{date.ToUniversalTime():yyyy-MM-dd'T'HH:mm:ss.fff'Z'}\"";

      if (value is DateTimeOffset dateZ)
        return $"\"{dateZ.UtcDateTime:yyyy-MM-dd'T'HH:mm:ss.fff'Z'}\"";

      if (value is string str)
        return StringToJson(str);

      return StringToJson(value?.ToString());
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    protected JElement(string name, bool allowEmpty) {
      if (allowEmpty)
        Name = name ?? "";
      else {
        Name = name ?? throw new ArgumentNullException(nameof(name));

        if (string.IsNullOrEmpty(name))
          throw new ArgumentException("Empty name is not allowed", nameof(name));
      }
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Value
    /// </summary>
    public string Value { get; protected set; }

    /// <summary>
    /// Build
    /// </summary>
    public string Build() => string.IsNullOrEmpty(Name)
      ? Value
      : $"{StringToJson(Name)}: {Value}";

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => Build();

    #endregion Public

    #region Operators

    /// <summary>
    /// Build
    /// </summary>
    public static implicit operator string(JElement item) => item?.Build();

    /// <summary>
    /// From Pair
    /// </summary>
    public static implicit operator JElement((string name, object value) pair) => new JValue(pair.name, pair.value);

    /// <summary>
    /// From Single value
    /// </summary>
    public static implicit operator JElement(string name) => new JValue(name);

    #endregion Operators
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// JSON value
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class JValue : JElement {
    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public JValue(string name, object value) : base(name, false) {
      Value = ObjectToJson(value);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public JValue(string name)
      : this(name, null) { }

    #endregion Create

    #region Operators

    /// <summary>
    /// 
    /// </summary>
    public static implicit operator JValue((string name, object value) pair) => new(pair.name, pair.value);

    #endregion Operators
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// JSON Array
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class JArray : JElement {
    #region Create

    /// <summary>
    /// 
    /// </summary>
    public JArray(string name, IEnumerable<object> source) : base(name, false) {
      Value = source is null
        ? "null"
        : $"[{string.Join(", ", source.Select(item => ObjectToJson(item)))}]";
    }

    #endregion Create

    #region Operators

    /// <summary>
    /// 
    /// </summary>
    public static implicit operator JArray((string name, IEnumerable<object> values) pair) =>
      new(pair.name, pair.values);

    #endregion Operators
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// JSON Array (typed)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class JArray<T> : JArray {
    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public JArray(string name, IEnumerable<T> source) : base(name, source?.Select(x => (object)x)) { }

    #endregion Create

    #region Operators

    /// <summary>
    /// 
    /// </summary>
    public static implicit operator JArray<T>((string name, IEnumerable<T> values) pair) =>
      new(pair.name, pair.values);

    #endregion Operators
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// JSON Object
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class JObject : JElement {
    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public JObject(string name, IEnumerable<JElement> elements) : base(name, true) {
      if (elements is null)
        throw new ArgumentNullException(nameof(elements));

      Value = $"{{{string.Join(", ", elements.Select(item => item.ToString()))}}}";
    }

    /// <summary>
    /// Standard Constructor 
    /// </summary>
    public JObject(string name, params JElement[] elements) : base(name, true) {
      if (elements is null)
        throw new ArgumentNullException(nameof(elements));

      Value = $"{{{string.Join(", ", elements.Select(item => item.ToString()))}}}";
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public JObject(IEnumerable<JElement> elements) :
      this(null, elements) { }

    /// <summary>
    /// Standard Constructor 
    /// </summary>
    public JObject(params JElement[] elements) : base(null, true) {
      if (elements is null)
        throw new ArgumentNullException(nameof(elements));

      Value = $"{{{string.Join(", ", elements.Select(item => item.ToString()))}}}";
    }

    #endregion Create

    #region Operators

    /// <summary>
    /// 
    /// </summary>
    public static implicit operator JObject((string name, IEnumerable<JElement> values) pair) =>
      new(pair.name, pair.values);

    #endregion Operators
  }

}
