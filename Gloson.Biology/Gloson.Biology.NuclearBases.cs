using System;

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
    public static DnaNuclearbase Complement(this DnaNuclearbase value) =>
      value switch {
        DnaNuclearbase.A => DnaNuclearbase.T,
        DnaNuclearbase.C => DnaNuclearbase.G,
        DnaNuclearbase.G => DnaNuclearbase.C,
        DnaNuclearbase.T => DnaNuclearbase.A,
        _ => unchecked((DnaNuclearbase)(-1)),
      };

    /// <summary>
    /// RNA Complement
    /// </summary>
    public static RnaNuclearbase RnaComplement(this DnaNuclearbase value) =>
      value switch {
        DnaNuclearbase.A => RnaNuclearbase.U,
        DnaNuclearbase.C => RnaNuclearbase.G,
        DnaNuclearbase.G => RnaNuclearbase.C,
        DnaNuclearbase.T => RnaNuclearbase.A,
        _ => unchecked((RnaNuclearbase)(-1)),
      };

    /// <summary>
    /// To Char
    /// </summary>
    public static char ToChar(this DnaNuclearbase value) => value switch {
      DnaNuclearbase.A => 'A',
      DnaNuclearbase.C => 'C',
      DnaNuclearbase.G => 'G',
      DnaNuclearbase.T => 'T',
      _ => '?',
    };

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
    public static RnaNuclearbase Complement(this RnaNuclearbase value) => value switch {
      RnaNuclearbase.A => RnaNuclearbase.U,
      RnaNuclearbase.C => RnaNuclearbase.G,
      RnaNuclearbase.G => RnaNuclearbase.C,
      RnaNuclearbase.U => RnaNuclearbase.A,
      _ => unchecked((RnaNuclearbase) (-1)),
    };


    /// <summary>
    /// DNA Complement
    /// </summary>
    public static DnaNuclearbase DnaComplement(this RnaNuclearbase value) => value switch {
      RnaNuclearbase.A => DnaNuclearbase.T,
      RnaNuclearbase.C => DnaNuclearbase.G,
      RnaNuclearbase.G => DnaNuclearbase.C,
      RnaNuclearbase.U => DnaNuclearbase.A,
      _ => unchecked((DnaNuclearbase) (-1)),
    };

    /// <summary>
    /// To Char
    /// </summary>
    public static char ToChar(this RnaNuclearbase value) => value switch {
      RnaNuclearbase.A => 'A',
      RnaNuclearbase.C => 'C',
      RnaNuclearbase.G => 'G',
      RnaNuclearbase.U => 'U',
      _ => '?',
    };
      
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
