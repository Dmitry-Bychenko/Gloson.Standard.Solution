using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gloson.Text;
using Gloson.Collections.Generic;

namespace Gloson.Biology {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Amino Acid
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class AminoAcid 
    : IComparable<AminoAcid>, 
      IEquatable<AminoAcid> {
    
    #region Private Data

    private static List<AminoAcid> s_Items = new List<AminoAcid>();

    private static Dictionary<char, AminoAcid> s_DictOneLetter =
      new Dictionary<char, AminoAcid>(CharacterComparer.OrdinalIgnoreCase);

    private static Dictionary<string, AminoAcid> s_DictThreeLetter = new Dictionary<string, AminoAcid>();

    #endregion Private Data

    #region Algorithm

    private static void RegiterAminoCodon(char code, params string[] codons) {
      var keys = codons
        .SelectMany(item => new string[] { item, item.Replace("U", "T") })
        .Distinct();

      AminoAcid acid = code;

      foreach (string key in keys)
        s_DictThreeLetter.Add(key, acid);
    }

    private static void RegisterCodons() {
      RegiterAminoCodon('A', "GCU", "GCC", "GCA", "GCG");
      RegiterAminoCodon('R', "CGU", "CGC", "CGA", "CGG", "AGA", "AGG");
      RegiterAminoCodon('N', "AAU", "AAC");
      RegiterAminoCodon('D', "GAU", "GAC");
      RegiterAminoCodon('C', "UGU", "UGC");
      RegiterAminoCodon('Q', "CAA", "CAG");
      RegiterAminoCodon('E', "GAA", "GAG");
      RegiterAminoCodon('G', "GGU", "GGC", "GGA", "GGG");
      RegiterAminoCodon('H', "CAU", "CAC");
      RegiterAminoCodon('I', "AUU", "AUC", "AUA");

      RegiterAminoCodon('L', "UUA", "UUG", "CUU", "CUC", "CUA", "CUG");
      RegiterAminoCodon('K', "AAA", "AAG");
      RegiterAminoCodon('M', "AUG"); // Start
      RegiterAminoCodon('F', "UUU", "UUC");
      RegiterAminoCodon('P', "CCU", "CCC", "CCA", "CCG");
      RegiterAminoCodon('S', "UCU", "UCC", "UCA", "UCG", "AGU", "AGC");
      RegiterAminoCodon('T', "ACU", "ACC", "ACA", "ACG");
      RegiterAminoCodon('W', "UGG");
      RegiterAminoCodon('Y', "UAU", "UAC");
      RegiterAminoCodon('V', "GUU", "GUC", "GUA", "GUG");

      RegiterAminoCodon('U', "UGA");
      RegiterAminoCodon('O', "UAG");
      RegiterAminoCodon('O', "UAA"); // ?
    }

    #endregion Algorithm

    #region Create

    static AminoAcid() {
      new AminoAcid("Alanine", "Ala", 'A', 71.03711);
      new AminoAcid("Arginine", "Arg", 'R', 156.10111);
      new AminoAcid("Asparagine", "Asn", 'N', 114.04293);
      new AminoAcid("Aspartic acid", "Asp", 'D', 115.02694);
      new AminoAcid("Cysteine", "Cys", 'C', 103.00919);
      new AminoAcid("Glutamic acid", "Glu", 'E', 129.04259);
      new AminoAcid("Glutamine", "Gln", 'Q', 128.05858);
      new AminoAcid("Glycine", "Gly", 'G', 57.02146);
      new AminoAcid("Histidine", "His", 'H', 137.05891);
      new AminoAcid("Isoleucine", "Ile", 'I', 113.08406);
      new AminoAcid("Leucine", "Leu", 'L', 113.08406);
      new AminoAcid("Lysine", "Lys", 'K', 128.09496);
      new AminoAcid("Methionine", "Met", 'M', 131.04049);
      new AminoAcid("Phenylalanine", "Phe", 'F', 147.06841);
      new AminoAcid("Proline", "Pro", 'P', 97.05276);
      new AminoAcid("Serine", "Ser", 'S', 87.03203);
      new AminoAcid("Threonine", "Thr", 'T', 101.04768);
      new AminoAcid("Tryptophan", "Trp", 'W', 186.07931);
      new AminoAcid("Tyrosine", "Tyr", 'Y', 163.06333);
      new AminoAcid("Valine", "Val", 'V', 99.06841);

      new AminoAcid("Selenocysteine", "Sec", 'U') {
        Mandatory = false,
        IsStop = true
      };

      new AminoAcid("Pyrrolysine", "Pyl", 'O') {
        Mandatory = false,
        IsStop = true
      };

      new AminoAcid("Asparagine or aspartic acid", "Asx", 'B') {
        Mandatory = false,
        Residue = false
      };

      new AminoAcid("Glutamine or glutamic acid", "Glx", 'Z') {
        Mandatory = false,
        Residue = false
      };

      new AminoAcid("Leucine or Isoleucine", "Xle", 'J') {
        Mandatory = false,
        Residue = false
      };

      new AminoAcid("Unspecified or unknown amino acid", "Xaa", 'X') {
        Mandatory = false,
        Residue = false
      };

      new AminoAcid("Stop", "***", '*') {
        Mandatory = false,
        Residue = false
      };

      RegisterCodons();
    }

    private AminoAcid(String title, String isoCode, Char code, Double mass)
      : base() {

      Title = title;
      IsoCode = isoCode;
      Code = code;
      Mass = mass;

      Mandatory = true;

      s_Items.Add(this);
      s_DictOneLetter.Add(Code, this);

      s_DictThreeLetter.Add(IsoCode, this);
      s_DictThreeLetter.Add(Code.ToString(), this);
    }

    // Standard constructor
    private AminoAcid(String title, String isoCode, Char code)
      : this(title, isoCode, code, 0.0) {
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Compare
    /// </summary>
    public static int Compare(AminoAcid left, AminoAcid right) {
      if (ReferenceEquals(left, right))
        return 0;
      else if (ReferenceEquals(left, null))
        return -1;
      else if (ReferenceEquals(null, right))
        return 1;

      return left.Code.CompareTo(right.Code);
    }

    /// <summary>
    /// All Amino Acids
    /// </summary>
    public static IReadOnlyList<AminoAcid> Items => s_Items;

    /// <summary>
    /// One letter code
    /// </summary>
    public Char Code {
      get;
      private set;
    }

    /// <summary>
    /// Three letter ISO code
    /// </summary>
    public String IsoCode {
      get;
      private set;
    }

    /// <summary>
    /// Title
    /// </summary>
    public String Title {
      get;
      private set;
    }

    /// <summary>
    /// Average mass
    /// </summary>
    public Double Mass {
      get;
      private set;
    }

    /// <summary>
    /// Mandatory
    /// </summary>
    public Boolean Mandatory {
      get;
      private set;
    }

    /// <summary>
    /// Residue
    /// </summary>
    public Boolean Residue {
      get;
      private set;
    }

    /// <summary>
    /// Is stop
    /// </summary>
    public Boolean IsStop {
      get;
      private set;
    }

    /// <summary>
    /// To String (Code)
    /// </summary>
    public override string ToString() => Code.ToString();

    /// <summary>
    /// To Char
    /// </summary>
    public char ToChar() => Code;

    #endregion Public

    #region Operators

    #region Cast

    /// <summary>
    /// From Char
    /// </summary>
    public static implicit operator AminoAcid (char value) {
      return s_DictOneLetter.TryGetValue(value, out var result)
        ? result
        : throw new FormatException($"Unknown amino acid '{value}'");
    }

    /// <summary>
    /// From String
    /// </summary>
    public static implicit operator AminoAcid(string value) {
      return s_DictThreeLetter.TryGetValue(value, out var result)
        ? result
        : throw new FormatException($"Unknown amino acid \"{value}\"");
    }

    #endregion Cast

    #region Comparison

    public static bool operator ==(AminoAcid left, AminoAcid right) => Compare(left, right) == 0;

    public static bool operator !=(AminoAcid left, AminoAcid right) => Compare(left, right) != 0;

    public static bool operator >=(AminoAcid left, AminoAcid right) => Compare(left, right) >= 0;

    public static bool operator <=(AminoAcid left, AminoAcid right) => Compare(left, right) <= 0;

    public static bool operator >(AminoAcid left, AminoAcid right) => Compare(left, right) > 0;

    public static bool operator <(AminoAcid left, AminoAcid right) => Compare(left, right) < 0;

    #endregion Comparison

    #endregion Operators

    #region IComparable<AminoAcid>

    /// <summary>
    /// Compare To
    /// </summary>
    public int CompareTo(AminoAcid other) => Compare(this, other);

    #endregion IComparable<AminoAcid>

    #region IEquatable<AminoAcid>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(AminoAcid other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (ReferenceEquals(null, other))
        return false;

      return Code == other.Code;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as AminoAcid);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => Code;

    #endregion IEquatable<AminoAcid>
  }
}
