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
    public void TestAssertionHolds ()
    {
      Rubicon.Utilities.Assertion.Assert (true);
    }

    [Test]
    [ExpectedException (typeof (Rubicon.Utilities.AssertionException))]
    public void TestAssertionFails ()
    {
      Rubicon.Utilities.Assertion.Assert (false);
    }
  }
}
