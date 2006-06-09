using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
[Serializable]
public class AccessControlList : AccessControlObject
{
  // types

  // static members and constants

  public static new AccessControlList GetObject (ObjectID id)
  {
    return (AccessControlList) DomainObject.GetObject (id);
  }

  public static new AccessControlList GetObject (ObjectID id, bool includeDeleted)
  {
    return (AccessControlList) DomainObject.GetObject (id, includeDeleted);
  }

  public static new AccessControlList GetObject (ObjectID id, ClientTransaction clientTransaction)
  {
    return (AccessControlList) DomainObject.GetObject (id, clientTransaction);
  }

  public static new AccessControlList GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
  {
    return (AccessControlList) DomainObject.GetObject (id, clientTransaction, includeDeleted);
  }

  // member fields

  // construction and disposing

  public AccessControlList (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected AccessControlList (DataContainer dataContainer) : base (dataContainer)
  {
  // This infrastructure constructor is necessary for the DomainObjects framework.
  // Do not remove the constructor or place any code here.
  }

  // methods and properties

  public SecurableClassDefinition ClassDefinition
  {
    get { return (SecurableClassDefinition) GetRelatedObject ("ClassDefinition"); }
    set { SetRelatedObject ("ClassDefinition", value); }
  }

  public DomainObjectCollection StateCombinations
  {
    get { return (DomainObjectCollection) GetRelatedObjects ("StateCombinations"); }
    set { } // marks property StateCombinations as modifiable
  }

  public DomainObjectCollection AccessControlEntries
  {
    get { return (DomainObjectCollection) GetRelatedObjects ("AccessControlEntries"); }
    set { } // marks property AccessControlEntries as modifiable
  }

}
}
