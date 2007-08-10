using System;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Domain
{
  public class DatabaseFixtures
  {
    private readonly OrganizationalStructureFactory _factory;

    public DatabaseFixtures ()
    {
      _factory = new OrganizationalStructureFactory ();
    }

    public void CreateEmptyDomain ()
    {
      DatabaseHelper dbHelper = new DatabaseHelper ();
      dbHelper.SetupDB ();
    }

    public void CreateSecurableClassDefinitionWithLocalizedNames (ClientTransaction transaction)
    {
      CreateEmptyDomain ();

      using (transaction.EnterScope ())
      {
        SecurableClassDefinition classDefinition = CreateOrderSecurableClassDefinition (ClientTransactionScope.CurrentTransaction);

        Culture germanCulture = Culture.NewObject (ClientTransactionScope.CurrentTransaction, "de");
        Culture englishCulture = Culture.NewObject (ClientTransactionScope.CurrentTransaction, "en");
        Culture russianCulture = Culture.NewObject (ClientTransactionScope.CurrentTransaction, "ru");

        LocalizedName classInGerman = LocalizedName.NewObject (ClientTransactionScope.CurrentTransaction, "Klasse", germanCulture, classDefinition);
        LocalizedName classInEnglish = LocalizedName.NewObject (ClientTransactionScope.CurrentTransaction, "Class", englishCulture, classDefinition);

        ClientTransactionScope.CurrentTransaction.Commit();
      }
    }

    public Tenant CreateOrganizationalStructureWithTwoTenants (ClientTransaction transaction)
    {
      CreateEmptyDomain ();

      using (transaction.EnterScope ())
      {
        AbstractRoleDefinition qualityManagerRole = AbstractRoleDefinition.NewObject (
            ClientTransactionScope.CurrentTransaction,
            Guid.NewGuid(),
            "QualityManager|Rubicon.SecurityManager.UnitTests.TestDomain.ProjectRoles, Rubicon.SecurityManager.UnitTests",
            0);
        qualityManagerRole.Index = 1;
        AbstractRoleDefinition developerRole =AbstractRoleDefinition.NewObject (
            ClientTransactionScope.CurrentTransaction,
            Guid.NewGuid(),
            "Developer|Rubicon.SecurityManager.UnitTests.TestDomain.ProjectRoles, Rubicon.SecurityManager.UnitTests",
            1);
        developerRole.Index = 0;

        Position globalPosition = CreatePosition (ClientTransactionScope.CurrentTransaction, "Global");
        globalPosition.Delegation = Delegation.Enabled;
        Position officialPosition = CreatePosition (ClientTransactionScope.CurrentTransaction, "Official");
        officialPosition.Delegation = Delegation.Enabled;
        Position managerPosition = CreatePosition (ClientTransactionScope.CurrentTransaction, "Manager");
        managerPosition.Delegation = Delegation.Disabled;

        GroupType groupType1 = CreateGroupType (ClientTransactionScope.CurrentTransaction, "groupType 1");
        GroupType groupType2 = CreateGroupType (ClientTransactionScope.CurrentTransaction, "groupType 2");

        GroupTypePosition groupType1_managerPosition = GroupTypePosition.NewObject (ClientTransactionScope.CurrentTransaction);
        groupType1_managerPosition.GroupType = groupType1;
        groupType1_managerPosition.Position = managerPosition;
        GroupTypePosition groupType1_officialPosition = GroupTypePosition.NewObject (ClientTransactionScope.CurrentTransaction);
        groupType1_officialPosition.GroupType = groupType1;
        groupType1_officialPosition.Position = officialPosition;
        GroupTypePosition groupType2_officialPosition = GroupTypePosition.NewObject (ClientTransactionScope.CurrentTransaction);
        groupType2_officialPosition.GroupType = groupType2;
        groupType2_officialPosition.Position = officialPosition;

        Tenant tenant1 = CreateTenant (ClientTransactionScope.CurrentTransaction, "TestTenant");
        tenant1.UniqueIdentifier = "UID: testTenant";
        Group rootGroup = CreateGroup (ClientTransactionScope.CurrentTransaction, "rootGroup", "UID: rootGroup", null, tenant1);
        for (int i = 0; i < 2; i++)
        {
          Group parentGroup =
              CreateGroup (ClientTransactionScope.CurrentTransaction, string.Format ("parentGroup{0}", i), string.Format ("UID: parentGroup{0}", i), rootGroup, tenant1);
          parentGroup.GroupType = groupType1;

          Group group = CreateGroup (ClientTransactionScope.CurrentTransaction, string.Format ("group{0}", i), string.Format ("UID: group{0}", i), parentGroup, tenant1);
          group.GroupType = groupType2;

          User user1 = CreateUser (ClientTransactionScope.CurrentTransaction, string.Format ("group{0}/user1", i), string.Empty, "user1", string.Empty, group, tenant1);
          User user2 = CreateUser (ClientTransactionScope.CurrentTransaction, string.Format ("group{0}/user2", i), string.Empty, "user2", string.Empty, group, tenant1);

          CreateRole (ClientTransactionScope.CurrentTransaction, user1, parentGroup, managerPosition);
          CreateRole (ClientTransactionScope.CurrentTransaction, user2, parentGroup, officialPosition);
        }

        Group testRootGroup = CreateGroup (ClientTransactionScope.CurrentTransaction, "testRootGroup", "UID: testRootGroup", null, tenant1);
        Group testParentOfOwningGroup = CreateGroup (ClientTransactionScope.CurrentTransaction, "testParentOfOwningGroup", "UID: testParentOfOwningGroup", testRootGroup, tenant1);
        Group testOwningGroup = CreateGroup (ClientTransactionScope.CurrentTransaction, "testOwningGroup", "UID: testOwningGroup", testParentOfOwningGroup, tenant1);
        Group testGroup = CreateGroup (ClientTransactionScope.CurrentTransaction, "testGroup", "UID: testGroup", null, tenant1);
        User testUser = CreateUser (ClientTransactionScope.CurrentTransaction, "test.user", "test", "user", "Dipl.Ing.(FH)", testOwningGroup, tenant1);

        CreateRole (ClientTransactionScope.CurrentTransaction, testUser, testGroup, officialPosition);
        CreateRole (ClientTransactionScope.CurrentTransaction, testUser, testGroup, managerPosition);
        CreateRole (ClientTransactionScope.CurrentTransaction, testUser, testOwningGroup, managerPosition);
        CreateRole (ClientTransactionScope.CurrentTransaction, testUser, testRootGroup, officialPosition);

        Tenant tenant2 = CreateTenant (ClientTransactionScope.CurrentTransaction, "Tenant 2");
        Group groupTenant2 = CreateGroup (ClientTransactionScope.CurrentTransaction, "Group Tenant 2", "UID: group Tenant 2", null, tenant2);
        User userTenant2 = CreateUser (ClientTransactionScope.CurrentTransaction, "User.Tenant2", "User", "Tenant 2", string.Empty, groupTenant2, tenant2);

        ClientTransactionScope.CurrentTransaction.Commit();
        return tenant1;
      }
    }

    public SecurableClassDefinition[] CreateSecurableClassDefinitionsWithSubClassesEach (int classDefinitionCount, int derivedClassDefinitionCount, ClientTransaction transaction)
    {
      CreateEmptyDomain ();
      using (transaction.EnterScope ())
      {
        SecurableClassDefinition[] classDefinitions = CreateSecurableClassDefinitions (ClientTransactionScope.CurrentTransaction, classDefinitionCount, derivedClassDefinitionCount);

        ClientTransactionScope.CurrentTransaction.Commit();

        return classDefinitions;
      }
    }

    public SecurableClassDefinition[] CreateSecurableClassDefinitions (int classDefinitionCount, ClientTransaction transaction)
    {
      CreateEmptyDomain ();
      using (transaction.EnterScope ())
      {
        SecurableClassDefinition[] classDefinitions = CreateSecurableClassDefinitions (ClientTransactionScope.CurrentTransaction, classDefinitionCount, 0);

        ClientTransactionScope.CurrentTransaction.Commit();

        return classDefinitions;
      }
    }

    public SecurableClassDefinition CreateSecurableClassDefinitionWithStates (ClientTransaction transaction)
    {
      CreateEmptyDomain ();
      using (transaction.EnterScope ())
      {
        SecurableClassDefinition classDefinition = CreateOrderSecurableClassDefinition (ClientTransactionScope.CurrentTransaction);

        classDefinition.AddStateProperty (CreateFileStateProperty (ClientTransactionScope.CurrentTransaction));
        classDefinition.AddStateProperty (CreateConfidentialityProperty (ClientTransactionScope.CurrentTransaction));

        ClientTransactionScope.CurrentTransaction.Commit();

        return classDefinition;
      }
    }

    public SecurableClassDefinition CreateSecurableClassDefinitionWithAccessTypes (int accessTypes, ClientTransaction transaction)
    {
      CreateEmptyDomain ();
      using (transaction.EnterScope ())
      {
        SecurableClassDefinition classDefinition = CreateSecurableClassDefinitionWithAccessTypes (ClientTransactionScope.CurrentTransaction, accessTypes);

        ClientTransactionScope.CurrentTransaction.Commit();

        return classDefinition;
      }
    }

    public SecurableClassDefinition CreateSecurableClassDefinitionWithAccessControlLists (int accessControlLists, ClientTransaction transaction)
    {
      CreateEmptyDomain ();

      using (transaction.EnterScope ())
      {
        SecurableClassDefinition classDefinition = CreateOrderSecurableClassDefinition (ClientTransactionScope.CurrentTransaction);
        for (int i = 0; i < accessControlLists; i++)
        {
          AccessControlList acl = AccessControlList.NewObject (ClientTransactionScope.CurrentTransaction);
          acl.Class = classDefinition;
          acl.CreateAccessControlEntry();
          if (i == 0)
            CreateStateCombination (ClientTransactionScope.CurrentTransaction, acl);
          else
            CreateStateCombination (ClientTransactionScope.CurrentTransaction, acl, string.Format ("Property {0}", i));
        }

        ClientTransactionScope.CurrentTransaction.Commit();

        return classDefinition;
      }
    }

    public AccessControlList CreateAccessControlListWithAccessControlEntries (int accessControlEntries, ClientTransaction transaction)
    {
      CreateEmptyDomain();

      using (transaction.EnterScope())
      {
        SecurableClassDefinition classDefinition = CreateOrderSecurableClassDefinition (ClientTransactionScope.CurrentTransaction);
        AccessControlList acl = AccessControlList.NewObject (ClientTransactionScope.CurrentTransaction);
        acl.Class = classDefinition;
        acl.CreateStateCombination();

        for (int i = 0; i < accessControlEntries; i++)
          acl.CreateAccessControlEntry();

        ClientTransactionScope.CurrentTransaction.Commit();

        return acl;
      }
    }

    public AccessControlList CreateAccessControlListWithStateCombinations (int stateCombinations, ClientTransaction transaction)
    {
      CreateEmptyDomain ();
      using (transaction.EnterScope ())
      {
        SecurableClassDefinition classDefinition = CreateOrderSecurableClassDefinition (ClientTransactionScope.CurrentTransaction);
        AccessControlList acl = AccessControlList.NewObject (ClientTransactionScope.CurrentTransaction);
        acl.Class = classDefinition;
        acl.CreateAccessControlEntry();

        for (int i = 0; i < stateCombinations; i++)
        {
          if (i == 0)
            CreateStateCombination (ClientTransactionScope.CurrentTransaction, acl);
          else
            CreateStateCombination (ClientTransactionScope.CurrentTransaction, acl, string.Format ("Property {0}", i));
        }

        ClientTransactionScope.CurrentTransaction.Commit();

        return acl;
      }
    }

    public void CreateAdministratorAbstractRole (ClientTransaction transaction)
    {
      CreateEmptyDomain ();

      using (transaction.EnterScope ())
      {
        Guid metadataItemID = new Guid ("00000004-0001-0000-0000-000000000000");
        string abstractRoleName = "Administrator|Rubicon.Security.UnitTests.TestDomain.SpecialAbstractRoles, Rubicon.Security.UnitTests.TestDomain";
        AbstractRoleDefinition administratorAbstractRole = AbstractRoleDefinition.NewObject (ClientTransactionScope.CurrentTransaction, metadataItemID, abstractRoleName, 0);

        ClientTransactionScope.CurrentTransaction.Commit();
      }
    }

    public ObjectID CreateAccessControlEntryWithPermissions (int permissions, ClientTransaction transaction)
    {
      CreateEmptyDomain ();

      using (transaction.EnterScope ())
      {
        SecurableClassDefinition classDefinition = CreateSecurableClassDefinitionWithAccessTypes (ClientTransactionScope.CurrentTransaction, permissions);
        AccessControlList acl = classDefinition.CreateAccessControlList();
        AccessControlEntry ace = acl.CreateAccessControlEntry();

        ClientTransactionScope.CurrentTransaction.Commit();

        return ace.ID;
      }
    }

    private Group CreateGroup (ClientTransaction transaction, string name, string uniqueIdentifier, Group parent, Tenant tenant)
    {
      using (transaction.EnterScope ())
      {
        Group group = _factory.CreateGroup (ClientTransactionScope.CurrentTransaction);
        group.Name = name;
        group.Parent = parent;
        group.Tenant = tenant;
        group.UniqueIdentifier = uniqueIdentifier;

        return group;
      }
    }

    private Tenant CreateTenant (ClientTransaction transaction, string name)
    {
      using (transaction.EnterScope ())
      {
        Tenant tenant = _factory.CreateTenant (ClientTransactionScope.CurrentTransaction);
        tenant.Name = name;

        return tenant;
      }
    }

    private User CreateUser (ClientTransaction transaction, string userName, string firstName, string lastName, string title, Group group, Tenant tenant)
    {
      using (transaction.EnterScope ())
      {
        User user = _factory.CreateUser (ClientTransactionScope.CurrentTransaction);
        user.UserName = userName;
        user.FirstName = firstName;
        user.LastName = lastName;
        user.Title = title;
        user.Tenant = tenant;
        user.OwningGroup = group;

        return user;
      }
    }

    private SecurableClassDefinition CreateOrderSecurableClassDefinition (ClientTransaction transaction)
    {
      using (transaction.EnterScope ())
      {
        SecurableClassDefinition classDefinition = CreateSecurableClassDefinition (
            ClientTransactionScope.CurrentTransaction,
            new Guid ("b8621bc9-9ab3-4524-b1e4-582657d6b420"),
            "Rubicon.SecurityManager.UnitTests.TestDomain.Order, Rubicon.SecurityManager.UnitTests");
        return classDefinition;
      }
    }

    private SecurableClassDefinition[] CreateSecurableClassDefinitions (
        ClientTransaction transaction,
        int classDefinitionCount,
        int derivedClassDefinitionCount)
    {
      using (transaction.EnterScope ())
      {
        SecurableClassDefinition[] classDefinitions = new SecurableClassDefinition[classDefinitionCount];
        for (int i = 0; i < classDefinitionCount; i++)
        {
          SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject (ClientTransactionScope.CurrentTransaction);
          classDefinition.MetadataItemID = Guid.NewGuid();
          classDefinition.Name = string.Format ("Class {0}", i);
          classDefinition.Index = i;
          classDefinitions[i] = classDefinition;
          CreateDerivedSecurableClassDefinitions (classDefinition, derivedClassDefinitionCount);
        }
        return classDefinitions;
      }
    }

    private void CreateDerivedSecurableClassDefinitions (SecurableClassDefinition baseClassDefinition, int classDefinitionCount)
    {
      using (baseClassDefinition.ClientTransaction.EnterScope())
      {
        for (int i = 0; i < classDefinitionCount; i++)
        {
          SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject (ClientTransactionScope.CurrentTransaction);
          classDefinition.MetadataItemID = Guid.NewGuid();
          classDefinition.Name = string.Format ("{0} - Subsclass {0}", baseClassDefinition.Name, i);
          classDefinition.Index = i;
          classDefinition.BaseClass = baseClassDefinition;
        }
      }
    }

    private SecurableClassDefinition CreateSecurableClassDefinitionWithAccessTypes (ClientTransaction transaction, int accessTypes)
    {
      using (transaction.EnterScope ())
      {
        SecurableClassDefinition classDefinition = CreateOrderSecurableClassDefinition (ClientTransactionScope.CurrentTransaction);

        for (int i = 0; i < accessTypes; i++)
        {
          AccessTypeDefinition accessType = CreateAccessType (ClientTransactionScope.CurrentTransaction, Guid.NewGuid(), string.Format ("Access Type {0}", i));
          accessType.Index = i;
          classDefinition.AddAccessType (accessType);
        }

        return classDefinition;
      }
    }

    private GroupType CreateGroupType (ClientTransaction transaction, string name)
    {
      using (transaction.EnterScope ())
      {
        GroupType groupType = GroupType.NewObject (ClientTransactionScope.CurrentTransaction);
        groupType.Name = name;

        return groupType;
      }
    }

    private Position CreatePosition (ClientTransaction transaction, string name)
    {
      using (transaction.EnterScope ())
      {
        Position position = _factory.CreatePosition (ClientTransactionScope.CurrentTransaction);
        position.Name = name;

        return position;
      }
    }

    private Role CreateRole (ClientTransaction transaction, User user, Group group, Position position)
    {
      using (transaction.EnterScope ())
      {
        Role role = Role.NewObject (ClientTransactionScope.CurrentTransaction);
        role.User = user;
        role.Group = group;
        role.Position = position;

        return role;
      }
    }

    private SecurableClassDefinition CreateSecurableClassDefinition (ClientTransaction transaction, Guid metadataItemID, string name)
    {
      using (transaction.EnterScope ())
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject (ClientTransactionScope.CurrentTransaction);
        classDefinition.MetadataItemID = metadataItemID;
        classDefinition.Name = name;

        return classDefinition;
      }
    }

    private StatePropertyDefinition CreateFileStateProperty (ClientTransaction transaction)
    {
      using (transaction.EnterScope ())
      {
        StatePropertyDefinition fileStateProperty =
            StatePropertyDefinition.NewObject (ClientTransactionScope.CurrentTransaction, new Guid ("9e689c4c-3758-436e-ac86-23171289fa5e"), "FileState");
        fileStateProperty.AddState ("Open", 0);
        fileStateProperty.AddState ("Cancelled", 1);
        fileStateProperty.AddState ("Reaccounted", 2);
        fileStateProperty.AddState ("HandledBy", 3);
        fileStateProperty.AddState ("Approved", 4);

        return fileStateProperty;
      }
    }

    private StatePropertyDefinition CreateConfidentialityProperty (ClientTransaction transaction)
    {
      using (transaction.EnterScope ())
      {
        StatePropertyDefinition confidentialityProperty =
            StatePropertyDefinition.NewObject (ClientTransactionScope.CurrentTransaction, new Guid ("93969f13-65d7-49f4-a456-a1686a4de3de"), "Confidentiality");
        confidentialityProperty.AddState ("Public", 0);
        confidentialityProperty.AddState ("Secret", 1);
        confidentialityProperty.AddState ("TopSecret", 2);

        return confidentialityProperty;
      }
    }

    private AccessTypeDefinition CreateAccessType (ClientTransaction transaction, Guid metadataItemID, string name)
    {
      using (transaction.EnterScope ())
      {
        AccessTypeDefinition accessType = AccessTypeDefinition.NewObject (ClientTransactionScope.CurrentTransaction);
        accessType.MetadataItemID = metadataItemID;
        accessType.Name = name;

        return accessType;
      }
    }

    private void CreateStateCombination (ClientTransaction transaction, AccessControlList acl, params string[] propertyNames)
    {
      using (transaction.EnterScope())
      {
        StateCombination stateCombination = acl.CreateStateCombination();
        foreach (string propertyName in propertyNames)
        {
          StatePropertyDefinition stateProperty = StatePropertyDefinition.NewObject (ClientTransactionScope.CurrentTransaction, Guid.NewGuid(), propertyName);
          StateDefinition stateDefinition = StateDefinition.NewObject (ClientTransactionScope.CurrentTransaction, "value", 0);
          stateProperty.AddState (stateDefinition);
          acl.Class.AddStateProperty (stateProperty);
          stateCombination.AttachState (stateDefinition);
        }
      }
    }
  }
}
