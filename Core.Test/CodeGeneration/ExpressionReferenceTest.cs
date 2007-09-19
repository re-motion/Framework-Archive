using System;
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Rubicon.CodeGeneration;
using Rubicon.CodeGeneration.DPExtensions;
using Rubicon.Core.UnitTests.CodeGeneration.SampleTypes;

namespace Rubicon.Core.UnitTests.CodeGeneration
{
  [TestFixture]
  public class ExpressionReferenceTest : CodeGenerationBaseTest
  {
    [Test]
    public void ExpressionReference ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "Foo", typeof (ClassWithStringMethod));
      CustomMethodEmitter methodEmitter = classEmitter.CreateMethodOverride (typeof (ClassWithStringMethod).GetMethod ("StringMethod"));
      
      ExpressionReference expressionReference = new ExpressionReference (typeof (string), new ConstReference ("bla").ToExpression(), methodEmitter);
      methodEmitter.ImplementByReturning (new ReferenceExpression (expressionReference));

      ClassWithStringMethod method = (ClassWithStringMethod) Activator.CreateInstance (classEmitter.BuildType ());
      Assert.AreEqual ("bla", method.StringMethod ());
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Expressions cannot be assigned to.")]
    public void ExpressionReferenceCannotBeStored ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "Foo", typeof (ClassWithStringMethod));
      CustomMethodEmitter methodEmitter = classEmitter.CreateMethodOverride (typeof (ClassWithStringMethod).GetMethod ("StringMethod"));

      try
      {
        ExpressionReference expressionReference = new ExpressionReference (typeof (string), new ConstReference ("bla").ToExpression (), methodEmitter);
        expressionReference.StoreReference (null);
      }
      finally
      {
        classEmitter.BuildType ();
      }
    }

    [Test]
    public void LoadAddressOfExpressionReference ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "Foo", typeof (ClassWithStringMethod));
      CustomMethodEmitter methodEmitter = classEmitter.CreateMethodOverride (typeof (ClassWithStringMethod).GetMethod ("StringMethod"));

      ExpressionReference expressionReference =
          new ExpressionReference (typeof (StructWithMethod), new InitObjectExpression (methodEmitter, typeof (StructWithMethod)), methodEmitter);
      ExpressionReference addressReference =
          new ExpressionReference (typeof (StructWithMethod).MakeByRefType(), expressionReference.ToAddressOfExpression(), methodEmitter);
      MethodInvocationExpression methodCall =
          new MethodInvocationExpression (addressReference, typeof (StructWithMethod).GetMethod ("Method"));

      methodEmitter.ImplementByReturning (methodCall);

      ClassWithStringMethod method = (ClassWithStringMethod) Activator.CreateInstance (classEmitter.BuildType());
      Assert.AreEqual ("StructMethod", method.StringMethod());
    }
  }
}