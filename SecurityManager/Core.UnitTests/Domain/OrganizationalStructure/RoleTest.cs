using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Security.Principal;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects;
using Rubicon.Security;
using Rubicon.Security.Configuration;
using Rubicon.SecurityManager.Domain;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.SecurityManager.UnitTests.Configuration;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class RoleTest : DomainTest
  {
    private ObjectID _expectedClientID;
    private ObjectID _expectedRootGroupID;
    private ObjectID _expectedParentGroup0ID;
    private ObjectID _expectedGroup0ID;
    private MockRepository _mocks;
    private ISecurityProvider _mockSecurityProvider;
    private IUserProvider _mockUserProvider;
    private IFunctionalSecurityStrategy _stubFunctionalSecurityStrategy;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();

      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      Client client = dbFixtures.CreateOrganizationalStructureWithTwoClients ();
      _expectedClientID = client.ID;

      DomainObjectCollection groups = Group.FindByClientID (_expectedClientID, client.ClientTransaction);
      foreach (Group group in groups)
      {
        if (group.UniqueIdentifier == "UID: rootGroup")
          _expectedRootGroupID = group.ID;
        else if (group.UniqueIdentifier == "UID: parentGroup0")
          _expectedParentGroup0ID = group.ID;
        else if (group.UniqueIdentifier == "UID: group0")
          _expectedGroup0ID = group.ID;
      }
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _mocks = new MockRepository ();
      _mockSecurityProvider = (ISecurityProvider) _mocks.CreateMultiMock (typeof (ProviderBase), typeof (ISecurityProvider));
      SetupResult.For (_mockSecurityProvider.IsNull).Return (false);
      _mockUserProvider = (IUserProvider) _mocks.CreateMultiMock (typeof (ProviderBase), typeof (IUserProvider));
      _stubFunctionalSecurityStrategy = _mocks.CreateMock<IFunctionalSecurityStrategy> ();

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
      SecurityConfiguration.Current.SecurityProvider = _mockSecurityProvider;
      SecurityConfiguration.Current.UserProvider = _mockUserProvider;
      SecurityConfiguration.Current.FunctionalSecurityStrategy = _stubFunctionalSecurityStrategy;

    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
    }

    [Test]
    public void GetPossibleGroups ()
    {
      IPrincipal principal = new GenericPrincipal (new GenericIdentity ("group0/user1"), new string[0]);
      SetupResult.For (_mockUserProvider.GetUser ()).Return (principal);
      ExpectSecurityProviderGetAccessForGroup ("UID: rootGroup", principal);
      ExpectSecurityProviderGetAccessForGroup ("UID: parentGroup0", principal, SecurityManagerAccessTypes.AssignRole);
      ExpectSecurityProviderGetAccessForGroup ("UID: group0", principal, SecurityManagerAccessTypes.AssignRole);
      ExpectSecurityProviderGetAccessForGroup ("UID: parentGroup1", principal);
      ExpectSecurityProviderGetAccessForGroup ("UID: group1", principal);
      ExpectSecurityProviderGetAccessForGroup ("UID: testRootGroup", principal);
      ExpectSecurityProviderGetAccessForGroup ("UID: testParentOfOwnerGroup", principal);
      ExpectSecurityProviderGetAccessForGroup ("UID: testOwnerGroup", principal);
      ExpectSecurityProviderGetAccessForGroup ("UID: testGroup", principal);
      ClientTransaction transaction = new ClientTransaction ();
      Role role = new Role (transaction);
      _mocks.ReplayAll ();

      List<Group> groups = role.GetPossibleGroups (_expectedClientID);

      _mocks.VerifyAll ();
      Assert.AreEqual (2, groups.Count);
      foreach (string groupUnqiueIdentifier in new string[] { "UID: parentGroup0", "UID: group0" })
      {
        Assert.IsTrue (
            groups.Exists (delegate (Group current) { return groupUnqiueIdentifier == current.UniqueIdentifier; }),
            "Group '{0}' was not found.",
            groupUnqiueIdentifier);
      }
    }

    [Test]
    public void GetPossibleGroups_WithoutSecurityProvider ()
    {
      SecurityConfiguration.Current.SecurityProvider = new NullSecurityProvider ();
      SecurityConfiguration.Current.UserProvider = new ThreadUserProvider ();
      ClientTransaction transaction = new ClientTransaction ();
      Role role = new Role (transaction);

      List<Group> groups = role.GetPossibleGroups (_expectedClientID);

      Assert.AreEqual (9, groups.Count);
    }

    [Test]
    public void GetPossiblePositions ()
    {
      IPrincipal principal = new GenericPrincipal (new GenericIdentity ("group0/user1"), new string[0]);
      SetupResult.For (_mockUserProvider.GetUser ()).Return (principal);
      SetupResultSecurityProviderGetAccessForPosition (Delegation.Enabled, principal, SecurityManagerAccessTypes.AssignRole);
      SetupResultSecurityProviderGetAccessForPosition (Delegation.Disabled, principal);
      ClientTransaction transaction = new ClientTransaction ();
      Role role = new Role (transaction);
      Group parentGroup = Group.GetObject (_expectedParentGroup0ID, transaction);
      _mocks.ReplayAll ();

      List<Position> positions = role.GetPossiblePositions (parentGroup);

      _mocks.VerifyAll ();
      Assert.AreEqual (1, positions.Count);
      Assert.AreEqual ("Official", positions[0].Name);
    }

    [Test]
    public void GetPossiblePositions_WithoutGroupType ()
    {
      IPrincipal principal = new GenericPrincipal (new GenericIdentity ("group0/user1"), new string[0]);
      SetupResult.For (_mockUserProvider.GetUser ()).Return (principal);
      SetupResultSecurityProviderGetAccessForPosition (Delegation.Enabled, principal, SecurityManagerAccessTypes.AssignRole);
      SetupResultSecurityProviderGetAccessForPosition (Delegation.Disabled, principal);
      ClientTransaction transaction = new ClientTransaction ();
      Role role = new Role (transaction);
      Group rootGroup = Group.GetObject (_expectedRootGroupID, transaction);
      _mocks.ReplayAll ();

      List<Position> positions = role.GetPossiblePositions (rootGroup);

      _mocks.VerifyAll ();
      Assert.AreEqual (2, positions.Count);
      foreach (string positionName in new string[] { "Official", "Global" })
      {
        Assert.IsTrue (
            positions.Exists (delegate (Position current) { return positionName == current.Name; }),
            "Position '{0}' was not found.",
            positionName);
      }
    }

    [Test]
    public void GetPossiblePositions_WithoutSecurityProvider ()
    {
      SecurityConfiguration.Current.SecurityProvider = new NullSecurityProvider ();
      SecurityConfiguration.Current.UserProvider = new ThreadUserProvider ();
      ClientTransaction transaction = new ClientTransaction ();
      Role role = new Role (transaction);
      Group parentGroup = Group.GetObject (_expectedParentGroup0ID, transaction);

      List<Position> positions = role.GetPossiblePositions (parentGroup);

      Assert.AreEqual (2, positions.Count);
      foreach (string positionName in new string[] { "Official", "Manager" })
      {
        Assert.IsTrue (
            positions.Exists (delegate (Position current) { return positionName == current.Name; }),
            "Position '{0}' was not found.",
            positionName);
      }
    }

    [Test]
    public void GetPossiblePositions_WithoutGroupTypeAndWithoutSecurityProvider ()
    {
      SecurityConfiguration.Current.SecurityProvider = new NullSecurityProvider ();
      SecurityConfiguration.Current.UserProvider = new ThreadUserProvider ();
      ClientTransaction transaction = new ClientTransaction ();
      Role role = new Role (transaction);
      Group rootGroup = Group.GetObject (_expectedRootGroupID, transaction);

      List<Position> positions = role.GetPossiblePositions (rootGroup);

      Assert.AreEqual (3, positions.Count);
    }

    private void ExpectSecurityProviderGetAccessForGroup (string ownerGroup, IPrincipal principal, params Enum[] returnedAccessTypeEnums)
    {
      Type classType = typeof (Group);
      string owner = string.Empty;
      string ownerClient = string.Empty;
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      List<Enum> abstractRoles = new List<Enum> ();
      SecurityContext securityContext = new SecurityContext (classType, owner, ownerGroup, ownerClient, states, abstractRoles);

      AccessType[] returnedAccessTypes = Array.ConvertAll<Enum, AccessType> (returnedAccessTypeEnums, AccessType.Get);

      Expect.Call (_mockSecurityProvider.GetAccess (securityContext, principal)).Return (returnedAccessTypes);
    }

    private void SetupResultSecurityProviderGetAccessForPosition (Delegation delegation, IPrincipal principal, params Enum[] returnedAccessTypeEnums)
    {
      Type classType = typeof (Position);
      string owner = string.Empty;
      string ownerGroup = string.Empty;
      string ownerClient = string.Empty;
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      states.Add ("Delegation", delegation);
      List<Enum> abstractRoles = new List<Enum> ();
      SecurityContext securityContext = new SecurityContext (classType, owner, ownerGroup, ownerClient, states, abstractRoles);

      AccessType[] returnedAccessTypes = Array.ConvertAll<Enum, AccessType> (returnedAccessTypeEnums, AccessType.Get);

      SetupResult.For (_mockSecurityProvider.GetAccess (securityContext, principal)).Return (returnedAccessTypes);
    }
  }
}