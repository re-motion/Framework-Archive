using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.UnitTests.TestDomain;

namespace Rubicon.SecurityManager.Domain.UnitTests.AccessControl
{
  public class AccessControlTestHelper
  {
    public const int OrderClassPropertyCount = 2;

    private ClientTransaction _transaction;

    public AccessControlTestHelper ()
    {
      _transaction = new ClientTransaction ();
    }

    public ClientTransaction Transaction
    {
      get { return _transaction; }
    }

    public SecurableClassDefinition CreateOrderClassDefinition ()
    {
      return CreateClassDefinition ("Rubicon.SecurityManager.Domain.UnitTests.TestDomain.Order");
    }

    public SecurableClassDefinition CreateInvoiceClassDefinition ()
    {
      return CreateClassDefinition ("Rubicon.SecurityManager.Domain.UnitTests.TestDomain.Invoice");
    }

    public SecurableClassDefinition CreateClassDefinition (string name)
    {
      SecurableClassDefinition classDefinition = new SecurableClassDefinition (_transaction);
      classDefinition.Name = name;

      return classDefinition;
    }

    public SecurableClassDefinition CreateOrderClassDefinitionWithProperties ()
    {
      SecurableClassDefinition classDefinition = CreateOrderClassDefinition ();
      StatePropertyDefinition orderStateProperty = CreateOrderStateProperty (classDefinition);
      StatePropertyDefinition paymentStateProperty = CreatePaymentStateProperty (classDefinition);

      return classDefinition;
    }

    public AccessControlList CreateAccessControlList (SecurableClassDefinition classDefinition, params StateDefinition[] states)
    {
      AccessControlList acl = new AccessControlList (_transaction);
      acl.ClassDefinition = classDefinition;
      StateCombination stateCombination = CreateStateCombination (acl);

      foreach (StateDefinition state in states)
        stateCombination.AttachState (state);

      return acl;
    }

    public StateCombination CreateStateCombination (AccessControlList acl)
    {
      StateCombination stateCombination = new StateCombination (_transaction);
      stateCombination.AccessControlList = acl;
      stateCombination.ClassDefinition = acl.ClassDefinition;

      return stateCombination;
    }

    public StateCombination CreateStateCombination (SecurableClassDefinition classDefinition, params StateDefinition[] states)
    {
      AccessControlList acl = CreateAccessControlList (classDefinition, states);
      return (StateCombination) acl.StateCombinations[0];
    }

    public StatePropertyDefinition CreateStateProperty (string name)
    {
      return new StatePropertyDefinition (_transaction, Guid.NewGuid (), name);
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

    public List<AccessControlList> CreateAccessControlLists (SecurableClassDefinition classDefinition)
    {
      StatePropertyDefinition orderState = CreateOrderStateProperty (classDefinition);
      StatePropertyDefinition paymentState = CreatePaymentStateProperty (classDefinition);

      List<AccessControlList> acls = new List<AccessControlList> ();
      acls.Add (CreateAccessControlList (classDefinition, orderState["Received"], paymentState["None"]));
      acls.Add (CreateAccessControlList (classDefinition, orderState["Received"], paymentState["Paid"]));
      acls.Add (CreateAccessControlList (classDefinition, orderState["Delivered"], paymentState["None"]));
      acls.Add (CreateAccessControlList (classDefinition, orderState["Delivered"], paymentState["Paid"]));
      acls.Add (CreateAccessControlList (classDefinition));

      return acls;
    }

    public AccessControlList GetAclForDeliveredAndUnpaidStates (SecurableClassDefinition classDefinition)
    {
      List<AccessControlList> acls = CreateAccessControlLists (classDefinition);
      return acls[2];
    }

    public AccessControlList GetAclForStateless (SecurableClassDefinition classDefinition)
    {
      List<AccessControlList> acls = CreateAccessControlLists (classDefinition);
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

    public void AttachAccessType (SecurableClassDefinition classDefinition, Guid metadataItemID, string name, long value)
    {
      AccessTypeDefinition accessType = new AccessTypeDefinition (_transaction, metadataItemID, name, value);
      classDefinition.AddAccessType (accessType);
    }

    public void AttachJournalizeAccessType (SecurableClassDefinition classDefinition)
    {
      classDefinition.AddAccessType (CreateJournalizeAccessType ());
    }

    public AccessTypeDefinition CreateJournalizeAccessType ()
    {
      return new AccessTypeDefinition (_transaction, Guid.NewGuid (), "Journalize", 42);
    }
  }
}
