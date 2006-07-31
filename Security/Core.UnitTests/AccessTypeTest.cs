using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Rubicon.Development.UnitTesting;
using Rubicon.Security;
using Rubicon.Security.UnitTests.SampleDomain;

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
      AccessType accessType = AccessType.Get (new EnumWrapper (TestAccessTypes.First));

      Assert.AreEqual (new EnumWrapper (TestAccessTypes.First), accessType.Value);
    }

    [Test]
    [Ignore]
    [ExpectedException (typeof (ArgumentException),
        "Enumerated type 'Rubicon.Security.UnitTests.SampleDomain.TestAccessTypesWithoutAccessTypeAttribute' cannot be used as an access type. "
        + "Valid access types must have the Rubicon.Security.AccessTypeAttribute applied.\r\nParameter name: accessType")]
    public void GetAccessTypeFromEnumWithoutAccessTypeAttribute ()
    {
      AccessType.Get (new EnumWrapper (TestAccessTypesWithoutAccessTypeAttribute.First));
    }

    [Test]
    public void GetFromCache ()
    {
      Assert.AreSame (AccessType.Get (TestAccessTypes.First), AccessType.Get (TestAccessTypes.First));
      Assert.AreSame (AccessType.Get (TestAccessTypes.Second), AccessType.Get (TestAccessTypes.Second));
      Assert.AreSame (AccessType.Get (TestAccessTypes.Third), AccessType.Get (TestAccessTypes.Third));
    }

    [Test]
    public void Test_ToString ()
    {
      EnumWrapper wrapper = new EnumWrapper(TestAccessTypes.First);
      AccessType accessType = AccessType.Get (wrapper);

      Assert.AreEqual (wrapper.ToString (), accessType.ToString ());
    }


    private void ClearAccessTypeCache ()
    {
      PrivateInvoke.SetNonPublicStaticField (typeof (AccessType), "s_cache", new Dictionary<EnumWrapper, AccessType> ());
    }
  }

}