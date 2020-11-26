using System;
using System.Collections.Generic;

namespace Gloson.Data.Oracle {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Oracle Object Type
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class OracleObjectType
    : IEquatable<OracleObjectType>,
      IComparable<OracleObjectType> {

    #region Private Data

    private static readonly Dictionary<int, OracleObjectType> s_FromId
      = new Dictionary<int, OracleObjectType>();

    private static readonly Dictionary<string, OracleObjectType> s_FromName
      = new Dictionary<string, OracleObjectType>(StringComparer.OrdinalIgnoreCase);

    private static readonly List<OracleObjectType> s_Items = new List<OracleObjectType>();

    #endregion Private Data

    #region Create

    static OracleObjectType() {
#pragma warning disable CA1806 // Do not ignore method results
      new OracleObjectType(0, "NEXT OBJECT");
      new OracleObjectType(1, "INDEX");
      new OracleObjectType(2, "TABLE");
      new OracleObjectType(3, "CLUSTER");
      new OracleObjectType(4, "VIEW");
      new OracleObjectType(5, "SYNONYM");
      new OracleObjectType(6, "SEQUENCE");
      new OracleObjectType(7, "PROCEDURE");
      new OracleObjectType(8, "FUNCTION");
      new OracleObjectType(9, "PACKAGE");
      new OracleObjectType(11, "PACKAGE BODY");
      new OracleObjectType(12, "TRIGGER");
      new OracleObjectType(13, "TYPE");
      new OracleObjectType(14, "TYPE BODY");
      new OracleObjectType(19, "TABLE PARTITION");
      new OracleObjectType(20, "INDEX PARTITION");
      new OracleObjectType(21, "LOB");
      new OracleObjectType(22, "LIBRARY");
      new OracleObjectType(23, "DIRECTORY");
      new OracleObjectType(24, "QUEUE");
      new OracleObjectType(28, "JAVA SOURCE");
      new OracleObjectType(29, "JAVA CLASS");
      new OracleObjectType(30, "JAVA RESOURCE");
      new OracleObjectType(32, "INDEXTYPE");
      new OracleObjectType(33, "OPERATOR");
      new OracleObjectType(34, "TABLE SUBPARTITION");
      new OracleObjectType(35, "INDEX SUBPARTITION");
      new OracleObjectType(40, "LOB PARTITION");
      new OracleObjectType(41, "LOB SUBPARTITION");
      new OracleObjectType(42, "MATERIALIZED VIEW");
      new OracleObjectType(43, "DIMENSION");
      new OracleObjectType(44, "CONTEXT");
      new OracleObjectType(46, "RULE SET");
      new OracleObjectType(47, "RESOURCE PLAN");
      new OracleObjectType(48, "CONSUMER GROUP");
      new OracleObjectType(51, "SUBSCRIPTION");
      new OracleObjectType(52, "LOCATION");
      new OracleObjectType(55, "XML SCHEMA");
      new OracleObjectType(56, "JAVA DATA");
      new OracleObjectType(57, "SECURITY PROFILE");
      new OracleObjectType(59, "RULE");
      new OracleObjectType(60, "CAPTURE");
      new OracleObjectType(61, "APPLY");
      new OracleObjectType(62, "EVALUATION CONTEXT");
      new OracleObjectType(66, "JOB");
      new OracleObjectType(67, "PROGRAM");
      new OracleObjectType(68, "JOB CLASS");
      new OracleObjectType(69, "WINDOW");
      new OracleObjectType(72, "WINDOW GROUP");
      new OracleObjectType(74, "SCHEDULE");
      new OracleObjectType(77, "SCHEDULE ATTRIBUTE");
      new OracleObjectType(79, "CHAIN");
      new OracleObjectType(81, "FILE GROUP");
      new OracleObjectType(101, "DESTINATION");
#pragma warning restore CA1806 // Do not ignore method results
      s_Items.Sort();
    }

    private OracleObjectType(int id, string name) {
      Name = name?.Trim();
      Id = id;

      s_FromId.Add(Id, this);
      s_FromName.Add(Name, this);
      s_Items.Add(this);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Compare
    /// </summary>
    public static int Compare(OracleObjectType left, OracleObjectType right) {
      if (ReferenceEquals(left, right))
        return 0;
      else if (null == right)
        return -1;
      else if (left == null)
        return 1;

      return left.Id.CompareTo(right.Id);
    }

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// To String - Name
    /// </summary>
    public override string ToString() => Name;

    /// <summary>
    /// Items
    /// </summary>
    public static IReadOnlyList<OracleObjectType> Items => s_Items;

    #endregion Public

    #region Operators

    #region Compare

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(OracleObjectType left, OracleObjectType right) => Compare(left, right) == 0;

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(OracleObjectType left, OracleObjectType right) => Compare(left, right) != 0;

    /// <summary>
    /// More Or Equals
    /// </summary>
    public static bool operator >=(OracleObjectType left, OracleObjectType right) => Compare(left, right) >= 0;

    /// <summary>
    /// Less Or Equals
    /// </summary>
    public static bool operator <=(OracleObjectType left, OracleObjectType right) => Compare(left, right) <= 0;

    /// <summary>
    /// More
    /// </summary>
    public static bool operator >(OracleObjectType left, OracleObjectType right) => Compare(left, right) > 0;

    /// <summary>
    /// Less
    /// </summary>
    public static bool operator <(OracleObjectType left, OracleObjectType right) => Compare(left, right) < 0;

    #endregion Compare

    #region Cast

    /// <summary>
    /// To Int32 (Id)
    /// </summary>
    public static implicit operator int(OracleObjectType value) => null == value
      ? -1
      : value.Id;

    /// <summary>
    /// To String (Name)
    /// </summary>
    public static implicit operator string(OracleObjectType value) => value?.Name;

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(int id, out OracleObjectType value) =>
      s_FromId.TryGetValue(id, out value);

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string name, out OracleObjectType value) =>
      s_FromName.TryGetValue(name, out value);

    /// <summary>
    /// Parse
    /// </summary>
    public static OracleObjectType Parse(int id) {
      if (s_FromId.TryGetValue(id, out var value))
        return value;

      throw new FormatException($"Id {id} has not been found.");
    }

    /// <summary>
    /// Parse
    /// </summary>
    public static OracleObjectType Parse(string name) {
      if (s_FromName.TryGetValue(name, out var value))
        return value;

      throw new FormatException($"Name \"{name}\" has not been found.");
    }

    /// <summary>
    /// From Id
    /// </summary>
    public static implicit operator OracleObjectType(int id) =>
      TryParse(id, out OracleObjectType value) ? value : null;

    /// <summary>
    /// From Name
    /// </summary>
    public static implicit operator OracleObjectType(string name) =>
      TryParse(name, out OracleObjectType value) ? value : null;

    #endregion Cast 

    #endregion Operators

    #region IEquatable<OracleObjectType>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(OracleObjectType other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      return other.Id == Id;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as OracleObjectType);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => Id;

    #endregion IEquatable<OracleObjectType>

    #region IComparable<OracleObjectType>

    /// <summary>
    /// Compare To
    /// </summary>
    public int CompareTo(OracleObjectType other) {
      return Compare(this, other);
    }

    #endregion IComparable<OracleObjectType>
  }

}
