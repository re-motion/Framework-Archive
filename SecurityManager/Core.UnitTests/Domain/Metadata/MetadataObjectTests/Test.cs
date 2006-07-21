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
    private ClientTransaction _transaction;
    private MetadataObject _metadataObject;
    private Culture _cultureInvariant;
    private Culture _cultureDe;
    private Culture _cultureDeAt;
    private Culture _cultureRu;
    private CultureInfo _backupUICulture;
    private CultureInfo _backupCulture;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = new ClientTransaction ();
      _metadataObject = new SecurableClassDefinition (_transaction);
      _metadataObject.Name = "Technical Name";

      _cultureInvariant = new Culture (_transaction, string.Empty);
      _cultureDe = new Culture (_transaction, "de");
      _cultureDeAt = new Culture (_transaction, "de-AT");
      _cultureRu = new Culture (_transaction, "ru");

      _backupCulture = Thread.CurrentThread.CurrentCulture;
      _backupUICulture = Thread.CurrentThread.CurrentUICulture;
    }

    [TearDown]
    public void TearDown ()
    {
      Thread.CurrentThread.CurrentCulture = _backupCulture;
      Thread.CurrentThread.CurrentUICulture = _backupUICulture;
    }

    [Test]
    public void DisplayName_NeutralCulture ()
    {
      CreateLocalizedName (_metadataObject, _cultureInvariant, "Class Invariant");
      CreateLocalizedName (_metadataObject, _cultureDe, "Class de");
      CreateLocalizedName (_metadataObject, _cultureDeAt, "Class de-AT");
      Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo (_cultureDe.CultureName);

      Assert.AreEqual ("Class de", _metadataObject.DisplayName);
    }

    [Test]
    public void DisplayName_SpecificCulture ()
    {
      CreateLocalizedName (_metadataObject, _cultureInvariant, "Class Invariant");
      CreateLocalizedName (_metadataObject, _cultureDe, "Class de");
      CreateLocalizedName (_metadataObject, _cultureDeAt, "Class de-AT");
      Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo (_cultureDeAt.CultureName);

      Assert.AreEqual ("Class de-AT", _metadataObject.DisplayName);
    }

    [Test]
    public void DisplayName_Invariant ()
    {
      CreateLocalizedName (_metadataObject, _cultureInvariant, "Class Invariant");
      CreateLocalizedName (_metadataObject, _cultureDe, "Class de");
      CreateLocalizedName (_metadataObject, _cultureDeAt, "Class de-AT");
      Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

      Assert.AreEqual ("Class Invariant", _metadataObject.DisplayName);
    }

    [Test]
    public void DisplayName_UnknownCulture ()
    {
      Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo ("en");

      Assert.AreEqual ("Technical Name", _metadataObject.DisplayName);
    }

    [Test]
    public void DisplayName_FallbackToNeutralCulture ()
    {
      CreateLocalizedName (_metadataObject, _cultureInvariant, "Class Invariant");
      CreateLocalizedName (_metadataObject, _cultureDe, "Class de");
      Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo (_cultureDeAt.CultureName);

      Assert.AreEqual ("Class de", _metadataObject.DisplayName);
    }

    [Test]
    public void DisplayName_FallbackToInvariantCulture ()
    {
      CreateLocalizedName (_metadataObject, _cultureInvariant, "Class Invariant");
      Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo (_cultureDeAt.CultureName);

      Assert.AreEqual ("Class Invariant", _metadataObject.DisplayName);
    }

    [Test]
    public void GetLocalizedName_ExistingCulture ()
    {
      LocalizedName expectedLocalizedName = CreateLocalizedName (_metadataObject, _cultureDe, "Class de");

      Assert.AreSame (expectedLocalizedName, _metadataObject.GetLocalizedName (_cultureDe));
    }

    [Test]
    public void GetLocalizedName_NotExistingLocalizedNameForCulture ()
    {
      Assert.IsNull (_metadataObject.GetLocalizedName (_cultureRu));
    }

    [Test]
    public void GetLocalizedName_ExistingCultureName ()
    {
      LocalizedName expectedLocalizedName = CreateLocalizedName (_metadataObject, _cultureDe, "Class de");

      Assert.AreSame (expectedLocalizedName, _metadataObject.GetLocalizedName ("de"));
    }

    [Test]
    public void GetLocalizedName_NotExistingCultureName ()
    {
      LocalizedName localizedName = _metadataObject.GetLocalizedName ("ru");

      Assert.IsNull (localizedName);
    }

    [Test]
    public void SetAndGet_Index ()
    {
      _metadataObject.Index = 1;
      Assert.AreEqual (1, _metadataObject.Index);
    }

    private LocalizedName CreateLocalizedName (MetadataObject metadataObject, Culture culture, string text)
    {
      return new LocalizedName (metadataObject.ClientTransaction, text, culture, metadataObject);
    }
  }
}
