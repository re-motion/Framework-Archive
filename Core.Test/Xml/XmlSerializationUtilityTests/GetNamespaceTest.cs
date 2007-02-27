using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Serialization;
using Castle.DynamicProxy.Builder.CodeBuilder;
using Castle.DynamicProxy.Builder.CodeGenerators;
using NUnit.Framework;
using Rubicon.Xml;

namespace Rubicon.Core.UnitTests.Xml.XmlSerializationUtilityTests
{
  [TestFixture]
  public class GetNamespaceTest
  {
    [Test]
    public void Test_WithXmlTypeAttribute()
    {
      Type type = CreateType ("SampleType", CreateXmlTypeAttributeBuilder ("http://type-namespace"));
      Assert.AreEqual ("http://type-namespace", XmlSerializationUtility.GetNamespace (type));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "Cannot determine the xml namespace of type 'SampleType' because neither an "
        + "XmlTypeAttribute nor an XmlRootAttribute is used to define a namespace for the type.\r\nParameter name: type")]
    public void Test_WithXmlTypeAttributeWithoutNamespace()
    {
      Type type = CreateType ("SampleType", CreateXmlTypeAttributeBuilder (null));
      XmlSerializationUtility.GetNamespace (type);
    }

    [Test]
    public void Test_WithXmlRootAttribute()
    {
      Type type = CreateType ("SampleType", CreateXmlRootAttributeBuilder ("http://root-namespace"));
      Assert.AreEqual ("http://root-namespace", XmlSerializationUtility.GetNamespace (type));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "Cannot determine the xml namespace of type 'SampleType' because neither an "
        + "XmlTypeAttribute nor an XmlRootAttribute is used to define a namespace for the type.\r\nParameter name: type")]
    public void Test_WithXmlRootAttributeWithoutNamespace()
    {
      Type type = CreateType ("SampleType", CreateXmlRootAttributeBuilder (null));
      XmlSerializationUtility.GetNamespace (type);
    }

    [Test]
    public void Test_WithXmlRootAttributeWithTypeAlsoHavingAnXmlTypeAttribute()
    {
      Type type = CreateType (
          "SampleType",
          CreateXmlTypeAttributeBuilder ("http://type-namespace"),
          CreateXmlRootAttributeBuilder ("http://root-namespace"));
      Assert.AreEqual ("http://root-namespace", XmlSerializationUtility.GetNamespace (type));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "Cannot determine the xml namespace of type 'SampleType' because no neither an XmlTypeAttribute"
        + " nor an XmlRootAttribute has been provided.\r\nParameter name: type")]
    public void Test_WithoutXmlRootAttributeAndWithoutXmlTypeAttribute()
    {
      Type type = CreateType ("SampleType");
      XmlSerializationUtility.GetNamespace (type);
    }

    private Type CreateType (string typeName, params CustomAttributeBuilder[] attributeBuilders)
    {
      ModuleScope moduleScope = new ModuleScope();
      EasyType easyType = new EasyType (moduleScope, typeName, typeof (object), null, false);
      foreach (CustomAttributeBuilder attributeBuilder in attributeBuilders)
        easyType.TypeBuilder.SetCustomAttribute (attributeBuilder);

      return easyType.BuildType ();
    }

    private CustomAttributeBuilder CreateXmlTypeAttributeBuilder (string @namespace)
    {
      ConstructorInfo constructorInfo = typeof (XmlTypeAttribute).GetConstructor (new Type[0]);
      PropertyInfo namespacePropertyInfo = typeof (XmlTypeAttribute).GetProperty ("Namespace");

      return new CustomAttributeBuilder (constructorInfo, new object[0], new PropertyInfo[] {namespacePropertyInfo}, new object[] {@namespace});
    }

    private CustomAttributeBuilder CreateXmlRootAttributeBuilder (string @namespace)
    {
      ConstructorInfo constructorInfo = typeof (XmlRootAttribute).GetConstructor (new Type[0]);
      PropertyInfo namespacePropertyInfo = typeof (XmlRootAttribute).GetProperty ("Namespace");

      return new CustomAttributeBuilder (constructorInfo, new object[0], new PropertyInfo[] {namespacePropertyInfo}, new object[] {@namespace});
    }
  }
}