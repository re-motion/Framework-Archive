using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities
{
  [TestFixture]
  public class AssertionTest
  {
    [Test]
    public void TestIsTrueHolds ()
    {
      Rubicon.Utilities.Assertion.IsTrue (true);
    }

    [Test]
    [ExpectedException (typeof (Rubicon.Utilities.AssertionException))]
    public void TestIsTrueFails ()
    {
      Rubicon.Utilities.Assertion.IsTrue (false);
    }
  }
}
