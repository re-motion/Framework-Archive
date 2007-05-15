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
    private ClientTransaction _clientTransaction;
    private OrganizationalStructureFactory _factory;

    public override void SetUp ()
    {
      base.SetUp ();
      
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();
      
      _clientTransaction = new ClientTransaction();
      _factory = new OrganizationalStructureFactory ();
    }

    [Test]
    public void Saving_OneSecurityManagerDomainObject ()
    {
      Client client = _factory.CreateClient (_clientTransaction);

      _clientTransaction.Commit ();

      Assert.AreEqual (1, Revision.GetRevision (_clientTransaction));
    }

    [Test]
    public void Saving_DisacardedDomainObject ()
    {
      Client client = _factory.CreateClient (_clientTransaction);
      client.Delete ();

      _clientTransaction.Commit ();

      Assert.AreEqual (0, Revision.GetRevision (_clientTransaction));
    }
  }
}