using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata.AbstractRoleDefinitionTests
{
  [TestFixture]
  public class Test : DomainTest
  {
    private ClientTransaction _transaction;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _transaction = new ClientTransaction ();
    }

    [Test]
    public void FindAll_EmptyResult ()
    {
     DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();

      DomainObjectCollection result = AbstractRoleDefinition.FindAll (_transaction);

      Assert.AreEqual (0, result.Count);
    }
  }
}
