using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;
using System.Xml;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class CultureImporterTest : DomainTest
  {
    private ClientTransaction _transaction;
    private CultureImporter _importer;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = new ClientTransaction ();
      _importer = new CultureImporter (_transaction);
    }

    [Test]
    public void Import_EmptyCultureFile ()
    {
      string cultureXml = @"
          <localizedNames xmlns=""http://www.rubicon-it.com/Security/Metadata/Localization/1.0"" culture=""de"" />
          ";

      _importer.Import (GetXmlDocument (cultureXml));

      Assert.AreEqual (0, _importer.LocalizedNames.Count, "LocalizedNames count");
      Assert.IsNotNull (_importer.Cultures, "Cultures");
      Assert.AreEqual (1, _importer.Cultures.Count);
      Assert.AreEqual ("de", _importer.Cultures[0].CultureName);
    }

    [Test]
    public void Import_OneLocalizedName ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateSecurableClassDefinitionWithStates ();

      string cultureXml = @"
          <localizedNames xmlns=""http://www.rubicon-it.com/Security/Metadata/Localization/1.0"" culture=""de"">
            <localizedName ref=""b8621bc9-9ab3-4524-b1e4-582657d6b420"" comment=""Clerk|Rubicon.Security.UnitTests.TestDomain.DomainAbstractRole, Rubicon.Security.UnitTests.TestDomain"">
              Beamter
            </localizedName>
          </localizedNames>
          ";

      _importer.Import (GetXmlDocument (cultureXml));

      Assert.AreEqual (1, _importer.LocalizedNames.Count, "LocalizedNames count");
      Assert.AreEqual ("Beamter", _importer.LocalizedNames[0].Text);
      Assert.AreEqual (new Guid ("b8621bc9-9ab3-4524-b1e4-582657d6b420"), _importer.LocalizedNames[0].MetadataObject.MetadataItemID);
    }

    [Test]
    [ExpectedException (typeof (ImportException),
       "The metadata object with the ID 'ad1efa4c-cf5d-46b0-b775-d4e45f2dce7c' "
       + "('Clerk|Rubicon.Security.UnitTests.TestDomain.DomainAbstractRole, Rubicon.Security.UnitTests.TestDomain') "
       + "could not be found.")]
    public void Import_NotExistingMetadataObject ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateSecurableClassDefinitionWithStates ();

      string cultureXml = @"
          <localizedNames xmlns=""http://www.rubicon-it.com/Security/Metadata/Localization/1.0"" culture=""de"">
            <localizedName ref=""ad1efa4c-cf5d-46b0-b775-d4e45f2dce7c"" comment=""Clerk|Rubicon.Security.UnitTests.TestDomain.DomainAbstractRole, Rubicon.Security.UnitTests.TestDomain"">
              Beamter
            </localizedName>
          </localizedNames>
          ";

      _importer.Import (GetXmlDocument (cultureXml));
    }

    [Test]
    [ExpectedException (typeof (ImportException), "The metadata object with the ID 'ad1efa4c-cf5d-46b0-b775-d4e45f2dce7c' could not be found.")]
    public void Import_NotExistingMetadataObjectWithoutComment ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateSecurableClassDefinitionWithStates ();

      string cultureXml = @"
          <localizedNames xmlns=""http://www.rubicon-it.com/Security/Metadata/Localization/1.0"" culture=""de"">
            <localizedName ref=""ad1efa4c-cf5d-46b0-b775-d4e45f2dce7c"">
              Beamter
            </localizedName>
          </localizedNames>
          ";

      _importer.Import (GetXmlDocument (cultureXml));
    }

    [Test]
    public void Import_MultipleLocalizedNames ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateSecurableClassDefinitionWithStates ();

      string cultureXml = @"
          <localizedNames xmlns=""http://www.rubicon-it.com/Security/Metadata/Localization/1.0"" culture=""de"">
            <localizedName ref=""b8621bc9-9ab3-4524-b1e4-582657d6b420"" comment=""Clerk|Rubicon.Security.UnitTests.TestDomain.DomainAbstractRole, Rubicon.Security.UnitTests.TestDomain"">
              Beamter
            </localizedName>
            <localizedName ref=""93969f13-65d7-49f4-a456-a1686a4de3de"" comment=""Confidentiality"">
              Vertraulichkeit
            </localizedName>
            <localizedName ref=""93969f13-65d7-49f4-a456-a1686a4de3de|0"" comment=""Confidentiality|Public"">
              Öffentlich
            </localizedName>
          </localizedNames>
          ";

      _importer.Import (GetXmlDocument (cultureXml));

      Assert.AreEqual (3, _importer.LocalizedNames.Count, "LocalizedNames count");
      Assert.AreEqual ("Beamter", _importer.LocalizedNames[0].Text);
      Assert.AreEqual (new Guid ("b8621bc9-9ab3-4524-b1e4-582657d6b420"), _importer.LocalizedNames[0].MetadataObject.MetadataItemID);
      Assert.AreEqual ("Vertraulichkeit", _importer.LocalizedNames[1].Text);
      Assert.AreEqual (new Guid ("93969f13-65d7-49f4-a456-a1686a4de3de"), _importer.LocalizedNames[1].MetadataObject.MetadataItemID);
      Assert.AreEqual ("Öffentlich", _importer.LocalizedNames[2].Text);
    }

    [Test]
    [ExpectedException (typeof (System.Xml.Schema.XmlSchemaValidationException))]
    public void Import_InvalidXml ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateSecurableClassDefinitionWithStates ();

      string cultureXml = @"
          <localizedNames xmlns=""http://www.rubicon-it.com/Security/Metadata/Localization/1.0"" culture=""de"">
            <localizedName rf=""b8621bc9-9ab3-4524-b1e4-582657d6b420"" comment=""Clerk|Rubicon.Security.UnitTests.TestDomain.DomainAbstractRole, Rubicon.Security.UnitTests.TestDomain"">
              Beamter
            </localizedName>,
            <localizedName ref=""93969f13-65d7-49f4-a456-a1686a4de3de"" comment=""Confidentiality"">
              Vertraulichkeit
            </localizedName>
          </localizedNames>
          ";

      _importer.Import (GetXmlDocument (cultureXml));
    }

    private XmlDocument GetXmlDocument (string xml)
    {
      XmlDocument xmlDocument = new XmlDocument ();
      xmlDocument.LoadXml (xml);

      return xmlDocument;
    }
  }
}
