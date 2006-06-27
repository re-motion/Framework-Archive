using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Rubicon.Security.Web.UnitTests.Domain;
using Rubicon.Security.Web.UI;
using Rubicon.Security.Web.UnitTests.ExecutionEngine;

namespace Rubicon.Security.Web.UnitTests.UI
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