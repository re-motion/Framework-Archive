using System;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.ObjectBinding
{
  //TODO: doc
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class BindableObjectWithIdentityAttribute : UsesAttribute, IBindableObjectAttribute
  {
    public BindableObjectWithIdentityAttribute ()
        : base (typeof (BindableObjectWithIdentityMixin))
    {
    }
  }
}