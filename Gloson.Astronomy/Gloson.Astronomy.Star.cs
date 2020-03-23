using Gloson.Net.Http;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Gloson.Astronomy {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Star
  /// </summary>
  // 
  // https://github.com/astronexus/HYG-Database/blob/master/hygdata_v3.csv
  // https://raw.githubusercontent.com/astronexus/HYG-Database/master/hygdata_v3.csv
  // https://github.com/astronexus/HYG-Database
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class Star {
    #region Constants

    private const string Address = @"https://raw.githubusercontent.com/astronexus/HYG-Database/master/hygdata_v3.csv";

    #endregion Constants

    #region Private Data

    private static List<Star> s_Catalog;

    #endregion Private Data

    #region Algorithm

    private static void CoreLoad() {
      s_Catalog = new List<Star>();

      foreach (string[] record in HttpData.ReadCsv(Address).Skip(1))
        s_Catalog.Add(new Star(record));
    }

    private static double Parse(string value) {
      if (string.IsNullOrWhiteSpace(value))
        return double.NaN;

      return double.Parse(value, CultureInfo.InvariantCulture);
    }

    #endregion Algorithm

    #region Create

    static Star() {
      CoreLoad();
    }

    // Standard Constructor
    private Star(string[] record) {
      Id = record[0];

      HipparcosId = record[1] ?? "";
      HenryDraperId = record[2] ?? "";
      HarvardRevisedId = record[3] ?? "";
      GlieseId = record[4] ?? "";
      BayerFlamsteed = record[5] ?? "";
      Name = record[6] ?? "";

      RightAscension = Parse(record[7]);
      Declination = Parse(record[8]);

      Distance = Parse(record[9]);

      ProperMotionAscension = Parse(record[10]);
      ProperMotionDeclination = Parse(record[11]);

      RadialVelocity = Parse(record[12]);
      Magnitude = Parse(record[13]);
      MagnitudeAbsolute = Parse(record[14]);

      Spectral = record[15].Trim();

      ColorIndex = Parse(record[16]);

      X = Parse(record[17]);
      Y = Parse(record[18]);
      Z = Parse(record[19]);

      VelocityX = Parse(record[20]);
      VelocityY = Parse(record[21]);
      VelocityZ = Parse(record[22]);

      RadiansRA = Parse(record[23]);
      RadiansDec = Parse(record[24]);
      RadiansVelocityRA = Parse(record[25]);
      RadiansVelocityDec = Parse(record[26]);

      Bayer = record[27].Trim();
      Flamsteed = record[28].Trim();

      if (Constellation.TryParse(record[29].Trim(), out var con))
        Constellation = con;
      else
        Constellation = Constellation.Unknown;

      Companion = record[30].Trim();
      CompanionPrimary = record[31].Trim();
      CompanionBase = record[32].Trim();

      Luminosity = Parse(record[33]);

      VariableDesignation = record[34].Trim();

      VariableMin = Parse(record[35]);
      VariableMax = Parse(record[36]);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Ctalog
    /// </summary>
    public static IReadOnlyList<Star> Catalog => s_Catalog;

    /// <summary>
    /// Id
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// The star's ID in the Hipparcos catalog, if known
    /// </summary>
    public string HipparcosId { get; }

    /// <summary>
    /// The star's ID in the Henry Draper catalog, if known.
    /// </summary>
    public string HenryDraperId { get; }

    /// <summary>
    /// The star's ID in the Harvard Revised catalog, which is the same as its number in the Yale Bright Star Catalog.
    /// </summary>
    public string HarvardRevisedId { get; }

    /// <summary>
    /// The star's ID in the third edition of the Gliese Catalog of Nearby Stars.
    /// </summary>
    public string GlieseId { get; }

    /// <summary>
    /// The Bayer / Flamsteed designation, primarily from the Fifth Edition of the Yale Bright Star Catalog. 
    /// This is a combination of the two designations. The Flamsteed number, if present, is given first; 
    /// then a three-letter abbreviation for the Bayer Greek letter; the Bayer superscript number, if present; 
    /// and finally, the three-letter constellation abbreviation. Thus Alpha Andromedae has the field value 
    /// "21Alp And", and Kappa1 Sculptoris (no Flamsteed number) has "Kap1Scl
    /// </summary>
    public string BayerFlamsteed { get; }

    /// <summary>
    /// A common name for the star, such as "Barnard's Star" or "Sirius"
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Right Ascension for epoch and equinox 2000.0
    /// </summary>
    public double RightAscension { get; }

    /// <summary>
    /// Declination for epoch and equinox 2000.0
    /// </summary>
    public double Declination { get; }

    /// <summary>
    /// The star's distance in parsecs
    /// A value >= 100000 indicates missing or dubious (e.g., negative) parallax data in Hipparcos.
    /// </summary>
    public double Distance { get; }

    /// <summary>
    /// The star's proper motion in Right Ascension, in milliarcseconds per year
    /// </summary>
    public double ProperMotionAscension { get; }

    /// <summary>
    /// The star's proper motion in Declination, in milliarcseconds per year
    /// </summary>
    public double ProperMotionDeclination { get; }

    /// <summary>
    /// The star's radial velocity in km/sec, where known.
    /// </summary>
    public double RadialVelocity { get; }

    /// <summary>
    /// The star's apparent visual magnitude.
    /// </summary>
    public double Magnitude { get; }

    /// <summary>
    /// The star's absolute visual magnitude (its apparent magnitude from a distance of 10 parsecs).
    /// </summary>
    public double MagnitudeAbsolute { get; }

    /// <summary>
    /// Spectral Type
    /// </summary>
    public string Spectral { get; }

    /// <summary>
    /// The star's color index (blue magnitude - visual magnitude), where known 
    /// </summary>
    public double ColorIndex { get; }

    /// <summary>
    /// The Cartesian coordinates of the star, in a system based on the equatorial coordinates as seen from Earth
    /// X is in the direction of the vernal equinox (at epoch 2000)
    /// </summary>
    public double X { get; }

    /// <summary>
    /// The Cartesian coordinates of the star, in a system based on the equatorial coordinates as seen from Earth
    /// Y in the direction of R.A. 6 hours, declination 0 degrees
    /// </summary>
    public double Y { get; }

    /// <summary>
    /// The Cartesian coordinates of the star, in a system based on the equatorial coordinates as seen from Earth
    /// Z towards the north celestial pole
    /// </summary>
    public double Z { get; }

    /// <summary>
    /// The Cartesian coordinates of the star, in a system based on the equatorial coordinates as seen from Earth
    /// X is in the direction of the vernal equinox (at epoch 2000)
    /// </summary>
    public double VelocityX { get; }

    /// <summary>
    /// The Cartesian coordinates of the star, in a system based on the equatorial coordinates as seen from Earth
    /// Y in the direction of R.A. 6 hours, declination 0 degrees
    /// </summary>
    public double VelocityY { get; }

    /// <summary>
    /// The Cartesian coordinates of the star, in a system based on the equatorial coordinates as seen from Earth
    /// Z towards the north celestial pole
    /// </summary>
    public double VelocityZ { get; }

    /// <summary>
    /// The position in radians Right Ascension
    /// </summary>
    public double RadiansRA { get; }

    /// <summary>
    /// The position in radians Declination
    /// </summary>
    public double RadiansDec { get; }

    /// <summary>
    /// Velocity in radians per year Right Ascension
    /// </summary>
    public double RadiansVelocityRA { get; }

    /// <summary>
    /// Velocity in radians per year Declination
    /// </summary>
    public double RadiansVelocityDec { get; }

    /// <summary>
    /// The Bayer designation as a distinct value 
    /// </summary>
    public string Bayer { get; }

    /// <summary>
    /// The Flamsteed designation as a distinct value 
    /// </summary>
    public string Flamsteed { get; }

    /// <summary>
    /// The Flamsteed designation as a distinct value 
    /// </summary>
    public Constellation Constellation { get; }

    /// <summary>
    /// ID of companion star
    /// </summary>
    public string Companion { get; }

    /// <summary>
    /// ID of primary star for this component
    /// </summary>
    public string CompanionPrimary { get; }

    /// <summary>
    /// Catalog ID or name for this multi-star system. Currently only used for Gliese stars.
    /// </summary>
    public string CompanionBase { get; }

    /// <summary>
    /// Star's luminosity as a multiple of Solar luminosity.
    /// </summary>
    public double Luminosity { get; }

    /// <summary>
    /// Star's standard variable star designation, when known.
    /// </summary>
    public string VariableDesignation { get; }

    /// <summary>
    /// Star's approximate magnitude range, for variables (min) 
    /// This value is based on the Hp magnitudes for the range in the original Hipparcos catalog, 
    /// adjusted to the V magnitude scale to match the "mag" field.
    /// </summary>
    public double VariableMin { get; }

    /// <summary>
    /// Star's approximate magnitude range, for variables (max)
    /// This value is based on the Hp magnitudes for the range in the original Hipparcos catalog, 
    /// adjusted to the V magnitude scale to match the "mag" field.
    /// </summary>
    public double VariableMax { get; }

    #endregion Public
  }
}
