using System;
using NUnit.Framework;
using Remotion.Security.Web.UI;
using Remotion.Security.UnitTests.Web.ExecutionEngine;

namespace Remotion.Security.UnitTests.Web.UI
{
  [TestFixture]
  public class DemandTargetWxeFunctionPermissionAttributeTest
  {
    [Test]
    public void Initialize ()
    {
      DemandTargetWxeFunctionPermissionAttribute attribute = new DemandTargetWxeFunctionPermissionAttribute (typeof (TestFunctionWithThisObject));

      Assert.AreEqual (PermissionSource.WxeFunction, attribute.PermissionSource);
      Assert.AreEqual (typeof (TestFunctionWithThisObject), attribute.FunctionType);
    }
  }
}