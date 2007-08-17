using System;
using NUnit.Framework;
using Rubicon.Security.Web.ExecutionEngine;
using Rubicon.Security.UnitTests.Web.Domain;

namespace Rubicon.Security.UnitTests.Web.ExecutionEngine
{
  [TestFixture]
  public class WxeDemandCreatePermissionAttributeTest
  {
    [Test]
    public void Initialize ()
    {
      WxeDemandCreatePermissionAttribute attribute = new WxeDemandCreatePermissionAttribute (typeof (SecurableObject));

      Assert.AreEqual (MethodType.Constructor, attribute.MethodType);
      Assert.AreSame ( typeof (SecurableObject), attribute.SecurableClass);
    }
  }
}
