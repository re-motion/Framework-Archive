using System;
using System.Collections.Specialized;
using System.Web;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UnitTests.AspNetFramework;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class WxeFunctionTest : WxeTest
  {
    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
    }

    [Test]
    public void SerializeParameters ()
    {
      TestFunctionWithSerializableParameters function =
          new TestFunctionWithSerializableParameters ("Hello World", null, 1);
      NameValueCollection parameters = function.SerializeParametersForQueryString();
      Assert.AreEqual (3, parameters.Count);
      Assert.AreEqual ("Hello World", parameters["StringValue"]);
      Assert.AreEqual ("", parameters["NaInt32Value"]);
      Assert.AreEqual ("1", parameters["IntValue"]);
    }

    [Test]
    public void SerializeParametersWithInt32Null ()
    {
      TestFunctionWithSerializableParameters function = new TestFunctionWithSerializableParameters();
      function.Variables["StringValue"] = "Hello World";
      function.Variables["NaInt32Value"] = 1;
      function.Variables["Int32Value"] = null;

      NameValueCollection parameters = function.SerializeParametersForQueryString();

      Assert.AreEqual (2, parameters.Count);
      Assert.AreEqual ("Hello World", parameters["StringValue"]);
      Assert.AreEqual ("1", parameters["NaInt32Value"]);
    }

    [Test]
    public void InitializeParametersThroughQueryStringWithEmptyString ()
    {
      HttpContext context = HttpContextHelper.CreateHttpContext (
          "GET", "default.html", "StringValue=&NaInt32Value=2&IntValue=1");

      TestFunctionWithSerializableParameters function = new TestFunctionWithSerializableParameters();
      function.InitializeParameters (context.Request.Params);

      Assert.AreEqual ("", function.StringValue);
      Assert.AreEqual (2, function.NaInt32Value);
      Assert.AreEqual (1, function.IntValue);
    }

    [Test]
    public void InitializeParametersThroughQueryStringWithNaInt32BeingEmpty ()
    {
      HttpContext context = HttpContextHelper.CreateHttpContext (
          "GET", "default.html", "StringValue=Hello+World&NaInt32Value=&IntValue=1");

      TestFunctionWithSerializableParameters function = new TestFunctionWithSerializableParameters();
      function.InitializeParameters (context.Request.Params);

      Assert.AreEqual ("Hello World", function.StringValue);
      Assert.AreEqual (null, function.NaInt32Value);
      Assert.AreEqual (1, function.IntValue);
    }

    [Test]
    [ExpectedException (typeof (ApplicationException))]
    public void InitializeParametersThroughQueryStringWitInt32BeingEmpty ()
    {
      HttpContext context = HttpContextHelper.CreateHttpContext (
          "GET", "default.html", "StringValue=Hello+World&NaInt32Value=1&IntValue=");

      TestFunctionWithSerializableParameters function = new TestFunctionWithSerializableParameters();
      function.InitializeParameters (context.Request.Params);
    }

    [Test]
    public void InitializeParameters ()
    {
      TestFunctionWithSerializableParameters function = new TestFunctionWithSerializableParameters();
      NameValueCollection parameters = new NameValueCollection();
      parameters.Add ("StringValue", "Hello World");
      parameters.Add ("NaInt32Value", "");
      parameters.Add ("IntValue", "1");

      function.InitializeParameters (parameters);

      Assert.AreEqual ("Hello World", function.StringValue);
      Assert.AreEqual (null, function.NaInt32Value);
      Assert.AreEqual (1, function.IntValue);
    }

    [Test]
    public void OnExecutionStartingFinished ()
    {
      ExecutionStartedFinishedTestFunction function = new ExecutionStartedFinishedTestFunction ();
      Assert.IsFalse (function.ExecutionStartedCalled);
      Assert.IsFalse (function.ExecutionFinishedCalled);
      function.Execute (new WxeContextMock (WxeContextTest.CreateHttpContext ()));
      Assert.IsTrue (function.ExecutionStartedCalled);
      Assert.IsTrue (function.ExecutionFinishedCalled);
    }

    private WxeContextMock CreateContext ()
    {
      return new WxeContextMock (WxeContextTest.CreateHttpContext ());
    }

    [Test]
    public void OnExecutionFinishedIsLastEvenWhenManuallyAddingStepsAfterConstruction ()
    {
      ExecutionStartedFinishedTestFunction function = new ExecutionStartedFinishedTestFunction ();
      function.Add (new WxeMethodStep (function.CheckNotExecutionFinishedCalled));
      function.Execute (CreateContext());
    }

    [Test]
    public void OnExecutionFinishedIsLastEvenWhenManuallyAddingStepsWhileExecuting ()
    {
      ExecutionStartedFinishedTestFunction function = new ExecutionStartedFinishedTestFunction ();
      function.Add (new WxeMethodStep (function.AddAdditionalCheckStep));
      function.Execute (CreateContext());
    }

    [Test]
    public void OnExecutionFinishedOccursBeforeSurroundingFunctionIsFinished ()
    {
      EncapsulatedExecutionStartedFinishedTestFunction function = new EncapsulatedExecutionStartedFinishedTestFunction ();
      SurroundingExecutionStartedFinishedTestFunction surroundingFunction = new SurroundingExecutionStartedFinishedTestFunction (function);

      surroundingFunction.Execute (CreateContext ());
      Assert.IsTrue (surroundingFunction.ExecutionStartedCalled);
      Assert.IsTrue (surroundingFunction.ExecutionFinishedCalled);
      Assert.IsTrue (surroundingFunction.InnerFunction.ExecutionStartedCalled);
      Assert.IsTrue (surroundingFunction.InnerFunction.ExecutionFinishedCalled);

      Assert.That (surroundingFunction.Steps, Is.EquivalentTo (new string[] {
        "Surrounding.Started",
        "Surrounding.First",
        "Encapsulated.Started",
        "Encapsulated.Step1",
        "Encapsulated.Finished",
        "Surrounding.Last",
        "Surrounding.Finished"
      }));
    }
  }
}