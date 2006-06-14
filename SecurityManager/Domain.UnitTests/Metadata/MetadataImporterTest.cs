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
    private MetadataTestHelper _testHelper;
    private MetadataImporter _importer;

    public override void SetUp ()
    {
      base.SetUp ();

      _testHelper = new MetadataTestHelper ();
      _importer = new MetadataImporter (_testHelper.Transaction);
    }

    [Test]
    public void Import_EmptyMetadataFile ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes />
            <stateProperties />
            <accessTypes />
            <abstractRoles />
          </securityMetadata>
          ";

      _importer.Import (GetXmlDocument (metadataXml));

      Assert.AreEqual (0, _importer.Classes.Count, "Class count");
      Assert.AreEqual (0, _importer.StateProperties.Count, "State property count");
      Assert.AreEqual (0, _importer.AbstractRoles.Count, "Abstract role count");
      Assert.AreEqual (0, _importer.AccessTypes.Count, "Access type count");
    }

    [Test]
    public void Import_1SecurableClass ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Rubicon.Security.UnitTests.TestDomain.File"" />
            </classes>
            <stateProperties />
            <accessTypes />
            <abstractRoles />
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
    public void Import_2SecurableClasses ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Rubicon.Security.UnitTests.TestDomain.File"" />
              <class id=""00000000-0000-0000-0002-000000000000"" name=""Rubicon.Security.UnitTests.TestDomain.Directory"" />
            </classes>
            <stateProperties />
            <accessTypes />
            <abstractRoles />
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
    public void Import_3AbstractRoles ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes />
            <stateProperties />
            <accessTypes />
            <abstractRoles>
              <abstractRole id=""00000003-0001-0000-0000-000000000000"" typeName=""Rubicon.Security.UnitTests.TestDomain.DomainAbstractRole, Rubicon.Security.UnitTests.TestDomain"" name=""Clerk"" value=""0"" />
              <abstractRole id=""00000003-0002-0000-0000-000000000000"" typeName=""Rubicon.Security.UnitTests.TestDomain.DomainAbstractRole, Rubicon.Security.UnitTests.TestDomain"" name=""Secretary"" value=""1"" />
              <abstractRole id=""00000004-0001-0000-0000-000000000000"" typeName=""Rubicon.Security.UnitTests.TestDomain.SpecialAbstractRole, Rubicon.Security.UnitTests.TestDomain"" name=""Administrator"" value=""0"" />
            </abstractRoles>
          </securityMetadata>
          ";

      _importer.Import (GetXmlDocument (metadataXml));

      Assert.AreEqual (0, _importer.Classes.Count, "Class count");
      Assert.AreEqual (0, _importer.StateProperties.Count, "State property count");
      Assert.AreEqual (3, _importer.AbstractRoles.Count, "Abstract role count");
      Assert.AreEqual (0, _importer.AccessTypes.Count, "Access type count");

      AbstractRoleDefinition expectedRole = _testHelper.CreateClerkAbstractRole ();
      MetadataObjectAssert.AreEqual (expectedRole, _importer.AbstractRoles[expectedRole.MetadataItemID], "Abstract Role Clerk");

      expectedRole = _testHelper.CreateSecretaryAbstractRole ();
      MetadataObjectAssert.AreEqual (expectedRole, _importer.AbstractRoles[expectedRole.MetadataItemID], "Abstract Role Secretary");

      expectedRole = _testHelper.CreateAdministratorAbstractRole ();
      MetadataObjectAssert.AreEqual (expectedRole, _importer.AbstractRoles[expectedRole.MetadataItemID], "Abstract Role Administrator");
    }

    [Test]
    public void Import_3AccessTypes ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes />
            <stateProperties />
            <accessTypes>
              <accessType id=""1d6d25bc-4e85-43ab-a42d-fb5a829c30d5"" typeName=""Rubicon.Security.GeneralAccessType, Rubicon.Security"" name=""Create"" value=""0"" />
              <accessType id=""62dfcd92-a480-4d57-95f1-28c0f5996b3a"" typeName=""Rubicon.Security.GeneralAccessType, Rubicon.Security"" name=""Read"" value=""1"" />
              <accessType id=""11186122-6de0-4194-b434-9979230c41fd"" typeName=""Rubicon.Security.GeneralAccessType, Rubicon.Security"" name=""Edit"" value=""2"" />
            </accessTypes>
            <abstractRoles />
          </securityMetadata>
          ";

      _importer.Import (GetXmlDocument (metadataXml));

      Assert.AreEqual (0, _importer.Classes.Count, "Class count");
      Assert.AreEqual (0, _importer.StateProperties.Count, "State property count");
      Assert.AreEqual (0, _importer.AbstractRoles.Count, "Abstract role count");
      Assert.AreEqual (3, _importer.AccessTypes.Count, "Access type count");

      AccessTypeDefinition expectedAccessType = _testHelper.CreateAccessTypeCreate ();
      MetadataObjectAssert.AreEqual (expectedAccessType, _importer.AccessTypes[expectedAccessType.MetadataItemID], "Access Type Create");

      expectedAccessType = _testHelper.CreateAccessTypeRead ();
      MetadataObjectAssert.AreEqual (expectedAccessType, _importer.AccessTypes[expectedAccessType.MetadataItemID], "Access Type Read");

      expectedAccessType = _testHelper.CreateAccessTypeEdit ();
      MetadataObjectAssert.AreEqual (expectedAccessType, _importer.AccessTypes[expectedAccessType.MetadataItemID], "Access Type Edit");
    }

    [Test]
    public void Import_2StateProperties ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes />
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
            <accessTypes />
            <abstractRoles />
          </securityMetadata>
          ";

      _importer.Import (GetXmlDocument (metadataXml));

      Assert.AreEqual (0, _importer.Classes.Count, "Class count");
      Assert.AreEqual (2, _importer.StateProperties.Count, "State property count");
      Assert.AreEqual (0, _importer.AbstractRoles.Count, "Abstract role count");
      Assert.AreEqual (0, _importer.AccessTypes.Count, "Access type count");

      StatePropertyDefinition expectedProperty = _testHelper.CreateFileStateProperty ();
      StatePropertyDefinition actualProperty = _importer.StateProperties[expectedProperty.MetadataItemID];
      Assert.IsNotNull (actualProperty, "State property not found");
      MetadataObjectAssert.AreEqual (expectedProperty, actualProperty, "State property");

      expectedProperty = _testHelper.CreateConfidentialityProperty ();
      actualProperty = _importer.StateProperties[expectedProperty.MetadataItemID];
      Assert.IsNotNull (actualProperty, "Confidentiality property not found");
      MetadataObjectAssert.AreEqual (expectedProperty, actualProperty, "Confidentiality property");
    }

    [Test]
    public void Import_ABaseClassAndADerivedClass ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Rubicon.Security.UnitTests.TestDomain.File"" />
              <class id=""00000000-0000-0000-0002-000000000000"" name=""Rubicon.Security.UnitTests.TestDomain.PaperFile"" base=""00000000-0000-0000-0001-000000000000"" />
            </classes>
            <stateProperties />
            <accessTypes />
            <abstractRoles />
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
    public void Import_1ClassAnd2StateProperties ()
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
            <accessTypes />
            <abstractRoles />
          </securityMetadata>";

      _importer.Import (GetXmlDocument (metadataXml));

      Assert.AreEqual (1, _importer.Classes.Count, "Class count");
      Assert.AreEqual (2, _importer.StateProperties.Count, "State property count");
      Assert.AreEqual (0, _importer.AbstractRoles.Count, "Abstract role count");
      Assert.AreEqual (0, _importer.AccessTypes.Count, "Access type count");

      SecurableClassDefinition classDefinition = _importer.Classes[new Guid ("00000000-0000-0000-0001-000000000000")];
      StatePropertyDefinition property1 = _importer.StateProperties[new Guid ("00000000-0000-0000-0002-000000000001")];
      StatePropertyDefinition property2 = _importer.StateProperties[new Guid ("00000000-0000-0000-0001-000000000001")];

      Assert.AreEqual (1, classDefinition.StateProperties.Count, "State property count");
      Assert.AreSame (property2, classDefinition.StateProperties[0]);
    }

    [Test]
    public void Import_1ClassAnd8AccessTypes ()
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

            <stateProperties />

            <accessTypes>
              <accessType id=""1d6d25bc-4e85-43ab-a42d-fb5a829c30d5"" typeName=""Rubicon.Security.GeneralAccessType, Rubicon.Security"" name=""Create"" value=""0"" />
              <accessType id=""62dfcd92-a480-4d57-95f1-28c0f5996b3a"" typeName=""Rubicon.Security.GeneralAccessType, Rubicon.Security"" name=""Read"" value=""1"" />
              <accessType id=""11186122-6de0-4194-b434-9979230c41fd"" typeName=""Rubicon.Security.GeneralAccessType, Rubicon.Security"" name=""Edit"" value=""2"" />
              <accessType id=""305fbb40-75c8-423a-84b2-b26ea9e7cae7"" typeName=""Rubicon.Security.GeneralAccessType, Rubicon.Security"" name=""Delete"" value=""3"" />
              <accessType id=""67cea479-0be7-4e2f-b2e0-bb1fcc9ea1d6"" typeName=""Rubicon.Security.GeneralAccessType, Rubicon.Security"" name=""Search"" value=""4"" />
              <accessType id=""203b7478-96f1-4bf1-b4ea-5bdd1206252c"" typeName=""Rubicon.Security.GeneralAccessType, Rubicon.Security"" name=""Find"" value=""5"" />
              <accessType id=""00000002-0001-0000-0000-000000000000"" typeName=""Rubicon.Security.UnitTests.TestDomain.DomainAccessType, Rubicon.Security.UnitTests.TestDomain"" name=""Journalize"" value=""0"" />
              <accessType id=""00000002-0002-0000-0000-000000000000"" typeName=""Rubicon.Security.UnitTests.TestDomain.DomainAccessType, Rubicon.Security.UnitTests.TestDomain"" name=""Archive"" value=""1"" />
            </accessTypes>
            
            <abstractRoles />
          </securityMetadata>";

      _importer.Import (GetXmlDocument (metadataXml));

      Assert.AreEqual (1, _importer.Classes.Count, "Class count");
      Assert.AreEqual (0, _importer.StateProperties.Count, "State property count");
      Assert.AreEqual (0, _importer.AbstractRoles.Count, "Abstract role count");
      Assert.AreEqual (8, _importer.AccessTypes.Count, "Access type count");

      SecurableClassDefinition classDefinition = _importer.Classes[new Guid ("00000000-0000-0000-0001-000000000000")];
      AccessTypeDefinition accessType = _importer.AccessTypes[new Guid ("62dfcd92-a480-4d57-95f1-28c0f5996b3a")];

      Assert.AreEqual (1, classDefinition.AccessTypes.Count, "Access type count");
      Assert.AreSame (accessType, classDefinition.AccessTypes[0]);
    }

    [Test]
    public void Import_ABaseClassAndADerivedClassWith2StatePropertiesAnd8AccessTypesAnd3AbstractRoles ()
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
              <accessType id=""1d6d25bc-4e85-43ab-a42d-fb5a829c30d5"" typeName=""Rubicon.Security.GeneralAccessType, Rubicon.Security"" name=""Create"" value=""0"" />
              <accessType id=""62dfcd92-a480-4d57-95f1-28c0f5996b3a"" typeName=""Rubicon.Security.GeneralAccessType, Rubicon.Security"" name=""Read"" value=""1"" />
              <accessType id=""11186122-6de0-4194-b434-9979230c41fd"" typeName=""Rubicon.Security.GeneralAccessType, Rubicon.Security"" name=""Edit"" value=""2"" />
              <accessType id=""305fbb40-75c8-423a-84b2-b26ea9e7cae7"" typeName=""Rubicon.Security.GeneralAccessType, Rubicon.Security"" name=""Delete"" value=""3"" />
              <accessType id=""67cea479-0be7-4e2f-b2e0-bb1fcc9ea1d6"" typeName=""Rubicon.Security.GeneralAccessType, Rubicon.Security"" name=""Search"" value=""4"" />
              <accessType id=""203b7478-96f1-4bf1-b4ea-5bdd1206252c"" typeName=""Rubicon.Security.GeneralAccessType, Rubicon.Security"" name=""Find"" value=""5"" />
              <accessType id=""00000002-0001-0000-0000-000000000000"" typeName=""Rubicon.Security.UnitTests.TestDomain.DomainAccessType, Rubicon.Security.UnitTests.TestDomain"" name=""Journalize"" value=""0"" />
              <accessType id=""00000002-0002-0000-0000-000000000000"" typeName=""Rubicon.Security.UnitTests.TestDomain.DomainAccessType, Rubicon.Security.UnitTests.TestDomain"" name=""Archive"" value=""1"" />
            </accessTypes>

            <abstractRoles>
              <abstractRole id=""00000003-0001-0000-0000-000000000000"" typeName=""Rubicon.Security.UnitTests.TestDomain.DomainAbstractRole, Rubicon.Security.UnitTests.TestDomain"" name=""Clerk"" value=""0"" />
              <abstractRole id=""00000003-0002-0000-0000-000000000000"" typeName=""Rubicon.Security.UnitTests.TestDomain.DomainAbstractRole, Rubicon.Security.UnitTests.TestDomain"" name=""Secretary"" value=""1"" />
              <abstractRole id=""00000004-0001-0000-0000-000000000000"" typeName=""Rubicon.Security.UnitTests.TestDomain.SpecialAbstractRole, Rubicon.Security.UnitTests.TestDomain"" name=""Administrator"" value=""0"" />
            </abstractRoles>
          </securityMetadata>";

      _importer.Import (GetXmlDocument (metadataXml));

      Assert.AreEqual (2, _importer.Classes.Count, "Class count");
      Assert.AreEqual (2, _importer.StateProperties.Count, "State property count");
      Assert.AreEqual (3, _importer.AbstractRoles.Count, "Abstract role count");
      Assert.AreEqual (8, _importer.AccessTypes.Count, "Access type count");
    }

    [Test]
    [ExpectedException (typeof (System.Xml.Schema.XmlSchemaValidationException))]
    public void Import_InvalidXml ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes />
          </securityMetadata>";

      _importer.Import (GetXmlDocument (metadataXml));
    }

    private XmlDocument GetXmlDocument (string metadataXml)
    {
      XmlDocument metadataXmlDocument = new XmlDocument ();
      metadataXmlDocument.LoadXml (metadataXml);

      return metadataXmlDocument;
    }
  }
}
