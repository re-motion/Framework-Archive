using System;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests
{

[TestFixture]
public class StringUtilityTest
{
  [Test]
	public void NullToEmpty()
	{
    Assertion.AssertEquals (string.Empty, StringUtility.NullToEmpty (null));
    Assertion.AssertEquals ("1", StringUtility.NullToEmpty ("1"));
	}

  [Test]
  public void IsNullOrEmpty()
  {
    Assertion.AssertEquals (true, StringUtility.IsNullOrEmpty (null));
    Assertion.AssertEquals (true, StringUtility.IsNullOrEmpty (""));
    Assertion.AssertEquals (false, StringUtility.IsNullOrEmpty (" "));
  }

  [Test]
  public void AreEqual()
  {
    Assertion.AssertEquals (true, StringUtility.AreEqual ("test1", "test1", false));
    Assertion.AssertEquals (true, StringUtility.AreEqual ("test1", "test1", true));
    Assertion.AssertEquals (false, StringUtility.AreEqual ("test1", "TEST1", false));
    Assertion.AssertEquals (true, StringUtility.AreEqual ("test1", "TEST1", true));
    Assertion.AssertEquals (false, StringUtility.AreEqual ("täst1", "TÄST1", false));
    Assertion.AssertEquals (true, StringUtility.AreEqual ("täst1", "TÄST1", true));
  }

}

}
