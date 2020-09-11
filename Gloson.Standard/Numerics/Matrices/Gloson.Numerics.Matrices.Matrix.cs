using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Gloson.Numerics.Matrices {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Real Value Matrix 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class Matrix
    : ICloneable,
      IEquatable<Matrix>,
      IFormattable,
      IEnumerable<double> {

    #region Private Data

    // Items
    internal double[][] m_Items;

    private double m_Determinant = double.NaN;

    private int m_Rank = -1;

    private double[] m_LinearSolutions;

    #endregion Private Data

    #region Algorithm 
    #endregion Algorithm

    #region Create

    // Empty Constructor
    private Matrix() { }

    // Low level constructor
    private Matrix(double[][] items)
      : this() {
      m_Items = items;
    }

    // Standard Constructor
    private Matrix(int lines, int columns)
      : this() {

      if (lines <= 0)
        throw new ArgumentOutOfRangeException(nameof(lines));
      else if (columns <= 0)
        throw new ArgumentOutOfRangeException(nameof(columns));

      m_Items = Enumerable
        .Range(0, lines)
        .Select(_ => new double[columns])
        .ToArray();
    }

    // Standard Constructor (square matrix)
    private Matrix(int size) : this(size, size) { }

    /// <summary>
    /// Union matrix
    /// </summary>
    /// <param name="size">Size</param>
    public static Matrix Union(int size) {
      Matrix result = new Matrix(size);

      for (int i = result.LineCount - 1; i >= 0; --i)
        result.m_Items[i][i] = 1;

      return result;
    }

    /// <summary>
    /// Zero matrix
    /// </summary>
    /// <param name="size">Size</param>
    public static Matrix Zero(int size) => new Matrix(size);
 
    /// <summary>
    /// Create Matrix
    /// </summary>
    /// <param name="lines">Lines</param>
    /// <param name="columns">Columns</param>
    /// <param name="createItem">Item at line and column</param>
    /// <returns></returns>
    public static Matrix Create(int lines, int columns, Func<int, int, double> createItem) {
      if (lines <= 0)
        throw new ArgumentOutOfRangeException(nameof(lines));
      else if (columns <= 0)
        throw new ArgumentOutOfRangeException(nameof(columns));
      else if (null == createItem)
        throw new ArgumentNullException(nameof(createItem));

      Matrix result = new Matrix(lines, columns);

      for (int r = 0; r < lines; ++r)
        for (int c = 0; c < columns; ++c)
          result.m_Items[r][c] = createItem(r, c);

      return result;
    }

    /// <summary>
    /// Create Square Matrix
    /// </summary>
    /// <param name="size">Size</param>
    /// <param name="createItem">Item at line and column</param>
    /// <returns></returns>
    public static Matrix Create(int size, Func<int, int, double> createItem) => Create(size, size, createItem);

    /// <summary>
    /// Create
    /// </summary>
    public static Matrix Create(IEnumerable<IEnumerable<double>> data) {
      if (null == data)
        throw new ArgumentNullException(nameof(data));

      var copy = data
        .Select(line => line.ToArray())
        .ToArray();

      return new Matrix(copy);
    }

    #endregion Create

    #region Public

    #region General

    /// <summary>
    /// Perform a func and create a new matrix
    /// </summary>
    /// <param name="func">func: value, line, column returns a new value</param>
    /// <returns>New Matrix</returns>
    public Matrix Perform(Func<double, int, int, double> func) {
      if (null == func)
        throw new ArgumentNullException(nameof(func));

      Matrix result = new Matrix(LineCount, ColumnCount);

      for (int r = 0; r < m_Items.Length; ++r)
        for (int c = 0; c < m_Items[r].Length; ++c)
          result.m_Items[r][c] = func(m_Items[r][c], r, c);

      return result;
    }

    /// <summary>
    /// Perform a func and create a new matrix
    /// </summary>
    /// <param name="func">func: line, column returns a new value</param>
    /// <returns>New Matrix</returns>
    public Matrix Perform(Func<int, int, double> func) {
      if (null == func)
        throw new ArgumentNullException(nameof(func));

      Matrix result = new Matrix(LineCount, ColumnCount);

      for (int r = 0; r < m_Items.Length; ++r)
        for (int c = 0; c < m_Items[r].Length; ++c)
          result.m_Items[r][c] = func(r, c);

      return result;
    }

    /// <summary>
    /// Perform a func and create a new matrix
    /// </summary>
    /// <param name="func">func: value returns a new value</param>
    /// <returns>New Matrix</returns>
    public Matrix Perform(Func<double, double> func) {
      if (null == func)
        throw new ArgumentNullException(nameof(func));

      Matrix result = new Matrix(LineCount, ColumnCount);

      for (int r = 0; r < m_Items.Length; ++r)
        for (int c = 0; c < m_Items[r].Length; ++c)
          result.m_Items[r][c] = func(m_Items[r][c]);

      return result;
    }

    /// <summary>
    /// Lines
    /// </summary>
    public int LineCount => m_Items.Length;

    /// <summary>
    /// Columns
    /// </summary>
    public int ColumnCount => m_Items[0].Length;

    /// <summary>
    /// Item Count
    /// </summary>
    public int ItemCount => m_Items.Length * m_Items[0].Length;

    /// <summary>
    /// Lines
    /// </summary>
    public IEnumerable<double[]> Lines {
      get {
        foreach (var line in m_Items)
          yield return line.ToArray();
      }
    }

    /// <summary>
    /// Lines
    /// </summary>
    public IEnumerable<double[]> Columns {
      get {
        int N = ColumnCount;

        for (int i = 0; i < N; ++i) {
          double[] result = new double[m_Items.Length];

          for (int j = 0; j < result.Length; ++j)
            result[j] = m_Items[i][j];

          yield return result;
        }
      }
    }

    /// <summary>
    /// Items
    /// </summary>
    public IEnumerable<double> Items {
      get {
        foreach (var line in m_Items)
          foreach (var item in line)
            yield return item;
      }
    }

    /// <summary>
    /// Cell
    /// </summary>
    /// <param name="line">Line</param>
    /// <param name="column">Column</param>
    public double Cell(int line, int column) {
      return m_Items[line][column];
    }

    /// <summary>
    /// Cell
    /// </summary>
    /// <param name="line">Line</param>
    /// <param name="column">Column</param>
    public double this[int line, int column] => Cell(line, column);

    #endregion General

    #region Standard

    /// <summary>
    /// Transpose
    /// </summary>
    public Matrix Transpose() {
      Matrix result = new Matrix(ColumnCount, LineCount);

      for (int r = result.m_Items.Length - 1; r >= 0; --r)
        for (int c = result.m_Items[0].Length - 1; c >= 0; --c)
          result.m_Items[r][c] = m_Items[c][r];

      return result;
    }

    /// <summary>
    /// Determinant
    /// </summary>
    public double Determinant {
      get {
        if (double.IsNaN(m_Determinant))
          m_Determinant = MatrixLowLevel.Determinant(m_Items);

        return m_Determinant;
      }
    }

    /// <summary>
    /// Rank
    /// </summary>
    public int Rank {
      get {
        if (m_Rank < 0)
          m_Rank = MatrixLowLevel.Rank(m_Items);

        return m_Rank;
      }
    }

    /// <summary>
    /// Inverse
    /// </summary>
    public Matrix Inverse() {
      if (ColumnCount != LineCount)
        throw new InvalidOperationException("Only square matrix can be inversed.");

      try {
        return new Matrix(MatrixLowLevel.Inverse(m_Items));
      }
      catch (ArgumentException) {
        throw new InvalidOperationException("Degenerated matrix can't be inversed.");
      }
    }

    /// <summary>
    /// Pseudo Inverse
    /// </summary>
    public Matrix PseudoInverse() {
      double[][] tran = MatrixLowLevel.Transpose(m_Items);

      double[][] result = MatrixLowLevel.Multiply(MatrixLowLevel.Inverse(MatrixLowLevel.Multiply(tran, m_Items)), tran);

      return new Matrix(result);
    }

    /// <summary>
    /// Linear Solutions
    /// </summary>
    public double[] LinearSoution {
      get {
        if (null == m_LinearSolutions)
          m_LinearSolutions = MatrixLowLevel.Solve(m_Items);

        return m_LinearSolutions;
      }
    }

    #endregion Standard

    #region Decomposition

    /// <summary>
    /// Cholesky Decomposition
    /// </summary>
    /// <seealso cref="https://en.wikipedia.org/wiki/Cholesky_decomposition"/>
    public Matrix Cholesky() => new Matrix(MatrixLowLevel.Cholesky(m_Items));

    /// <summary>
    /// Householder factors (QR-decomposition)
    /// H1, H2, ..., Hn, R
    /// Q = H1 * H2 * ... * Hn
    /// </summary>
    /// <see cref="http://www.seas.ucla.edu/~vandenbe/133A/lectures/qr.pdf"/>
    public IEnumerable<Matrix> HouseholderFactors() {
      foreach (var item in MatrixLowLevel.HouseholderFactors(m_Items))
        yield return new Matrix(item);
    }

    /// <summary>
    /// QR Decomposition
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/QR_decomposition"/>
    public void QR(out Matrix q, out Matrix r) {
      MatrixLowLevel.QR(m_Items, out var qq, out var rr);

      q = new Matrix(qq);
      r = new Matrix(rr);
    }

    #endregion Decomposition

    #endregion Public

    #region Operators

    #region Comparison

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(Matrix left, Matrix right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (null == right)
        return false;
      else if (null == left)
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(Matrix left, Matrix right) {
      if (ReferenceEquals(left, right))
        return false;
      else if (null == right)
        return true;
      else if (null == left)
        return true;

      return !left.Equals(right);
    }

    #endregion Comparison

    #region Arithmetics

    /// <summary>
    /// Unary +
    /// </summary>
    public static Matrix operator +(Matrix value) {
      if (null == value)
        throw new ArgumentNullException(nameof(value));

      return value;
    }

    /// <summary>
    /// Unary -
    /// </summary>
    public static Matrix operator -(Matrix value) {
      if (null == value)
        throw new ArgumentNullException(nameof(value));

      Matrix result = value.Clone();

      foreach (double[] line in result.m_Items)
        for (int i = line.Length; i >= 0; --i)
          line[i] = -line[i];

      return result;
    }

    /// <summary>
    /// Multiplication by number
    /// </summary>
    public static Matrix operator *(Matrix matrix, double value) {
      if (null == matrix)
        throw new ArgumentNullException(nameof(matrix));

      if (value == 1)
        return matrix;

      Matrix result = matrix.Clone();

      foreach (double[] line in result.m_Items)
        for (int i = line.Length; i >= 0; --i)
          line[i] = line[i] * value;

      return result;
    }

    /// <summary>
    /// Multiplication by number
    /// </summary>
    public static Matrix operator *(double value, Matrix matrix) => matrix * value;

    /// <summary>
    /// Division by number
    /// </summary>
    public static Matrix operator /(Matrix matrix, double value) {
      if (null == matrix)
        throw new ArgumentNullException(nameof(matrix));

      if (value == 1)
        return matrix;

      Matrix result = matrix.Clone();

      foreach (double[] line in result.m_Items)
        for (int i = line.Length; i >= 0; --i)
          line[i] = line[i] / value;

      return result;
    }

    /// <summary>
    /// Matrix Addition
    /// </summary>
    public static Matrix operator +(Matrix left, Matrix right) {
      if (null == left)
        throw new ArgumentNullException(nameof(left));
      else if (null == right)
        throw new ArgumentNullException(nameof(right));

      if (left.LineCount != right.LineCount)
        throw new ArgumentException($"Right matrix must have {left.LineCount} lines, actual {right.LineCount}", nameof(right));
      else if (left.ColumnCount != right.ColumnCount)
        throw new ArgumentException($"Right matrix must have {left.ColumnCount} columns, actual {right.ColumnCount}", nameof(right));

      Matrix result = new Matrix(left.LineCount, left.LineCount);

      for (int r = right.LineCount - 1; r >= 0; --r)
        for (int c = right.ColumnCount - 1; c >= 0; --c)
          result.m_Items[r][c] = left.m_Items[r][c] + right.m_Items[r][c];

      return result;
    }

    /// <summary>
    /// Matrix Subtractions
    /// </summary>
    public static Matrix operator -(Matrix left, Matrix right) {
      if (null == left)
        throw new ArgumentNullException(nameof(left));
      else if (null == right)
        throw new ArgumentNullException(nameof(right));

      if (left.LineCount != right.LineCount)
        throw new ArgumentException($"Right matrix must have {left.LineCount} lines, actual {right.LineCount}", nameof(right));
      else if (left.ColumnCount != right.ColumnCount)
        throw new ArgumentException($"Right matrix must have {left.ColumnCount} columns, actual {right.ColumnCount}", nameof(right));

      Matrix result = new Matrix(left.LineCount, left.LineCount);

      for (int r = right.LineCount - 1; r >= 0; --r)
        for (int c = right.ColumnCount - 1; c >= 0; --c)
          result.m_Items[r][c] = left.m_Items[r][c] - right.m_Items[r][c];

      return result;
    }

    /// <summary>
    /// Matrix Mutiplication
    /// </summary>
    public static Matrix operator *(Matrix left, Matrix right) {
      if (null == left)
        throw new ArgumentNullException(nameof(left));
      else if (null == right)
        throw new ArgumentNullException(nameof(right));

      if (left.ColumnCount != right.LineCount)
        throw new ArgumentException($"Right matrix must have {left.ColumnCount} liness, actual {right.LineCount}", nameof(right));

      Matrix result = new Matrix(left.LineCount, right.ColumnCount);

      for (int r = result.m_Items.Length - 1; r >= 0; --r)
        for (int c = result.m_Items[0].Length - 1; c >= 0; --c) {
          double v = 0.0;

          for (int i = right.LineCount - 1; i >= 0; --i)
            v += left.m_Items[r][i] * right.m_Items[i][c];

          result.m_Items[r][c] = v;
        }

      return result;
    }

    /// <summary>
    /// Matrix Division
    /// </summary>
    public static Matrix operator /(Matrix left, Matrix right) {
      if (null == left)
        throw new ArgumentNullException(nameof(left));
      else if (null == right)
        throw new ArgumentNullException(nameof(right));

      if (left.ColumnCount != right.LineCount)
        throw new ArgumentException($"Right matrix must have {left.ColumnCount} liness, actual {right.LineCount}", nameof(right));
      else if (right.ColumnCount != right.LineCount)
        throw new ArgumentException("Divisor must be a square matrix.", nameof(right));

      return new Matrix(MatrixLowLevel.Multiply(left.m_Items, MatrixLowLevel.Inverse(right.m_Items)));
    }

    /// <summary>
    /// Matrix Division
    /// </summary>
    public static Matrix operator /(double left, Matrix right) {
      if (null == right)
        throw new ArgumentNullException(nameof(right));

      if (right.ColumnCount != right.LineCount)
        throw new ArgumentException("Divisor must be a square matrix.", nameof(right));

      double[][] result = MatrixLowLevel.Inverse(right.m_Items);

      for (int i = result.Length - 1; i >= 0; --i) {
        double[] line = result[i];

        for (int j = line.Length - 1; j >= 0; --j)
          line[j] = left * line[j];
      }

      return new Matrix(result);
    }

    #endregion Arithmetics

    #endregion Operators

    #region ICloneable

    /// <summary>
    /// Deep copy
    /// </summary>
    public Matrix Clone() {
      return new Matrix() {
        m_Items = m_Items
                    .Select(line => {
                      double[] result = new double[line.Length];
                      Array.Copy(line, 0, result, 0, line.Length);
                      return result;
                    })
                    .ToArray()
      };
    }

    /// <summary>
    /// Deep Copy
    /// </summary>
    object ICloneable.Clone() => this.Clone();

    #endregion ICloneable

    #region IEquatable<Matrix>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(Matrix other, double tolerance) {
      if (tolerance < 0)
        throw new ArgumentOutOfRangeException(nameof(tolerance));

      if (ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      if (this.ColumnCount != other.ColumnCount || this.LineCount != other.LineCount)
        return false;

      for (int r = LineCount - 1; r >= 0; --r)
        for (int c = ColumnCount - 1; c >= 0; --c)
          if (Math.Abs(m_Items[r][c] - other.m_Items[r][c]) < tolerance)
            continue; // for double.NaN case or alike
          else
            return false;

      return true;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(Matrix other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      if (this.ColumnCount != other.ColumnCount || this.LineCount != other.LineCount)
        return false;

      return m_Items
        .Zip(other.m_Items, (left, right) => left.SequenceEqual(right))
        .All(item => item);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as Matrix);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => unchecked((LineCount << 16) ^ ColumnCount ^ m_Items[0][0].GetHashCode());

    #endregion IEquatable<Matrix>

    #region IFormattable

    /// <summary>
    /// To String
    /// </summary>
    public string ToString(string format, IFormatProvider formatProvider) {
      if (null == formatProvider)
        formatProvider = CultureInfo.InvariantCulture;

      if (string.IsNullOrEmpty(format) || "g".Equals(format, StringComparison.OrdinalIgnoreCase))
        return ToString();

      int p = format.IndexOf('|');

      string delimiter = p >= 0
        ? format.Substring(p + 1)
        : "\t";

      if (p > 0)
        format = format.Substring(0, p);

      return string.Join(Environment.NewLine, m_Items
        .Select(line => string.Join(delimiter, line.Select(item => item.ToString(format, formatProvider)))));
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      return string.Join(Environment.NewLine, m_Items
        .Select(line => string.Join("\t", line.Select(item => item.ToString(CultureInfo.InvariantCulture)))));
    }

    #endregion IFormattable

    #region IEnumerable<double>

    /// <summary>
    /// Enumerator
    /// </summary>
    /// <returns></returns>
    public IEnumerator<double> GetEnumerator() => Items.GetEnumerator();

    /// <summary>
    /// Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

    #endregion IEnumerable<double>

  }
}
