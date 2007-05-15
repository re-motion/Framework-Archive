using System;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl
{
  public class AccessControlTestHelper
  {
    public const int OrderClassPropertyCount = 2;

    private ClientTransaction _transaction;
    private OrganizationalStructureFactory _factory;

    public AccessControlTestHelper ()
    {
      _transaction = new ClientTransaction ();
      _factory = new OrganizationalStructureFactory ();
    }

    public ClientTransaction Transaction
    {
      get { return _transaction; }
    }

    public SecurableClassDefinition CreateOrderClassDefinition ()
    {
      return CreateClassDefinition ("Rubicon.SecurityManager.UnitTests.TestDomain.Order");
    }

    public SecurableClassDefinition CreateSpecialOrderClassDefinition (SecurableClassDefinition orderClassDefinition)
    {
      return CreateClassDefinition ("Rubicon.SecurityManager.UnitTests.TestDomain.SpecialOrder", orderClassDefinition);
    }

    public SecurableClassDefinition CreatePremiumOrderClassDefinition (SecurableClassDefinition orderClassDefinition)
    {
      return CreateClassDefinition ("Rubicon.SecurityManager.UnitTests.TestDomain.PremiumOrder", orderClassDefinition);
    }

    public SecurableClassDefinition CreateInvoiceClassDefinition ()
    {
      return CreateClassDefinition ("Rubicon.SecurityManager.UnitTests.TestDomain.Invoice");
    }

    public SecurableClassDefinition CreateClassDefinition (string name)
    {
      return CreateClassDefinition (name, null);
    }

    public SecurableClassDefinition CreateClassDefinition (string name, SecurableClassDefinition baseClass)
    {
      SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject (_transaction);
      classDefinition.Name = name;
      classDefinition.BaseClass = baseClass;

      return classDefinition;
    }

    public SecurableClassDefinition CreateOrderClassDefinitionWithProperties ()
    {
      SecurableClassDefinition classDefinition = CreateOrderClassDefinition ();
      StatePropertyDefinition orderStateProperty = CreateOrderStateProperty (classDefinition);
      StatePropertyDefinition paymentStateProperty = CreatePaymentStateProperty (classDefinition);

      return classDefinition;
    }

    public AccessControlList CreateAcl (SecurableClassDefinition classDefinition, params StateDefinition[] states)
    {
      AccessControlList acl = AccessControlList.NewObject (_transaction);
      acl.Class = classDefinition;
      StateCombination stateCombination = CreateStateCombination (acl);

      foreach (StateDefinition state in states)
        stateCombination.AttachState (state);

      return acl;
    }

    public StateCombination CreateStateCombination (AccessControlList acl)
    {
      StateCombination stateCombination = StateCombination.NewObject (_transaction);
      stateCombination.AccessControlList = acl;
      stateCombination.Class = acl.Class;

      return stateCombination;
    }

    public StateCombination CreateStateCombination (SecurableClassDefinition classDefinition, params StateDefinition[] states)
    {
      AccessControlList acl = CreateAcl (classDefinition, states);
      return (StateCombination) acl.StateCombinations[0];
    }

    public StatePropertyDefinition CreateStateProperty (string name)
    {
      return StatePropertyDefinition.NewObject  (_transaction, Guid.NewGuid (), name);
    }

    public StatePropertyDefinition CreateOrderStateProperty (SecurableClassDefinition classDefinition)
    {
      StatePropertyDefinition orderStateProperty = CreateStateProperty ("State");
      orderStateProperty.AddState ("Received", 0);
      orderStateProperty.AddState ("Delivered", 1);
      classDefinition.AddStateProperty (orderStateProperty);

      return orderStateProperty;
    }

    public StatePropertyDefinition CreatePaymentStateProperty (SecurableClassDefinition classDefinition)
    {
      StatePropertyDefinition paymentStateProperty = CreateStateProperty ("Payment");
      paymentStateProperty.AddState ("None", 0);
      paymentStateProperty.AddState ("Paid", 1);
      classDefinition.AddStateProperty (paymentStateProperty);

      return paymentStateProperty;
    }

    public StatePropertyDefinition CreateDeliveryStateProperty (SecurableClassDefinition classDefinition)
    {
      StatePropertyDefinition deliveryStateProperty = CreateStateProperty ("Delivery");
      deliveryStateProperty.AddState ("Dhl", 0);
      deliveryStateProperty.AddState ("Post", 1);
      classDefinition.AddStateProperty (deliveryStateProperty);

      return deliveryStateProperty;
    }

    public List<StateCombination> CreateStateCombinationsForOrder ()
    {
      SecurableClassDefinition orderClass = CreateOrderClassDefinition ();
      return CreateOrderStateAndPaymentStateCombinations (orderClass);
    }

    public List<StateCombination> CreateOrderStateAndPaymentStateCombinations (SecurableClassDefinition classDefinition)
    {
      StatePropertyDefinition orderState = CreateOrderStateProperty (classDefinition);
      StatePropertyDefinition paymentState = CreatePaymentStateProperty (classDefinition);

      List<StateCombination> stateCombinations = new List<StateCombination> ();
      stateCombinations.Add (CreateStateCombination (classDefinition, orderState["Received"], paymentState["None"]));
      stateCombinations.Add (CreateStateCombination (classDefinition, orderState["Received"], paymentState["Paid"]));
      stateCombinations.Add (CreateStateCombination (classDefinition, orderState["Delivered"], paymentState["None"]));
      stateCombinations.Add (CreateStateCombination (classDefinition, orderState["Delivered"], paymentState["Paid"]));
      stateCombinations.Add (CreateStateCombination (classDefinition));

      return stateCombinations;
    }

    public StateCombination GetStateCombinationForDeliveredAndUnpaidOrder ()
    {
      List<StateCombination> stateCombinations = CreateStateCombinationsForOrder ();
      return stateCombinations[2];
    }

    public StateCombination GetStateCombinationWithoutStates ()
    {
      List<StateCombination> stateCombinations = CreateStateCombinationsForOrder ();
      return stateCombinations[4];
    }

    public List<AccessControlList> CreateAclsForOrderAndPaymentStates (SecurableClassDefinition classDefinition)
    {
      StatePropertyDefinition orderState = CreateOrderStateProperty (classDefinition);
      StatePropertyDefinition paymentState = CreatePaymentStateProperty (classDefinition);

      List<AccessControlList> acls = new List<AccessControlList> ();
      acls.Add (CreateAcl (classDefinition, orderState["Received"], paymentState["None"]));
      acls.Add (CreateAcl (classDefinition, orderState["Received"], paymentState["Paid"]));
      acls.Add (CreateAcl (classDefinition, orderState["Delivered"], paymentState["None"]));
      acls.Add (CreateAcl (classDefinition, orderState["Delivered"], paymentState["Paid"]));
      acls.Add (CreateAcl (classDefinition));

      return acls;
    }

    public List<AccessControlList> CreateAclsForOrderAndPaymentAndDeliveryStates (SecurableClassDefinition classDefinition)
    {
      StatePropertyDefinition orderState = CreateOrderStateProperty (classDefinition);
      StatePropertyDefinition paymentState = CreatePaymentStateProperty (classDefinition);
      StatePropertyDefinition deliveryState = CreateDeliveryStateProperty (classDefinition);

      List<AccessControlList> acls = new List<AccessControlList> ();
      acls.Add (CreateAcl (classDefinition, orderState["Received"], paymentState["None"], deliveryState["Dhl"]));
      acls.Add (CreateAcl (classDefinition, orderState["Received"], paymentState["Paid"], deliveryState["Dhl"]));
      acls.Add (CreateAcl (classDefinition, orderState["Delivered"], paymentState["None"], deliveryState["Dhl"]));
      acls.Add (CreateAcl (classDefinition, orderState["Delivered"], paymentState["Paid"], deliveryState["Dhl"]));
      acls.Add (CreateAcl (classDefinition, orderState["Received"], paymentState["None"], deliveryState["Post"]));
      acls.Add (CreateAcl (classDefinition, orderState["Received"], paymentState["Paid"], deliveryState["Post"]));
      acls.Add (CreateAcl (classDefinition, orderState["Delivered"], paymentState["None"], deliveryState["Post"]));
      acls.Add (CreateAcl (classDefinition, orderState["Delivered"], paymentState["Paid"], deliveryState["Post"]));
      acls.Add (CreateAcl (classDefinition));

      return acls;
    }

    public AccessControlList GetAclForDeliveredAndUnpaidAndDhlStates (SecurableClassDefinition classDefinition)
    {
      List<AccessControlList> acls = CreateAclsForOrderAndPaymentAndDeliveryStates (classDefinition);
      return acls[2];
    }

    public AccessControlList GetAclForDeliveredAndUnpaidStates (SecurableClassDefinition classDefinition)
    {
      List<AccessControlList> acls = CreateAclsForOrderAndPaymentStates (classDefinition);
      return acls[2];
    }

    public AccessControlList GetAclForStateless (SecurableClassDefinition classDefinition)
    {
      List<AccessControlList> acls = CreateAclsForOrderAndPaymentStates (classDefinition);
      return acls[4];
    }

    public List<StateDefinition> GetDeliveredAndUnpaidStateList (SecurableClassDefinition classDefinition)
    {
      List<StateDefinition> states = new List<StateDefinition> ();

      foreach (StatePropertyDefinition property in classDefinition.StateProperties)
      {
        if (property.Name == "State")
          states.Add (property["Delivered"]);

        if (property.Name == "Payment")
          states.Add (property["None"]);
      }

      return states;
    }

    public StatePropertyDefinition CreateTestProperty ()
    {
      StatePropertyDefinition property = CreateStateProperty ("Test");
      property.AddState ("Test1", 0);
      property.AddState ("Test2", 1);

      return property;
    }

    public AccessTypeDefinition AttachAccessType (SecurableClassDefinition classDefinition, Guid metadataItemID, string name, int value)
    {
      AccessTypeDefinition accessType = AccessTypeDefinition.NewObject  (_transaction, metadataItemID, name, value);
      classDefinition.AddAccessType (accessType);
      
      return accessType;
    }

    public AccessTypeDefinition AttachJournalizeAccessType (SecurableClassDefinition classDefinition)
    {
      AccessTypeDefinition accessType = CreateJournalizeAccessType ();
      classDefinition.AddAccessType (accessType);
     
      return accessType;
    }

    public AccessTypeDefinition CreateJournalizeAccessType ()
    {
      return AccessTypeDefinition.NewObject  (_transaction, Guid.NewGuid (), "Journalize", 42);
    }

    public SecurityToken CreateEmptyToken ()
    {
      return CreateToken (null, null, null, null);
    }

    public SecurityToken CreateTokenWithOwningClient (User user, Client owningClient)
    {
      return CreateToken (user, owningClient, null, null);
    }

    public SecurityToken CreateTokenWithAbstractRole (params AbstractRoleDefinition[] roleDefinitions)
    {
      return CreateToken (null, null, null, roleDefinitions);
    }

    public SecurityToken CreateTokenWithOwningGroups (User user, params Group[] owningGroups)
    {
      return CreateToken (user, null, owningGroups, null);
    }

    public SecurityToken CreateToken (User user, Client owningClient, Group[] owningGroups, AbstractRoleDefinition[] abstractRoleDefinitions)
    {
      List<Group> owningGroupList = new List<Group> ();
      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();

      if (owningGroups != null)
        owningGroupList.AddRange (owningGroups);

      if (abstractRoleDefinitions != null)
        abstractRoles.AddRange (abstractRoleDefinitions);

      return new SecurityToken (user, owningClient, owningGroupList, abstractRoles);
    }

    public AbstractRoleDefinition CreateTestAbstractRole ()
    {
      return CreateAbstractRoleDefinition ("Test", 42);
    }

    public AbstractRoleDefinition CreateAbstractRoleDefinition (string name, int value)
    {
      return AbstractRoleDefinition.NewObject (_transaction, Guid.NewGuid (), name, value);
    }

    public AccessControlEntry CreateAceWithOwningClient ()
    {
      AccessControlEntry entry = AccessControlEntry.NewObject (_transaction);
      entry.Client = ClientSelection.OwningClient;

      return entry;
    }

    public AccessControlEntry CreateAceWithSpecficClient (Client client)
    {
      AccessControlEntry entry = AccessControlEntry.NewObject (_transaction);
      entry.Client = ClientSelection.SpecificClient;
      entry.SpecificClient = client;

      return entry;
    }

    public AccessControlEntry CreateAceWithAbstractRole ()
    {
      AccessControlEntry entry = AccessControlEntry.NewObject (_transaction);
      entry.SpecificAbstractRole = CreateTestAbstractRole ();

      return entry;
    }

    public AccessControlEntry CreateAceWithPosition (Position position, GroupSelection groupSelection)
    {
      AccessControlEntry entry = AccessControlEntry.NewObject (_transaction);
      entry.User = UserSelection.SpecificPosition;
      entry.SpecificPosition = position;
      entry.Group = groupSelection;

      return entry;
    }

    public AccessControlList CreateAcl (params AccessControlEntry[] aces)
    {
      AccessControlList acl = AccessControlList.NewObject (_transaction);

      foreach (AccessControlEntry ace in aces)
        acl.AccessControlEntries.Add (ace);

      return acl;
    }

    public void AttachAccessType (AccessControlEntry ace, AccessTypeDefinition accessType, bool? allowed)
    {
      ace.AttachAccessType (accessType);
      if (allowed.HasValue && allowed.Value)
        ace.AllowAccess (accessType);
    }

    public AccessTypeDefinition CreateReadAccessType (AccessControlEntry ace, bool? allowAccess)
    {
      return CreateAccessTypeForAce (ace, allowAccess, Guid.NewGuid (), "Read", 0);
    }

    public AccessTypeDefinition CreateWriteAccessType (AccessControlEntry ace, bool? allowAccess)
    {
      return CreateAccessTypeForAce (ace, allowAccess, Guid.NewGuid (), "Write", 1);
    }

    public AccessTypeDefinition CreateDeleteAccessType (AccessControlEntry ace, bool? allowAccess)
    {
      return CreateAccessTypeForAce (ace, allowAccess, Guid.NewGuid (), "Delete", 2);
    }

    public AccessTypeDefinition CreateAccessTypeForAce (AccessControlEntry ace, bool? allowAccess, Guid metadataItemID, string name, int value)
    {
      AccessTypeDefinition accessType = AccessTypeDefinition.NewObject  (_transaction, metadataItemID, name, value);
      AttachAccessType (ace, accessType, allowAccess);

      return accessType;
    }

    public Client CreateClient (string name)
    {
      Client client = _factory.CreateClient (_transaction);
      client.Name = name;

      return client;
    }

    public Group CreateGroup (string name, Group parent, Client client)
    {
      Group group = _factory.CreateGroup (_transaction);
      group.Name = name;
      group.Parent = parent;
      group.Client = client;

      return group;
    }

    public User CreateUser (string userName, string firstName, string lastName, string title, Group owningGroup, Client client)
    {
      User user = _factory.CreateUser (_transaction);
      user.UserName = userName;
      user.FirstName = firstName;
      user.LastName = lastName;
      user.Title = title;
      user.Client = client;
      user.OwningGroup = owningGroup;

      return user;
    }

    public Position CreatePosition (string name)
    {
      Position position = _factory.CreatePosition (_transaction);
      position.Name = name;

      return position;
    }

    public Role CreateRole (User user, Group group, Position position)
    {
      Role role = Role.NewObject (_transaction);
      role.User = user;
      role.Group = group;
      role.Position = position;

      return role;
    }
  }
}
