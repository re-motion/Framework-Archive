using System;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Rubicon.CodeGeneration;
using Rubicon.CodeGeneration.DPExtensions;
using Rubicon.Core.UnitTests.CodeGeneration.SampleTypes;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Core.UnitTests.CodeGeneration
{
  [TestFixture]
  public class FieldInfoReferenceTest : CodeGenerationBaseTest
  {
    public override void SetUp ()
    {
      base.SetUp ();
      ClassWithPublicFields.StaticReferenceTypeField = "InitialStatic";
    }

    [Test]
    public void LoadAndStoreStatic ()
    {
      FieldInfo fieldInfo = typeof (ClassWithPublicFields).GetField ("StaticReferenceTypeField");
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "FieldInfoReferenceTest", typeof (object));
      CustomMethodEmitter methodEmitter = classEmitter.CreateMethod ("Do", MethodAttributes.Public)
        .SetReturnType (typeof (string));

      LocalReference local = methodEmitter.DeclareLocal (typeof (string));
      FieldInfoReference fieldReference = new FieldInfoReference (null, fieldInfo);
      methodEmitter
          .AddStatement (new AssignStatement (local, fieldReference.ToExpression ()))
          .AddStatement (new AssignStatement (fieldReference, new ConstReference ("Replacement").ToExpression ()))
          .AddStatement (new ReturnStatement (local));

      object instance = Activator.CreateInstance (classEmitter.BuildType ());

      Assert.AreEqual ("InitialStatic", ClassWithPublicFields.StaticReferenceTypeField);
      object returnValue = PrivateInvoke.InvokePublicMethod (instance, "Do");

      Assert.AreEqual ("InitialStatic", returnValue);
      Assert.AreEqual ("Replacement", ClassWithPublicFields.StaticReferenceTypeField);
    }

    [Test]
    public void LoadAndStoreInstance ()
    {
      FieldInfo fieldInfo = typeof (ClassWithPublicFields).GetField ("ReferenceTypeField");
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "FieldInfoReferenceTest", typeof (object));
      CustomMethodEmitter methodEmitter = classEmitter.CreateMethod ("Do", MethodAttributes.Public)
        .SetParameterTypes (typeof (ClassWithPublicFields))
        .SetReturnType (typeof (string));

      LocalReference local = methodEmitter.DeclareLocal (typeof (string));
      FieldInfoReference fieldReference = new FieldInfoReference (methodEmitter.ArgumentReferences[0], fieldInfo);
      methodEmitter
          .AddStatement (new AssignStatement (local, fieldReference.ToExpression ()))
          .AddStatement (new AssignStatement (fieldReference, new ConstReference ("Replacement").ToExpression ()))
          .AddStatement (new ReturnStatement (local));

      object instance = Activator.CreateInstance (classEmitter.BuildType ());

      ClassWithPublicFields parameter = new ClassWithPublicFields ();
      Assert.AreEqual ("Initial", parameter.ReferenceTypeField);
      object returnValue = PrivateInvoke.InvokePublicMethod (instance, "Do", parameter);

      Assert.AreEqual ("Initial", returnValue);
      Assert.AreEqual ("Replacement", parameter.ReferenceTypeField);
    }

    [Test]
    public void LoadAndStoreAddressStatic ()
    {
      FieldInfo fieldInfo = typeof (ClassWithPublicFields).GetField ("StaticReferenceTypeField");
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "FieldInfoReferenceTest", typeof (object));
      CustomMethodEmitter methodEmitter = classEmitter.CreateMethod ("Do", MethodAttributes.Public)
        .SetReturnType (typeof (string));

      LocalReference local = methodEmitter.DeclareLocal (typeof (string));
      FieldInfoReference fieldReference = new FieldInfoReference (null, fieldInfo);

      Expression addressOfFieldExpression = fieldReference.ToAddressOfExpression ();
      Reference indirectReference =
          new IndirectReference (new ExpressionReference (typeof (string).MakeByRefType(), addressOfFieldExpression, methodEmitter));
      
      methodEmitter
          .AddStatement (new AssignStatement (local, indirectReference.ToExpression ()))
          .AddStatement (new AssignStatement (indirectReference, new ConstReference ("Replacement").ToExpression ()))
          .AddStatement (new ReturnStatement (local));

      object instance = Activator.CreateInstance (classEmitter.BuildType ());

      Assert.AreEqual ("InitialStatic", ClassWithPublicFields.StaticReferenceTypeField);
      object returnValue = PrivateInvoke.InvokePublicMethod (instance, "Do");

      Assert.AreEqual ("InitialStatic", returnValue);
      Assert.AreEqual ("Replacement", ClassWithPublicFields.StaticReferenceTypeField);
    }

    [Test]
    public void LoadAndStoreAddressInstance ()
    {
      FieldInfo fieldInfo = typeof (ClassWithPublicFields).GetField ("ReferenceTypeField");
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "FieldInfoReferenceTest", typeof (object));
      CustomMethodEmitter methodEmitter = classEmitter.CreateMethod ("Do", MethodAttributes.Public)
        .SetParameterTypes (typeof (ClassWithPublicFields))
        .SetReturnType (typeof (string));

      LocalReference local = methodEmitter.DeclareLocal (typeof (string));
      FieldInfoReference fieldReference = new FieldInfoReference (methodEmitter.ArgumentReferences[0], fieldInfo);

      Expression addressOfFieldExpression = fieldReference.ToAddressOfExpression ();
      Reference indirectReference =
          new IndirectReference (new ExpressionReference (typeof (string).MakeByRefType (), addressOfFieldExpression, methodEmitter));

      methodEmitter
          .AddStatement (new AssignStatement (local, indirectReference.ToExpression ()))
          .AddStatement (new AssignStatement (indirectReference, new ConstReference ("Replacement").ToExpression ()))
          .AddStatement (new ReturnStatement (local));

      object instance = Activator.CreateInstance (classEmitter.BuildType ());

      ClassWithPublicFields parameter = new ClassWithPublicFields ();
      Assert.AreEqual ("Initial", parameter.ReferenceTypeField);
      object returnValue = PrivateInvoke.InvokePublicMethod (instance, "Do", parameter);

      Assert.AreEqual ("Initial", returnValue);
      Assert.AreEqual ("Replacement", parameter.ReferenceTypeField);
    }
  }
}