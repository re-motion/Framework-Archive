using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Data.DomainObjects;
using System.Globalization;
using System.Threading;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata.MetadataObjectTests
{
  [TestFixture]
  public class Test : DomainTest
  {
    // TODO: Remove database dependency
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();

      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateSecurableClassDefinitionWithLocalizedNames ();
    }

    [Test]
    public void DisplayName_German ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      MetadataObject metadataObject = new SecurableClassDefinition (transaction);
      Culture culture = new Culture (transaction, "de");
      LocalizedName name = new LocalizedName (transaction, "Testklasse", culture, metadataObject);

      CultureInfo threadCulture = Thread.CurrentThread.CurrentCulture;
      CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture ("de");
      Thread.CurrentThread.CurrentUICulture = cultureInfo;

      Assert.AreEqual ("Testklasse", metadataObject.DisplayName);
    }

    [Test]
    public void GetLocalizedName_ExistingCulture ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Culture culture = Culture.Find (transaction, "de");
      MetadataObject metadataObject = MetadataObject.Find (transaction, "b8621bc9-9ab3-4524-b1e4-582657d6b420");

      LocalizedName localizedName = metadataObject.GetLocalizedName (culture);

      Assert.IsNotNull (localizedName);
      Assert.AreEqual ("Klasse", localizedName.Text);
    }

    [Test]
    public void GetLocalizedName_NotExistingCulture ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Culture culture = Culture.Find (transaction, "ru");
      MetadataObject metadataObject = MetadataObject.Find (transaction, "b8621bc9-9ab3-4524-b1e4-582657d6b420");

      LocalizedName localizedName = metadataObject.GetLocalizedName (culture);

      Assert.IsNull (localizedName);
    }

    [Test]
    public void GetLocalizedName_ExistingCultureName ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      MetadataObject metadataObject = MetadataObject.Find (transaction, "b8621bc9-9ab3-4524-b1e4-582657d6b420");

      LocalizedName localizedName = metadataObject.GetLocalizedName ("de");

      Assert.IsNotNull (localizedName);
      Assert.AreEqual ("Klasse", localizedName.Text);
    }

    [Test]
    public void GetLocalizedName_NotExistingCultureName ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      MetadataObject metadataObject = MetadataObject.Find (transaction, "b8621bc9-9ab3-4524-b1e4-582657d6b420");

      LocalizedName localizedName = metadataObject.GetLocalizedName ("ru");

      Assert.IsNull (localizedName);
    }

    [Test]
    public void SetAndGet_Index ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      MetadataObject metadataObject = new SecurableClassDefinition (transaction);

      metadataObject.Index = 1;
      Assert.AreEqual (1, metadataObject.Index);
    }
  }
}
