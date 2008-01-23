using System;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  [Serializable]
  public class BindingClientTransaction : RootClientTransaction
  {
    public BindingClientTransaction ()
    {
      AddListener (new BindingClientTransactionListener (this));
    }

    public override ClientTransaction CreateSubTransaction ()
    {
      throw new InvalidOperationException ("Binding transactions cannot have subtransactions.");
    }

    protected internal override bool DoEnlistDomainObject (DomainObject domainObject)
    {
      if (!domainObject.IsBoundToSpecificTransaction || domainObject.ClientTransaction != this)
      {
        string message =
            string.Format (
                "Cannot enlist the domain object {0} in this binding transaction, because it has originally been loaded in another transaction.",
                domainObject.ID);
        throw new InvalidOperationException (message);
      }
      return base.DoEnlistDomainObject (domainObject);
    }
  }
}