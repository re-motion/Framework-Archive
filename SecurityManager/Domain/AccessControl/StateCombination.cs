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
public class StateCombination : AccessControlObject
{
  // types

  // static members and constants

  public static new StateCombination GetObject (ObjectID id)
  {
    return (StateCombination) DomainObject.GetObject (id);
  }

  public static new StateCombination GetObject (ObjectID id, bool includeDeleted)
  {
    return (StateCombination) DomainObject.GetObject (id, includeDeleted);
  }

  public static new StateCombination GetObject (ObjectID id, ClientTransaction clientTransaction)
  {
    return (StateCombination) DomainObject.GetObject (id, clientTransaction);
  }

  public static new StateCombination GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
  {
    return (StateCombination) DomainObject.GetObject (id, clientTransaction, includeDeleted);
  }

  // member fields

  // construction and disposing

  public StateCombination (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected StateCombination (DataContainer dataContainer) : base (dataContainer)
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

  public DomainObjectCollection StateUsages
  {
    get { return (DomainObjectCollection) GetRelatedObjects ("StateUsages"); }
    set { } // marks property StateUsages as modifiable
  }

  public AccessControlList AccessControlList
  {
    get { return (AccessControlList) GetRelatedObject ("AccessControlList"); }
    set { SetRelatedObject ("AccessControlList", value); }
  }

}
}
