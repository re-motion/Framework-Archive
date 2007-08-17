using System;
using NUnit.Framework;
using Rubicon.Security.UnitTests.Core.SampleDomain;
using Rubicon.Utilities;

namespace Rubicon.Security.UnitTests.Core
{
  [TestFixture]
  public class DemandMethodPermissionAttributeTest
  {
    [Test]
    public void AcceptValidAccessType ()
    {
      DemandMethodPermissionAttribute methodPermissionAttribute = new DemandMethodPermissionAttribute (TestAccessTypes.Second);
      Assert.AreEqual (TestAccessTypes.Second, methodPermissionAttribute.AccessTypes[0]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Enumerated Type 'Rubicon.Security.UnitTests.Core.SampleDomain.TestAccessTypesWithoutAccessTypeAttribute' cannot be used as an access type. "
        + "Valid access types must have the Rubicon.Security.AccessTypeAttribute applied.\r\nParameter name: accessType")]
    public void RejectAccessTypeWithoutAccessTypeAttribute ()
    {
      DemandMethodPermissionAttribute methodPermissionAttribute = 
          new DemandMethodPermissionAttribute (TestAccessTypesWithoutAccessTypeAttribute.First);
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemTypeException))]
    public void RejectOtherObjectTypes ()
    {
      DemandMethodPermissionAttribute methodPermissionAttribute = new DemandMethodPermissionAttribute (new SimpleType());
    }

    [Test]
    public void AcceptMultipleAccessTypes ()
    {
      DemandMethodPermissionAttribute methodPermissionAttribute =
          new DemandMethodPermissionAttribute (TestAccessTypes.Second, TestAccessTypes.Fourth);

      Assert.AreEqual (2, methodPermissionAttribute.AccessTypes.Length);
      Assert.Contains (TestAccessTypes.Second, methodPermissionAttribute.AccessTypes);
      Assert.Contains (TestAccessTypes.Fourth, methodPermissionAttribute.AccessTypes);
    }
  }
}
