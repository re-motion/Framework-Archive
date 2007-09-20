using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.CodeGeneration.DPExtensions;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Rubicon.Collections;
using Rubicon.Core.UnitTests.CodeGeneration.SampleTypes;
using Rubicon.Utilities;
using Rubicon.CodeGeneration;
using Rubicon.Development.UnitTesting;
using NUnit.Framework.SyntaxHelpers;

namespace Rubicon.Core.UnitTests.CodeGeneration
{
  [TestFixture]
  public class ExpressionExtensionTest : CodeGenerationBaseTest
  {
    [Test]
    public void CustomAttributeExpression ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "ExpressionExtensionTest", typeof (object));
      CustomMethodEmitter methodEmitter = classEmitter.CreateMethod ("TestMethod", MethodAttributes.Public)
          .SetReturnType (typeof (Tuple<SimpleAttribute, SimpleAttribute>));

      LocalReference attributeOwner = methodEmitter.DeclareLocal (typeof (Type));
      methodEmitter.AddStatement (new AssignStatement (attributeOwner, new TypeTokenExpression (typeof (ClassWithCustomAttribute))));

      ConstructorInfo tupleCtor =
          typeof (Tuple<SimpleAttribute, SimpleAttribute>).GetConstructor (new Type[] {typeof (SimpleAttribute), typeof (SimpleAttribute)});
      Expression tupleExpression = new NewInstanceExpression (tupleCtor,
          new CustomAttributeExpression (attributeOwner, typeof (SimpleAttribute), 0, true),
          new CustomAttributeExpression (attributeOwner, typeof (SimpleAttribute), 1, true));

      methodEmitter.AddStatement (new ReturnStatement (tupleExpression));

      object[] attributes = typeof (ClassWithCustomAttribute).GetCustomAttributes (typeof (SimpleAttribute), true);

      object instance = Activator.CreateInstance (classEmitter.BuildType ());
      Tuple<SimpleAttribute, SimpleAttribute> attributeTuple = 
          (Tuple<SimpleAttribute, SimpleAttribute>) PrivateInvoke.InvokePublicMethod (instance, "TestMethod");
      Assert.That (new object[] { attributeTuple.A, attributeTuple.B }, Is.EquivalentTo (attributes));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage = "Argument attributeOwner is a System.String, which cannot be assigned "
        + "to type System.Reflection.ICustomAttributeProvider.\r\nParameter name: attributeOwner")]
    public void CustomAttributeExpressionThrowsOnWrongReferenceType ()
    {
      new CustomAttributeExpression (new LocalReference (typeof (string)), typeof (SimpleAttribute), 0, true);
    }

  }
}