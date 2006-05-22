using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using NUnit.Framework;

using Rubicon.Security.Metadata;
using Rubicon.Security.UnitTests.XmlAsserter;

namespace Rubicon.Security.UnitTests.Metadata
{
  [TestFixture]
  public class MetadataExtractorTest
  {
    private string _xmlTempFilename;
    private MetadataExtractor _extractor;

    [SetUp]
    public void SetUp ()
    {
      _xmlTempFilename = Path.GetTempFileName ();
      _extractor = new MetadataExtractor (new MetadataToXmlConverter ());
    }

    [TearDown]
    public void TearDown ()
    {
      File.Delete (_xmlTempFilename);
    }

    [Test]
    public void NoAssemblies ()
    {
      _extractor.Save (_xmlTempFilename);

      XmlDocument xmlDocument = new XmlDocument ();
      xmlDocument.Load (_xmlTempFilename);

      string expectedXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"" />";

      XmlAssert.AreDocumentsEqual (expectedXml, xmlDocument);
    }

    [Test]
    public void OneAssembly ()
    {
      Assembly testDomainAssembly = typeof (Rubicon.Security.UnitTests.TestDomain.File).Assembly;
      _extractor.AddAssembly (testDomainAssembly);

      _extractor.Save (_xmlTempFilename);

      XmlDocument xmlDocument = new XmlDocument ();
      xmlDocument.Load (_xmlTempFilename);

      string expectedXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Rubicon.Security.UnitTests.TestDomain.File"">
                <derivedClasses>
                  <classRef>00000000-0000-0000-0002-000000000000</classRef>
                </derivedClasses>

                <stateProperties>
                  <statePropertyRef>00000000-0000-0000-0001-000000000001</statePropertyRef>
                </stateProperties>

                <accessTypes>
                  <accessTypeRef>1d6d25bc-4e85-43ab-a42d-fb5a829c30d5</accessTypeRef>
                  <accessTypeRef>62dfcd92-a480-4d57-95f1-28c0f5996b3a</accessTypeRef>
                  <accessTypeRef>11186122-6de0-4194-b434-9979230c41fd</accessTypeRef>
                  <accessTypeRef>305fbb40-75c8-423a-84b2-b26ea9e7cae7</accessTypeRef>
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
              <accessType id=""203b7478-96f1-4bf1-b4ea-5bdd1206252c"" name=""Find"" value=""4"" />
              <accessType id=""00000002-0001-0000-0000-000000000000"" name=""Journalize"" value=""0"" />
            </accessTypes>

            <abstractRoles>
              <abstractRole id=""00000003-0001-0000-0000-000000000000"" name=""Clerk"" value=""0"" />
              <abstractRole id=""00000003-0002-0000-0000-000000000000"" name=""Secretary"" value=""1"" />
              <abstractRole id=""00000004-0001-0000-0000-000000000000"" name=""Administrator"" value=""0"" />
            </abstractRoles>
          </securityMetadata>";

      XmlAssert.AreDocumentsSimilar (expectedXml, xmlDocument);
    }

    [Test]
    public void OneAssemblyByPathName ()
    {
      _extractor.AddAssembly ("Rubicon.Security.UnitTests.TestDomain");

      _extractor.Save (_xmlTempFilename);

      XmlDocument xmlDocument = new XmlDocument ();
      xmlDocument.Load (_xmlTempFilename);

      string expectedXml = @"
          <securityMetadata xmlns=""http://www.rubicon-it.com/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Rubicon.Security.UnitTests.TestDomain.File"">
                <derivedClasses>
                  <classRef>00000000-0000-0000-0002-000000000000</classRef>
                </derivedClasses>

                <stateProperties>
                  <statePropertyRef>00000000-0000-0000-0001-000000000001</statePropertyRef>
                </stateProperties>

                <accessTypes>
                  <accessTypeRef>1d6d25bc-4e85-43ab-a42d-fb5a829c30d5</accessTypeRef>
                  <accessTypeRef>62dfcd92-a480-4d57-95f1-28c0f5996b3a</accessTypeRef>
                  <accessTypeRef>11186122-6de0-4194-b434-9979230c41fd</accessTypeRef>
                  <accessTypeRef>305fbb40-75c8-423a-84b2-b26ea9e7cae7</accessTypeRef>
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
              <accessType id=""203b7478-96f1-4bf1-b4ea-5bdd1206252c"" name=""Find"" value=""4"" />
              <accessType id=""00000002-0001-0000-0000-000000000000"" name=""Journalize"" value=""0"" />
            </accessTypes>

            <abstractRoles>
              <abstractRole id=""00000003-0001-0000-0000-000000000000"" name=""Clerk"" value=""0"" />
              <abstractRole id=""00000003-0002-0000-0000-000000000000"" name=""Secretary"" value=""1"" />
              <abstractRole id=""00000004-0001-0000-0000-000000000000"" name=""Administrator"" value=""0"" />
            </abstractRoles>
          </securityMetadata>";

      XmlAssert.AreDocumentsSimilar (expectedXml, xmlDocument);
    }
  }
}
