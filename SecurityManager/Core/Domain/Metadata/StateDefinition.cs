using System;
using System.ComponentModel;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.AccessControl;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  public abstract class StateDefinition : EnumValueDefinition
  {
    // types

    // static members and constants

    public static StateDefinition NewObject (ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return NewObject<StateDefinition> ().With ();
      }
    }

        public static StateDefinition NewObject (ClientTransaction clientTransaction, string name, int value)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return NewObject<StateDefinition> ().With (name, value);
      }
    }

    public static new StateDefinition GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (StateDefinition) DomainObject.GetObject (id, clientTransaction);
    }

    // member fields

    // construction and disposing

    protected StateDefinition ()
    {
    }

    protected StateDefinition (string name, int value)
    {
      Name = name;
      Value = value;
    }

    // methods and properties

    [DBBidirectionalRelation ("DefinedStates")]
    [Mandatory]
    public abstract StatePropertyDefinition StateProperty { get; set; }

    public override sealed Guid MetadataItemID
    {
      get { throw new NotSupportedException ("States do not support MetadataItemID"); }
      set { throw new NotSupportedException ("States do not support MetadataItemID"); }
    }

    //TODO: Rename to StateUsages
    [EditorBrowsable( EditorBrowsableState.Never)]
    [DBBidirectionalRelation ("StateDefinition")]
    protected abstract ObjectList<StateUsage> Usages { get; }
  }
}
