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
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();
      
      Assert.AreEqual (0, Revision.GetRevision (new ClientTransaction ()));
    }

    [Test]
    public void IncrementRevision ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();
      Revision.IncrementRevision (new ClientTransaction ());

      Assert.AreEqual (1, Revision.GetRevision (new ClientTransaction ()));
    }
  }
}