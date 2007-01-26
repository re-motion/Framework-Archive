using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Rubicon.Configuration;
using Rubicon.Utilities;
using NUnit.Framework;
using System.Configuration;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Core.UnitTests.Configuration
{
  [TestFixture]
  public class TypeElementTest
  {
    [Test]
    public void Initialize ()
    {
      TypeElement<SampleType> typeElement = new TypeElement<SampleType> ();

      ConfigurationPropertyCollection properties = (ConfigurationPropertyCollection) PrivateInvoke.GetNonPublicProperty (typeElement, "Properties");
      Assert.IsNotNull (properties);
      ConfigurationProperty property = properties["type"];
      Assert.IsNotNull (property);
      Assert.IsNull (property.DefaultValue);
      Assert.IsInstanceOfType (typeof (Rubicon.Utilities.TypeNameConverter), property.Converter);
      Assert.IsInstanceOfType (typeof (SubclassTypeValidator), property.Validator);
      Assert.IsTrue (property.IsRequired);
    }

    [Test]
    public void Deserialize_WithValidType ()
    {
      TypeElement<SampleType> typeElement = new TypeElement<SampleType> ();

      string xmlFragment = @"<theElement type=""Rubicon.Core.UnitTests::Configuration.SampleType"" />";
      DeserializeElement (typeElement, xmlFragment);

      Assert.AreEqual (typeof (SampleType), typeElement.Type);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException))]
    public void Deserialize_WithInvalidType ()
    {
      TypeElement<SampleType> typeElement = new TypeElement<SampleType> ();

      string xmlFragment = @"<theElement type=""System.Object, mscorlib"" />";
      DeserializeElement (typeElement, xmlFragment);

      object dummy = typeElement.Type;
    }

    private void DeserializeElement (ConfigurationElement configurationElement, string xmlFragment)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("xmlFragment", xmlFragment);

      XmlDocument document = new XmlDocument ();
      document.LoadXml (xmlFragment);

      MemoryStream stream = new MemoryStream ();
      document.Save (stream);
      stream.Position = 0;
      XmlReader reader = XmlReader.Create (stream);

      reader.IsStartElement ();
      PrivateInvoke.InvokeNonPublicMethod (configurationElement, "DeserializeElement", reader, false);
    }
  }
}