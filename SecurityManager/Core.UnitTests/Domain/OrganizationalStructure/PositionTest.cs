using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.Security;
using Rubicon.Security.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.SecurityManager.UnitTests.Domain.AccessControl;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class PositionTest : DomainTest
  {
    private OrganizationalStructureTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp ();
      _testHelper = new OrganizationalStructureTestHelper ();
      _testHelper.Transaction.EnterScope ();
    }

    [Test]
    public void FindAll ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateOrganizationalStructureWithTwoTenants ();
      ClientTransaction transaction = new ClientTransaction ();

      DomainObjectCollection positions = Position.FindAll (transaction);

      Assert.AreEqual (3, positions.Count);
    }

    [Test]
    public void DeletePosition_WithAccessControlEntry ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterScope ())
      {
        Position position = testHelper.CreatePosition ("Position");
        AccessControlEntry ace = testHelper.CreateAceWithPosition (position, GroupSelection.All);

        position.Delete();

        Assert.IsTrue (ace.IsDiscarded);
      }
    }

    [Test]
    public void DeletePosition_WithRole ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant", "UID: testTenant");
      Group userGroup = _testHelper.CreateGroup ("UserGroup", Guid.NewGuid ().ToString(), null, tenant);
      Group roleGroup = _testHelper.CreateGroup ("RoleGroup", Guid.NewGuid ().ToString (), null, tenant);
      User user = _testHelper.CreateUser ("user", "Firstname", "Lastname", "Title", userGroup, tenant);
      Position position = _testHelper.CreatePosition ("Position");
      Role role = _testHelper.CreateRole (user, roleGroup, position);

      position.Delete ();

      Assert.IsTrue (role.IsDiscarded);
    }

    [Test]
    public void DeletePosition_WithGroupTypePosition ()
    {
      GroupType groupType = _testHelper.CreateGroupType ("GroupType");
      Position position = _testHelper.CreatePosition ("Position");
      GroupTypePosition concretePosition = _testHelper.CreateGroupTypePosition (groupType, position);

      position.Delete ();

      Assert.IsTrue (concretePosition.IsDiscarded);
    }

    [Test]
    public void GetDisplayName ()
    {
      Position position = _testHelper.CreatePosition ("PositionName");

      Assert.AreEqual ("PositionName", position.DisplayName);
    }

    [Test]
    public void GetSecurityStrategy ()
    {
      ISecurableObject position = _testHelper.CreatePosition ("PositionName");

      IObjectSecurityStrategy objectSecurityStrategy = position.GetSecurityStrategy ();
      Assert.IsNotNull (objectSecurityStrategy);
      Assert.IsInstanceOfType (typeof (DomainObjectSecurityStrategy), objectSecurityStrategy);
      DomainObjectSecurityStrategy domainObjectSecurityStrategy = (DomainObjectSecurityStrategy) objectSecurityStrategy;
      Assert.AreEqual (RequiredSecurityForStates.None, domainObjectSecurityStrategy.RequiredSecurityForStates);
    }

    [Test]
    public void GetSecurityStrategy_SameTwice ()
    {
      ISecurableObject position = _testHelper.CreatePosition ("PositionName");

      Assert.AreSame (position.GetSecurityStrategy (), position.GetSecurityStrategy ());
    }

    [Test]
    public void GetSecurableType ()
    {
      ISecurableObject position = _testHelper.CreatePosition ("PositionName");

      Assert.AreSame (typeof (Position), position.GetSecurableType());
    }

    [Test]
    public void DomainObjectSecurityContextFactoryImplementation ()
    {
      Position position = _testHelper.CreatePosition ("PositionName");
      IDomainObjectSecurityContextFactory factory = position;

      Assert.IsFalse (factory.IsDiscarded);
      Assert.IsTrue (factory.IsNew);
      Assert.IsFalse (factory.IsDeleted);

      position.Delete ();

      Assert.IsTrue (factory.IsDiscarded);
    }

    [Test]
    public void CreateSecurityContext ()
    {
      Position position = _testHelper.CreatePosition ("PositionName");
      position.Delegation = Delegation.Enabled;

      SecurityContext securityContext = ((ISecurityContextFactory) position).CreateSecurityContext ();
      Assert.AreEqual (position.GetPublicDomainObjectType (), Type.GetType (securityContext.Class));
      Assert.IsEmpty (securityContext.Owner);
      Assert.IsEmpty (securityContext.OwnerGroup);
      Assert.IsEmpty (securityContext.OwnerTenant);
      Assert.IsEmpty (securityContext.AbstractRoles);
      Assert.AreEqual (1, securityContext.GetNumberOfStates());
      Assert.AreEqual (new EnumWrapper (Delegation.Enabled), securityContext.GetState ("Delegation"));
    }
  }
}
