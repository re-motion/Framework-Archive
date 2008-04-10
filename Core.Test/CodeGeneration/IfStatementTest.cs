using System;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.CodeGeneration;
using Remotion.CodeGeneration.DPExtensions;

namespace Remotion.UnitTests.CodeGeneration
{
  [TestFixture]
  public class IfStatementTest : SnippetGenerationBaseTest
  {
    [Test]
    public void IfWithTrueCondition ()
    {
      CustomMethodEmitter methodEmitter = GetMethodEmitter (false);
      methodEmitter.SetReturnType (typeof (string));
      methodEmitter.AddStatement (new IfStatement (new SameConditionExpression (NullExpression.Instance, NullExpression.Instance),
          new ReturnStatement (new ConstReference ("True"))));
      methodEmitter.AddStatement (new ReturnStatement (new ConstReference ("False")));

      Assert.AreEqual ("True", InvokeMethod());
    }

    [Test]
    public void FalseCondition ()
    {
      CustomMethodEmitter methodEmitter = GetMethodEmitter (false);
      methodEmitter.SetReturnType (typeof (string));
      methodEmitter.AddStatement (new IfStatement (new SameConditionExpression (NullExpression.Instance, new ConstReference ("5").ToExpression()),
          new ReturnStatement (new ConstReference ("True"))));
      methodEmitter.AddStatement (new ReturnStatement (new ConstReference ("False")));

      Assert.AreEqual ("False", InvokeMethod ());
    }
  }
}