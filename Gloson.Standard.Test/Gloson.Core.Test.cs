// https://gigi.nullneuron.net/gigilabs/data-driven-tests-with-nunit/
// https://github.com/nunit/docs/wiki/TestCaseSource-Attribute

using NUnit.Framework;

namespace Tests {

  /// <summary>
  /// 
  /// </summary>
  public class Tests {
    [SetUp]
    public void Setup() {
    }

    [Test]
    public void Test1() {
      Assert.Pass();
    }
  }
}