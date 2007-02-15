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

    public override void SetUp ()
    {
      base.SetUp ();
      
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();
      
      _clientTransaction = new ClientTransaction();
    }

    [Test]
    public void Saving_OneSecurityManagerDomainObject ()
    {
      Client client = new Client (_clientTransaction);

      _clientTransaction.Commit ();

      Assert.AreEqual (1, Revision.GetRevision (_clientTransaction));
    }

    [Test]
    public void Saving_DisacardedDomainObject ()
    {
      Client client = new Client (_clientTransaction);
      client.Delete ();

      _clientTransaction.Commit ();

      Assert.AreEqual (0, Revision.GetRevision (_clientTransaction));
    }
  }
}