using System;
using System.Diagnostics;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  [Serializable]
  [DebuggerDisplay ("{UniqueIdentifier} ({((Rubicon.Mixins.IMixinTarget)this).Configuration.Type.FullName})")]
  public abstract class BindableObjectWithIdentityMixin : BindableObjectMixin, IBusinessObjectWithIdentity
  {
    public BindableObjectWithIdentityMixin ()
    {
    }

    public abstract string UniqueIdentifier { get; }
  }
}