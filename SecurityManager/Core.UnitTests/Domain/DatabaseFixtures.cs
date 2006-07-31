using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.SecurityManager.Configuration;
using Rubicon.SecurityManager.Domain.AccessControl;

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

      Culture germanCulture = new Culture (transaction, "de");
      Culture englishCulture = new Culture (transaction, "en");
      Culture russianCulture = new Culture (transaction, "ru");

      LocalizedName classInGerman = new LocalizedName (transaction, "Klasse", germanCulture, classDefinition);
      LocalizedName classInEnglish = new LocalizedName (transaction, "Class", englishCulture, classDefinition);

      transaction.Commit ();
    }

    public Client CreateOrganizationalStructureWithTwoClients ()
    {
      CreateEmptyDomain ();

      ClientTransaction transaction = new ClientTransaction ();

      AbstractRoleDefinition qualityManagerRole = new AbstractRoleDefinition (transaction, Guid.NewGuid (), "QualityManager|Rubicon.SecurityManager.UnitTests.TestDomain.ProjectRole, Rubicon.SecurityManager.UnitTests", 0);
      qualityManagerRole.Index = 1;
      AbstractRoleDefinition developerRole = new AbstractRoleDefinition (transaction, Guid.NewGuid (), "Developer|Rubicon.SecurityManager.UnitTests.TestDomain.ProjectRole, Rubicon.SecurityManager.UnitTests", 1);
      developerRole.Index = 0;

      Client client1 = CreateClient (transaction, "Testclient");
      Group rootGroup = CreateGroup (transaction, "rootGroup", "UnqiueIdentifier: rootGroup", null, client1);
      Group parentOfOwnerGroup = CreateGroup (transaction, "parentOfOwnerGroup", "UnqiueIdentifier: parentOfOwnerGroup", rootGroup, client1);
      Group ownerGroup = CreateGroup (transaction, "ownerGroup", "UnqiueIdentifier: ownerGroup", parentOfOwnerGroup, client1);
      Group group = CreateGroup (transaction, "Testgroup", "UnqiueIdentifier: Testgroup", ownerGroup, client1);
      User user1 = CreateUser (transaction, "test.user", "test", "user", "Dipl.Ing.(FH)", group, client1);
      User user2 = CreateUser (transaction, "other.test.user", "other test", "user", "Dipl.Ing.(FH)", group, client1);
      Position officialPosition = CreatePosition (transaction, "Official");
      Position managerPosition = CreatePosition (transaction, "Manager");

      Role officialInGroup = CreateRole (transaction, user1, group, officialPosition);
      Role managerInGroup = CreateRole (transaction, user1, group, managerPosition);
      Role managerInOwnerGroup = CreateRole (transaction, user1, ownerGroup, managerPosition);
      Role officialInRootGroup = CreateRole (transaction, user1, rootGroup, officialPosition);
      
      GroupType groupType1 = CreateGroupType (transaction, "groupType 1");
      GroupType groupType2 = CreateGroupType (transaction, "groupType 2");

      Client client2 = CreateClient (transaction, "Client 2");
      Group groupClient2 = CreateGroup (transaction, "Group Client 2", "UnqiueIdentifier: group Client 2", null, client2);
      User userClient2 = CreateUser (transaction, "User.Client2", "User", "Client 2", string.Empty, groupClient2, client2);
      Position position2 = CreatePosition (transaction, "Position Client 2");
      GroupType groupTypeClient2 = CreateGroupType (transaction, "GroupType Client 2");

      transaction.Commit ();

      return client1;
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
        AccessControlList acl = new AccessControlList (transaction);
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
      AccessControlList acl = new AccessControlList (transaction);
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
      AccessControlList acl = new AccessControlList (transaction);
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
      AbstractRoleDefinition administratorAbstractRole = new AbstractRoleDefinition (transaction, metadataItemID, abstractRoleName, 0);

      transaction.Commit ();
    }

    public ObjectID CreateAccessControlEntryWithPermissions (int permissions)
    {
      CreateEmptyDomain ();

      ClientTransaction transaction = new ClientTransaction ();

      SecurableClassDefinition classDefinition = CreateSecurableClassDefinitionWithAccessTypes (transaction, permissions);
      AccessControlList acl = classDefinition.CreateAccessControlList();
      AccessControlEntry ace = acl.CreateAccessControlEntry ();

      transaction.Commit ();

      return ace.ID;
    }

    private Group CreateGroup (ClientTransaction transaction, string name, string uniqueIdentifier, Group parent, Client client)
    {
      Group group = _factory.CreateGroup (transaction);
      group.Name = name;
      group.Parent = parent;
      group.Client = client;
      group.UniqueIdentifier = uniqueIdentifier;

      return group;
    }

    private Client CreateClient (ClientTransaction transaction, string name)
    {
      Client client = new Client (transaction);
      client.Name = name;

      return client;
    }

    private User CreateUser (ClientTransaction transaction, string userName, string firstName, string lastName, string title, Group group, Client client)
    {
      User user = _factory.CreateUser (transaction);
      user.UserName = userName;
      user.FirstName = firstName;
      user.LastName = lastName;
      user.Title = title;
      user.Client = client;
      user.Group = group;

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
        SecurableClassDefinition classDefinition = new SecurableClassDefinition (transaction);
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
        SecurableClassDefinition classDefinition = new SecurableClassDefinition (baseClassDefinition.ClientTransaction);
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
      GroupType groupType = new GroupType (transaction);
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
      Role role = new Role (transaction);
      role.User = user;
      role.Group = group;
      role.Position = position;

      return role;
    }

    private SecurableClassDefinition CreateSecurableClassDefinition (ClientTransaction transaction, Guid metadataItemID, string name)
    {
      SecurableClassDefinition classDefinition = new SecurableClassDefinition (transaction);
      classDefinition.MetadataItemID = metadataItemID;
      classDefinition.Name = name;

      return classDefinition;
    }

    private StatePropertyDefinition CreateFileStateProperty (ClientTransaction transaction)
    {
      StatePropertyDefinition fileStateProperty = new StatePropertyDefinition (transaction, new Guid ("9e689c4c-3758-436e-ac86-23171289fa5e"), "FileState");
      fileStateProperty.AddState ("Open", 0);
      fileStateProperty.AddState ("Cancelled", 1);
      fileStateProperty.AddState ("Reaccounted", 2);
      fileStateProperty.AddState ("HandledBy", 3);
      fileStateProperty.AddState ("Approved", 4);

      return fileStateProperty;
    }

    private StatePropertyDefinition CreateConfidentialityProperty (ClientTransaction transaction)
    {
      StatePropertyDefinition confidentialityProperty = new StatePropertyDefinition (transaction, new Guid ("93969f13-65d7-49f4-a456-a1686a4de3de"), "Confidentiality");
      confidentialityProperty.AddState ("Public", 0);
      confidentialityProperty.AddState ("Secret", 1);
      confidentialityProperty.AddState ("TopSecret", 2);

      return confidentialityProperty;
    }

    private AccessTypeDefinition CreateAccessType (ClientTransaction transaction, Guid metadataItemID, string name)
    {
      AccessTypeDefinition accessType = new AccessTypeDefinition (transaction);
      accessType.MetadataItemID = metadataItemID;
      accessType.Name = name;

      return accessType;
    }

    private void CreateStateCombination (ClientTransaction transaction, AccessControlList acl, params string[] propertyNames)
    {
      StateCombination stateCombination = acl.CreateStateCombination ();
      foreach (string propertyName in propertyNames)
      {
        StatePropertyDefinition stateProperty = new StatePropertyDefinition (transaction, Guid.NewGuid (), propertyName);
        StateDefinition stateDefinition = new StateDefinition (transaction, "value", 0);
        stateProperty.AddState (stateDefinition);
        acl.Class.AddStateProperty (stateProperty);
        stateCombination.AttachState (stateDefinition);
      }
    }
  }
}
