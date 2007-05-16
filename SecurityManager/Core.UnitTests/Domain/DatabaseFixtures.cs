using System;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Domain
{
  public class DatabaseFixtures
  {
    private OrganizationalStructureFactory _factory;

    public DatabaseFixtures ()
    {
      _factory = new OrganizationalStructureFactory ();
    }

    public void CreateEmptyDomain ()
    {
      DatabaseHelper dbHelper = new DatabaseHelper ();
      dbHelper.SetupDB ();
    }

    public void CreateSecurableClassDefinitionWithLocalizedNames ()
    {
      CreateEmptyDomain ();

      ClientTransaction transaction = new ClientTransaction ();

      SecurableClassDefinition classDefinition = CreateOrderSecurableClassDefinition (transaction);

      Culture germanCulture = Culture.NewObject (transaction, "de");
      Culture englishCulture = Culture.NewObject (transaction, "en");
      Culture russianCulture = Culture.NewObject (transaction, "ru");

      LocalizedName classInGerman = LocalizedName.NewObject (transaction, "Klasse", germanCulture, classDefinition);
      LocalizedName classInEnglish = LocalizedName.NewObject (transaction, "Class", englishCulture, classDefinition);

      transaction.Commit ();
    }

    public Tenant CreateOrganizationalStructureWithTwoTenants ()
    {
      CreateEmptyDomain ();

      ClientTransaction transaction = new ClientTransaction ();

      AbstractRoleDefinition qualityManagerRole = AbstractRoleDefinition.NewObject (transaction, Guid.NewGuid (), "QualityManager|Rubicon.SecurityManager.UnitTests.TestDomain.ProjectRoles, Rubicon.SecurityManager.UnitTests", 0);
      qualityManagerRole.Index = 1;
      AbstractRoleDefinition developerRole = AbstractRoleDefinition.NewObject (transaction, Guid.NewGuid (), "Developer|Rubicon.SecurityManager.UnitTests.TestDomain.ProjectRoles, Rubicon.SecurityManager.UnitTests", 1);
      developerRole.Index = 0;

      Position globalPosition = CreatePosition (transaction, "Global");
      globalPosition.Delegation = Delegation.Enabled;
      Position officialPosition = CreatePosition (transaction, "Official");
      officialPosition.Delegation = Delegation.Enabled;
      Position managerPosition = CreatePosition (transaction, "Manager");
      managerPosition.Delegation = Delegation.Disabled;

      GroupType groupType1 = CreateGroupType (transaction, "groupType 1");
      GroupType groupType2 = CreateGroupType (transaction, "groupType 2");

      GroupTypePosition groupType1_managerPosition = GroupTypePosition.NewObject (transaction);
      groupType1_managerPosition.GroupType = groupType1;
      groupType1_managerPosition.Position = managerPosition;
      GroupTypePosition groupType1_officialPosition = GroupTypePosition.NewObject (transaction);
      groupType1_officialPosition.GroupType = groupType1;
      groupType1_officialPosition.Position = officialPosition;
      GroupTypePosition groupType2_officialPosition = GroupTypePosition.NewObject (transaction);
      groupType2_officialPosition.GroupType = groupType2;
      groupType2_officialPosition.Position = officialPosition;

      Tenant tenant1 = CreateTenant (transaction, "TestTenant");
      tenant1.UniqueIdentifier = "UID: testTenant";
      Group rootGroup = CreateGroup (transaction, "rootGroup", "UID: rootGroup", null, tenant1);
      for (int i = 0; i < 2; i++)
      {
        Group parentGroup = CreateGroup (transaction, string.Format ("parentGroup{0}", i), string.Format ("UID: parentGroup{0}", i), rootGroup, tenant1);
        parentGroup.GroupType = groupType1;
        
        Group group = CreateGroup (transaction, string.Format ("group{0}", i), string.Format ("UID: group{0}", i), parentGroup, tenant1);
        group.GroupType = groupType2;
        
        User user1 = CreateUser (transaction, string.Format ("group{0}/user1", i), string.Empty, "user1", string.Empty, group, tenant1);
        User user2 = CreateUser (transaction, string.Format ("group{0}/user2", i), string.Empty, "user2", string.Empty, group, tenant1);

        CreateRole (transaction, user1, parentGroup, managerPosition);
        CreateRole (transaction, user2, parentGroup, officialPosition);
      }

      Group testRootGroup = CreateGroup (transaction, "testRootGroup", "UID: testRootGroup", null, tenant1);
      Group testParentOfOwningGroup = CreateGroup (transaction, "testParentOfOwningGroup", "UID: testParentOfOwningGroup", testRootGroup, tenant1);
      Group testOwningGroup = CreateGroup (transaction, "testOwningGroup", "UID: testOwningGroup", testParentOfOwningGroup, tenant1);
      Group testGroup = CreateGroup (transaction, "testGroup", "UID: testGroup", null, tenant1);
      User testUser = CreateUser (transaction, "test.user", "test", "user", "Dipl.Ing.(FH)", testOwningGroup, tenant1);

      CreateRole (transaction, testUser, testGroup, officialPosition);
      CreateRole (transaction, testUser, testGroup, managerPosition);
      CreateRole (transaction, testUser, testOwningGroup, managerPosition);
      CreateRole (transaction, testUser, testRootGroup, officialPosition);

      Tenant tenant2 = CreateTenant (transaction, "Tenant 2");
      Group groupTenant2 = CreateGroup (transaction, "Group Tenant 2", "UID: group Tenant 2", null, tenant2);
      User userTenant2 = CreateUser (transaction, "User.Tenant2", "User", "Tenant 2", string.Empty, groupTenant2, tenant2);

      transaction.Commit ();

      return tenant1;
    }

    public SecurableClassDefinition[] CreateSecurableClassDefinitionsWithSubClassesEach (int classDefinitionCount, int derivedClassDefinitionCount)
    {
      CreateEmptyDomain ();
      ClientTransaction transaction = new ClientTransaction ();

      SecurableClassDefinition[] classDefinitions = CreateSecurableClassDefinitions (transaction, classDefinitionCount, derivedClassDefinitionCount);

      transaction.Commit ();

      return classDefinitions;
    }

    public SecurableClassDefinition[] CreateSecurableClassDefinitions (int classDefinitionCount)
    {
      CreateEmptyDomain ();
      ClientTransaction transaction = new ClientTransaction ();

      SecurableClassDefinition[] classDefinitions = CreateSecurableClassDefinitions (transaction, classDefinitionCount, 0);

      transaction.Commit ();

      return classDefinitions;
    }

    public SecurableClassDefinition CreateSecurableClassDefinitionWithStates ()
    {
      CreateEmptyDomain ();
      ClientTransaction transaction = new ClientTransaction ();

      SecurableClassDefinition classDefinition = CreateOrderSecurableClassDefinition (transaction);

      classDefinition.AddStateProperty (CreateFileStateProperty (transaction));
      classDefinition.AddStateProperty (CreateConfidentialityProperty (transaction));

      transaction.Commit ();

      return classDefinition;
    }

    public SecurableClassDefinition CreateSecurableClassDefinitionWithAccessTypes (int accessTypes)
    {
      CreateEmptyDomain ();
      ClientTransaction transaction = new ClientTransaction ();

      SecurableClassDefinition classDefinition = CreateSecurableClassDefinitionWithAccessTypes (transaction, accessTypes);

      transaction.Commit ();

      return classDefinition;
    }

    public SecurableClassDefinition CreateSecurableClassDefinitionWithAccessControlLists (int accessControlLists)
    {
      CreateEmptyDomain ();
      ClientTransaction transaction = new ClientTransaction ();

      SecurableClassDefinition classDefinition = CreateOrderSecurableClassDefinition (transaction);
      for (int i = 0; i < accessControlLists; i++)
      {
        AccessControlList acl = AccessControlList.NewObject (transaction);
        acl.Class = classDefinition;
        acl.CreateAccessControlEntry ();
        if (i == 0)
          CreateStateCombination (transaction, acl);
        else
          CreateStateCombination (transaction, acl, string.Format ("Property {0}", i));
      }

      transaction.Commit ();

      return classDefinition;
    }

    public AccessControlList CreateAccessControlListWithAccessControlEntries (int accessControlEntries)
    {
      CreateEmptyDomain ();
      ClientTransaction transaction = new ClientTransaction ();

      SecurableClassDefinition classDefinition = CreateOrderSecurableClassDefinition (transaction);
      AccessControlList acl = AccessControlList.NewObject (transaction);
      acl.Class = classDefinition;
      acl.CreateStateCombination ();

      for (int i = 0; i < accessControlEntries; i++)
        acl.CreateAccessControlEntry ();

      transaction.Commit ();

      return acl;
    }

    public AccessControlList CreateAccessControlListWithStateCombinations (int stateCombinations)
    {
      CreateEmptyDomain ();
      ClientTransaction transaction = new ClientTransaction ();

      SecurableClassDefinition classDefinition = CreateOrderSecurableClassDefinition (transaction);
      AccessControlList acl = AccessControlList.NewObject (transaction);
      acl.Class = classDefinition;
      acl.CreateAccessControlEntry ();

      for (int i = 0; i < stateCombinations; i++)
      {
        if (i == 0)
          CreateStateCombination (transaction, acl);
        else
          CreateStateCombination (transaction, acl, string.Format ("Property {0}", i));
      }

      transaction.Commit ();

      return acl;
    }

    public void CreateAdministratorAbstractRole ()
    {
      CreateEmptyDomain ();
      ClientTransaction transaction = new ClientTransaction ();

      Guid metadataItemID = new Guid ("00000004-0001-0000-0000-000000000000");
      string abstractRoleName = "Administrator|Rubicon.Security.UnitTests.TestDomain.SpecialAbstractRoles, Rubicon.Security.UnitTests.TestDomain";
      AbstractRoleDefinition administratorAbstractRole = AbstractRoleDefinition.NewObject (transaction, metadataItemID, abstractRoleName, 0);

      transaction.Commit ();
    }

    public ObjectID CreateAccessControlEntryWithPermissions (int permissions)
    {
      CreateEmptyDomain ();

      ClientTransaction transaction = new ClientTransaction ();

      SecurableClassDefinition classDefinition = CreateSecurableClassDefinitionWithAccessTypes (transaction, permissions);
      AccessControlList acl = classDefinition.CreateAccessControlList ();
      AccessControlEntry ace = acl.CreateAccessControlEntry ();

      transaction.Commit ();

      return ace.ID;
    }

    private Group CreateGroup (ClientTransaction transaction, string name, string uniqueIdentifier, Group parent, Tenant tenant)
    {
      Group group = _factory.CreateGroup (transaction);
      group.Name = name;
      group.Parent = parent;
      group.Tenant = tenant;
      group.UniqueIdentifier = uniqueIdentifier;

      return group;
    }

    private Tenant CreateTenant (ClientTransaction transaction, string name)
    {
      Tenant tenant = _factory.CreateTenant (transaction);
      tenant.Name = name;

      return tenant;
    }

    private User CreateUser (ClientTransaction transaction, string userName, string firstName, string lastName, string title, Group group, Tenant tenant)
    {
      User user = _factory.CreateUser (transaction);
      user.UserName = userName;
      user.FirstName = firstName;
      user.LastName = lastName;
      user.Title = title;
      user.Tenant = tenant;
      user.OwningGroup = group;

      return user;
    }

    private SecurableClassDefinition CreateOrderSecurableClassDefinition (ClientTransaction transaction)
    {
      SecurableClassDefinition classDefinition = CreateSecurableClassDefinition (
          transaction,
          new Guid ("b8621bc9-9ab3-4524-b1e4-582657d6b420"),
          "Rubicon.SecurityManager.UnitTests.TestDomain.Order, Rubicon.SecurityManager.UnitTests");
      return classDefinition;
    }

    private SecurableClassDefinition[] CreateSecurableClassDefinitions (
        ClientTransaction transaction,
        int classDefinitionCount,
        int derivedClassDefinitionCount)
    {
      SecurableClassDefinition[] classDefinitions = new SecurableClassDefinition[classDefinitionCount];
      for (int i = 0; i < classDefinitionCount; i++)
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject (transaction);
        classDefinition.MetadataItemID = Guid.NewGuid ();
        classDefinition.Name = string.Format ("Class {0}", i);
        classDefinition.Index = i;
        classDefinitions[i] = classDefinition;
        CreateDerivedSecurableClassDefinitions (classDefinition, derivedClassDefinitionCount);
      }

      return classDefinitions;
    }

    private void CreateDerivedSecurableClassDefinitions (SecurableClassDefinition baseClassDefinition, int classDefinitionCount)
    {
      for (int i = 0; i < classDefinitionCount; i++)
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject (baseClassDefinition.ClientTransaction);
        classDefinition.MetadataItemID = Guid.NewGuid ();
        classDefinition.Name = string.Format ("{0} - Subsclass {0}", baseClassDefinition.Name, i);
        classDefinition.Index = i;
        classDefinition.BaseClass = baseClassDefinition;
      }
    }

    private SecurableClassDefinition CreateSecurableClassDefinitionWithAccessTypes (ClientTransaction transaction, int accessTypes)
    {
      SecurableClassDefinition classDefinition = CreateOrderSecurableClassDefinition (transaction);

      for (int i = 0; i < accessTypes; i++)
      {
        AccessTypeDefinition accessType = CreateAccessType (transaction, Guid.NewGuid (), string.Format ("Access Type {0}", i));
        accessType.Index = i;
        classDefinition.AddAccessType (accessType);
      }

      return classDefinition;
    }

    private GroupType CreateGroupType (ClientTransaction transaction, string name)
    {
      GroupType groupType = GroupType.NewObject (transaction);
      groupType.Name = name;

      return groupType;
    }

    private Position CreatePosition (ClientTransaction transaction, string name)
    {
      Position position = _factory.CreatePosition (transaction);
      position.Name = name;

      return position;
    }

    private Role CreateRole (ClientTransaction transaction, User user, Group group, Position position)
    {
      Role role = Role.NewObject (transaction);
      role.User = user;
      role.Group = group;
      role.Position = position;

      return role;
    }

    private SecurableClassDefinition CreateSecurableClassDefinition (ClientTransaction transaction, Guid metadataItemID, string name)
    {
      SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject (transaction);
      classDefinition.MetadataItemID = metadataItemID;
      classDefinition.Name = name;

      return classDefinition;
    }

    private StatePropertyDefinition CreateFileStateProperty (ClientTransaction transaction)
    {
      StatePropertyDefinition fileStateProperty = StatePropertyDefinition.NewObject  (transaction, new Guid ("9e689c4c-3758-436e-ac86-23171289fa5e"), "FileState");
      fileStateProperty.AddState ("Open", 0);
      fileStateProperty.AddState ("Cancelled", 1);
      fileStateProperty.AddState ("Reaccounted", 2);
      fileStateProperty.AddState ("HandledBy", 3);
      fileStateProperty.AddState ("Approved", 4);

      return fileStateProperty;
    }

    private StatePropertyDefinition CreateConfidentialityProperty (ClientTransaction transaction)
    {
      StatePropertyDefinition confidentialityProperty = StatePropertyDefinition.NewObject  (transaction, new Guid ("93969f13-65d7-49f4-a456-a1686a4de3de"), "Confidentiality");
      confidentialityProperty.AddState ("Public", 0);
      confidentialityProperty.AddState ("Secret", 1);
      confidentialityProperty.AddState ("TopSecret", 2);

      return confidentialityProperty;
    }

    private AccessTypeDefinition CreateAccessType (ClientTransaction transaction, Guid metadataItemID, string name)
    {
      AccessTypeDefinition accessType = AccessTypeDefinition.NewObject  (transaction);
      accessType.MetadataItemID = metadataItemID;
      accessType.Name = name;

      return accessType;
    }

    private void CreateStateCombination (ClientTransaction transaction, AccessControlList acl, params string[] propertyNames)
    {
      StateCombination stateCombination = acl.CreateStateCombination ();
      foreach (string propertyName in propertyNames)
      {
        StatePropertyDefinition stateProperty = StatePropertyDefinition.NewObject  (transaction, Guid.NewGuid (), propertyName);
        StateDefinition stateDefinition = StateDefinition.NewObject (transaction, "value", 0);
        stateProperty.AddState (stateDefinition);
        acl.Class.AddStateProperty (stateProperty);
        stateCombination.AttachState (stateDefinition);
      }
    }
  }
}
