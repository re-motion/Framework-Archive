using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using NUnit.Framework;

using Rubicon.Data.DomainObjects;

using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.Domain.UnitTests.Metadata
{
  [TestFixture]
  public class MetadataImporterTest
  {
    private ClientTransaction _transaction;
    private MetadataImporter _importer;

    [SetUp]
    public void SetUp ()
    {
      _transaction = new ClientTransaction ();
      _importer = new MetadataImporter (_transaction);
    }

    [Test]
    public void EmptyMetadataFile ()
    {
      string metadataXml = @"<securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"" />";

      _importer.Import (GetXmlDocument (metadataXml));

      Assert.AreEqual (0, _importer.ImportedObjects.Count);
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

      _importer.Import (GetXmlDocument (metadataXml));

      Assert.AreEqual (1, _importer.ImportedObjects.Count);

      DomainObjectFilterCriteria fileClassFilter = DomainObjectFilterCriteria.ExpectType (typeof (SecurableClassDefinition))
          .ExpectPropertyValue ("Name", "Rubicon.Security.UnitTests.TestDomain.File")
          .ExpectPropertyValue ("MetadataItemID", new Guid ("00000000-0000-0000-0001-000000000000"));

      RpfAssert.Contains (fileClassFilter, _importer.ImportedObjects);
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

      _importer.Import (GetXmlDocument (metadataXml));

      Assert.AreEqual (2, _importer.ImportedObjects.Count);

      DomainObjectFilterCriteria fileClassFilter = DomainObjectFilterCriteria.ExpectType (typeof (SecurableClassDefinition))
          .ExpectPropertyValue ("Name", "Rubicon.Security.UnitTests.TestDomain.File")
          .ExpectPropertyValue ("MetadataItemID", new Guid ("00000000-0000-0000-0001-000000000000"));
      DomainObjectFilterCriteria directoryClassFilter = DomainObjectFilterCriteria.ExpectType (typeof (SecurableClassDefinition))
          .ExpectPropertyValue ("Name", "Rubicon.Security.UnitTests.TestDomain.Directory")
          .ExpectPropertyValue ("MetadataItemID", new Guid ("00000000-0000-0000-0002-000000000000"));

      RpfAssert.Contains (fileClassFilter, _importer.ImportedObjects);
      RpfAssert.Contains (directoryClassFilter, _importer.ImportedObjects);
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

      _importer.Import (GetXmlDocument (metadataXml));
      DomainObjectFilter filter = new DomainObjectFilter (_importer.ImportedObjects);

      Assert.AreEqual (3, _importer.ImportedObjects.Count);

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

    [Test]
    public void ImportAccessTypes ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <accessTypes>
              <accessType id=""1d6d25bc-4e85-43ab-a42d-fb5a829c30d5"" name=""Create"" value=""0"" />
              <accessType id=""62dfcd92-a480-4d57-95f1-28c0f5996b3a"" name=""Read"" value=""1"" />
              <accessType id=""11186122-6de0-4194-b434-9979230c41fd"" name=""Edit"" value=""2"" />
            </accessTypes>
          </securityMetadata>
          ";

      _importer.Import (GetXmlDocument (metadataXml));
      DomainObjectFilter filter = new DomainObjectFilter (_importer.ImportedObjects);

      Assert.AreEqual (3, _importer.ImportedObjects.Count);

      AccessTypeDefinition accessType1 = filter.GetObject<AccessTypeDefinition> (DomainObjectFilterCriteria.ExpectType (typeof (AccessTypeDefinition))
          .ExpectPropertyValue ("MetadataItemID", new Guid ("1d6d25bc-4e85-43ab-a42d-fb5a829c30d5")));
      Assert.AreEqual ("Create", accessType1.Name);
      Assert.AreEqual (0, accessType1.Value);

      AccessTypeDefinition accessType2 = filter.GetObject<AccessTypeDefinition> (DomainObjectFilterCriteria.ExpectType (typeof (AccessTypeDefinition))
          .ExpectPropertyValue ("MetadataItemID", new Guid ("62dfcd92-a480-4d57-95f1-28c0f5996b3a")));
      Assert.AreEqual ("Read", accessType2.Name);
      Assert.AreEqual (1, accessType2.Value);

      AccessTypeDefinition accessType3 = filter.GetObject<AccessTypeDefinition> (DomainObjectFilterCriteria.ExpectType (typeof (AccessTypeDefinition))
          .ExpectPropertyValue ("MetadataItemID", new Guid ("11186122-6de0-4194-b434-9979230c41fd")));
      Assert.AreEqual ("Edit", accessType3.Name);
      Assert.AreEqual (2, accessType3.Value);
    }

    [Test]
    public void ImportStateProperties ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <stateProperties>
              <stateProperty id=""00000000-0000-0000-0002-000000000001"" name=""State"">
                <state name=""New"" value=""0"" />
                <state name=""Normal"" value=""1"" />
                <state name=""Archived"" value=""2"" />
              </stateProperty>

              <stateProperty id=""00000000-0000-0000-0001-000000000001"" name=""Confidentiality"">
                <state name=""Normal"" value=""0"" />
                <state name=""Confidential"" value=""1"" />
                <state name=""Private"" value=""2"" />
              </stateProperty>
            </stateProperties>
          </securityMetadata>
          ";

      _importer.Import (GetXmlDocument (metadataXml));
      DomainObjectFilter filter = new DomainObjectFilter (_importer.ImportedObjects);

      Assert.AreEqual (2, _importer.ImportedObjects.Count);

      StatePropertyDefinition prop1 = filter.GetObject<StatePropertyDefinition> (DomainObjectFilterCriteria.ExpectType (typeof (StatePropertyDefinition))
          .ExpectPropertyValue ("MetadataItemID", new Guid ("00000000-0000-0000-0002-000000000001")));
      Assert.AreEqual ("State", prop1.Name);
      Assert.AreEqual (3, prop1.DefinedStates.Count);

      DomainObjectFilterCriteria newStateFilter = new DomainObjectFilterCriteria (typeof (StateDefinition))
          .ExpectPropertyValue ("Name", "New").ExpectPropertyValue ("Value", (long) 0);
      DomainObjectFilterCriteria normalStateFilter = new DomainObjectFilterCriteria (typeof (StateDefinition))
          .ExpectPropertyValue ("Name", "Normal").ExpectPropertyValue ("Value", (long) 1);
      DomainObjectFilterCriteria archivedStateFilter = new DomainObjectFilterCriteria (typeof (StateDefinition))
          .ExpectPropertyValue ("Name", "Archived").ExpectPropertyValue ("Value", (long) 2);

      RpfAssert.Contains (newStateFilter, prop1.DefinedStates);
      RpfAssert.Contains (normalStateFilter, prop1.DefinedStates);
      RpfAssert.Contains (archivedStateFilter, prop1.DefinedStates);

      StatePropertyDefinition prop2 = filter.GetObject<StatePropertyDefinition> (DomainObjectFilterCriteria.ExpectType (typeof (StatePropertyDefinition))
          .ExpectPropertyValue ("MetadataItemID", new Guid ("00000000-0000-0000-0001-000000000001")));
      Assert.AreEqual ("State", prop1.Name);
      Assert.AreEqual (3, prop2.DefinedStates.Count);

      DomainObjectFilterCriteria normalConfidentialityFilter = new DomainObjectFilterCriteria (typeof (StateDefinition))
          .ExpectPropertyValue ("Name", "Normal").ExpectPropertyValue ("Value", (long) 0);
      DomainObjectFilterCriteria confidentialConfidentialityFilter = new DomainObjectFilterCriteria (typeof (StateDefinition))
          .ExpectPropertyValue ("Name", "Confidential").ExpectPropertyValue ("Value", (long) 1);
      DomainObjectFilterCriteria privateConfidentialityFilter = new DomainObjectFilterCriteria (typeof (StateDefinition))
          .ExpectPropertyValue ("Name", "Private").ExpectPropertyValue ("Value", (long) 2);

      RpfAssert.Contains (normalConfidentialityFilter, prop2.DefinedStates);
      RpfAssert.Contains (confidentialConfidentialityFilter, prop2.DefinedStates);
      RpfAssert.Contains (privateConfidentialityFilter, prop2.DefinedStates);
    }

    private XmlDocument GetXmlDocument (string metadataXml)
    {
      XmlDocument metadataXmlDocument = new XmlDocument ();
      metadataXmlDocument.LoadXml (metadataXml);

      return metadataXmlDocument;
    }
  }
}
