using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class CultureTest : DomainTest
  {
    private ClientTransaction _transaction;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
    
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateSecurableClassDefinitionWithLocalizedNames ();
    }

    public override void SetUp ()
    {
      base.SetUp ();

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
