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
    private Culture _cultureDe;
    private Culture _cultureRu;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = new ClientTransaction ();
      _metadataObject = new SecurableClassDefinition (_transaction);
      _cultureDe = new Culture (_transaction, "de");
      _cultureRu = new Culture (_transaction, "ru");
    }

    [Test]
    public void DisplayName_German ()
    {
      CreateLocalizedName (_metadataObject, _cultureDe, "Klasse");

      CultureInfo threadCulture = Thread.CurrentThread.CurrentCulture;
      CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture ("de");
      Thread.CurrentThread.CurrentUICulture = cultureInfo;

      Assert.AreEqual ("Klasse", _metadataObject.DisplayName);
    }

    [Test]
    public void GetLocalizedName_ExistingCulture ()
    {
      LocalizedName expectedLocalizedName = CreateLocalizedName (_metadataObject, _cultureDe, "Klasse");

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
      LocalizedName expectedLocalizedName = CreateLocalizedName (_metadataObject, _cultureDe, "Klasse");

      Assert.AreSame (expectedLocalizedName, _metadataObject.GetLocalizedName (_cultureDe.CultureName));
    }

    [Test]
    public void GetLocalizedName_NotExistingCultureName ()
    {
      LocalizedName localizedName = _metadataObject.GetLocalizedName (_cultureRu.CultureName);

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
