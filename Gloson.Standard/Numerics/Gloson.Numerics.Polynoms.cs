using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Numerics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Polynom 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class Polynom : IEquatable<Polynom> {
    #region Private Data

    private readonly List<double> m_Items;

    #endregion Private Data

    #region Algorithm

    private static double C(int n, int k) {
      if (n == k || k == 0)
        return 1;

      double result = 1;

      if (k > n / 2)
        k = n - k;

      for (int i = n; i > n - k; --i)
        result *= i;

      for (int i = 1; i <= k; ++i)
        result /= i;

      return result;
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public Polynom(IEnumerable<double> items) {
      m_Items = items
        ?.Reverse()
        ?.SkipWhile(x => x == 0)
        ?.Reverse()
        ?.ToList() ?? throw new ArgumentNullException(nameof(items));
    }

    /// <summary>
    /// Reconstruct polynom from its values
    /// for P(0) = 1, P(1) = ?, P(2) = 9, P(3) = 16, P(4) = 25, P(5) = ?
    /// var result = Reconstruct(0, new double[] {1, double.NaN, 9, 16, 25, double.NaN});
    /// </summary>
    /// <param name="startAt">starting point</param>
    /// <param name="values">values (put double.NaN for abscent points)</param>
    /// <returns></returns>
    public static Polynom Reconstruct(int startAt, IEnumerable<double> values) {
      if (null == values)
        throw new ArgumentNullException(nameof(values));

      var points = values
        .Select((v, i) => (x: (double)i + startAt, y: v))
        .Where(item => !double.IsNaN(item.y));

      return Interpolation.InterpolatonPolynom(points, p => (p.x, p.y));
    }

    /// <summary>
    /// Reconstruct polynom from its values
    /// for P(0) = 1, P(1) = ?, P(2) = 9, P(3) = 16, P(4) = 25, P(5) = ?
    /// var result = Reconstruct(1, double.NaN, 9, 16, 25, double.NaN);
    /// </summary>
    /// <param name="values">values (put double.NaN for abscent points)</param>
    /// <returns></returns>
    public static Polynom ReconstructZeroStarting(params double[] values) =>
      Reconstruct(0, values);

    /// <summary>
    /// Reconstruct polynom from its values
    /// for P(1) = 1, P(2) = ?, P(3) = 9, P(4) = 16, P(5) = 25, P(6) = ?
    /// var result = Reconstruct(1, double.NaN, 9, 16, 25, double.NaN);
    /// </summary>
    /// <param name="values">values (put double.NaN for abscent points)</param>
    /// <returns></returns>
    public static Polynom ReconstructOneStarting(params double[] values) =>
      Reconstruct(1, values);

    #endregion Create

    #region Public

    /// <summary>
    /// Zero
    /// </summary>
    public static Polynom Zero { get; } = new Polynom(new double[0]);

    /// <summary>
    /// Zero
    /// </summary>
    public static Polynom One { get; } = new Polynom(new double[] { 1 });

    /// <summary>
    /// NaN
    /// </summary>
    public static Polynom NaN { get; } = new Polynom(new double[] { double.NaN});

    /// <summary>
    /// Compute At point x 
    /// </summary>
    public double At(double x) {
      double value = 1.0;
      double result = 0.0;

      for (int i = 0; i < m_Items.Count; ++i) {
        result += m_Items[i] * value;

        value *= x;
      }

      return result;
    }

    /// <summary>
    /// Items
    /// </summary>
    public IReadOnlyList<double> Items => m_Items;

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_Items.Count;

    /// <summary>
    /// Items
    /// </summary>
    public double this[int index] {
      get {
        if (index < 0 || index >= m_Items.Count)
          return 0;
        else
          return m_Items[index];
      }
    }

    /// <summary>
    /// Add Shift
    /// (x) => (x + shift)
    /// e.g. for shift = 2 we have 4 * (x + 2)**2 + 3 * (x + 2) + 1 => 4x**2 + 19x + 23
    /// </summary>
    /// <param name="shift"></param>
    /// <returns></returns>
    public Polynom WithShift(double shift) {
      if (0 == shift)
        return this;

      double[] a = Items.ToArray();
      double[] r = new double[a.Length];

      int n = a.Length - 1;

      for (int k = 0; k <= n; ++k) {
        double coef = 0;

        for (int i = 0; i <= n - k; ++i) 
          coef += a[n - i] * C(n - i, k) * Math.Pow(shift, n - k - i);

        r[k] = coef;
      }

      return new Polynom(r);
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      if (m_Items.Count <= 0)
        return "0";

      StringBuilder sb = new StringBuilder();

      for (int i = m_Items.Count - 1; i >= 0; --i) {
        double v = m_Items[i];

        if (0 == v)
          continue;

        if (v < 0) {
          if (sb.Length > 0) {
            sb.Append(" - ");
            sb.Append(Math.Abs(v));
          }
          else
            sb.Append(v);
        }
        else {
          if (sb.Length > 0)
            sb.Append(" + ");

          sb.Append(v);
        }

        if (i == 1)
          sb.Append(" * x");
        else if (i > 1)
          sb.Append($" * x ** {i}");
      }

      return sb.ToString();
    }

    /// <summary>
    /// Divide and find Remainder
    /// </summary>
    public Polynom DivRem(Polynom value, out Polynom remainder) {
      if (null == value)
        throw new ArgumentNullException(nameof(value));
      else if (value.Count <= 0)
        throw new DivideByZeroException();

      if (value.Count > Count) {
        remainder = this;

        return Zero;
      }

      List<double> quotinent = new List<double>();
      List<double> rem = m_Items.ToList();

      for (int i = 0; i <= Count - value.Count; ++i) {
        double coef = rem[rem.Count - i - 1] / value[value.m_Items.Count - 1];

        quotinent.Add(coef);

        for (int j = 0; j < value.m_Items.Count; ++j)
          rem[rem.Count - i - j - 1] -= value.m_Items[value.m_Items.Count - j - 1] * coef;
      }

      remainder = new Polynom(rem.Take(value.Count - 1));

      quotinent.Reverse();

      return new Polynom(quotinent);
    }

    /// <summary>
    /// Derivative
    /// </summary>
    public Polynom Derivative(int count = 1) {
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof(count));
      else if (count == 0)
        return this;
      else if (count >= Count)
        return Zero;

      List<double> list = new List<double>(m_Items.Count - count);

      double coef = Gloson.Numerics.SpecialFunctions.GammaFunctions.Factorial(count);

      if (coef > long.MaxValue)
        for (int i = count; i < m_Items.Count; ++i) {
          list.Add(coef * m_Items[i]);

          coef = coef / (i - count + 1) * (i + 1);
        }
      else {
        long cc = (long)(coef + 0.5);

        for (int i = count; i < m_Items.Count; ++i) {
          list.Add(cc * m_Items[i]);

          cc = cc / (i - count + 1) * (i + 1);
        }
      }

      return new Polynom(list);
    }

    #endregion Public

    #region Operators

    #region Comparison

    /// <summary>
    /// Equal
    /// </summary>
    public static bool operator ==(Polynom left, Polynom right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (null == left || null == right)
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Not Equal
    /// </summary>
    public static bool operator !=(Polynom left, Polynom right) {
      if (ReferenceEquals(left, right))
        return false;
      else if (null == left || null == right)
        return true;

      return !left.Equals(right);
    }

    #endregion Comparison

    #region Arithmetics

    /// <summary>
    /// Unary +
    /// </summary>
    public static Polynom operator +(Polynom value) => value;

    /// <summary>
    /// Unary -
    /// </summary>
    public static Polynom operator -(Polynom value) => value == null
      ? null
      : new Polynom(value.m_Items.Select(x => -x));

    /// <summary>
    /// Binary + 
    /// </summary>
    public static Polynom operator +(Polynom left, Polynom right) {
      if (null == left)
        throw new ArgumentNullException(nameof(left));
      else if (null == right)
        throw new ArgumentNullException(nameof(right));

      List<double> result = new List<double>(Math.Max(left.Count, right.Count));

      for (int i = 0; i < result.Count; ++i)
        result.Add(left[i] + right[i]);

      return new Polynom(result);
    }

    /// <summary>
    /// Binary - 
    /// </summary>
    public static Polynom operator -(Polynom left, Polynom right) {
      if (null == left)
        throw new ArgumentNullException(nameof(left));
      else if (null == right)
        throw new ArgumentNullException(nameof(right));

      List<double> result = new List<double>(Math.Max(left.Count, right.Count));

      for (int i = 0; i < result.Count; ++i)
        result.Add(left[i] - right[i]);

      return new Polynom(result);
    }

    /// <summary>
    /// Binary * 
    /// </summary>
    public static Polynom operator *(Polynom value, double coef) {
      if (null == value)
        throw new ArgumentNullException(nameof(coef));

      return new Polynom(value.m_Items.Select(x => x * coef));
    }

    /// <summary>
    /// Binary * 
    /// </summary>
    public static Polynom operator *(double coef, Polynom value) {
      if (null == value)
        throw new ArgumentNullException(nameof(coef));

      return new Polynom(value.m_Items.Select(x => x * coef));
    }

    /// <summary>
    /// Binary / 
    /// </summary>
    public static Polynom operator /(Polynom value, double coef) {
      if (null == value)
        throw new ArgumentNullException(nameof(coef));

      return new Polynom(value.m_Items.Select(x => x / coef));
    }

    /// <summary>
    /// Binary *
    /// </summary>
    public static Polynom operator *(Polynom left, Polynom right) {
      if (null == left)
        throw new ArgumentNullException(nameof(left));
      else if (null == right)
        throw new ArgumentNullException(nameof(right));

      if (left.Count <= 0 || right.Count <= 0)
        return Zero;

      List<double> list = new List<double>(left.Count + right.Count + 1);

      for (int power = 0; power <= left.Count + right.Count; ++power) {
        double s = 0;

        for (int i = power; i >= 0; --i) {
          int p1 = i;
          int p2 = power - i;

          if (p1 < left.Count && p2 < right.Count)
            s += left[p1] * right[p2];
        }

        list.Add(s);
      }

      return new Polynom(list);
    }

    /// <summary>
    /// Division /
    /// </summary>
    public static Polynom operator /(Polynom left, Polynom right) {
      if (null == left)
        throw new ArgumentNullException(nameof(left));
      else if (null == right)
        throw new ArgumentNullException(nameof(right));

      return left.DivRem(right, out var _);
    }

    /// <summary>
    /// Remainder %
    /// </summary>
    public static Polynom operator %(Polynom left, Polynom right) {
      if (null == left)
        throw new ArgumentNullException(nameof(left));
      else if (null == right)
        throw new ArgumentNullException(nameof(right));

      left.DivRem(right, out var remainder);

      return remainder;
    }

    #endregion Arithmetics

    #endregion Operators

    #region IEquatable<Polynom>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(Polynom other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      return m_Items.SequenceEqual(other.m_Items);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as Polynom);

    /// <summary>
    /// Get Hash Code
    /// </summary>
    public override int GetHashCode() {
      return m_Items.Count <= 0
        ? 0
        : m_Items.Count ^ m_Items[0].GetHashCode();
    }

    #endregion IEquatable<Polynom>
  }

}
