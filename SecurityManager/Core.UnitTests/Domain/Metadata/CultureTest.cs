using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class CultureTest : DomainTest
  {
    private ClientTransaction _transaction;

    public override void SetUp ()
    {
      base.SetUp ();

      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateSecurableClassDefinitionWithLocalizedNames ();

      _transaction = new ClientTransaction ();
    }

    [Test]
    public void Find_Existing ()
    {
      Culture foundCulture = Culture.Find (_transaction, "de");

      Assert.IsNotNull (foundCulture);
      Assert.AreNotEqual (StateType.New, foundCulture.State);
      Assert.AreEqual ("de", foundCulture.CultureName);
    }

    [Test]
    public void Find_NotExisting ()
    {
      Culture foundCulture = Culture.Find (_transaction, "hu");

      Assert.IsNull (foundCulture);
    }
  }
}
