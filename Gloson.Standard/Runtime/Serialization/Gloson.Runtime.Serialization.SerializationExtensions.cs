using System;
using System.Linq;
using System.Reflection;

using System.Runtime.Serialization;

namespace Gloson.Runtime.Serialization {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Serializables
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class Serializables {
    #region Public

    /// <summary>
    /// Clone 
    /// </summary>
    public static T CloneSerializable<T>(T original, StreamingContextStates states) where T : ISerializable {
      if (original is null)
        return default;

      var constructor = original
        .GetType()
        .GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        .Where(ci => ci
          .GetParameters()
          .Select(p => p.ParameterType)
          .SequenceEqual(new Type[] { typeof(SerializationInfo), typeof(StreamingContext) }))
        .FirstOrDefault();

      if (constructor is null)
        throw new ArgumentException($"Type {typeof(T).Name} doesn't have {typeof(T).Name}(SerializationInfo info, StreamingContext context) constructor", nameof(original));

      SerializationInfo info = new SerializationInfo(original.GetType(), new FormatterConverter());
      StreamingContext context = new StreamingContext(states);

      (original as ISerializable).GetObjectData(info, context);

      return (T)(constructor.Invoke(new object[] { info, context }));
    }

    /// <summary>
    /// Clone 
    /// </summary>
    public static T CloneSerializable<T>(T original) where T : ISerializable =>
      CloneSerializable<T>(original, StreamingContextStates.All);

    /// <summary>
    /// Try Clone
    /// </summary>
    public static bool TryCloneSerializable(object original, out object result, StreamingContextStates states) {
      result = default;

      if (original is null)
        return true;

      if (original is not ISerializable source)
        return false;

      var constructor = original
        .GetType()
        .GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        .Where(ci => ci
          .GetParameters()
          .Select(p => p.ParameterType)
          .SequenceEqual(new Type[] { typeof(SerializationInfo), typeof(StreamingContext) }))
        .FirstOrDefault();

      if (constructor is null)
        return false;

      SerializationInfo info = new SerializationInfo(original.GetType(), new FormatterConverter());
      StreamingContext context = new StreamingContext(states);

      source.GetObjectData(info, context);

      result = constructor.Invoke(new object[] { info, context });

      return true;
    }

    /// <summary>
    /// Try Clone
    /// </summary>
    public static bool TryCloneSerializable(object original, out object result) =>
      TryCloneSerializable(original, out result, StreamingContextStates.All);

    #endregion Public
  }
}
