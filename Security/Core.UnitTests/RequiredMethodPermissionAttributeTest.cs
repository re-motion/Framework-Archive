using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Rubicon.Utilities;

using Rubicon.Security.UnitTests.SampleDomain;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class RequiredMethodPermissionAttributeTest
  {
    [Test]
    public void AcceptValidAccessType ()
    {
      DemandMethodPermissionAttribute methodPermissionAttribute = new DemandMethodPermissionAttribute (TestAccessType.Second);
      Assert.AreEqual (TestAccessType.Second, methodPermissionAttribute.AccessTypes[0]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        "Enumerated Type 'Rubicon.Security.UnitTests.SampleDomain.TestAccessTypeWithoutAccessTypeAttribute' cannot be used as an access type. "
        + "Valid access types must have the Rubicon.Security.AccessTypeAttribute applied.\r\nParameter name: accessType")]
    public void RejectAccessTypeWithoutAccessTypeAttribute ()
    {
      DemandMethodPermissionAttribute methodPermissionAttribute = 
          new DemandMethodPermissionAttribute (TestAccessTypeWithoutAccessTypeAttribute.First);
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
          new DemandMethodPermissionAttribute (TestAccessType.Second, TestAccessType.Fourth);

      Assert.AreEqual (2, methodPermissionAttribute.AccessTypes.Length);
      Assert.Contains (TestAccessType.Second, methodPermissionAttribute.AccessTypes);
      Assert.Contains (TestAccessType.Fourth, methodPermissionAttribute.AccessTypes);
    }
  }
}
