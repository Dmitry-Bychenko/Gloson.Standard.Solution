using Gloson.ComponentModel;
using Gloson.Numerics.Calculus;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;

namespace Gloson.Numerics.Distributions {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Continuous Probability Distribution (like Normal, LogNormal, Triangle etc.)
  /// Thread Safe
  /// </summary>
  /// <see cref="https://en.wikipedia.org/wiki/Probability_distribution#Continuous_probability_distribution"/>
  /// <threadsafety static="true" instance="true"/>
  // 
  //-------------------------------------------------------------------------------------------------------------------

  public interface IContinuousProbabilityDistribution {
    /// <summary>
    /// Probability Density Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Probability_density_function"/>
    double Pdf(double x);

    /// <summary>
    /// Cumulative Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Cumulative_distribution_function"/>
    double Cdf(double x);

    /// <summary>
    /// Quantile Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Quantile_function"/>
    double Qdf(double x);
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Extensions on IContinuousProbabilityDistribution
  /// Thread Safe
  /// </summary>
  /// <see cref="IContinuousProbabilityDistribution/>
  /// <threadsafety static="true" instance="true"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class ContinuousProbabilityDistributionExtensions {
    #region Public

    /// <summary>
    /// Random Distribution Function (Random Generator)
    /// </summary>
    /// <param name="distribution">Distribution to use</param>
    /// <param name="seed">Seed</param>
    /// <returns>Random value, distributed according to the distribution</returns>
    public static ContinuousRandom Rdf(IContinuousProbabilityDistribution distribution, int seed) {
      if (null == distribution)
        throw new ArgumentNullException(nameof(distribution));

      return new ContinuousRandom(distribution, seed);
    }

    /// <summary>
    /// Random Generator
    /// </summary>
    /// <param name="distribution">Distribution to use</param>
    /// <param name="seed">Seed</param>
    /// <returns>Random value, distributed according to the distribution</returns>
    public static ContinuousRandom Random(IContinuousProbabilityDistribution distribution) {
      if (null == distribution)
        throw new ArgumentNullException(nameof(distribution));

      return new ContinuousRandom(distribution);
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Abstract IContinuousProbabilityDistribution implementation
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  [LocalizedDescription("Abstract Probability Distribution")]
  public abstract class ContinuousProbabilityDistribution : IContinuousProbabilityDistribution {
    #region Private Data

    // Thread Safe Random
    private static readonly ThreadLocal<Random> s_Random = new ThreadLocal<Random>(() => {
      int seed;

      using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider()) {
        byte[] seedData = new byte[sizeof(int)];

        provider.GetBytes(seedData);

        seed = BitConverter.ToInt32(seedData, 0);
      }

      return new Random(seed);
    });

    #endregion Private Data

    #region IContinuousProbabilityDistribution

    /// <summary>
    /// Cumulative Density Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Cumulative_distribution_function"/>
    public abstract double Cdf(double x);

    /// <summary>
    /// Probability Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Probability_density_function"/>
    public abstract double Pdf(double x);

    /// <summary>
    /// Quantile Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Quantile_function"/>
    public virtual double Qdf(double x) {
      if (x == 0)
        return double.NegativeInfinity;
      else if (x == 1)
        return double.PositiveInfinity;
      else if (x < 0 || x > 1)
        throw new ArgumentOutOfRangeException(nameof(x));

      Func<double, double> cdf = Cdf;

      return cdf.Inverse(double.MinValue, double.MaxValue)(x);
    }

    /// <summary>
    /// Random Distribution Function (not a part of IContinuousProbabilityDistribution)
    /// </summary>
    /// <see cref="ContinuousRandom"/>
    /// <returns>Random Value distribured according the distribution</returns>
    public double Rdf() {
      return Qdf(s_Random.Value.NextDouble());
    }

    #endregion IContinuousProbabilityDistribution

    #region Public

    /// <summary>
    /// Mean
    /// </summary>
    public abstract double Mean { get; }

    /// <summary>
    /// Standard Error
    /// </summary>
    public abstract double StandardError { get; }

    /// <summary>
    /// Standard Error
    /// </summary>
    public virtual double Variance {
      get {
        double se = StandardError;

        return se * se;
      }
    }

    /// <summary>
    /// Localized Title
    /// </summary>
    public virtual string LocalizedTitle {
      get {
        return GetType().GetCustomAttribute<DescriptionAttribute>()?.Description ?? GetType().Name;
      }
    }

    /// <summary>
    /// To String (debug only)
    /// </summary>
    public override abstract string ToString(); // <- We force developers to implement it

    #endregion Public
  }

}
