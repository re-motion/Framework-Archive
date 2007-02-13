using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Collections;
using Rubicon.Utilities;
using NUnit.Framework;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class NullSecurityServiceTest
  {
    private ISecurityService _provider;

    [SetUp]
    public void SetUp ()
    {
      _provider = new NullSecurityService ();
    }

    [Test]
    public void Test_GetAccess_ReturnsEmptyList ()
    {
      AccessType[] accessTypes = _provider.GetAccess (null, null);
      Assert.IsNotNull (accessTypes);
      Assert.AreEqual (0, accessTypes.Length);
    }

    [Test]
    public void Test_GetRevision_ReturnsZero ()
    {
      Assert.AreEqual (0, _provider.GetRevision ());
    }
  }
}