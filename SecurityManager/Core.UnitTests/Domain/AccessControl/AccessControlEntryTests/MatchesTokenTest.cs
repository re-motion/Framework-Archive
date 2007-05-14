using System;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryTests
{
  [TestFixture]
  public class MatchesTokenTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
    }

    [Test]
    public void EmptyToken_EmptyAce ()
    {
      SecurityToken token = _testHelper.CreateEmptyToken();
      AccessControlEntry entry = new AccessControlEntry (_testHelper.Transaction);

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void EmptyToken_AceForAbstractRole ()
    {
      SecurityToken token = _testHelper.CreateEmptyToken();
      AccessControlEntry entry = _testHelper.CreateAceWithAbstractRole();

      Assert.IsFalse (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithAbstractRole_AceForAbstractRole ()
    {
      AccessControlEntry entry = _testHelper.CreateAceWithAbstractRole();
      SecurityToken token = _testHelper.CreateTokenWithAbstractRole (entry.SpecificAbstractRole);

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithAbstractRole_EmptyAce ()
    {
      AccessControlEntry entry = new AccessControlEntry (_testHelper.Transaction);
      SecurityToken token = _testHelper.CreateTokenWithAbstractRole (_testHelper.CreateTestAbstractRole());

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void EmptyTokenAndAce_PositionFromGroupOfOwner ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Position managerPosition = _testHelper.CreatePosition ("Manager");
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition, GroupSelection.OwningGroup);
      SecurityToken token = _testHelper.CreateEmptyToken();

      Assert.IsFalse (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithRoleAndAce_PositionFromOwningGroup ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Position managerPosition = _testHelper.CreatePosition ("Manager");
      Group group = _testHelper.CreateGroup ("Testgroup", null, client);
      User user = _testHelper.CreateUser ("test.user", "Test", "User", "Dipl.Ing.(FH)", group, client);
      Role role = _testHelper.CreateRole (user, group, managerPosition);
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition, GroupSelection.OwningGroup);
      SecurityToken token = _testHelper.CreateTokenWithGroups (user, group);

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithRoleAndAbstractRole_AceWithPositionFromOwningGroup ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Position managerPosition = _testHelper.CreatePosition ("Manager");
      Group group = _testHelper.CreateGroup ("Testgroup", null, client);
      User user = _testHelper.CreateUser ("test.user", "Test", "User", "Dipl.Ing.(FH)", group, client);
      Role role = _testHelper.CreateRole (user, group, managerPosition);
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition, GroupSelection.OwningGroup);
      SecurityToken token = _testHelper.CreateToken (user, new Group[] {group}, new AbstractRoleDefinition[] {_testHelper.CreateTestAbstractRole()});

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithRole_AceWithPositionFromOwningGroupAndAbstractRole ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Position managerPosition = _testHelper.CreatePosition ("Manager");
      Group group = _testHelper.CreateGroup ("Testgroup", null, client);
      User user = _testHelper.CreateUser ("test.user", "Test", "User", "Dipl.Ing.(FH)", group, client);
      Role role = _testHelper.CreateRole (user, group, managerPosition);
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition, GroupSelection.OwningGroup);
      entry.SpecificAbstractRole = _testHelper.CreateTestAbstractRole();
      SecurityToken token = _testHelper.CreateTokenWithGroups (user, group);

      Assert.IsFalse (entry.MatchesToken (token));
    }

    [Test]
    public void EmptyToken_AceWithPositionFromAllGroups ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Position managerPosition = _testHelper.CreatePosition ("Manager");
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition, GroupSelection.All);
      SecurityToken token = _testHelper.CreateEmptyToken();

      Assert.IsFalse (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithRole_AceWithPositionFromAllGroups ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Position managerPosition = _testHelper.CreatePosition ("Manager");
      Group group = _testHelper.CreateGroup ("Testgroup", null, client);
      User user = _testHelper.CreateUser ("test.user", "Test", "User", "Dipl.Ing.(FH)", group, client);
      Role role = _testHelper.CreateRole (user, group, managerPosition);
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition, GroupSelection.All);
      SecurityToken token = _testHelper.CreateToken (user, null, null);

      Assert.IsTrue (entry.MatchesToken (token));
    }
  }
}