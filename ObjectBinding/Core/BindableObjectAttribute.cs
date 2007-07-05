using System;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.ObjectBinding
{
  //TODO: doc
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class BindableObjectAttribute : UsesAttribute, IBindableObjectAttribute
  {
    public BindableObjectAttribute ()
        : base (typeof (BindableObjectMixin))
    {
    }
  }
}