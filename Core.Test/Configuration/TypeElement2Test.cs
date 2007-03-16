using System;
using System.Configuration;
using NUnit.Framework;
using Rubicon.Configuration;
using Rubicon.Development.UnitTesting;
using Rubicon.Development.UnitTesting.Configuration;

namespace Rubicon.Core.UnitTests.Configuration
{
  [TestFixture]
  public class TypeElement2Test
  {
    [Test]
    public void Initialize()
    {
      TypeElement<SampleType, DerivedSampleType> typeElement = new TypeElement<SampleType, DerivedSampleType>();

      ConfigurationPropertyCollection properties = (ConfigurationPropertyCollection) PrivateInvoke.GetNonPublicProperty (typeElement, "Properties");
      Assert.IsNotNull (properties);
      ConfigurationProperty property = properties["type"];
      Assert.IsNotNull (property);
      Assert.AreEqual (typeof (DerivedSampleType), property.DefaultValue);
      Assert.IsInstanceOfType (typeof (Rubicon.Utilities.TypeNameConverter), property.Converter);
      Assert.IsInstanceOfType (typeof (SubclassTypeValidator), property.Validator);
      Assert.IsTrue (property.IsRequired);
    }

    [Test]
    public void GetType_WithDefaultValue()
    {
      TypeElement<SampleType, DerivedSampleType> typeElement = new TypeElement<SampleType, DerivedSampleType>();

      Assert.AreEqual (typeof (DerivedSampleType), typeElement.Type);
    }

    [Test]
    public void CreateInstance_WithoutType()
    {
      TypeElement<SampleType, DerivedSampleType> typeElement = new TypeElement<SampleType, DerivedSampleType>();

      Assert.IsInstanceOfType (typeof (DerivedSampleType), typeElement.CreateInstance());
    }

    [Test]
    public void Deserialize_WithValidType()
    {
      TypeElement<SampleType, DerivedSampleType> typeElement = new TypeElement<SampleType, DerivedSampleType>();

      string xmlFragment = @"<theElement type=""Rubicon.Core.UnitTests::Configuration.SampleType"" />";
      ConfigurationHelper.DeserializeElement (typeElement, xmlFragment);

      Assert.AreEqual (typeof (SampleType), typeElement.Type);
    }
  }
}