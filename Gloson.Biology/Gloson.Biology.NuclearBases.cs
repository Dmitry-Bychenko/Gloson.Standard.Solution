using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Biology {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// DNA Nuclear base
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum DnaNuclearbase : byte {
    /// <summary>
    /// Adenine
    /// </summary>
    A = 0,
    /// <summary>
    /// Cytosine
    /// </summary>
    C = 1,
    /// <summary>
    /// Guanine
    /// </summary>
    G = 2,
    /// <summary>
    /// Thymine
    /// </summary>
    T = 3
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// DNA Nuclear base Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class DnaNuclearbaseExtensions {
    #region Public

    /// <summary>
    /// Complement
    /// </summary>
    public static DnaNuclearbase Complement(this DnaNuclearbase value) {
      switch (value) {
        case DnaNuclearbase.A :
          return DnaNuclearbase.T;
        case DnaNuclearbase.C:
          return DnaNuclearbase.G;
        case DnaNuclearbase.G:
          return DnaNuclearbase.C;
        case DnaNuclearbase.T:
          return DnaNuclearbase.A;
        default:
          return value;
      }
    }

    /// <summary>
    /// RNA Complement
    /// </summary>
    public static RnaNuclearbase RnaComplement(this DnaNuclearbase value) {
      switch (value) {
        case DnaNuclearbase.A:
          return RnaNuclearbase.U;
        case DnaNuclearbase.C:
          return RnaNuclearbase.G;
        case DnaNuclearbase.G:
          return RnaNuclearbase.C;
        case DnaNuclearbase.T:
          return RnaNuclearbase.A;
        default:
          return RnaNuclearbase.A;
      }
    }

    /// <summary>
    /// To Char
    /// </summary>
    public static char ToChar(this DnaNuclearbase value) {
      switch (value) {
        case DnaNuclearbase.A:
          return 'A';
        case DnaNuclearbase.C:
          return 'C';
        case DnaNuclearbase.G:
          return 'G';
        case DnaNuclearbase.T:
          return 'T';
        default:
          return '?';
      }
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// DNA Nuclear base helper
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class DnaNuclearbaseHelper {
    #region Public

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(char item, out DnaNuclearbase result) {
      if (item == 'a' || item == 'A') {
        result = DnaNuclearbase.A;

        return true;
      }
      else if (item == 'c' || item == 'C') {
        result = DnaNuclearbase.C;

        return true;
      }
      else if (item == 'g' || item == 'G') {
        result = DnaNuclearbase.G;

        return true;
      }
      else if (item == 't' || item == 'T') {
        result = DnaNuclearbase.T;

        return true;
      }

      result = DnaNuclearbase.A;

      return false;
    }

    /// <summary>
    /// Parse
    /// </summary>
    public static DnaNuclearbase Parse(char item) {
      if (TryParse(item, out var result))
        return result;
      else
        throw new FormatException($"nuclear base {item} is not valid");
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// RNA Nuclear base
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum RnaNuclearbase : byte {
    /// <summary>
    /// Adenine
    /// </summary>
    A = 0,
    /// <summary>
    /// Cytosine
    /// </summary>
    C = 1,
    /// <summary>
    /// Guanine
    /// </summary>
    G = 2,
    /// <summary>
    /// Uracile
    /// </summary>
    U = 3
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// RNA Nuclear base Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class RnaNuclearbaseExtensions {
    #region Public

    /// <summary>
    /// Complement
    /// </summary>
    public static RnaNuclearbase Complement(this RnaNuclearbase value) {
      switch (value) {
        case RnaNuclearbase.A:
          return RnaNuclearbase.U;
        case RnaNuclearbase.C:
          return RnaNuclearbase.G;
        case RnaNuclearbase.G:
          return RnaNuclearbase.C;
        case RnaNuclearbase.U:
          return RnaNuclearbase.A;
        default:
          return value;
      }
    }

    /// <summary>
    /// DNA Complement
    /// </summary>
    public static DnaNuclearbase DnaComplement(this RnaNuclearbase value) {
      switch (value) {
        case RnaNuclearbase.A:
          return DnaNuclearbase.T;
        case RnaNuclearbase.C:
          return DnaNuclearbase.G;
        case RnaNuclearbase.G:
          return DnaNuclearbase.C;
        case RnaNuclearbase.U:
          return DnaNuclearbase.A;
        default:
          return DnaNuclearbase.A;
      }
    }

    /// <summary>
    /// To Char
    /// </summary>
    public static char ToChar(this RnaNuclearbase value) {
      switch (value) {
        case RnaNuclearbase.A:
          return 'A';
        case RnaNuclearbase.C:
          return 'C';
        case RnaNuclearbase.G:
          return 'G';
        case RnaNuclearbase.U:
          return 'U';
        default:
          return '?';
      }
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// RNA Nuclear base helper
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class RnaNuclearbaseHelper {
    #region Public

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(char item, out RnaNuclearbase result) {
      if (item == 'a' || item == 'A') {
        result = RnaNuclearbase.A;

        return true;
      }
      else if (item == 'c' || item == 'C') {
        result = RnaNuclearbase.C;

        return true;
      }
      else if (item == 'g' || item == 'G') {
        result = RnaNuclearbase.G;

        return true;
      }
      else if (item == 'u' || item == 'U') {
        result = RnaNuclearbase.U;

        return true;
      }

      result = RnaNuclearbase.A;

      return false;
    }

    /// <summary>
    /// Parse
    /// </summary>
    public static RnaNuclearbase Parse(char item) {
      if (TryParse(item, out var result))
        return result;
      else
        throw new FormatException($"nuclear base {item} is not valid");
    }

    #endregion Public
  }



}
