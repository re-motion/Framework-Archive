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
      RequiredMethodPermissionAttribute methodPermissionAttribute = new RequiredMethodPermissionAttribute (TestAccessType.Second);
      Assert.AreEqual (TestAccessType.Second, methodPermissionAttribute.AccessType);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        "Enumerated Type 'Rubicon.Security.UnitTests.SampleDomain.TestAccessTypeWithoutAccessTypeAttribute' cannot be used as an access type. "
        + "Valid access types must have the Rubicon.Security.AccessTypeAttribute applied.\r\nParameter name: accessType")]
    public void RejectAccessTypeWithoutAccessTypeAttribute ()
    {
      RequiredMethodPermissionAttribute methodPermissionAttribute = 
          new RequiredMethodPermissionAttribute (TestAccessTypeWithoutAccessTypeAttribute.First);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void RejectOtherObjectTypes ()
    {
      RequiredMethodPermissionAttribute methodPermissionAttribute = new RequiredMethodPermissionAttribute (new SimpleType());
    }
  }
}
