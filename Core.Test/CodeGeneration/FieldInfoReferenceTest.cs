using System;
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.CodeGeneration;
using Remotion.CodeGeneration.DPExtensions;
using Remotion.UnitTests.CodeGeneration.SampleTypes;
using Remotion.UnitTests.CodeGeneration.SampleTypes;

namespace Remotion.UnitTests.CodeGeneration
{
  [TestFixture]
  public class FieldInfoReferenceTest : SnippetGenerationBaseTest
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
      CustomMethodEmitter methodEmitter = GetMethodEmitter (false)
        .SetReturnType (typeof (string));

      LocalReference local = methodEmitter.DeclareLocal (typeof (string));
      FieldInfoReference fieldReference = new FieldInfoReference (null, fieldInfo);
      methodEmitter
          .AddStatement (new AssignStatement (local, fieldReference.ToExpression ()))
          .AddStatement (new AssignStatement (fieldReference, new ConstReference ("Replacement").ToExpression ()))
          .AddStatement (new ReturnStatement (local));

      Assert.AreEqual ("InitialStatic", ClassWithPublicFields.StaticReferenceTypeField);
      Assert.AreEqual ("InitialStatic", InvokeMethod ());
      Assert.AreEqual ("Replacement", ClassWithPublicFields.StaticReferenceTypeField);
    }

    [Test]
    public void LoadAndStoreInstance ()
    {
      FieldInfo fieldInfo = typeof (ClassWithPublicFields).GetField ("ReferenceTypeField");
      CustomMethodEmitter methodEmitter = GetMethodEmitter (false)
          .SetParameterTypes (typeof (ClassWithPublicFields))
          .SetReturnType (typeof (string));

      LocalReference local = methodEmitter.DeclareLocal (typeof (string));
      FieldInfoReference fieldReference = new FieldInfoReference (methodEmitter.ArgumentReferences[0], fieldInfo);
      methodEmitter
          .AddStatement (new AssignStatement (local, fieldReference.ToExpression ()))
          .AddStatement (new AssignStatement (fieldReference, new ConstReference ("Replacement").ToExpression ()))
          .AddStatement (new ReturnStatement (local));

      ClassWithPublicFields parameter = new ClassWithPublicFields ();
      Assert.AreEqual ("Initial", parameter.ReferenceTypeField);
      Assert.AreEqual ("Initial", InvokeMethod (parameter));
      Assert.AreEqual ("Replacement", parameter.ReferenceTypeField);
    }

    [Test]
    public void LoadAndStoreAddressStatic ()
    {
      FieldInfo fieldInfo = typeof (ClassWithPublicFields).GetField ("StaticReferenceTypeField");
      CustomMethodEmitter methodEmitter = GetMethodEmitter (false)
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

      Assert.AreEqual ("InitialStatic", ClassWithPublicFields.StaticReferenceTypeField);
      Assert.AreEqual ("InitialStatic", InvokeMethod());
      Assert.AreEqual ("Replacement", ClassWithPublicFields.StaticReferenceTypeField);
    }

    [Test]
    public void LoadAndStoreAddressInstance ()
    {
      FieldInfo fieldInfo = typeof (ClassWithPublicFields).GetField ("ReferenceTypeField");
      CustomMethodEmitter methodEmitter = GetMethodEmitter (false)
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

      ClassWithPublicFields parameter = new ClassWithPublicFields ();
      Assert.AreEqual ("Initial", parameter.ReferenceTypeField);
      Assert.AreEqual ("Initial", InvokeMethod (parameter));
      Assert.AreEqual ("Replacement", parameter.ReferenceTypeField);
    }
  }
}