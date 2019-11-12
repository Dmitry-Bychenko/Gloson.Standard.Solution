using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Gloson.Numerics.Calculus;

namespace Gloson.Standard.Test.Numerics.Calculus {
  public class IntegralsTest {
    #region Tests

    [Test]
    public void TestConstant() {
      double value = Integrals.SimpsonAt((x) => 1.0, 0, 5);

      Assert.AreEqual(5.0, value, 0.0001);
    }

    [Test]
    public void TestLinear() {
      double value = Integrals.SimpsonAt((x) => 3.0 - x / 2, 1, 4);

      Assert.AreEqual(5.25, value, 0.0001);
    }

    #endregion Tests
  }
}
