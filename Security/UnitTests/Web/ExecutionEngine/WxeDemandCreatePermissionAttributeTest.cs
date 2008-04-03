using System;
using NUnit.Framework;
using Remotion.Security.Web.ExecutionEngine;
using Remotion.Security.UnitTests.Web.Domain;

namespace Remotion.Security.UnitTests.Web.ExecutionEngine
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
