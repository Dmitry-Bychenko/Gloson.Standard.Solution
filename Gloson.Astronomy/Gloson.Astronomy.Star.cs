using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Gloson.Astronomy {

  // ModuleInitializerAttribute

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

  public sealed class Star {
    #region Constants

    private const string Address = @"https://raw.githubusercontent.com/astronexus/HYG-Database/master/hygdata_v3.csv";

    #endregion Constants

    #region Private Data

    private static Task<IReadOnlyList<Star>> s_MainTask;

    private static List<Star> s_Catalog = new();

    private static HttpClient s_Http;

    private static CookieContainer s_CookieContainer;

    #endregion Private Data

    #region Algorithm

    [ModuleInitializer]
    internal static void CoreModuleLoad() {
      s_MainTask = CoreLoad();
    }

    private static void CoreCreateClient() {
      try {
        ServicePointManager.SecurityProtocol =
          SecurityProtocolType.Tls |
          SecurityProtocolType.Tls11 |
          SecurityProtocolType.Tls12;
      }
      catch (NotSupportedException) {
        ;
      }

      s_CookieContainer = new CookieContainer();

      var handler = new HttpClientHandler() {
        CookieContainer = s_CookieContainer,
        Credentials = CredentialCache.DefaultCredentials,
      };

      s_Http = new HttpClient(handler);
    }

    private static async Task<IReadOnlyList<Star>> CoreLoad() {
      s_Catalog = new List<Star>();

      using var req = new HttpRequestMessage {
        Method = HttpMethod.Get,
        RequestUri = new Uri(Address),
        Headers = {
          { HttpRequestHeader.Accept.ToString(), "text/csv" },
        },
        Content = new StringContent("", Encoding.UTF8, "text/csv")
      };

      var response = await s_Http
        .SendAsync(req)
        .ConfigureAwait(false);

      using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

      var reader = new StreamReader(stream);

      await Task.Run(() => {
        bool firstTime = true;

        while (true) {
          string line = reader.ReadLine();

          if (line is null)
            break;

          if (string.IsNullOrWhiteSpace(line))
            continue;

          if (firstTime) {
            firstTime = false;

            continue;
          }

          string[] array = line.Split(',');

          s_Catalog.Add(new Star(array));
        }
      });

      return s_Catalog;
    }

    private static double Parse(string value) => string.IsNullOrWhiteSpace(value)
      ? double.NaN
      : double.Parse(value, CultureInfo.InvariantCulture);

    #endregion Algorithm

    #region Create

    static Star() {
      CoreCreateClient();
    }

    private Star(string[] items) {
      Id = items[0];

      HipparcosId = items[1] ?? "";
      HenryDraperId = items[2] ?? "";
      HarvardRevisedId = items[3] ?? "";
      GlieseId = items[4] ?? "";
      BayerFlamsteed = items[5] ?? "";
      Name = items[6] ?? "";

      RightAscension = Parse(items[7]);
      Declination = Parse(items[8]);

      Distance = Parse(items[9]);

      ProperMotionAscension = Parse(items[10]);
      ProperMotionDeclination = Parse(items[11]);

      RadialVelocity = Parse(items[12]);
      Magnitude = Parse(items[13]);
      MagnitudeAbsolute = Parse(items[14]);

      Spectral = items[15].Trim();

      ColorIndex = Parse(items[16]);

      X = Parse(items[17]);
      Y = Parse(items[18]);
      Z = Parse(items[19]);

      VelocityX = Parse(items[20]);
      VelocityY = Parse(items[21]);
      VelocityZ = Parse(items[22]);

      RadiansRA = Parse(items[23]);
      RadiansDec = Parse(items[24]);
      RadiansVelocityRA = Parse(items[25]);
      RadiansVelocityDec = Parse(items[26]);

      Bayer = items[27].Trim();
      Flamsteed = items[28].Trim();

      Constellation = Constellation.TryParse(items[29].Trim(), out var con)
        ? con
        : Constellation.Unknown;

      Companion = items[30].Trim();
      CompanionPrimary = items[31].Trim();
      CompanionBase = items[32].Trim();

      Luminosity = Parse(items[33]);

      VariableDesignation = items[34].Trim();

      VariableMin = Parse(items[35]);
      VariableMax = Parse(items[36]);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Catalog
    /// </summary>
    public static async Task<IReadOnlyList<Star>> Catalog() {
      await s_MainTask;

      return s_Catalog;
    }

    /// <summary>
    /// Stars
    /// </summary>
    public static Task<IReadOnlyList<Star>> Stars => CoreLoad();

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
