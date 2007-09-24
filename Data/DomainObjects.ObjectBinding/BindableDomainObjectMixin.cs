using System;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
  [Serializable]
  public class BindableDomainObjectMixin : BindableObjectMixinBase<DomainObject>
  {
    protected override BindableObjectClass InitializeBindableObjectClass ()
    {
      // TODO: Configure the BindableObjectClass to instantiate the right ClassReflectors and PropertyReflectors
      return BindableObjectProvider.Current.GetBindableObjectClass (Configuration.TargetClass.Type);
    }
  }
}
