using System;
using System.Collections;
using System.Web;
using System.Web.SessionState;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using Rubicon.NullableValueTypes;
using Rubicon.Development.UnitTesting;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Collections;
using Rubicon.Web.UnitTests.AspNetFramework;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{

[TestFixture]
public class WxeFunctionTest: WxeTest
{
  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
  }

  [Test]
  public void SerializeParameters()
  {
    TestFunctionWithSerializableParameters function = 
        new TestFunctionWithSerializableParameters ("Hello World", NaInt32.Null, 1);
    NameValueCollection parameters = function.SerializeParametersForQueryString();
    Assert.AreEqual ("Hello World", parameters["StringValue"].ToString());
    Assert.AreEqual (NaInt32.Null.ToString(), parameters["NaInt32Value"].ToString());
    Assert.AreEqual ("1", parameters["IntValue"].ToString());
  }
}

}
