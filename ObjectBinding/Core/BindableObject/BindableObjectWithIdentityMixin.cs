using System;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  [Serializable]
  public abstract class BindableObjectWithIdentityMixin : BindableObjectMixin, IBusinessObjectWithIdentity
  {
    public BindableObjectWithIdentityMixin ()
    {
    }

    public abstract string UniqueIdentifier { get; }
  }
}