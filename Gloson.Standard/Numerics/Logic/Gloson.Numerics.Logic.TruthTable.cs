using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Numerics.Logic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Truth Table
  /// </summary>
  // 
  //-------------------------------------------------------------------------------------------------------------------

  public static class TruthTable {
    #region Public

    /// <summary>
    /// Generate Truth table for given delegate
    /// </summary>
    public static IEnumerable<(bool[] data, bool result)> Generate(Delegate function) {
      if (function is null)
        throw new ArgumentNullException(nameof(function));

      if (function.Method.ReturnType != typeof(bool))
        throw new ArgumentException("function must return bool", nameof(function));
      else if (function.Method.GetParameters().Any(p => p.ParameterType != typeof(bool)))
        throw new ArgumentException("function must accept bool only", nameof(function));
      else if (function.Method.GetParameters().Any(p => p.IsOut || p.IsRetval))
        throw new ArgumentException("no out parameters are allowed", nameof(function));

      bool[] arguments = new bool[function.Method.GetParameters().Length];

      do {
        object[] args = arguments.Select(x => (object)x).ToArray();
        bool result = (bool)(function.DynamicInvoke(args));

        yield return (arguments.ToArray(), result);

        for (int i = arguments.Length - 1; i >= 0; --i)
          if (arguments[i])
            arguments[i] = false;
          else {
            arguments[i] = true;

            break;
          }
      }
      while (!arguments.All(x => !x));
    }

    /// <summary>
    /// Generate Truth table for given delegate
    /// </summary>
    public static IEnumerable<(bool[] data, bool result)> Generate(Func<bool> function) => Generate(function as Delegate);

    /// <summary>
    /// Generate Truth table for given delegate
    /// </summary>
    public static IEnumerable<(bool[] data, bool result)> Generate(Func<bool, bool> function) => Generate(function as Delegate);

    /// <summary>
    /// Generate Truth table for given delegate
    /// </summary>
    public static IEnumerable<(bool[] data, bool result)> Generate(Func<bool, bool, bool> function) => Generate(function as Delegate);

    /// <summary>
    /// Generate Truth table for given delegate
    /// </summary>
    public static IEnumerable<(bool[] data, bool result)> Generate(Func<bool, bool, bool, bool> function) => Generate(function as Delegate);

    /// <summary>
    /// Generate Truth table for given delegate
    /// </summary>
    public static IEnumerable<(bool[] data, bool result)> Generate(Func<bool, bool, bool, bool, bool> function)
      => Generate(function as Delegate);

    /// <summary>
    /// Generate Truth table for given delegate
    /// </summary>
    public static IEnumerable<(bool[] data, bool result)> Generate(Func<bool, bool, bool, bool, bool, bool> function)
      => Generate(function as Delegate);

    /// <summary>
    /// Generate Truth table for given delegate
    /// </summary>
    public static IEnumerable<(bool[] data, bool result)> Generate(Func<bool, bool, bool, bool, bool, bool, bool> function)
      => Generate(function as Delegate);

    /// <summary>
    /// Generate Truth table for given delegate
    /// </summary>
    public static IEnumerable<(bool[] data, bool result)> Generate(Func<bool, bool, bool, bool, bool, bool, bool, bool> function)
      => Generate(function as Delegate);

    /// <summary>
    /// Generate Truth table for given delegate
    /// </summary>
    public static IEnumerable<(bool[] data, bool result)> Generate(Func<bool, bool, bool, bool, bool, bool, bool, bool, bool> function)
      => Generate(function as Delegate);

    /// <summary>
    /// Generate Truth table for given delegate
    /// </summary>
    public static IEnumerable<(bool[] data, bool result)> Generate(Func<bool, bool, bool, bool, bool, bool, bool, bool, bool, bool> function)
      => Generate(function as Delegate);

    /// <summary>
    /// Generate Truth table for given delegate
    /// </summary>
    public static IEnumerable<(bool[] data, bool result)> Generate(Func<bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool> function)
      => Generate(function as Delegate);

    /// <summary>
    /// Generate Truth table for given delegate
    /// </summary>
    public static IEnumerable<(bool[] data, bool result)> Generate(Func<bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool> function)
      => Generate(function as Delegate);

    /// <summary>
    /// Generate Truth table for given delegate
    /// </summary>
    public static IEnumerable<(bool[] data, bool result)> Generate(Func<bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool> function)
      => Generate(function as Delegate);

    /// <summary>
    /// Generate Truth table for given delegate
    /// </summary>
    public static IEnumerable<(bool[] data, bool result)> Generate(Func<bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool> function)
      => Generate(function as Delegate);

    /// <summary>
    /// Generate Truth table for given delegate
    /// </summary>
    public static IEnumerable<(bool[] data, bool result)> Generate(Func<bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool> function)
      => Generate(function as Delegate);

    /// <summary>
    /// Generate Truth table for given delegate
    /// </summary>
    public static IEnumerable<(bool[] data, bool result)> Generate(Func<bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool> function)
      => Generate(function as Delegate);

    /// <summary>
    /// Generate Truth table for given delegate
    /// </summary>
    public static IEnumerable<(bool[] data, bool result)> Generate(Func<bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool> function)
      => Generate(function as Delegate);

    #endregion Public
  }
}
