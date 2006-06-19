using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Xml.Schema;
using System.Xml;
using Rubicon.Xml;

namespace Rubicon.Core.UnitTests.Xml
{
  [TestFixture]
  public class SchemaBaseTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SchemaBaseTest ()
    {
    }

    // methods and properties

    [Test]
    public void LoadSchemaWithSchemaFile ()
    {
      SchemaBaseMock schemaBaseMock = new SchemaBaseMock ("http://www.rubicon-it.com/Core/Test/Xml/SchemaBaseMock");
      XmlSchema xmlSchema = schemaBaseMock.LoadSchema ("SchemaBaseMock.xsd");
      Assert.IsNotNull (xmlSchema);
      Assert.AreEqual ("http://www.rubicon-it.com/Core/Test/Xml/SchemaBaseMock", xmlSchema.TargetNamespace);
    }

    [Test]
    public void LoadSchemaReaderWithInvalidFileName ()
    {
      try
      {
        SchemaBaseMock schemaBaseMock = new SchemaBaseMock ("http://www.rubicon-it.com/Core/Test/Xml/SchemaBaseMock");
        schemaBaseMock.LoadSchema ("invalidSchemaFileName.xsd");

        Assert.Fail ("ApplicationException was expected.");
      }
      catch (ApplicationException ex)
      {
        string expectedMessage = string.Format (
            "Error loading schema resource 'invalidSchemaFileName.xsd' from assembly '{0}'.", typeof (SchemaBaseMock).Assembly.FullName);

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }

    [Test]
    public void LoadSchemaSet ()
    {
      SchemaBase schemaBaseMock = new SchemaBaseMock ("http://www.rubicon-it.com/Core/Test/Xml/SchemaBaseMock");
      XmlSchemaSet xmlSchemaSet = schemaBaseMock.LoadSchemaSet ();
      Assert.AreEqual (1, xmlSchemaSet.Count);
      Assert.IsTrue (xmlSchemaSet.Contains ("http://www.rubicon-it.com/Core/Test/Xml/SchemaBaseMock"));
    }
  }
}
