using System;
using System.Configuration;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NUnit.Framework;
using Rubicon.Xml;
using System.IO;

namespace Rubicon.Core.UnitTests.Xml.XmlSerializationUtilityTests
{
  [TestFixture]
  public class DeserializeUsingSchemaWithXmlReaderTest
  {
    [Test]
    public void Test_WithContextAndTypeAndSchemaSet ()
    {
      SampleClass actual = (SampleClass) XmlSerializationUtility.DeserializeUsingSchema (
          GetReaderForDefaultFragment (1),
          "root/" + SampleClass.ElementName,
          typeof (SampleClass),
          GetXmlSchemaSet());

      Assert.IsNotNull (actual);
      Assert.AreEqual (1, actual.Value);
    }

    [Test]
    public void Test_WithContextAndTypeAndSchemaUriAndSchemaReader ()
    {
      SampleClass actual = (SampleClass) XmlSerializationUtility.DeserializeUsingSchema (
          GetReaderForDefaultFragment (1),
          "root/" + SampleClass.ElementName,
          typeof (SampleClass),
          SampleClass.SchemaUri,
          SampleClass.GetSchemaReader ());

      Assert.IsNotNull (actual);
      Assert.AreEqual (1, actual.Value);
    }

    [Test]
    public void Test_WithContextAndTypeAndNamespaceAndSchemaSet ()
    {
      SampleClass actual = (SampleClass) XmlSerializationUtility.DeserializeUsingSchema (
          GetReaderForDefaultFragment (1),
          "root/" + SampleClass.ElementName,
          typeof (SampleClass),
          "http://www.rubicon-it.com/commons/core/unitTests",
          GetXmlSchemaSet ());

      Assert.IsNotNull (actual);
      Assert.AreEqual (1, actual.Value);
    }

    [Test]
    [ExpectedException (typeof (XmlSchemaValidationException),
        "Schema verification failed with 1 errors and 0 warnings in 'root/sampleClass'. First error: The element 'sampleClass' in namespace "
        + "'http://www.rubicon-it.com/commons/core/unitTests' has invalid child element 'value1' in namespace "
        + "'http://www.rubicon-it.com/commons/core/unitTests'. List of possible elements expected: 'value' in namespace "
        + "'http://www.rubicon-it.com/commons/core/unitTests'. Line 2, position 14.")]
    public void Test_WithContextAndTypeAndNamespaceAndSchemaSet_HavingInvalidXmlFragmentViolatingTheSchema ()
    {
      XmlSerializationUtility.DeserializeUsingSchema (
          GetReaderForFragmentWithInvalidSchema (),
          "root/" + SampleClass.ElementName,
          typeof (SampleClass),
          "http://www.rubicon-it.com/commons/core/unitTests",
          GetXmlSchemaSet ());
    }

    [Test]
    [ExpectedException (typeof (XmlException),
        "Error reading 'root/sampleClass'. The value of whitespace '' could not be parsed: Input string was not in a correct format. "
        + "Line 2, position 32.")]
    public void Test_WithContextAndTypeAndNamespaceAndSchemaSet_HavingInvalidDataTypeInXmlFragment ()
    {
      XmlSerializationUtility.DeserializeUsingSchema (
          GetReaderForDefaultFragment ("data"),
          "root/" + SampleClass.ElementName,
          typeof (SampleClass),
          "http://www.rubicon-it.com/commons/core/unitTests",
          GetXmlSchemaSet ());
    }

    [Test]
    public void Test_WithContextAndTypeAndNamespaceAndXmlReaderSettings ()
    {
      SampleClass actual = (SampleClass) XmlSerializationUtility.DeserializeUsingSchema (
          GetReaderForDefaultFragment (1),
          "root/" + SampleClass.ElementName,
          typeof (SampleClass),
          "http://www.rubicon-it.com/commons/core/unitTests",
          GetXmlReaderSettings ());

      Assert.IsNotNull (actual);
      Assert.AreEqual (1, actual.Value);
    }

    [Test]
    [ExpectedException (typeof (XmlSchemaValidationException),
        "Schema verification failed with 1 errors and 0 warnings in 'root/sampleClass'. First error: The element 'sampleClass' in namespace "
        + "'http://www.rubicon-it.com/commons/core/unitTests' has invalid child element 'value1' in namespace "
        + "'http://www.rubicon-it.com/commons/core/unitTests'. List of possible elements expected: 'value' in namespace "
        + "'http://www.rubicon-it.com/commons/core/unitTests'. Line 2, position 14.")]
    public void Test_WithContextAndTypeAndNamespaceAndXmlReaderSettings_HavingInvalidXmlFragmentViolatingTheSchema ()
    {
      XmlSerializationUtility.DeserializeUsingSchema (
          GetReaderForFragmentWithInvalidSchema (),
          "root/" + SampleClass.ElementName,
          typeof (SampleClass),
          "http://www.rubicon-it.com/commons/core/unitTests",
          GetXmlReaderSettings ());
    }

    [Test]
    [ExpectedException (typeof (XmlException),
        "Error reading 'root/sampleClass'. The value of whitespace '' could not be parsed: Input string was not in a correct format. "
        + "Line 2, position 32.")]
    public void Test_WithContextAndTypeAndNamespaceAndXmlReaderSettings_HavingInvalidDataTypeInXmlFragment ()
    {
      XmlSerializationUtility.DeserializeUsingSchema (
          GetReaderForDefaultFragment ("data"),
          "root/" + SampleClass.ElementName,
          typeof (SampleClass),
          "http://www.rubicon-it.com/commons/core/unitTests",
          GetXmlReaderSettings ());
    }

    [Test]
    [ExpectedException (typeof (XmlException),
        "Error reading 'root/sampleClass'. The value of whitespace '' could not be parsed: Only positive integer values are allowed.\r\n"
        + "Parameter name: Value\r\nActual value was -1. Line 2, position 30.")]
    public void Test_WithContextAndTypeAndNamespaceAndXmlReaderSettings_HavingInvalidValueInXmlFragment ()
    {
      XmlSerializationUtility.DeserializeUsingSchema (
          GetReaderForDefaultFragment (-1),
          "root/" + SampleClass.ElementName,
          typeof (SampleClass),
          "http://www.rubicon-it.com/commons/core/unitTests",
          GetXmlReaderSettings ());
    }

    private XmlReader GetReaderForDefaultFragment (object value)
    {
      string xmlFragment =
          @"<sampleClass xmlns=""http://www.rubicon-it.com/commons/core/unitTests"">
            <value>{0}</value>
          </sampleClass>";
      return GetReader (string.Format (xmlFragment, value));
    }

    private XmlReader GetReaderForFragmentWithInvalidSchema ()
    {
      string xmlFragment =
          @"<sampleClass xmlns=""http://www.rubicon-it.com/commons/core/unitTests"">
            <value1>data</value1>
          </sampleClass>";
      return GetReader (xmlFragment);
    }

    private XmlReader GetReader (string xmlFragment)
    {
      StringReader stringReader = new StringReader (xmlFragment);
      
      return XmlReader.Create (stringReader);
    }

    private XmlSchemaSet GetXmlSchemaSet ()
    {
      XmlSchemaSet schemas = new XmlSchemaSet ();
      schemas.Add (SampleClass.SchemaUri, SampleClass.GetSchemaReader ());
      return schemas;
    }

    private XmlReaderSettings GetXmlReaderSettings ()
    {
      XmlReaderSettings settings = new XmlReaderSettings ();
      settings.Schemas = GetXmlSchemaSet ();
      settings.ValidationType = ValidationType.Schema;

      return settings;
    }
  }
}