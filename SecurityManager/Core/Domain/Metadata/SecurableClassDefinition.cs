using System;
using System.Collections.Generic;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

using Rubicon.SecurityManager.Domain.AccessControl;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  public class SecurableClassDefinition : MetadataObject
  {
    // types

    // static members and constants

    public static SecurableClassDefinition FindByName (ClientTransaction transaction, string name)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      Query query = new Query ("Rubicon.SecurityManager.Domain.Metadata.SecurableClassDefinition.FindByName");
      query.Parameters.Add ("@name", name);

      DomainObjectCollection result = transaction.QueryManager.GetCollection (query);
      if (result.Count == 0)
        return null;

      return (SecurableClassDefinition) result[0];
    }

    public static DomainObjectCollection FindAll (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      Query query = new Query ("Rubicon.SecurityManager.Domain.Metadata.SecurableClassDefinition.FindAll");
      return transaction.QueryManager.GetCollection (query);
    }

    public static new SecurableClassDefinition GetObject (ObjectID id)
    {
      return (SecurableClassDefinition) DomainObject.GetObject (id);
    }

    public static new SecurableClassDefinition GetObject (ObjectID id, bool includeDeleted)
    {
      return (SecurableClassDefinition) DomainObject.GetObject (id, includeDeleted);
    }

    public static new SecurableClassDefinition GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (SecurableClassDefinition) DomainObject.GetObject (id, clientTransaction);
    }

    public static new SecurableClassDefinition GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (SecurableClassDefinition) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    private DomainObjectCollection _stateProperties;
    private DomainObjectCollection _accessTypes;

    // construction and disposing

    public SecurableClassDefinition (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected SecurableClassDefinition (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public SecurableClassDefinition BaseClass
    {
      get { return (SecurableClassDefinition) GetRelatedObject ("BaseClass"); }
      set { SetRelatedObject ("BaseClass", value); }
    }

    public DomainObjectCollection DerivedClasses
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("DerivedClasses"); }
      set { } // marks property DerivedClasses as modifiable
    }

    public DomainObjectCollection StateProperties
    {
      get
      {
        if (_stateProperties == null)
        {
          DomainObjectCollection stateProperties = new DomainObjectCollection ();

          foreach (StatePropertyReference propertyReference in StatePropertyReferences)
            stateProperties.Add (propertyReference.StateProperty);

          _stateProperties = new DomainObjectCollection (stateProperties, true);
        }

        return _stateProperties;
      }
    }

    public DomainObjectCollection AccessTypes
    {
      get
      {
        if (_accessTypes == null)
        {
          DomainObjectCollection accessTypes = new DomainObjectCollection ();

          foreach (AccessTypeReference accessTypeReference in AccessTypeReferences)
            accessTypes.Add (accessTypeReference.AccessType);

          _accessTypes = new DomainObjectCollection (accessTypes, true);
        }

        return _accessTypes;
      }
    }

    public DomainObjectCollection StateCombinations
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("StateCombinations"); }
      set { } // marks property StateCombinations as modifiable
    }

    public DomainObjectCollection AccessControlLists
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("AccessControlLists"); }
      set { } // marks property AccessControlLists as modifiable
    }

    public void AddAccessType (AccessTypeDefinition accessType)
    {
      AccessTypeReference reference = new AccessTypeReference (ClientTransaction);
      reference.AccessType = accessType;

      AccessTypeReferences.Add (reference);
      _accessTypes = null;
    }

    public void AddStateProperty (StatePropertyDefinition stateProperty)
    {
      StatePropertyReference reference = new StatePropertyReference (ClientTransaction);
      reference.StateProperty = stateProperty;

      StatePropertyReferences.Add (reference);
      _stateProperties = null;
    }

    public StateCombination FindStateCombination (List<StateDefinition> states)
    {
      foreach (StateCombination stateCombination in StateCombinations)
      {
        if (stateCombination.MatchesStates (states))
          return stateCombination;
      }

      return null;
    }

    private DomainObjectCollection StatePropertyReferences
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("StatePropertyReferences"); }
    }

    private DomainObjectCollection AccessTypeReferences
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("AccessTypeReferences"); }
    }

    //TODO: Rewrite with test
    public AccessControlList CreateAccessControlList ()
    {
      AccessControlList accessControlList = new AccessControlList (ClientTransaction);
      accessControlList.ClassDefinition = this;
      accessControlList.CreateStateCombination ();
      accessControlList.CreateAccessControlEntry ();

      return accessControlList;
    }
  }
}
