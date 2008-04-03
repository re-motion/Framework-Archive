using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.Core.UnitTests.Utilities
{
  [TestFixture]
  public class AssertionTest
  {
    [Test]
    public void TestIsTrueHolds ()
    {
      Remotion.Utilities.Assertion.IsTrue (true);
    }

    [Test]
    [ExpectedException (typeof (Remotion.Utilities.AssertionException))]
    public void TestIsTrueFails ()
    {
      Remotion.Utilities.Assertion.IsTrue (false);
    }
  }
}
