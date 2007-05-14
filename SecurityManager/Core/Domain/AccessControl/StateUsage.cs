using System;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  [Serializable]
  public class StateUsage : AccessControlObject
  {
    // types

    // static members and constants

    public static new StateUsage GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (StateUsage) DomainObject.GetObject (id, clientTransaction);
    }

    public static new StateUsage GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (StateUsage) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public StateUsage (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected StateUsage (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public StateDefinition StateDefinition
    {
      get { return (StateDefinition) GetRelatedObject ("StateDefinition"); }
      set { SetRelatedObject ("StateDefinition", value); }
    }

    public StateCombination StateCombination
    {
      get { return (StateCombination) GetRelatedObject ("StateCombination"); }
      set { SetRelatedObject ("StateCombination", value); }
    }

    protected override void OnCommitting (EventArgs args)
    {
      base.OnCommitting (args);

      if (StateCombination != null && StateCombination.Class != null)
        StateCombination.Class.Touch ();
    }
  }
}
