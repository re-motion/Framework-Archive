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
  public class MetadataImporterTest : DomainTest
  {
    private ClientTransaction _transaction;
    private MetadataImporter _importer;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = new ClientTransaction ();
      _importer = new MetadataImporter (_transaction);
    }

    [Test]
    public void EmptyMetadataFile ()
    {
      string metadataXml = @"<securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"" />";

      _importer.Import (GetXmlDocument (metadataXml));

      Assert.AreEqual (0, _importer.Classes.Count, "Class count");
      Assert.AreEqual (0, _importer.StateProperties.Count, "State property count");
      Assert.AreEqual (0, _importer.AbstractRoles.Count, "Abstract role count");
      Assert.AreEqual (0, _importer.AccessTypes.Count, "Access type count");
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

      Assert.AreEqual (1, _importer.Classes.Count, "Class count");
      Assert.AreEqual (0, _importer.StateProperties.Count, "State property count");
      Assert.AreEqual (0, _importer.AbstractRoles.Count, "Abstract role count");
      Assert.AreEqual (0, _importer.AccessTypes.Count, "Access type count");

      SecurableClassDefinition class1 = _importer.Classes[new Guid ("00000000-0000-0000-0001-000000000000")];
      Assert.AreEqual ("Rubicon.Security.UnitTests.TestDomain.File", class1.Name);
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

      Assert.AreEqual (2, _importer.Classes.Count, "Class count");
      Assert.AreEqual (0, _importer.StateProperties.Count, "State property count");
      Assert.AreEqual (0, _importer.AbstractRoles.Count, "Abstract role count");
      Assert.AreEqual (0, _importer.AccessTypes.Count, "Access type count");

      SecurableClassDefinition class1 = _importer.Classes[new Guid ("00000000-0000-0000-0001-000000000000")];
      Assert.AreEqual ("Rubicon.Security.UnitTests.TestDomain.File", class1.Name);

      SecurableClassDefinition class2 = _importer.Classes[new Guid ("00000000-0000-0000-0002-000000000000")];
      Assert.AreEqual ("Rubicon.Security.UnitTests.TestDomain.Directory", class2.Name);
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

      Assert.AreEqual (0, _importer.Classes.Count, "Class count");
      Assert.AreEqual (0, _importer.StateProperties.Count, "State property count");
      Assert.AreEqual (3, _importer.AbstractRoles.Count, "Abstract role count");
      Assert.AreEqual (0, _importer.AccessTypes.Count, "Access type count");

      AbstractRoleDefinition role1 = _importer.AbstractRoles[new Guid ("00000003-0001-0000-0000-000000000000")];
      Assert.AreEqual ("Clerk", role1.Name);
      Assert.AreEqual (0, role1.Value);

      AbstractRoleDefinition role2 = _importer.AbstractRoles[new Guid ("00000003-0002-0000-0000-000000000000")];
      Assert.AreEqual ("Secretary", role2.Name);
      Assert.AreEqual (1, role2.Value);

      AbstractRoleDefinition role3 = _importer.AbstractRoles[new Guid ("00000004-0001-0000-0000-000000000000")];
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

      Assert.AreEqual (0, _importer.Classes.Count, "Class count");
      Assert.AreEqual (0, _importer.StateProperties.Count, "State property count");
      Assert.AreEqual (0, _importer.AbstractRoles.Count, "Abstract role count");
      Assert.AreEqual (3, _importer.AccessTypes.Count, "Access type count");

      AccessTypeDefinition accessType1 = _importer.AccessTypes[new Guid ("1d6d25bc-4e85-43ab-a42d-fb5a829c30d5")];
      Assert.AreEqual ("Create", accessType1.Name);
      Assert.AreEqual (0, accessType1.Value);

      AccessTypeDefinition accessType2 = _importer.AccessTypes[new Guid ("62dfcd92-a480-4d57-95f1-28c0f5996b3a")];
      Assert.AreEqual ("Read", accessType2.Name);
      Assert.AreEqual (1, accessType2.Value);

      AccessTypeDefinition accessType3 = _importer.AccessTypes[new Guid ("11186122-6de0-4194-b434-9979230c41fd")];
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

      Assert.AreEqual (0, _importer.Classes.Count, "Class count");
      Assert.AreEqual (2, _importer.StateProperties.Count, "State property count");
      Assert.AreEqual (0, _importer.AbstractRoles.Count, "Abstract role count");
      Assert.AreEqual (0, _importer.AccessTypes.Count, "Access type count");

      StatePropertyDefinition prop1 = _importer.StateProperties[new Guid ("00000000-0000-0000-0002-000000000001")];
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

      StatePropertyDefinition prop2 = _importer.StateProperties[new Guid ("00000000-0000-0000-0001-000000000001")];
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

    [Test]
    public void ImportDerivedClasses ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Rubicon.Security.UnitTests.TestDomain.File"" />
              <class id=""00000000-0000-0000-0002-000000000000"" name=""Rubicon.Security.UnitTests.TestDomain.PaperFile"" base=""00000000-0000-0000-0001-000000000000"" />
            </classes>
          </securityMetadata>";

      _importer.Import (GetXmlDocument (metadataXml));

      Assert.AreEqual (2, _importer.Classes.Count, "Class count");
      Assert.AreEqual (0, _importer.StateProperties.Count, "State property count");
      Assert.AreEqual (0, _importer.AbstractRoles.Count, "Abstract role count");
      Assert.AreEqual (0, _importer.AccessTypes.Count, "Access type count");

      SecurableClassDefinition baseClass = _importer.Classes[new Guid ("00000000-0000-0000-0001-000000000000")];
      SecurableClassDefinition derivedClass = _importer.Classes[new Guid ("00000000-0000-0000-0002-000000000000")];

      Assert.AreEqual (1, baseClass.DerivedClasses.Count);
      Assert.AreSame (derivedClass, baseClass.DerivedClasses[0]);
      Assert.IsNull (baseClass.BaseClass);

      Assert.AreEqual (0, derivedClass.DerivedClasses.Count);
      Assert.AreSame (baseClass, derivedClass.BaseClass);
    }

    [Test]
    public void ImportWithStatePropertyReferences ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Rubicon.Security.UnitTests.TestDomain.File"">
                <stateProperties>
                  <statePropertyRef>00000000-0000-0000-0001-000000000001</statePropertyRef>
                </stateProperties>
              </class>
            </classes>

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
          </securityMetadata>";

      _importer.Import (GetXmlDocument (metadataXml));

      Assert.AreEqual (1, _importer.Classes.Count, "Class count");
      Assert.AreEqual (2, _importer.StateProperties.Count, "State property count");
      Assert.AreEqual (0, _importer.AbstractRoles.Count, "Abstract role count");
      Assert.AreEqual (0, _importer.AccessTypes.Count, "Access type count");

      SecurableClassDefinition classDefinition = _importer.Classes[new Guid ("00000000-0000-0000-0001-000000000000")];
      StatePropertyDefinition property1 = _importer.StateProperties[new Guid ("00000000-0000-0000-0002-000000000001")];
      StatePropertyDefinition property2 = _importer.StateProperties[new Guid ("00000000-0000-0000-0001-000000000001")];

      Assert.AreEqual (1, classDefinition.StatePropertyReferences.Count, "State property reference count");
      StatePropertyReference propertyReference = (StatePropertyReference) classDefinition.StatePropertyReferences[0];
      Assert.AreSame (property2, propertyReference.StateProperty);
    }

    [Test]
    public void ImportWithAccessTypeReferences ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Rubicon.Security.UnitTests.TestDomain.File"">
                <accessTypes>
                  <accessTypeRef>62dfcd92-a480-4d57-95f1-28c0f5996b3a</accessTypeRef>
                </accessTypes>
              </class>
            </classes>

            <accessTypes>
              <accessType id=""1d6d25bc-4e85-43ab-a42d-fb5a829c30d5"" name=""Create"" value=""0"" />
              <accessType id=""62dfcd92-a480-4d57-95f1-28c0f5996b3a"" name=""Read"" value=""1"" />
              <accessType id=""11186122-6de0-4194-b434-9979230c41fd"" name=""Edit"" value=""2"" />
              <accessType id=""305fbb40-75c8-423a-84b2-b26ea9e7cae7"" name=""Delete"" value=""3"" />
              <accessType id=""67cea479-0be7-4e2f-b2e0-bb1fcc9ea1d6"" name=""Search"" value=""4"" />
              <accessType id=""203b7478-96f1-4bf1-b4ea-5bdd1206252c"" name=""Find"" value=""5"" />
              <accessType id=""00000002-0001-0000-0000-000000000000"" name=""Journalize"" value=""0"" />
              <accessType id=""00000002-0002-0000-0000-000000000000"" name=""Archive"" value=""1"" />
            </accessTypes>
          </securityMetadata>";

      _importer.Import (GetXmlDocument (metadataXml));

      Assert.AreEqual (1, _importer.Classes.Count, "Class count");
      Assert.AreEqual (0, _importer.StateProperties.Count, "State property count");
      Assert.AreEqual (0, _importer.AbstractRoles.Count, "Abstract role count");
      Assert.AreEqual (8, _importer.AccessTypes.Count, "Access type count");

      SecurableClassDefinition classDefinition = _importer.Classes[new Guid ("00000000-0000-0000-0001-000000000000")];
      AccessTypeDefinition accessType = _importer.AccessTypes[new Guid ("62dfcd92-a480-4d57-95f1-28c0f5996b3a")];

      Assert.AreEqual (1, classDefinition.AccessTypeReferences.Count, "Access type reference count");
      AccessTypeReference accessTypeReference = (AccessTypeReference) classDefinition.AccessTypeReferences[0];
      Assert.AreSame (accessType, accessTypeReference.AccessType);
    }

    [Test]
    public void ImportSecurityDomain ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Rubicon.Security.UnitTests.TestDomain.File"">
                <stateProperties>
                  <statePropertyRef>00000000-0000-0000-0001-000000000001</statePropertyRef>
                </stateProperties>

                <accessTypes>
                  <accessTypeRef>1d6d25bc-4e85-43ab-a42d-fb5a829c30d5</accessTypeRef>
                  <accessTypeRef>62dfcd92-a480-4d57-95f1-28c0f5996b3a</accessTypeRef>
                  <accessTypeRef>11186122-6de0-4194-b434-9979230c41fd</accessTypeRef>
                  <accessTypeRef>305fbb40-75c8-423a-84b2-b26ea9e7cae7</accessTypeRef>
                  <accessTypeRef>67cea479-0be7-4e2f-b2e0-bb1fcc9ea1d6</accessTypeRef>
                  <accessTypeRef>203b7478-96f1-4bf1-b4ea-5bdd1206252c</accessTypeRef>
                  <accessTypeRef>00000002-0001-0000-0000-000000000000</accessTypeRef>
                </accessTypes>
              </class>

              <class id=""00000000-0000-0000-0002-000000000000"" name=""Rubicon.Security.UnitTests.TestDomain.PaperFile"" base=""00000000-0000-0000-0001-000000000000"">
                <stateProperties>
                  <statePropertyRef>00000000-0000-0000-0001-000000000001</statePropertyRef>
                  <statePropertyRef>00000000-0000-0000-0002-000000000001</statePropertyRef>
                </stateProperties>

                <accessTypes>
                  <accessTypeRef>1d6d25bc-4e85-43ab-a42d-fb5a829c30d5</accessTypeRef>
                  <accessTypeRef>62dfcd92-a480-4d57-95f1-28c0f5996b3a</accessTypeRef>
                  <accessTypeRef>11186122-6de0-4194-b434-9979230c41fd</accessTypeRef>
                  <accessTypeRef>305fbb40-75c8-423a-84b2-b26ea9e7cae7</accessTypeRef>
                  <accessTypeRef>67cea479-0be7-4e2f-b2e0-bb1fcc9ea1d6</accessTypeRef>
                  <accessTypeRef>203b7478-96f1-4bf1-b4ea-5bdd1206252c</accessTypeRef>
                  <accessTypeRef>00000002-0001-0000-0000-000000000000</accessTypeRef>
                </accessTypes>
              </class>
            </classes>

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

            <accessTypes>
              <accessType id=""1d6d25bc-4e85-43ab-a42d-fb5a829c30d5"" name=""Create"" value=""0"" />
              <accessType id=""62dfcd92-a480-4d57-95f1-28c0f5996b3a"" name=""Read"" value=""1"" />
              <accessType id=""11186122-6de0-4194-b434-9979230c41fd"" name=""Edit"" value=""2"" />
              <accessType id=""305fbb40-75c8-423a-84b2-b26ea9e7cae7"" name=""Delete"" value=""3"" />
              <accessType id=""67cea479-0be7-4e2f-b2e0-bb1fcc9ea1d6"" name=""Search"" value=""4"" />
              <accessType id=""203b7478-96f1-4bf1-b4ea-5bdd1206252c"" name=""Find"" value=""5"" />
              <accessType id=""00000002-0001-0000-0000-000000000000"" name=""Journalize"" value=""0"" />
              <accessType id=""00000002-0002-0000-0000-000000000000"" name=""Archive"" value=""1"" />
            </accessTypes>

            <abstractRoles>
              <abstractRole id=""00000003-0001-0000-0000-000000000000"" name=""Clerk"" value=""0"" />
              <abstractRole id=""00000003-0002-0000-0000-000000000000"" name=""Secretary"" value=""1"" />
              <abstractRole id=""00000004-0001-0000-0000-000000000000"" name=""Administrator"" value=""0"" />
            </abstractRoles>
          </securityMetadata>";

      _importer.Import (GetXmlDocument (metadataXml));

      Assert.AreEqual (2, _importer.Classes.Count, "Class count");
      Assert.AreEqual (2, _importer.StateProperties.Count, "State property count");
      Assert.AreEqual (3, _importer.AbstractRoles.Count, "Abstract role count");
      Assert.AreEqual (8, _importer.AccessTypes.Count, "Access type count");
    }

    private XmlDocument GetXmlDocument (string metadataXml)
    {
      XmlDocument metadataXmlDocument = new XmlDocument ();
      metadataXmlDocument.LoadXml (metadataXml);

      return metadataXmlDocument;
    }
  }
}
