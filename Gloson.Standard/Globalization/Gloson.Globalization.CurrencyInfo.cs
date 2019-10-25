using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Globalization {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Currency Info
  /// </summary>
  /// <seealso cref="https://www.iban.com/currency-codes"/>
  /// <seealso cref="https://en.wikipedia.org/wiki/ISO_4217"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class CurrencyInfo 
    : IEquatable<CurrencyInfo>,
      IComparable<CurrencyInfo> {

    #region Private

    private static Dictionary<int, CurrencyInfo> s_NumberDictionary
      = new Dictionary<int, CurrencyInfo>();

    private static Dictionary<string, CurrencyInfo> s_CodeDictionary
      = new Dictionary<string, CurrencyInfo>(StringComparer.OrdinalIgnoreCase);

    private static List<CurrencyInfo> s_Items = new List<CurrencyInfo>();

    #endregion Private

    #region Algorithm

    private static void FeedStandardISO4217Table() {
      new CurrencyInfo("AED", 784, 2, "United Arab Emirates dirham");
      new CurrencyInfo("AFN", 971, 2, "Afghan afghani");
      new CurrencyInfo("ALL", 008, 2, "Albanian lek");
      new CurrencyInfo("AMD", 051, 2, "Armenian dram");
      new CurrencyInfo("ANG", 532, 2, "Netherlands Antillean guilder");
      new CurrencyInfo("AOA", 973, 2, "Angolan kwanza");
      new CurrencyInfo("ARS", 032, 2, "Argentine peso");
      new CurrencyInfo("AUD", 036, 2, "Australian dollar");
      new CurrencyInfo("AWG", 533, 2, "Aruban florin");
      new CurrencyInfo("AZN", 944, 2, "Azerbaijani manat");
      new CurrencyInfo("BAM", 977, 2, "Bosnia and Herzegovina convertible mark");
      new CurrencyInfo("BBD", 052, 2, "Barbados dollar");
      new CurrencyInfo("BDT", 050, 2, "Bangladeshi taka");
      new CurrencyInfo("BGN", 975, 2, "Bulgarian lev");
      new CurrencyInfo("BHD", 048, 3, "Bahraini dinar");
      new CurrencyInfo("BIF", 108, 0, "Burundian franc");
      new CurrencyInfo("BMD", 060, 2, "Bermudian dollar");
      new CurrencyInfo("BND", 096, 2, "Brunei dollar");
      new CurrencyInfo("BOB", 068, 2, "Boliviano");
      new CurrencyInfo("BOV", 984, 2, "Bolivian Mvdol (funds code)");
      new CurrencyInfo("BRL", 986, 2, "Brazilian real");
      new CurrencyInfo("BSD", 044, 2, "Bahamian dollar");
      new CurrencyInfo("BTN", 064, 2, "Bhutanese ngultrum");
      new CurrencyInfo("BWP", 072, 2, "Botswana pula");
      new CurrencyInfo("BYN", 933, 2, "Belarusian ruble");
      new CurrencyInfo("BZD", 084, 2, "Belize dollar");
      new CurrencyInfo("CAD", 124, 2, "Canadian dollar");
      new CurrencyInfo("CDF", 976, 2, "Congolese franc");
      new CurrencyInfo("CHE", 947, 2, "WIR Euro (complementary currency)");
      new CurrencyInfo("CHF", 756, 2, "Swiss franc");
      new CurrencyInfo("CHW", 948, 2, "WIR Franc (complementary currency)");
      new CurrencyInfo("CLF", 990, 4, "Unidad de Fomento (funds code)");
      new CurrencyInfo("CLP", 152, 0, "Chilean peso");
      new CurrencyInfo("CNY", 156, 2, "Renminbi (Chinese) yuan[7]");
      new CurrencyInfo("COP", 170, 2, "Colombian peso");
      new CurrencyInfo("COU", 970, 2, "Unidad de Valor Real (UVR) (funds code)[8]");
      new CurrencyInfo("CRC", 188, 2, "Costa Rican colon");
      new CurrencyInfo("CUC", 931, 2, "Cuban convertible peso");
      new CurrencyInfo("CUP", 192, 2, "Cuban peso");
      new CurrencyInfo("CVE", 132, 2, "Cape Verdean escudo");
      new CurrencyInfo("CZK", 203, 2, "Czech koruna");
      new CurrencyInfo("DJF", 262, 0, "Djiboutian franc");
      new CurrencyInfo("DKK", 208, 2, "Danish krone");
      new CurrencyInfo("DOP", 214, 2, "Dominican peso");
      new CurrencyInfo("DZD", 012, 2, "Algerian dinar");
      new CurrencyInfo("EGP", 818, 2, "Egyptian pound");
      new CurrencyInfo("ERN", 232, 2, "Eritrean nakfa");
      new CurrencyInfo("ETB", 230, 2, "Ethiopian birr");
      new CurrencyInfo("EUR", 978, 2, "Euro");
      new CurrencyInfo("FJD", 242, 2, "Fiji dollar");
      new CurrencyInfo("FKP", 238, 2, "Falkland Islands pound");
      new CurrencyInfo("GBP", 826, 2, "Pound sterling");
      new CurrencyInfo("GEL", 981, 2, "Georgian lari");
      new CurrencyInfo("GHS", 936, 2, "Ghanaian cedi");
      new CurrencyInfo("GIP", 292, 2, "Gibraltar pound");
      new CurrencyInfo("GMD", 270, 2, "Gambian dalasi");
      new CurrencyInfo("GNF", 324, 0, "Guinean franc");
      new CurrencyInfo("GTQ", 320, 2, "Guatemalan quetzal");
      new CurrencyInfo("GYD", 328, 2, "Guyanese dollar");
      new CurrencyInfo("HKD", 344, 2, "Hong Kong dollar");
      new CurrencyInfo("HNL", 340, 2, "Honduran lempira");
      new CurrencyInfo("HRK", 191, 2, "Croatian kuna");
      new CurrencyInfo("HTG", 332, 2, "Haitian gourde");
      new CurrencyInfo("HUF", 348, 2, "Hungarian forint");
      new CurrencyInfo("IDR", 360, 2, "Indonesian rupiah");
      new CurrencyInfo("ILS", 376, 2, "Israeli new shekel");
      new CurrencyInfo("INR", 356, 2, "Indian rupee");
      new CurrencyInfo("IQD", 368, 3, "Iraqi dinar");
      new CurrencyInfo("IRR", 364, 2, "Iranian rial");
      new CurrencyInfo("ISK", 352, 0, "Icelandic króna");
      new CurrencyInfo("JMD", 388, 2, "Jamaican dollar");
      new CurrencyInfo("JOD", 400, 3, "Jordanian dinar");
      new CurrencyInfo("JPY", 392, 0, "Japanese yen");
      new CurrencyInfo("KES", 404, 2, "Kenyan shilling");
      new CurrencyInfo("KGS", 417, 2, "Kyrgyzstani som");
      new CurrencyInfo("KHR", 116, 2, "Cambodian riel");
      new CurrencyInfo("KMF", 174, 0, "Comoro franc");
      new CurrencyInfo("KPW", 408, 2, "North Korean won");
      new CurrencyInfo("KRW", 410, 0, "South Korean won");
      new CurrencyInfo("KWD", 414, 3, "Kuwaiti dinar");
      new CurrencyInfo("KYD", 136, 2, "Cayman Islands dollar");
      new CurrencyInfo("KZT", 398, 2, "Kazakhstani tenge");
      new CurrencyInfo("LAK", 418, 2, "Lao kip");
      new CurrencyInfo("LBP", 422, 2, "Lebanese pound");
      new CurrencyInfo("LKR", 144, 2, "Sri Lankan rupee");
      new CurrencyInfo("LRD", 430, 2, "Liberian dollar");
      new CurrencyInfo("LSL", 426, 2, "Lesotho loti");
      new CurrencyInfo("LYD", 434, 3, "Libyan dinar");
      new CurrencyInfo("MAD", 504, 2, "Moroccan dirham");
      new CurrencyInfo("MDL", 498, 2, "Moldovan leu");
      new CurrencyInfo("MGA", 969, 2, "Malagasy ariary");
      new CurrencyInfo("MKD", 807, 2, "Macedonian denar");
      new CurrencyInfo("MMK", 104, 2, "Myanmar kyat");
      new CurrencyInfo("MNT", 496, 2, "Mongolian tögrög");
      new CurrencyInfo("MOP", 446, 2, "Macanese pataca");
      new CurrencyInfo("MRU", 929, 2, "Mauritanian ouguiya");
      new CurrencyInfo("MUR", 480, 2, "Mauritian rupee");
      new CurrencyInfo("MVR", 462, 2, "Maldivian rufiyaa");
      new CurrencyInfo("MWK", 454, 2, "Malawian kwacha");
      new CurrencyInfo("MXN", 484, 2, "Mexican peso");
      new CurrencyInfo("MXV", 979, 2, "Mexican Unidad de Inversion (UDI) (funds code)");
      new CurrencyInfo("MYR", 458, 2, "Malaysian ringgit");
      new CurrencyInfo("MZN", 943, 2, "Mozambican metical");
      new CurrencyInfo("NAD", 516, 2, "Namibian dollar");
      new CurrencyInfo("NGN", 566, 2, "Nigerian naira");
      new CurrencyInfo("NIO", 558, 2, "Nicaraguan córdoba");
      new CurrencyInfo("NOK", 578, 2, "Norwegian krone");
      new CurrencyInfo("NPR", 524, 2, "Nepalese rupee");
      new CurrencyInfo("NZD", 554, 2, "New Zealand dollar");
      new CurrencyInfo("OMR", 512, 3, "Omani rial");
      new CurrencyInfo("PAB", 590, 2, "Panamanian balboa");
      new CurrencyInfo("PEN", 604, 2, "Peruvian sol");
      new CurrencyInfo("PGK", 598, 2, "Papua New Guinean kina");
      new CurrencyInfo("PHP", 608, 2, "Philippine peso[13]");
      new CurrencyInfo("PKR", 586, 2, "Pakistani rupee");
      new CurrencyInfo("PLN", 985, 2, "Polish złoty");
      new CurrencyInfo("PYG", 600, 0, "Paraguayan guaraní");
      new CurrencyInfo("QAR", 634, 2, "Qatari riyal");
      new CurrencyInfo("RON", 946, 2, "Romanian leu");
      new CurrencyInfo("RSD", 941, 2, "Serbian dinar");
      new CurrencyInfo("RUB", 643, 2, "Russian ruble");
      new CurrencyInfo("RWF", 646, 0, "Rwandan franc");
      new CurrencyInfo("SAR", 682, 2, "Saudi riyal");
      new CurrencyInfo("SBD", 090, 2, "Solomon Islands dollar");
      new CurrencyInfo("SCR", 690, 2, "Seychelles rupee");
      new CurrencyInfo("SDG", 938, 2, "Sudanese pound");
      new CurrencyInfo("SEK", 752, 2, "Swedish krona/kronor");
      new CurrencyInfo("SGD", 702, 2, "Singapore dollar");
      new CurrencyInfo("SHP", 654, 2, "Saint Helena pound");
      new CurrencyInfo("SLL", 694, 2, "Sierra Leonean leone");
      new CurrencyInfo("SOS", 706, 2, "Somali shilling");
      new CurrencyInfo("SRD", 968, 2, "Surinamese dollar");
      new CurrencyInfo("SSP", 728, 2, "South Sudanese pound");
      new CurrencyInfo("STN", 930, 2, "São Tomé and Príncipe dobra");
      new CurrencyInfo("SVC", 222, 2, "Salvadoran colón");
      new CurrencyInfo("SYP", 760, 2, "Syrian pound");
      new CurrencyInfo("SZL", 748, 2, "Swazi lilangeni");
      new CurrencyInfo("THB", 764, 2, "Thai baht");
      new CurrencyInfo("TJS", 972, 2, "Tajikistani somoni");
      new CurrencyInfo("TMT", 934, 2, "Turkmenistan manat");
      new CurrencyInfo("TND", 788, 3, "Tunisian dinar");
      new CurrencyInfo("TOP", 776, 2, "Tongan paʻanga");
      new CurrencyInfo("TRY", 949, 2, "Turkish lira");
      new CurrencyInfo("TTD", 780, 2, "Trinidad and Tobago dollar");
      new CurrencyInfo("TWD", 901, 2, "New Taiwan dollar");
      new CurrencyInfo("TZS", 834, 2, "Tanzanian shilling");
      new CurrencyInfo("UAH", 980, 2, "Ukrainian hryvnia");
      new CurrencyInfo("UGX", 800, 0, "Ugandan shilling");
      new CurrencyInfo("USD", 840, 2, "United States dollar");
      new CurrencyInfo("USN", 997, 2, "United States dollar (next day) (funds code)");
      new CurrencyInfo("UYI", 940, 0, "Uruguay Peso en Unidades Indexadas (URUIURUI) (funds code)");
      new CurrencyInfo("UYU", 858, 2, "Uruguayan peso");
      new CurrencyInfo("UYW", 927, 4, "Unidad previsional[15]");
      new CurrencyInfo("UZS", 860, 2, "Uzbekistan som");
      new CurrencyInfo("VES", 928, 2, "Venezuelan bolívar soberano[13]");
      new CurrencyInfo("VND", 704, 0, "Vietnamese đồng");
      new CurrencyInfo("VUV", 548, 0, "Vanuatu vatu");
      new CurrencyInfo("WST", 882, 2, "Samoan tala");
      new CurrencyInfo("XAF", 950, 0, "CFA franc BEAC");
      new CurrencyInfo("XAG", 961, 0, "Silver (one troy ounce)");
      new CurrencyInfo("XAU", 959, 0, "Gold (one troy ounce)");
      new CurrencyInfo("XBA", 955, 0, "European Composite Unit (EURCO) (bond market unit)");
      new CurrencyInfo("XBB", 956, 0, "European Monetary Unit (E.M.U.-6) (bond market unit)");
      new CurrencyInfo("XBC", 957, 0, "European Unit of Account 9 (E.U.A.-9) (bond market unit)");
      new CurrencyInfo("XBD", 958, 0, "European Unit of Account 17 (E.U.A.-17) (bond market unit)");
      new CurrencyInfo("XCD", 951, 2, "East Caribbean dollar");
      new CurrencyInfo("XDR", 960, 0, "Special drawing rights");
      new CurrencyInfo("XOF", 952, 0, "CFA franc BCEAO");
      new CurrencyInfo("XPD", 964, 0, "Palladium (one troy ounce)");
      new CurrencyInfo("XPF", 953, 0, "CFP franc (franc Pacifique)");
      new CurrencyInfo("XPT", 962, 0, "Platinum (one troy ounce)");
      new CurrencyInfo("XSU", 994, 0, "SUCRE");
      new CurrencyInfo("XTS", 963, 0, "Code reserved for testing");
      new CurrencyInfo("XUA", 965, 0, "ADB Unit of Account");
      new CurrencyInfo("XXX", 999, 0, "No currency");
      new CurrencyInfo("YER", 886, 2, "Yemeni rial");
      new CurrencyInfo("ZAR", 710, 2, "South African rand");
      new CurrencyInfo("ZMW", 967, 2, "Zambian kwacha");
      new CurrencyInfo("ZWL", 932, 2, "Zimbabwean dollar");
    }

    #endregion Algorithm

    #region Create

    static CurrencyInfo() {
      FeedStandardISO4217Table();

      s_Items.Sort();

      None = Parse("XXX");
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="number">Code</param>
    /// <param name="code">Symbol</param>
    /// <param name="name">Name</param>
    /// <param name="symbol">Symbol</param>
    private CurrencyInfo(int number, string code, string name, string symbol) {
      if (number < 0)
        throw new ArgumentOutOfRangeException(nameof(number));
      else if (string.IsNullOrWhiteSpace(code))
        throw new ArgumentNullException(nameof(code), "Must not be null or whitespace only.");
      else if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentNullException(nameof(name), "Must not be null or whitespace only.");
      else if (string.IsNullOrWhiteSpace(symbol))
        throw new ArgumentNullException(nameof(symbol), "Must not be null or whitespace only.");

      Number = number;
      Code = code.Trim().ToUpperInvariant();

      Name = name;
      Symbol = symbol;

      s_NumberDictionary.Add(Number, this);

      try {
        s_CodeDictionary.Add(Code, this);
      }
      catch {
        s_NumberDictionary.Remove(Number);

        throw;
      }

      s_Items.Add(this);
    }

    /// <summary>
    /// Constructor to create CurrencyInfo from ISO data
    /// </summary>
    /// <param name="number"></param>
    /// <param name="code"></param>
    /// <param name="exponent"></param>
    /// <param name="name"></param>
    private CurrencyInfo(string code, int number, int exponent, string name) {
      if (number < 0)
        throw new ArgumentOutOfRangeException(nameof(number));
      else if (string.IsNullOrWhiteSpace(code))
        throw new ArgumentNullException(nameof(code), "Must not be null or whitespace only.");
      else if (exponent < 0)
        throw new ArgumentOutOfRangeException(nameof(exponent));
      else if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentNullException(nameof(name), "Must not be null or whitespace only.");

      int factor = 1;

      for (int i = 0; i < exponent; ++i)
        factor *= 10;

      RatioMainToMinor = factor;

      Number = number;
      Code = code.Trim().ToUpperInvariant();

      Name = name;
      Symbol = code;

      s_NumberDictionary.Add(Number, this);

      try {
        s_CodeDictionary.Add(Code, this);
      }
      catch {
        s_NumberDictionary.Remove(Number);

        throw;
      }

      s_Items.Add(this);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// All currencies
    /// </summary>
    public static IReadOnlyList<CurrencyInfo> Currencies => s_Items;

    /// <summary>
    /// Compare
    /// </summary>
    public static int Compare(CurrencyInfo left, CurrencyInfo right) {
      if (ReferenceEquals(left, right))
        return 0;
      else if (ReferenceEquals(null, left))
        return -1;
      else if (ReferenceEquals(null, right))
        return 1;

      int result = string.Compare(left.Code, right.Code, StringComparison.OrdinalIgnoreCase);

      if (result != 0)
        return result;

      return left.Number.CompareTo(right.Number);
    }

    /// <summary>
    /// None
    /// </summary>
    public static CurrencyInfo None {
      get;
      private set;
    }

    /// <summary>
    /// ISO 4217 (Number) 
    /// </summary>
    public int Number { get; }
    /// <summary>
    /// ISO 4217 (Code)
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Main Name
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Minor Name
    /// </summary>
    public string MinorName { get; } = "";
    /// <summary>
    /// Minor The Second Name
    /// </summary>
    public string MinorSecondName { get; } = "";

    /// <summary>
    /// Main Symbol
    /// </summary>
    public string Symbol { get; }
    /// <summary>
    /// Auxiliary Symbol
    /// </summary>
    public string MinorSymbol { get; } = "";
    /// <summary>
    /// Auxiliary Minor Symbol
    /// </summary>
    public string MinorSecondSymbol { get; } = "";

    /// <summary>
    /// Ratio main
    /// </summary>
    public long RatioMainToMinor { get; } = 100;
    /// <summary>
    /// Ratio minor
    /// </summary>
    public int RatioMinorToSecondMinor { get; } = 1;

    /// <summary>
    /// Has Minor
    /// </summary>
    public bool HasMinor => RatioMainToMinor > 1;

    /// <summary>
    /// Has Second Minor
    /// </summary>
    public bool HasSecondMinor => RatioMinorToSecondMinor > 1;

    #endregion Public

    #region Operators

    #region Comparison

    /// <summary>
    /// Equal
    /// </summary>
    public static bool operator ==(CurrencyInfo left, CurrencyInfo right) => Compare(left, right) == 0;

    /// <summary>
    /// Not Equal
    /// </summary>
    public static bool operator !=(CurrencyInfo left, CurrencyInfo right) => Compare(left, right) != 0;

    /// <summary>
    /// More Or Equal
    /// </summary>
    public static bool operator >=(CurrencyInfo left, CurrencyInfo right) => Compare(left, right) >= 0;

    /// <summary>
    /// Less Or Equal
    /// </summary>
    public static bool operator <=(CurrencyInfo left, CurrencyInfo right) => Compare(left, right) <= 0;

    /// <summary>
    /// More
    /// </summary>
    public static bool operator >(CurrencyInfo left, CurrencyInfo right) => Compare(left, right) > 0;

    /// <summary>
    /// Less
    /// </summary>
    public static bool operator <(CurrencyInfo left, CurrencyInfo right) => Compare(left, right) < 0;

    #endregion Comparison

    #region Cast

    /// <summary>
    /// Try Get Currency Info
    /// </summary>
    /// <param name="number">ISO number</param>
    /// <param name="result">Currency Info</param>
    /// <returns>true, if found</returns>
    public static bool TryParse(int number, out CurrencyInfo result) => s_NumberDictionary.TryGetValue(number, out result);

    /// <summary>
    /// Parse
    /// </summary>
    public static CurrencyInfo Parse(int number) =>
      TryParse(number, out var result)
        ? result
        : throw new FormatException($"Currency with ISO code {number} has not been found.");

    /// <summary>
    /// Try Get Currency Info
    /// </summary>
    /// <param name="code">ISO symbol</param>
    /// <param name="result">Currency Info</param>
    /// <returns>true, if found</returns>
    public static bool TryParse(string code, out CurrencyInfo result) {
      result = null;

      if (null == code)
        return false;

      code = code.Trim();

      if (s_CodeDictionary.TryGetValue(code, out result))
        return true;
      else {
        if (int.TryParse(code, out int number))
          if (s_NumberDictionary.TryGetValue(number, out result))
            return true;
      }

      return false;
    }

    //s_CodeDictionary.TryGetValue(code?.Trim(), out result);

    /// <summary>
    /// Parse
    /// </summary>
    public static CurrencyInfo Parse(string code) =>
      TryParse(code, out var result)
        ? result
        : throw new FormatException($"Currency with ISO symbol {code} has not been found.");

    /// <summary>
    /// To String (ISO Symbol)
    /// </summary>
    public override string ToString() => Code;

    /// <summary>
    /// To Int32 (ISO Symbol)
    /// </summary>
    public int ToInt32() => Number;

    /// <summary>
    /// To int32 (ISO number)
    /// </summary>
    public static implicit operator int(CurrencyInfo value) =>
      value == null ? -1 : value.Number;

    /// <summary>
    /// To string (ISO code)
    /// </summary>
    public static implicit operator string(CurrencyInfo value) => value?.Code;

    #endregion Cast

    #endregion Operators

    #region IEquatable<CurrencyInfo>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(CurrencyInfo other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      return Number == other.Number &&
             string.Equals(Code, other.Code, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as CurrencyInfo);

    /// <summary>
    /// HashCode (ISO code)
    /// </summary>
    public override int GetHashCode() => Number;

    #endregion IEquatable<CurrencyInfo>

    #region IComparable<CurrencyInfo>

    /// <summary>
    /// Compare To
    /// </summary>
    public int CompareTo(CurrencyInfo other) {
      return Compare(this, other);
    }

    #endregion IComparable<CurrencyInfo>
  }

}
