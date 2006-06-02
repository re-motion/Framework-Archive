using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using NUnit.Framework;

using Rubicon.Data.DomainObjects;

using Rubicon.Security.Service.Domain.Metadata;

namespace Rubicon.Security.Service.Domain.UnitTests.Metadata
{
  [TestFixture]
  public class MetadataImporterTest
  {
    private ClientTransaction _transaction;
    private MetadataImporter _importer = new MetadataImporter ();

    [SetUp]
    public void SetUp ()
    {
      _transaction = new ClientTransaction ();
      _importer = new MetadataImporter ();
    }

    [Test]
    public void EmptyMetadataFile ()
    {
      string metadataXml = @"<securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"" />";

      DomainObjectCollection importedObjects = _importer.Import (_transaction, GetXmlDocument (metadataXml));

      Assert.AreEqual (0, importedObjects.Count);
    }

    [Test]
    public void ImportSecurableClass ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Rubicon.Security.UnitTests.TestDomain.File"">
              </class>
            </classes>
          </securityMetadata>
          ";

      DomainObjectCollection importedObjects = _importer.Import (_transaction, GetXmlDocument (metadataXml));

      Assert.AreEqual (1, importedObjects.Count);

      DomainObjectFilterCriteria fileClassFilter = DomainObjectFilterCriteria.ExpectType (typeof (SecurableClassDefinition))
          .ExpectPropertyValue ("Name", "Rubicon.Security.UnitTests.TestDomain.File")
          .ExpectPropertyValue ("MetadataItemID", new Guid ("00000000-0000-0000-0001-000000000000"));

      RpfAssert.Contains (fileClassFilter, importedObjects);
    }

    [Test]
    public void ImportSecurableClasses ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Rubicon.Security.UnitTests.TestDomain.File"">
              </class>
              <class id=""00000000-0000-0000-0002-000000000000"" name=""Rubicon.Security.UnitTests.TestDomain.Directory"">
              </class>
            </classes>
          </securityMetadata>
          ";

      DomainObjectCollection importedObjects = _importer.Import (_transaction, GetXmlDocument (metadataXml));

      Assert.AreEqual (2, importedObjects.Count);

      DomainObjectFilterCriteria fileClassFilter = DomainObjectFilterCriteria.ExpectType (typeof (SecurableClassDefinition))
          .ExpectPropertyValue ("Name", "Rubicon.Security.UnitTests.TestDomain.File")
          .ExpectPropertyValue ("MetadataItemID", new Guid ("00000000-0000-0000-0001-000000000000"));
      DomainObjectFilterCriteria directoryClassFilter = DomainObjectFilterCriteria.ExpectType (typeof (SecurableClassDefinition))
          .ExpectPropertyValue ("Name", "Rubicon.Security.UnitTests.TestDomain.Directory")
          .ExpectPropertyValue ("MetadataItemID", new Guid ("00000000-0000-0000-0002-000000000000"));

      RpfAssert.Contains (fileClassFilter, importedObjects);
      RpfAssert.Contains (directoryClassFilter, importedObjects);
    }

    [Test]
    public void ImportAbstractRoles ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <abstractRoles>
              <abstractRole id=""00000003-0001-0000-0000-000000000000"" name=""Clerk"" value=""0"" />
              <abstractRole id=""00000003-0002-0000-0000-000000000000"" name=""Secretary"" value=""1"" />
              <abstractRole id=""00000004-0001-0000-0000-000000000000"" name=""Administrator"" value=""0"" />
            </abstractRoles>
          </securityMetadata>
          ";

      DomainObjectCollection importedObjects = _importer.Import (_transaction, GetXmlDocument (metadataXml));
      DomainObjectFilter filter = new DomainObjectFilter (importedObjects);

      Assert.AreEqual (3, importedObjects.Count);

      AbstractRoleDefinition role1 = filter.GetObject<AbstractRoleDefinition> (DomainObjectFilterCriteria.ExpectType (typeof (AbstractRoleDefinition))
          .ExpectPropertyValue ("MetadataItemID", new Guid ("00000003-0001-0000-0000-000000000000")));
      Assert.AreEqual ("Clerk", role1.Name);
      Assert.AreEqual (0, role1.Value);

      AbstractRoleDefinition role2 = filter.GetObject<AbstractRoleDefinition> (DomainObjectFilterCriteria.ExpectType (typeof (AbstractRoleDefinition))
          .ExpectPropertyValue ("MetadataItemID", new Guid ("00000003-0002-0000-0000-000000000000")));
      Assert.AreEqual ("Secretary", role2.Name);
      Assert.AreEqual (1, role2.Value);

      AbstractRoleDefinition role3 = filter.GetObject<AbstractRoleDefinition> (DomainObjectFilterCriteria.ExpectType (typeof (AbstractRoleDefinition))
          .ExpectPropertyValue ("MetadataItemID", new Guid ("00000004-0001-0000-0000-000000000000")));
      Assert.AreEqual ("Administrator", role3.Name);
      Assert.AreEqual (0, role3.Value);
    }

    private XmlDocument GetXmlDocument (string metadataXml)
    {
      XmlDocument metadataXmlDocument = new XmlDocument ();
      metadataXmlDocument.LoadXml (metadataXml);

      return metadataXmlDocument;
    }
  }
}
