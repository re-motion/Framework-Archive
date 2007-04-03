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
    Assert.AreEqual ("abcdef", string.Concat (res));
  }

  [Test]
  public void TestConvert ()
  {
    object[] o1 = { "a", "b", "c", "d" };
    string[] res = ArrayUtility.Convert<object, string> (o1);
    Assert.AreEqual ("abcd", string.Concat (res));
  }

  [Test]
  public void TestInsertFirst()
  {
    string[] s1 = { "a", "b", "c", "d" };
    string[] res = ArrayUtility.Insert (s1, 0, "X");
    Assert.AreEqual ("Xabcd", string.Concat (res));
  }
  [Test]
  public void TestInsertMiddle()
  {
    string[] s1 = { "a", "b", "c", "d" };
    string[] res = ArrayUtility.Insert (s1, 2, "X");
    Assert.AreEqual ("abXcd", string.Concat (res));
  }
  [Test]
  public void TestInsertEnd()
  {
    string[] s1 = { "a", "b", "c", "d" };
    string[] res = ArrayUtility.Insert (s1, 4, "X");
    Assert.AreEqual ("abcdX", string.Concat (res));
  }
  [ExpectedException (typeof (IndexOutOfRangeException))]
  [Test]
  public void TestInsertPastEnd()
  {
    string[] s1 = { "a", "b", "c", "d" };
    string[] res = ArrayUtility.Insert (s1, 5, "X");
  }

  [Test]
  public void TestSkip ()
  {
    string[] s1 = { "a", "b", "c", "d" };
    string[] res = ArrayUtility.Skip (s1, 2);
    Assert.AreEqual ("cd", string.Concat (res));
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void TestSkipFail ()
  {
    ArrayUtility.Skip (new int[3], 4);
  }
}

}
