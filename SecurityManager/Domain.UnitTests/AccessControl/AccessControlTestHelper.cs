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
    private ClientTransaction _transaction;

    public AccessControlTestHelper ()
    {
      _transaction = new ClientTransaction ();
    }

    public ClientTransaction Transaction
    {
      get { return _transaction; }
    }

    public void AttachPropertyToClass (SecurableClassDefinition classDefinition, StatePropertyDefinition stateProperty)
    {
      StatePropertyReference propertyReference = new StatePropertyReference (_transaction);
      propertyReference.StateProperty = stateProperty;
      classDefinition.StatePropertyReferences.Add (propertyReference);
    }

    public SecurableClassDefinition CreateSecurableClassDefinition ()
    {
      SecurableClassDefinition classDefinition = new SecurableClassDefinition (_transaction);
      classDefinition.Name = "Rubicon.SecurityManager.Domain.UnitTests.TestDomain.Order";

      return classDefinition;
    }

    public SecurableClassDefinition CreateClassDefinitionWithProperties ()
    {
      SecurableClassDefinition classDefinition = CreateSecurableClassDefinition ();
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
      AttachPropertyToClass (classDefinition, orderStateProperty);

      return orderStateProperty;
    }

    public StatePropertyDefinition CreatePaymentStateProperty (SecurableClassDefinition classDefinition)
    {
      StatePropertyDefinition paymentStateProperty = CreateStateProperty ("Payment");
      paymentStateProperty.AddState ("None", 0);
      paymentStateProperty.AddState ("Paid", 1);
      AttachPropertyToClass (classDefinition, paymentStateProperty);

      return paymentStateProperty;
    }

    public List<StateCombination> CreateStateCombinations ()
    {
      SecurableClassDefinition classDefinition = CreateSecurableClassDefinition ();
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
  }
}
