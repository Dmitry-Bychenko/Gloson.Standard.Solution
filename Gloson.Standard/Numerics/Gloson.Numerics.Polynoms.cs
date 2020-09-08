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

    #endregion Create

    #region Public

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
        return new Polynom(new double[0]);

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
