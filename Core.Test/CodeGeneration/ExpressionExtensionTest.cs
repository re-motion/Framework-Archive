using System;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.CodeGeneration.DPExtensions;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Rubicon.Core.UnitTests.CodeGeneration.SampleTypes;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.CodeGeneration
{
  [TestFixture]
  [Ignore ("TODO: FS")]
  public class ExpressionExtensionTest
  {
    private MockRepository _mockRepository;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();
    }

    private Func<T> CreateMethod<T> (Proc<IMemberEmitter, ILGenerator> codeGenerator)
    {
      //TODO: FS
      DynamicMethod method = null;//new DynamicMethod ("TestMethod", typeof (T), Type.EmptyTypes);
      ILGenerator gen = method.GetILGenerator ();
      codeGenerator (null, gen);
      return (Func<T>) method.CreateDelegate (typeof (Func<T>));
    }

    [Test]
    public void CustomAttributeExpression ()
    {
      Func<SimpleAttribute> attributeGetter = CreateMethod<SimpleAttribute> (delegate (IMemberEmitter member, ILGenerator gen)
      {
        LocalReference attributeOwner = new LocalReference (typeof (Type));
        attributeOwner.Generate (gen);

        new AssignStatement (attributeOwner, new TypeTokenExpression (typeof (ClassWithCustomAttribute))).Emit (member, gen);

        CustomAttributeExpression expression = new CustomAttributeExpression (attributeOwner, typeof (SimpleAttribute), 0, true);

        new ReturnStatement (expression).Emit (member, gen);
        
      });

      SimpleAttribute attribute = attributeGetter ();
      Assert.IsNotNull (attribute);
      Assert.Contains (attribute.S, new string[] { "whazzup1", "whazzup2" });
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