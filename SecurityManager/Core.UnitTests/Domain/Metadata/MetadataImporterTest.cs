using System;
using System.Xml;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata
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

      SecurableClassDefinition actualClass1 = _importer.Classes[new Guid ("00000000-0000-0000-0001-000000000000")];
      Assert.AreEqual (0, actualClass1.Index);
      Assert.AreEqual ("Rubicon.Security.UnitTests.TestDomain.File", actualClass1.Name);
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

      SecurableClassDefinition actualClass1 = _importer.Classes[new Guid ("00000000-0000-0000-0001-000000000000")];
      Assert.AreEqual (0, actualClass1.Index);
      Assert.AreEqual ("Rubicon.Security.UnitTests.TestDomain.File", actualClass1.Name);

      SecurableClassDefinition actualClass2 = _importer.Classes[new Guid ("00000000-0000-0000-0002-000000000000")];
      Assert.AreEqual (1, actualClass2.Index);
      Assert.AreEqual ("Rubicon.Security.UnitTests.TestDomain.Directory", actualClass2.Name);
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
              <abstractRole id=""00000003-0001-0000-0000-000000000000"" name=""Clerk|Rubicon.Security.UnitTests.TestDomain.DomainAbstractRoles, Rubicon.Security.UnitTests.TestDomain"" value=""0"" />
              <abstractRole id=""00000003-0002-0000-0000-000000000000"" name=""Secretary|Rubicon.Security.UnitTests.TestDomain.DomainAbstractRoles, Rubicon.Security.UnitTests.TestDomain"" value=""1"" />
              <abstractRole id=""00000004-0001-0000-0000-000000000000"" name=""Administrator|Rubicon.Security.UnitTests.TestDomain.SpecialAbstractRoles, Rubicon.Security.UnitTests.TestDomain"" value=""0"" />
            </abstractRoles>
          </securityMetadata>
          ";

      _importer.Import (GetXmlDocument (metadataXml));

      Assert.AreEqual (0, _importer.Classes.Count, "Class count");
      Assert.AreEqual (0, _importer.StateProperties.Count, "State property count");
      Assert.AreEqual (3, _importer.AbstractRoles.Count, "Abstract role count");
      Assert.AreEqual (0, _importer.AccessTypes.Count, "Access type count");

      AbstractRoleDefinition expectedRole1 = _testHelper.CreateClerkAbstractRole (0);
      MetadataObjectAssert.AreEqual (expectedRole1, _importer.AbstractRoles[expectedRole1.MetadataItemID], "Abstract Role Clerk");

      AbstractRoleDefinition expectedRole2 = _testHelper.CreateSecretaryAbstractRole (1);
      MetadataObjectAssert.AreEqual (expectedRole2, _importer.AbstractRoles[expectedRole2.MetadataItemID], "Abstract Role Secretary");

      AbstractRoleDefinition expectedRole3 = _testHelper.CreateAdministratorAbstractRole (2);
      MetadataObjectAssert.AreEqual (expectedRole3, _importer.AbstractRoles[expectedRole3.MetadataItemID], "Abstract Role Administrator");
    }

    [Test]
    public void Import_3AccessTypes ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes />
            <stateProperties />
            <accessTypes>
              <accessType id=""1d6d25bc-4e85-43ab-a42d-fb5a829c30d5"" name=""Create|Rubicon.Security.GeneralAccessTypes, Rubicon.Security"" value=""0"" />
              <accessType id=""62dfcd92-a480-4d57-95f1-28c0f5996b3a"" name=""Read|Rubicon.Security.GeneralAccessTypes, Rubicon.Security"" value=""1"" />
              <accessType id=""11186122-6de0-4194-b434-9979230c41fd"" name=""Edit|Rubicon.Security.GeneralAccessTypes, Rubicon.Security"" value=""2"" />
            </accessTypes>
            <abstractRoles />
          </securityMetadata>
          ";

      _importer.Import (GetXmlDocument (metadataXml));

      Assert.AreEqual (0, _importer.Classes.Count, "Class count");
      Assert.AreEqual (0, _importer.StateProperties.Count, "State property count");
      Assert.AreEqual (0, _importer.AbstractRoles.Count, "Abstract role count");
      Assert.AreEqual (3, _importer.AccessTypes.Count, "Access type count");

      AccessTypeDefinition expectedAccessType1 = _testHelper.CreateAccessTypeCreate (0);
      MetadataObjectAssert.AreEqual (expectedAccessType1, _importer.AccessTypes[expectedAccessType1.MetadataItemID], "Access Type Create");

      AccessTypeDefinition expectedAccessType2 = _testHelper.CreateAccessTypeRead (1);
      MetadataObjectAssert.AreEqual (expectedAccessType2, _importer.AccessTypes[expectedAccessType2.MetadataItemID], "Access Type Read");

      AccessTypeDefinition expectedAccessType3 = _testHelper.CreateAccessTypeEdit (2);
      MetadataObjectAssert.AreEqual (expectedAccessType3, _importer.AccessTypes[expectedAccessType3.MetadataItemID], "Access Type Edit");
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

      StatePropertyDefinition expectedProperty1 = _testHelper.CreateFileStateProperty (0);
      StatePropertyDefinition actualProperty1 = _importer.StateProperties[expectedProperty1.MetadataItemID];
      Assert.IsNotNull (actualProperty1, "State property not found");
      MetadataObjectAssert.AreEqual (expectedProperty1, actualProperty1, "State property");

      StatePropertyDefinition expectedProperty2 = _testHelper.CreateConfidentialityProperty (1);
      StatePropertyDefinition actualProperty2 = _importer.StateProperties[expectedProperty2.MetadataItemID];
      Assert.IsNotNull (actualProperty2, "Confidentiality property not found");
      MetadataObjectAssert.AreEqual (expectedProperty2, actualProperty2, "Confidentiality property");
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
              <accessType id=""1d6d25bc-4e85-43ab-a42d-fb5a829c30d5"" name=""Create|Rubicon.Security.GeneralAccessTypes, Rubicon.Security"" value=""0"" />
              <accessType id=""62dfcd92-a480-4d57-95f1-28c0f5996b3a"" name=""Read|Rubicon.Security.GeneralAccessTypes, Rubicon.Security"" value=""1"" />
              <accessType id=""11186122-6de0-4194-b434-9979230c41fd"" name=""Edit|Rubicon.Security.GeneralAccessTypes, Rubicon.Security"" value=""2"" />
              <accessType id=""305fbb40-75c8-423a-84b2-b26ea9e7cae7"" name=""Delete|Rubicon.Security.GeneralAccessTypes, Rubicon.Security"" value=""3"" />
              <accessType id=""67cea479-0be7-4e2f-b2e0-bb1fcc9ea1d6"" name=""Search|Rubicon.Security.GeneralAccessTypes, Rubicon.Security"" value=""4"" />
              <accessType id=""203b7478-96f1-4bf1-b4ea-5bdd1206252c"" name=""Find|Rubicon.Security.GeneralAccessTypes, Rubicon.Security"" value=""5"" />
              <accessType id=""00000002-0001-0000-0000-000000000000"" name=""Journalize|Rubicon.Security.UnitTests.TestDomain.DomainAccessTypes, Rubicon.Security.UnitTests.TestDomain"" value=""0"" />
              <accessType id=""00000002-0002-0000-0000-000000000000"" name=""Archive|Rubicon.Security.UnitTests.TestDomain.DomainAccessTypes, Rubicon.Security.UnitTests.TestDomain"" value=""1"" />
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
              <accessType id=""1d6d25bc-4e85-43ab-a42d-fb5a829c30d5"" name=""Create|Rubicon.Security.GeneralAccessTypes, Rubicon.Security"" value=""0"" />
              <accessType id=""62dfcd92-a480-4d57-95f1-28c0f5996b3a"" name=""Read|Rubicon.Security.GeneralAccessTypes, Rubicon.Security"" value=""1"" />
              <accessType id=""11186122-6de0-4194-b434-9979230c41fd"" name=""Edit|Rubicon.Security.GeneralAccessTypes, Rubicon.Security"" value=""2"" />
              <accessType id=""305fbb40-75c8-423a-84b2-b26ea9e7cae7"" name=""Delete|Rubicon.Security.GeneralAccessTypes, Rubicon.Security"" value=""3"" />
              <accessType id=""67cea479-0be7-4e2f-b2e0-bb1fcc9ea1d6"" name=""Search|Rubicon.Security.GeneralAccessTypes, Rubicon.Security"" value=""4"" />
              <accessType id=""203b7478-96f1-4bf1-b4ea-5bdd1206252c"" name=""Find|Rubicon.Security.GeneralAccessTypes, Rubicon.Security"" value=""5"" />
              <accessType id=""00000002-0001-0000-0000-000000000000"" name=""Journalize|Rubicon.Security.UnitTests.TestDomain.DomainAccessTypes, Rubicon.Security.UnitTests.TestDomain"" value=""0"" />
              <accessType id=""00000002-0002-0000-0000-000000000000"" name=""Archive|Rubicon.Security.UnitTests.TestDomain.DomainAccessTypes, Rubicon.Security.UnitTests.TestDomain"" value=""1"" />
            </accessTypes>

            <abstractRoles>
              <abstractRole id=""00000003-0001-0000-0000-000000000000"" name=""Clerk|Rubicon.Security.UnitTests.TestDomain.DomainAbstractRoles, Rubicon.Security.UnitTests.TestDomain"" value=""0"" />
              <abstractRole id=""00000003-0002-0000-0000-000000000000"" name=""Secretary|Rubicon.Security.UnitTests.TestDomain.DomainAbstractRoles, Rubicon.Security.UnitTests.TestDomain"" value=""1"" />
              <abstractRole id=""00000004-0001-0000-0000-000000000000"" name=""Administrator|Rubicon.Security.UnitTests.TestDomain.SpecialAbstractRoles, Rubicon.Security.UnitTests.TestDomain"" value=""0"" />
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

    [Test]
    [ExpectedException (typeof (ImportException),
       ExpectedMessage = "The base class '00000000-0000-0000-0001-000000000000' referenced by the class '00000000-0000-0000-0002-000000000000' could not be found.")]
    public void Import_MissingBaseClass ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0002-000000000000"" name=""Rubicon.Security.UnitTests.TestDomain.Directory"" base=""00000000-0000-0000-0001-000000000000"" />
            </classes>
            <stateProperties />
            <accessTypes />
            <abstractRoles />
          </securityMetadata>
          ";

      _importer.Import (GetXmlDocument (metadataXml));
    }

    [Test]
    [ExpectedException (typeof (ImportException),
       ExpectedMessage = "The state property '00000000-0000-0000-0001-000000000001' referenced by the class '00000000-0000-0000-0001-000000000000' could not be found.")]
    public void Import_MissingStateProperty ()
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
            </stateProperties>
            <accessTypes />
            <abstractRoles />
          </securityMetadata>
          ";

      _importer.Import (GetXmlDocument (metadataXml));
    }

    [Test]
    [ExpectedException (typeof (ImportException),
        ExpectedMessage = "The access type '62dfcd92-a480-4d57-95f1-28c0f5996b3a' referenced by the class '00000000-0000-0000-0001-000000000000' could not be found.")]
    public void Import_MissingAccessType ()
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
              <accessType id=""1d6d25bc-4e85-43ab-a42d-fb5a829c30d5"" name=""Create|Rubicon.Security.GeneralAccessTypes, Rubicon.Security"" value=""0"" />
              <accessType id=""11186122-6de0-4194-b434-9979230c41fd"" name=""Edit|Rubicon.Security.GeneralAccessTypes, Rubicon.Security"" value=""2"" />
              <accessType id=""305fbb40-75c8-423a-84b2-b26ea9e7cae7"" name=""Delete|Rubicon.Security.GeneralAccessTypes, Rubicon.Security"" value=""3"" />
              <accessType id=""67cea479-0be7-4e2f-b2e0-bb1fcc9ea1d6"" name=""Search|Rubicon.Security.GeneralAccessTypes, Rubicon.Security"" value=""4"" />
              <accessType id=""203b7478-96f1-4bf1-b4ea-5bdd1206252c"" name=""Find|Rubicon.Security.GeneralAccessTypes, Rubicon.Security"" value=""5"" />
              <accessType id=""00000002-0001-0000-0000-000000000000"" name=""Journalize|Rubicon.Security.UnitTests.TestDomain.DomainAccessTypes, Rubicon.Security.UnitTests.TestDomain"" value=""0"" />
              <accessType id=""00000002-0002-0000-0000-000000000000"" name=""Archive|Rubicon.Security.UnitTests.TestDomain.DomainAccessTypes, Rubicon.Security.UnitTests.TestDomain"" value=""1"" />
            </accessTypes>
            
            <abstractRoles />
          </securityMetadata>
          ";

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
