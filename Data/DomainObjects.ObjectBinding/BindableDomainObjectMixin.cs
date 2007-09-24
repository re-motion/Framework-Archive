using System;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
  [Serializable]
  [GetObjectServiceType (typeof (IGetBindableDomainObjectService))]
  public class BindableDomainObjectMixin : BindableObjectMixinBase<DomainObject>, IBusinessObjectWithIdentity
  {
    protected override BindableObjectClass InitializeBindableObjectClass ()
    {
      // TODO: Configure the BindableObjectClass to instantiate the right ClassReflectors and PropertyReflectors
      return BindableObjectProvider.Current.GetBindableObjectClass (This.GetPublicDomainObjectType());
    }

    public string UniqueIdentifier
    {
      get { return This.ID.ToString(); }
    }
  }
}
