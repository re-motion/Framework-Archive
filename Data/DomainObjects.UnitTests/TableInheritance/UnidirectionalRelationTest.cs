using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
{
  [TestFixture]
  public class UnidirectionalRelationTest : TableInheritanceMappingTest
  {
    [Test]
    [Ignore ("TODO: Implement referential integrity for unidirectional relationships.")]
    public void DeleteAndCommitWithConcreteTableInheritance()
    {
      SetDatabaseModifyable ();

      ClassWithUnidirectionalRelation classWithUnidirectionalRelation =
          ClassWithUnidirectionalRelation.GetObject (DomainObjectIDs.ClassWithUnidirectionalRelation);
      classWithUnidirectionalRelation.DomainBase.Delete ();
      ClientTransactionScope.CurrentTransaction.Commit ();

      try
      {
        Dev.Null = classWithUnidirectionalRelation.DomainBase;
        Assert.Fail ("Expected ObjectDiscardedException");
      }
      catch (ObjectDiscardedException)
      {
        // succeed
      }

      using (ClientTransaction.NewTransaction().EnterScope())
      {
        ClassWithUnidirectionalRelation reloadedObject =
            ClassWithUnidirectionalRelation.GetObject (DomainObjectIDs.ClassWithUnidirectionalRelation);

        Assert.IsNull (reloadedObject.DomainBase);
      }
    }
  }
}