using System;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities
{

[TestFixture]
public class ArrayUtilityTest
{
  [Test]
  public void TestCombine()
  {
    string[] s1 = { "a", "b", "c" };
    string[] s2 = { "d" };
    string[] s3 = {};
    string[] s4 = { "e", "f" };

    string[] res = (string[]) ArrayUtility.Combine (s1, s2, s3, s4);
    //Assertion.AssertEquals (6, res.Length);
    Assertion.AssertEquals ("abcdef", string.Concat (res));
  }
}

}
