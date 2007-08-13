using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.SecurityManager.UnitTests.Domain;

namespace Rubicon.SecurityManager.UnitTests.Persistence
{
  [TestFixture]
  public class RevisionSqlProviderExtensionTest : DomainTest
  {
    private OrganizationalStructureFactory _factory;

    public override void SetUp ()
    {
      base.SetUp ();

      new ClientTransactionScope();

      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();
      
      _factory = new OrganizationalStructureFactory ();
    }

    [Test]
    public void Saving_OneSecurityManagerDomainObject ()
    {
      Tenant tenant = _factory.CreateTenant ();

      ClientTransactionScope.CurrentTransaction.Commit ();

      Assert.AreEqual (1, Revision.GetRevision ());
    }

    [Test]
    public void Saving_DisacardedDomainObject ()
    {
      Tenant tenant = _factory.CreateTenant ();
      tenant.Delete ();

      ClientTransactionScope.CurrentTransaction.Commit ();

      Assert.AreEqual (0, Revision.GetRevision ());
    }
  }
}