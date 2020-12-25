using System;
using System.Collections.Generic;

namespace Gloson.Astronomy {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Constellation
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class Constellation :
    IEquatable<Constellation>,
    IComparable<Constellation> {

    #region Private Data

    private static readonly Dictionary<string, Constellation> s_Dictionary;

    private static readonly List<Constellation> s_Items;

    #endregion Private Data

    #region Algorithm

    private static void CoreFeed() {
#pragma warning disable CA1806 // Do not ignore method results
      new Constellation("Unknown", "???", "???", "Unknown");
      new Constellation("Andromeda", "And", "Andr", "Andromedae");
      new Constellation("Antlia", "Ant", "Antl", "Antliae");
      new Constellation("Apus", "Aps", "Apus", "Apodis");
      new Constellation("Aquarius", "Aqr", "Aqar", "Aquarii");
      new Constellation("Aquila", "Aql", "Aqil", "Aquilae");
      new Constellation("Ara", "Ara", "Arae", "Arae");
      new Constellation("Aries", "Ari", "Arie", "Arietis");
      new Constellation("Auriga", "Aur", "Auri", "Aurigae");

      new Constellation("Boötes", "Boo", "Boot", "Boötis");

      new Constellation("Caelum", "Cae", "Cael", "Caeli");
      new Constellation("Camelopardalis", "Cam", "Caml", "Camelopardalis");
      new Constellation("Cancer", "Cnc", "Canc", "Cancri");
      new Constellation("Canes Venatici", "CVn", "CVen", "Canum Venaticorum");
      new Constellation("Canis Major", "CMa", "CMaj", "Canis Majoris");
      new Constellation("Canis Minor", "CMi", "CMin", "Canis Minoris");
      new Constellation("Capricornus", "Cap", "Capr", "Capricorni");
      new Constellation("Carina", "Car", "Cari", "Carinae");
      new Constellation("Cassiopeia", "Cas", "Cass", "Cassiopeiae");
      new Constellation("Centaurus", "Cen", "Cent", "Centauri");
      new Constellation("Cepheus", "Cep", "Ceph", "Cephei");
      new Constellation("Cetus", "Cet", "Ceti", "Ceti");
      new Constellation("Chamaeleon", "Cha", "Cham", "Chamaeleontis");

      new Constellation("Circinus", "Cir", "Circ", "Circini");
      new Constellation("Columba", "Col", "Colm", "Columbae");
      new Constellation("Coma Berenices", "Com", "Coma", "Comae Berenices");
      new Constellation("Corona Australis", "CrA", "CorA", "Coronae Australis");
      new Constellation("Corona Borealis", "CrB", "CorB", "Coronae Borealis");
      new Constellation("Corvus", "Crv", "Corv", "Corvi");
      new Constellation("Crater", "Crt", "Crat", "Crateris");
      new Constellation("Crux", "Cru", "Cruc", "Crucis");
      new Constellation("Cygnus", "Cyg", "Cygn", "Cygni");

      new Constellation("Delphinus", "Del", "Dlph", "Delphini");
      new Constellation("Dorado", "Dor", "Dora", "Doradus");
      new Constellation("Draco", "Dra", "Drac", "Draconis");

      new Constellation("Equuleus", "Equ", "Equl", "Equulei");
      new Constellation("Eridanus", "Eri", "Erid", "Eridani");

      new Constellation("Fornax", "For", "Forn", "Fornacis");

      new Constellation("Gemini", "Gem", "Gemi", "Geminorum");
      new Constellation("Grus", "Gru", "Grus", "Gruis");

      new Constellation("Hercules", "Her", "Herc", "Herculis");
      new Constellation("Horologium", "Hor", "Horo", "Horologii");
      new Constellation("Hydra", "Hya", "Hyda", "Hydrae");
      new Constellation("Hydrus", "Hyi", "Hydi", "Hydri");

      new Constellation("Indus", "Ind", "Indi", "Indi");

      new Constellation("Lacerta", "Lac", "Lacr", "Lacertae");
      new Constellation("Leo Minor", "LMi", "LMin", "Leonis Minoris");
      new Constellation("Leo", "Leo", "Leon", "Leonis");
      new Constellation("Lepus", "Lep", "Leps", "Leporis");
      new Constellation("Libra", "Lib", "Libr", "Librae");
      new Constellation("Lupus", "Lup", "Lupi", "Lupi");
      new Constellation("Lynx", "Lyn", "Lync", "Lyncis");
      new Constellation("Lyra", "Lyr", "Lyra", "Lyrae");

      new Constellation("Mensa", "Men", "Mens", "Mensae");
      new Constellation("Microscopium", "Mic", "Micr", "Microscopii");
      new Constellation("Monoceros", "Mon", "Mono", "Monocerotis");
      new Constellation("Musca", "Mus", "Musc", "Muscae");

      new Constellation("Norma", "Nor", "Norm", "Normae");

      new Constellation("Octans", "Oct", "Octn", "Octantis");
      new Constellation("Ophiuchus", "Oph", "Ophi", "Ophiuchi");
      new Constellation("Orion", "Ori", "Orio", "Orionis");

      new Constellation("Pavo", "Pav", "Pavo", "Pavonis");
      new Constellation("Pegasus", "Peg", "Pegs", "Pegasi");
      new Constellation("Perseus", "Per", "Pers", "Persei");
      new Constellation("Phoenix", "Phe", "Phoe", "Phoenicis");
      new Constellation("Pictor", "Pic", "Pict", "Pictoris");
      new Constellation("Pisces", "Psc", "Pisc", "Piscium");
      new Constellation("Piscis Austrinus", "PsA", "PscA", "Piscis Austrini");
      new Constellation("Puppis", "Pup", "Pupp", "Puppis");
      new Constellation("Pyxis", "Pyx", "Pyxi", "Pyxidis");

      new Constellation("Reticulum", "Ret", "Reti", "Reticuli");

      new Constellation("Sagitta", "Sge", "Sgte", "Sagittae");
      new Constellation("Sagittarius", "Sgr", "Sgtr", "Sagittarii");
      new Constellation("Scorpius", "Sco", "Scor", "Scorpii");
      new Constellation("Sculptor", "Scl", "Scul", "Sculptoris");
      new Constellation("Scutum", "Sct", "Scut", "Scuti");
      new Constellation("Serpens", "Ser", "Serp", "Serpentis");
      new Constellation("Sextans", "Sex", "Sext", "Sextantis");

      new Constellation("Taurus", "Tau", "Taur", "Tauri");
      new Constellation("Telescopium", "Tel", "Tele", "Telescopii");
      new Constellation("Triangulum Australe", "TrA", "TrAu", "Trianguli Australis");
      new Constellation("Triangulum", "Tri", "Tria", "Trianguli");
      new Constellation("Tucana", "Tuc", "Tucn", "Tucanae");

      new Constellation("Ursa Major", "UMa", "UMaj", "Ursae Majoris");
      new Constellation("Ursa Minor", "UMi", "UMin", "Ursae Minoris");

      new Constellation("Vela", "Vel", "Velr", "Velorum");
      new Constellation("Virgo", "Vir", "Virg", "Virginis");
      new Constellation("Volans", "Vol", "Voln", "Volantis");
      new Constellation("Vulpecula", "Vul", "Vulp", "Vulpeculae");
#pragma warning restore CA1806 // Do not ignore method results
    }

    #endregion Algorithm

    #region Create

    static Constellation() {
      s_Dictionary = new Dictionary<string, Constellation>(StringComparer.OrdinalIgnoreCase);
      s_Items = new List<Constellation>();

      CoreFeed();

      Unknown = s_Dictionary["???"];

      s_Dictionary.Add("", Unknown);
    }

    private Constellation(string name, string acronym, string nasa, string genitive) {
      Name = name ?? "";
      Acronym = acronym ?? "";
      AcronymNasa = nasa ?? "";
      Genitive = genitive ?? "";

      s_Dictionary.Add(Name, this);

      if (!s_Dictionary.ContainsKey(Acronym))
        s_Dictionary.Add(Acronym, this);

      if (!s_Dictionary.ContainsKey(AcronymNasa))
        s_Dictionary.Add(AcronymNasa, this);

      if (!s_Dictionary.ContainsKey(Genitive))
        s_Dictionary.Add(Genitive, this);

      s_Items.Add(this);
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string name, out Constellation result) => s_Dictionary.TryGetValue(name, out result);

    /// <summary>
    /// Parse
    /// </summary>
    public static Constellation Parse(string name) => TryParse(name, out var result)
      ? result
      : throw new FormatException($"{name} constellation is not found.");

    #endregion Create

    #region Public

    /// <summary>
    /// Items
    /// </summary>
    public static IReadOnlyList<Constellation> Items => s_Items;

    /// <summary>
    /// Unknown;
    /// </summary>
    public static Constellation Unknown { get; }

    /// <summary>
    /// Compare
    /// </summary>
    public static int Compare(Constellation left, Constellation right) {
      if (ReferenceEquals(left, right))
        return 0;
      else if (left is null)
        return -1;
      else if (right is null)
        return 1;

      return StringComparer.OrdinalIgnoreCase.Compare(left, right);
    }

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Name (Genetive)
    /// </summary>
    public string Genitive { get; }

    /// <summary>
    /// Acronym
    /// </summary>
    public string Acronym { get; }

    /// <summary>
    /// Acronym (Nasa)
    /// </summary>
    public string AcronymNasa { get; }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => $"{Name} ({Acronym})";

    #endregion Public

    #region Operators

    /// <summary>
    /// 
    /// </summary>
    public static bool operator ==(Constellation left, Constellation right) => Compare(left, right) == 0;

    /// <summary>
    /// 
    /// </summary>
    public static bool operator !=(Constellation left, Constellation right) => Compare(left, right) != 0;

    /// <summary>
    /// 
    /// </summary>
    public static bool operator >=(Constellation left, Constellation right) => Compare(left, right) >= 0;

    /// <summary>
    /// 
    /// </summary>
    public static bool operator <=(Constellation left, Constellation right) => Compare(left, right) <= 0;

    /// <summary>
    /// 
    /// </summary>
    public static bool operator >(Constellation left, Constellation right) => Compare(left, right) > 0;

    /// <summary>
    /// 
    /// </summary>
    public static bool operator <(Constellation left, Constellation right) => Compare(left, right) < 0;

    #endregion Operators

    #region IEquatable<Constellation>

    /// <summary>
    /// Compare
    /// </summary>
    public bool Equals(Constellation other) => Compare(this, other) == 0;

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as Constellation);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(this.Acronym);

    #endregion IEquatable<Constellation>

    #region IComparable<Constellation>

    /// <summary>
    /// Compare To
    /// </summary>
    public int CompareTo(Constellation other) => Compare(this, other);

    #endregion IComparable<Constellation>
  }

}
