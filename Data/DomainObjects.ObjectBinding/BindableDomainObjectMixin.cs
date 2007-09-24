using System;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
  class BindableDomainObjectMixin : BindableObjectMixinBase<DomainObject>
  {
    protected override BindableObjectClass InitializeBindableObjectClass ()
    {
      throw new NotImplementedException();
    }
  }
}
