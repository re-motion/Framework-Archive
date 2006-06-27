using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Rubicon.Security.Web.UnitTests.Domain;
using Rubicon.Security.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Security.Web.UnitTests.ExecutionEngine
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
