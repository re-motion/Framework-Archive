using System;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class StateUsage : AccessControlObject
  {
    public static StateUsage NewObject (ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return DomainObject.NewObject<StateUsage> ().With ();
      }
    }

    protected StateUsage ()
    {
    }

    [DBBidirectionalRelation ("Usages")]
    [Mandatory]
    public abstract StateDefinition StateDefinition { get; set; }

    [DBBidirectionalRelation ("StateUsages")]
    [Mandatory]
    public abstract StateCombination StateCombination { get; set; }

    protected override void OnCommitting (EventArgs args)
    {
      base.OnCommitting (args);

      if (StateCombination != null && StateCombination.Class != null)
        StateCombination.Class.Touch ();
    }
  }
}
