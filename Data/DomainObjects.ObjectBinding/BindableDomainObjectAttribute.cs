using System;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
  [AttributeUsage (AttributeTargets.Class)]
  public class BindableDomainObjectAttribute : UsesAttribute
  {
    public BindableDomainObjectAttribute ()
        : base (typeof (BindableDomainObjectMixin))
    {
    }
  }
}