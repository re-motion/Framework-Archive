using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Rubicon.Development.UnitTesting;
using Rubicon.Security;

namespace Rubicon.Security.UnitTests
{

  [TestFixture]
  public class AccessTypeTest
  {
    [SetUp]
    public void SetUp ()
    {
      ClearAccessTypeCache ();
    }

    [TearDown]
    public void TearDown ()
    {
      ClearAccessTypeCache ();
    }

    [Test]
    public void GetAccessTypeFromEnum ()
    {
      AccessType accessType = AccessType.Get (TestAccessType.First);

      Assert.AreEqual (TestAccessType.First, accessType.Value);
      Assert.AreEqual ("Rubicon.Security.UnitTests.TestAccessType.First, Rubicon.Security.UnitTests", accessType.ID);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        "Enumerated Type 'Rubicon.Security.UnitTests.TestAccessTypeWithoutAccessTypeAttribute' cannot be used as an access type. "
        + "Valid access types must have the Rubicon.Security.AccessTypeAttribute applied.\r\nParameter name: accessType")]
    public void GetAccessTypeFromEnumWithoutAccessTypeAttribute ()
    {
      AccessType.Get (TestAccessTypeWithoutAccessTypeAttribute.First);
    }

    [Test]
    public void GetFromCache ()
    {
      Assert.AreSame (AccessType.Get (TestAccessType.First), AccessType.Get (TestAccessType.First));
      Assert.AreSame (AccessType.Get (TestAccessType.Second), AccessType.Get (TestAccessType.Second));
      Assert.AreSame (AccessType.Get (TestAccessType.Third), AccessType.Get (TestAccessType.Third));
    }

    private void ClearAccessTypeCache ()
    {
      PrivateInvoke.SetNonPublicStaticField (typeof (AccessType), "s_cache", new Dictionary<Enum, AccessType> ());
    }
  }

}