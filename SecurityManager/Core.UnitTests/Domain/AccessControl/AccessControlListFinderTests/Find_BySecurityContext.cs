using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.Security;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.SecurityManager.UnitTests.TestDomain;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl.AccessControlListFinderTests
{
  [TestFixture]
  public class Find_BySecurityContext : DomainTest
  {
    private AccessControlTestHelper _testHelper;
    private SecurableClassDefinition _currentClassDefinition;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
 
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      _currentClassDefinition = dbFixtures.CreateSecurableClassDefinitionWithAccessControlLists (1);
    }

    public override void SetUp ()
    {
      base.SetUp ();
      _testHelper = new AccessControlTestHelper ();
      _currentClassDefinition.ClientTransaction.EnterScope ();
    }

    [Test]
    public void Succeed_WithValidSecurityContext ()
    {
      AccessControlList expectedAccessControlList = (AccessControlList) _currentClassDefinition.AccessControlLists[0];
      SecurityContext context = new SecurityContext (typeof (Order));
   
      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      AccessControlList foundAcl = aclFinder.Find (ClientTransactionScope.CurrentTransaction, context);

      Assert.AreEqual (expectedAccessControlList.ID, foundAcl.ID);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException),
        ExpectedMessage = "The securable class 'Rubicon.SecurityManager.UnitTests.TestDomain.PremiumOrder, Rubicon.SecurityManager.UnitTests' cannot be found.")]
    public void Fail_WithUnkownSecurableClassDefinition ()
    {
      SecurityContext context = new SecurityContext (typeof (PremiumOrder));

      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      aclFinder.Find (ClientTransactionScope.CurrentTransaction, context);
    }
  }
}
