using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;

namespace Remotion.Core.UnitTests.Utilities.AttributeUtilityTests
{
  [TestFixture]
  public class InstantiateAttributeDataTest
  {
    [Test]
    public void DefaultCtor ()
    {
      ComplexAttribute attribute = GetInstantiatedAttribute ("DefaultCtor");
      Assert.IsNull (attribute.T);
      Assert.IsNull (attribute.S);
      Assert.IsNull (attribute.Os);
    }

    [Test]
    public void DefaultCtorWithProperty ()
    {
      ComplexAttribute attribute = GetInstantiatedAttribute ("DefaultCtorWithProperty");
      Assert.IsNull (attribute.T);
      Assert.AreEqual ("foo", attribute.S);
      Assert.IsNull (attribute.Os);
    }

    [Test]
    public void DefaultCtorWithField ()
    {
      ComplexAttribute attribute = GetInstantiatedAttribute ("DefaultCtorWithField");
      Assert.AreEqual (typeof (object), attribute.T);
      Assert.IsNull (attribute.S);
      Assert.IsNull (attribute.Os);
    }

    [Test]
    public void CtorWithTypeAndProperty ()
    {
      ComplexAttribute attribute = GetInstantiatedAttribute ("CtorWithTypeAndProperty");
      Assert.AreEqual (typeof (void), attribute.T);
      Assert.AreEqual ("string", attribute.S);
      Assert.IsNull (attribute.Os);
    }

    [Test]
    public void CtorWithStringAndParamsArray ()
    {
      ComplexAttribute attribute = GetInstantiatedAttribute ("CtorWithStringAndParamsArray");
      Assert.IsNull (attribute.T);
      Assert.AreEqual ("s", attribute.S);
      Assert.That (attribute.Os, Is.EqualTo (new object[] { 1, 2, 3, "4" }));
    }

    [Test]
    public void CtorWithStringAndTypeParamsArray ()
    {
      ComplexAttribute attribute = GetInstantiatedAttribute ("CtorWithStringAndTypeParamsArray");
      Assert.AreEqual (typeof (double), attribute.T);
      Assert.IsNull (attribute.S);
      Assert.That (attribute.Ts, Is.EqualTo (new Type[] { typeof (int), typeof (string) }));
    }

    private ComplexAttribute GetInstantiatedAttribute (string methodName)
    {
      CustomAttributeData data = CustomAttributeData.GetCustomAttributes (typeof (ComplexAttributeTarget).GetMethod (methodName))[0];
      return (ComplexAttribute) AttributeUtility.InstantiateCustomAttributeData (data);
    }
  }
}