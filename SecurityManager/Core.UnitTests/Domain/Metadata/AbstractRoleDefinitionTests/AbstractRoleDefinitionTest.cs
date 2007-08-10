using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata.AbstractRoleDefinitionTests
{
  [TestFixture]
  public class Test : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp ();

      new ClientTransactionScope ();
    }

    [Test]
    public void FindAll_EmptyResult ()
    {
     DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();

      DomainObjectCollection result = AbstractRoleDefinition.FindAll (ClientTransactionScope.CurrentTransaction);

      Assert.AreEqual (0, result.Count);
    }
  }
}
