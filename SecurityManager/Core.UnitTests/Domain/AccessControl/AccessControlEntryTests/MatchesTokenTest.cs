using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using System.Threading;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryTests
{
  [TestFixture]
  public class MatchesTokenTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp ();
      _testHelper = new AccessControlTestHelper ();
    }

    [Test]
    public void EmptyToken_EmptyAce_Matches ()
    {
      AccessControlEntry entry = AccessControlEntry.NewObject (_testHelper.Transaction);
      SecurityToken token = _testHelper.CreateEmptyToken ();

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void EmptyToken_AceForOwningClient_DoesNotMatch ()
    {
      AccessControlEntry entry = _testHelper.CreateAceWithOwningClient ();
      SecurityToken token = _testHelper.CreateEmptyToken ();

      Assert.IsFalse (entry.MatchesToken (token));
    }

    [Test]
    public void EmptyToken_AceForAbstractRole_DoesNotMatch ()
    {
      AccessControlEntry entry = _testHelper.CreateAceWithAbstractRole ();
      SecurityToken token = _testHelper.CreateEmptyToken ();

      Assert.IsFalse (entry.MatchesToken (token));
    }

    [Test]
    public void EmptyToken_AceForPositionFromOwningGroup_DoesNotMatch ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Position managerPosition = _testHelper.CreatePosition ("Manager");
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition, GroupSelection.OwningGroup);
      SecurityToken token = _testHelper.CreateEmptyToken ();

      Assert.IsFalse (entry.MatchesToken (token));
    }

    [Test]
    public void EmptyToken_AceForPositionFromAllGroups_DoesNotMatch ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Position managerPosition = _testHelper.CreatePosition ("Manager");
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition, GroupSelection.All);
      SecurityToken token = _testHelper.CreateEmptyToken ();

      Assert.IsFalse (entry.MatchesToken (token));
    }


    [Test]
    public void TokenWithClient_EmptyAce_Matches ()
    {
      AccessControlEntry entry = AccessControlEntry.NewObject (_testHelper.Transaction);
      SecurityToken token = _testHelper.CreateTokenWithOwningClient (null, entry.SpecificClient);
 
      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithClient_AceForOwningClient_Matches ()
    {
      AccessControlEntry entry = _testHelper.CreateAceWithOwningClient ();
      Client client = _testHelper.CreateClient ("Testclient");
      Group group = _testHelper.CreateGroup ("Testgroup", null, client);
      User user = CreateUser (client, group);
      SecurityToken token = _testHelper.CreateTokenWithOwningClient (user, client);

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithClient_AceForOtherOwningClient_DoesNotMatch ()
    {
      AccessControlEntry entry = _testHelper.CreateAceWithOwningClient ();
      Client client = _testHelper.CreateClient ("Testclient");
      Group group = _testHelper.CreateGroup ("Testgroup", null, client);
      User user = CreateUser (client, group);
      SecurityToken token = _testHelper.CreateTokenWithOwningClient (user, _testHelper.CreateClient ("Other Client"));

      Assert.IsFalse (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithClient_AceForSpecificClient_Matches ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Group group = _testHelper.CreateGroup ("Testgroup", null, client);
      User user = CreateUser (client, group);
      AccessControlEntry entry = _testHelper.CreateAceWithSpecficClient (client);
      SecurityToken token = _testHelper.CreateToken (user, null, new Group[0], new AbstractRoleDefinition[0]);

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithClient_AceForOtherSpecificClient_DoesNotMatch ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Group group = _testHelper.CreateGroup ("Testgroup", null, client);
      User user = CreateUser (client, group);
      AccessControlEntry entry = _testHelper.CreateAceWithSpecficClient (_testHelper.CreateClient ("Other Client"));
      SecurityToken token = _testHelper.CreateToken (user, null, new Group[0], new AbstractRoleDefinition[0]);

      Assert.IsFalse (entry.MatchesToken (token));
    }


    [Test]
    public void TokenWithAbstractRole_EmptyAce_Matches ()
    {
      AccessControlEntry entry = AccessControlEntry.NewObject (_testHelper.Transaction);
      SecurityToken token = _testHelper.CreateTokenWithAbstractRole (_testHelper.CreateTestAbstractRole ());

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithAbstractRole_AceForAbstractRole_Matches ()
    {
      AccessControlEntry entry = _testHelper.CreateAceWithAbstractRole ();
      SecurityToken token = _testHelper.CreateTokenWithAbstractRole (entry.SpecificAbstractRole);

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithAbstractRole_AceForOtherAbstractRole_DoesNotMatch ()
    {
      AccessControlEntry entry = _testHelper.CreateAceWithAbstractRole ();
      SecurityToken token = _testHelper.CreateTokenWithAbstractRole (_testHelper.CreateTestAbstractRole ());

      Assert.IsFalse (entry.MatchesToken (token));
    }


    [Test]
    public void TokenWithRole_AceForPositionFromOwningGroup_Matches ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Position managerPosition = _testHelper.CreatePosition ("Manager");
      Group group = _testHelper.CreateGroup ("Testgroup", null, client);
      User user = CreateUser (client, group);
      Role role = _testHelper.CreateRole (user, group, managerPosition);
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition, GroupSelection.OwningGroup);
      SecurityToken token = _testHelper.CreateTokenWithOwningGroups (user, group);

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithRole_AceForPositionFromAllGroups_Matches ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Position managerPosition = _testHelper.CreatePosition ("Manager");
      Group group = _testHelper.CreateGroup ("Testgroup", null, client);
      User user = CreateUser (client, group);
      Role role = _testHelper.CreateRole (user, group, managerPosition);
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition, GroupSelection.All);
      SecurityToken token = _testHelper.CreateToken (user, null, null, null);

      Assert.IsTrue (entry.MatchesToken (token));
    }


    [Test]
    public void TokenWithRole_AceForPositionFromOwningGroupAndAbstractRole_DoesNotMatch ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Position managerPosition = _testHelper.CreatePosition ("Manager");
      Group group = _testHelper.CreateGroup ("Testgroup", null, client);
      User user = CreateUser (client, group);
      Role role = _testHelper.CreateRole (user, group, managerPosition);
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition, GroupSelection.OwningGroup);
      entry.SpecificAbstractRole = _testHelper.CreateTestAbstractRole ();
      SecurityToken token = _testHelper.CreateTokenWithOwningGroups (user, group);

      Assert.IsFalse (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithRoleAndAbstractRole_AceForPositionFromOwningGroup_Matches ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Position managerPosition = _testHelper.CreatePosition ("Manager");
      Group group = _testHelper.CreateGroup ("Testgroup", null, client);
      User user = CreateUser (client, group);
      Role role = _testHelper.CreateRole (user, group, managerPosition);
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition, GroupSelection.OwningGroup);
      SecurityToken token = _testHelper.CreateToken (user, null, new Group[] { group }, new AbstractRoleDefinition[] { _testHelper.CreateTestAbstractRole() });

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithClient_AceForOwningClientAndAbstractRole_DoesNotMatch ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Group group = _testHelper.CreateGroup ("Testgroup", null, client);
      User user = CreateUser (client, group);
      AccessControlEntry entry = _testHelper.CreateAceWithOwningClient ();
      entry.SpecificAbstractRole = _testHelper.CreateTestAbstractRole ();
      SecurityToken token = _testHelper.CreateTokenWithOwningClient (user, client);

      Assert.IsFalse (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithClientAndAbstractRole_AceForOwningClient_Matches ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Position managerPosition = _testHelper.CreatePosition ("Manager");
      Group group = _testHelper.CreateGroup ("Testgroup", null, client);
      User user = CreateUser (client, group);
      AccessControlEntry entry = _testHelper.CreateAceWithOwningClient ();
      SecurityToken token = _testHelper.CreateToken (user, client, new Group[0], new AbstractRoleDefinition[] { _testHelper.CreateTestAbstractRole () });

      Assert.IsTrue (entry.MatchesToken (token));
    }


    private User CreateUser (Client client, Group group)
    {
      return _testHelper.CreateUser ("test.user", "Test", "User", "Dipl.Ing.(FH)", group, client);
    }
  }
}
