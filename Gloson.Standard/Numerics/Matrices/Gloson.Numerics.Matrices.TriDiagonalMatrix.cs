using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Numerics.Matrices {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Tridiagonal matrix
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class TriDiagonalMatrix
    : IEquatable<TriDiagonalMatrix>,
      IReadOnlyList<(double a, double b, double c)> {

    #region Private Data

    private readonly List<double> m_A = new List<double>();
    private readonly List<double> m_B = new List<double>();
    private readonly List<double> m_C = new List<double>();

    double[] m_Bs;

    private double m_Determinant = double.NaN;

    #endregion Private Data

    #region Algorithm

    private double CoreDeterminant() {
      double[] f = new double[m_B.Count + 2];

      f[0] = 0;
      f[1] = 1;
      f[2] = m_B[0];

      for (int i = 1; i < m_B.Count; ++i) {
        f[i + 2] = m_B[i] * f[i - 1 + 2] - m_A[i] * m_C[i - 1] * f[i - 2 + 2];
      }

      return f[f.Length - 1];
    }

    #endregion Algorothm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public TriDiagonalMatrix(IEnumerable<(double a, double b, double c)> lines) {
      if (null == lines)
        throw new ArgumentNullException(nameof(lines));

      foreach (var (a, b, c) in lines) {
        m_A.Add(a);
        m_B.Add(b);
        m_C.Add(c);
      }

      if (m_A.Count > 0) {
        m_A[0] = 0.0;
        m_C[m_C.Count - 1] = 0.0;
      }
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Left line
    /// </summary>
    public IReadOnlyList<double> A => m_A;

    /// <summary>
    /// Central line
    /// </summary>
    public IReadOnlyList<double> B => m_B;

    /// <summary>
    /// Right line
    /// </summary>
    public IReadOnlyList<double> C => m_C;

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_B.Count;

    /// <summary>
    /// Cells
    /// </summary>
    public double this[int row, int column] {
      get {
        if (row < 0 || column < 0 || row >= m_B.Count || column >= m_B.Count)
          return 0;
        else if (column == row - 1)
          return m_A[column];
        else if (column == row)
          return m_B[column];
        else if (column == row + 1)
          return m_C[column];

        return 0;
      }
    }

    /// <summary>
    /// Determinant
    /// </summary>
    public double Determinant {
      get {
        if (!double.IsNaN(m_Determinant))
          return m_Determinant;

        m_Determinant = CoreDeterminant();

        return m_Determinant;
      }
    }

    /// <summary>
    /// Solve Matrix * x = f equation
    /// </summary>
    public double[] Solve(IEnumerable<double> f) {
      if (null == f)
        throw new ArgumentNullException(nameof(f));

      double[] free = f.Take(m_B.Count + 1).ToArray();

      if (m_B.Count != free.Length)
        throw new ArgumentOutOfRangeException(nameof(free));

      if (null == m_Bs) {
        m_Bs = new double[m_B.Count];
        m_Bs[0] = m_B[0];

        for (int i = 1; i < m_B.Count; ++i)
          m_Bs[i] = m_B[i] - m_A[i] / m_Bs[i - 1] * m_C[i - 1];
      }

      double[] Fs = new double[free.Length];

      Fs[0] = free[0];

      for (int i = 1; i < free.Length; ++i)
        Fs[i] = free[i] - m_A[i] / m_Bs[i - 1] * Fs[i - 1];

      double[] result = new double[free.Length];

      result[result.Length - 1] = Fs[Fs.Length - 1] / m_Bs[Fs.Length - 1];

      for (int i = free.Length - 2; i >= 0; --i)
        result[i] = (Fs[i] - m_C[i] * result[i + 1]) / m_Bs[i];

      return result;
    }

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals 
    /// </summary>
    public static bool operator ==(TriDiagonalMatrix left, TriDiagonalMatrix right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (null == left || null == right)
        return false;
      else
        return left.Equals(right);
    }

    /// <summary>
    /// Not Equals 
    /// </summary>
    public static bool operator !=(TriDiagonalMatrix left, TriDiagonalMatrix right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (null != left || null != right)
        return false;
      else
        return !left.Equals(right);
    }

    #endregion Operators

    #region IEquatable<TriDiagonalMatrix>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(TriDiagonalMatrix other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      return m_A.SequenceEqual(other.m_A) &&
             m_B.SequenceEqual(other.m_B) &&
             m_C.SequenceEqual(other.m_C);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => obj is TriDiagonalMatrix matrix && Equals(matrix);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() {
      if (m_B.Count <= 0)
        return 0;

      return m_B.Count ^ m_B[0].GetHashCode();
    }

    #endregion IEquatable<TriDiagonalMatrix>

    #region IReadOnlyList<(double a, double b, double c)>

    /// <summary>
    /// Indexer
    /// </summary>
    public (double a, double b, double c) this[int index] {
      get =>
        (index >= 0 && index < m_B.Count)
          ? (m_A[index], m_B[index], m_C[index])
          : throw new ArgumentOutOfRangeException(nameof(index));
    }

    /// <summary>
    /// Typed Enumerator
    /// </summary>
    public IEnumerator<(double a, double b, double c)> GetEnumerator() => Enumerable
      .Range(0, m_B.Count)
      .Select(i => (m_A[0], m_B[i], m_C[i]))
      .GetEnumerator();

    /// <summary>
    /// Typeless Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    #endregion IReadOnlyList<(double a, double b, double c)>
  }

}
