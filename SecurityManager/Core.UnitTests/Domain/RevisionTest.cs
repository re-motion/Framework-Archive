using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain;

namespace Rubicon.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class RevisionTest : DomainTest
  {
    [Test]
    public void GetRevision ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      dbFixtures.CreateEmptyDomain();

      using (ClientTransaction.NewTransaction ().EnterNonReturningScope ())
      {
        Assert.AreEqual (0, Revision.GetRevision ());
      }
    }

    [Test]
    public void IncrementRevision ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();
      using (ClientTransaction.NewTransaction ().EnterNonReturningScope ())
      {
        Revision.IncrementRevision ();
      }

      using (ClientTransaction.NewTransaction ().EnterNonReturningScope ())
      {
        Assert.AreEqual (1, Revision.GetRevision ());
      }
    }
  }
}