using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Security;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.UnitTests.TestDomain;

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

    [Test]
    public void Get_DisplayName ()
    {
      AbstractRoleDefinition abstractRole = new AbstractRoleDefinition (_transaction);
      abstractRole.Name = "Value|Namespace.TypeName, Assembly";

      Assert.AreEqual ("Value|Namespace.TypeName, Assembly", abstractRole.DisplayName);
    }
  }
}
