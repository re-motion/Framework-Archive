using System;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Development.UnitTests.UnitTesting
{
  [TestFixture]
  public class DevNullTest
  {
    [Test] public void GetNull ()
    {
      Assert.IsNull (Dev.Null);
    }

    [Test] public void SetNull ()
    {
      Dev.Null = new object();
      Assert.IsNull (Dev.Null);
    }
  }
}