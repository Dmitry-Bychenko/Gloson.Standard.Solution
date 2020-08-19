using Gloson.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Numerics.Distributions {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Uniform Distribution Sampler
  /// </summary>
  /// <see cref="https://en.wikipedia.org/wiki/Latin_hypercube_sampling"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface ISampler {
    /// <summary>
    /// Generate sample for [0..1] * [0..1] * ... * [0..1] (Dimension times) range
    /// </summary>
    /// <param name="dimensions">Dimensions</param>
    /// <param name="count">Approximate number of samples</param>
    IEnumerable<double[]> Generate(int dimensions, int count);
  }

  #region Standard Samplers

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Monte Carlo Sampler
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------  public sealed class SamplerMonteCarlo : ISampler {

  public sealed class SamplerMonteCarlo : ISampler {
    #region Private Data

    private readonly Random m_Random;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Sampling with Seed
    /// </summary>
    /// <param name="seed">Seed</param>
    public SamplerMonteCarlo(int seed) {
      m_Random = new Random(seed);
    }

    /// <summary>
    /// Sampling 
    /// </summary>
    public SamplerMonteCarlo() {
      m_Random = new Random();
    }

    #endregion Create

    #region ISampler

    /// <summary>
    /// Generate [0..1] * [0..1] * ... * [0..1] samples 
    /// </summary>
    /// <param name="dimensions">Dimensions</param>
    /// <param name="count">Approximate number of samples</param>
    public IEnumerable<double[]> Generate(int dimensions, int count) {
      if (dimensions <= 0)
        throw new ArgumentOutOfRangeException(nameof(dimensions));
      else if (count < 0)
        throw new ArgumentOutOfRangeException(nameof(count));

      for (int i = 0; i < count; ++i) {
        double[] result = new double[dimensions];

        for (int c = result.Length - 1; c >= 0; --c)
          result[c] = m_Random.NextDouble();

        yield return result;
      }
    }

    #endregion ISampler
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Latin Hypercube sampler 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class SamplerLatinHypercube : ISampler {
    #region Private Data

    private readonly Random m_Random;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Sampling with Seed
    /// </summary>
    /// <param name="seed">Seed</param>
    public SamplerLatinHypercube(int seed) {
      m_Random = new Random(seed);
    }

    /// <summary>
    /// Sampling 
    /// </summary>
    public SamplerLatinHypercube() {
      m_Random = new Random();
    }

    #endregion Create

    #region ISampler

    /// <summary>
    /// Generate [0..1] * [0..1] * ... * [0..1] samples 
    /// </summary>
    /// <param name="dimensions">Dimensions</param>
    /// <param name="count">Approximate number of samples</param>
    public IEnumerable<double[]> Generate(int dimensions, int count) {
      if (dimensions <= 0)
        throw new ArgumentOutOfRangeException(nameof(dimensions));
      else if (count < 0)
        throw new ArgumentOutOfRangeException(nameof(count));

      double h = 1.0 / count;

      List<int[]> ranges = Enumerable
        .Range(0, dimensions)
        .Select(dimension => Enumerable
          .Range(0, count)
          .OrderBy(x => m_Random.NextDouble())
          .ToArray())
        .ToList();

      for (int i = 0; i < count; ++i) {
        double[] result = new double[dimensions];

        for (int c = 0; c < result.Length; ++c) {
          int d = ranges[c][i];

          result[c] = h * d + m_Random.NextDouble() * h;
        }

        yield return result;
      }
    }

    #endregion ISampler
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Orthogonal sampler 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class SamplerOrthogonal : ISampler {
    #region Private Data

    private readonly Random m_Random;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Sampling with Seed
    /// </summary>
    /// <param name="seed">Seed</param>
    public SamplerOrthogonal(int seed) {
      m_Random = new Random(seed);
    }

    /// <summary>
    /// Sampling 
    /// </summary>
    public SamplerOrthogonal() {
      m_Random = new Random();
    }

    #endregion Create

    #region ISampler

    /// <summary>
    /// Generate [0..1] * [0..1] * ... * [0..1] samples 
    /// </summary>
    /// <param name="dimensions">Dimensions</param>
    /// <param name="count">Approximate number of samples</param>
    public IEnumerable<double[]> Generate(int dimensions, int count) {
      if (dimensions <= 0)
        throw new ArgumentOutOfRangeException(nameof(dimensions));
      else if (count < 0)
        throw new ArgumentOutOfRangeException(nameof(count));

      double root = Math.Pow(count, 1.0 / dimensions);

      int parts = (int)root + (root % 1 == 0 ? 0 : 1);
      double h = 1.0 / parts;

      int[] shifts = Enumerable
        .Range(0, parts)
        .Select(i => i)
        .ToArray();

      foreach (int[] record in shifts.OrderedWithReplacement(dimensions)) {
        double[] result = record
          .Select(i => i * h + m_Random.NextDouble() * h)
          .ToArray();

        yield return result;
      }
    }

    #endregion ISampler
  }

  #endregion Standard Samplers

}
