using Gloson.Numerics;
using NUnit.Framework;

namespace Gloson.Standard.Test.Numerics {
  public class DivisorsTest {
    #region Tests

    [TestCase(0, 0)]
    [TestCase(1, 1)]
    [TestCase(2, 1)]
    [TestCase(3, 2)]
    [TestCase(4, 2)]
    [TestCase(5, 4)]
    [TestCase(100, 40)]
    [TestCase(101, 100)]
    public void TestValues(int value, int expected) {
      Assert.IsTrue(Divisors.Totient(value) == expected);
    }

    #endregion Tests
  }
}
