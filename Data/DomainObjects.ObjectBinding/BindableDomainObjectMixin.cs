using System;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
  [Serializable]
  [GetObjectServiceType (typeof (GetBindableDomainObjectService))]
  public class BindableDomainObjectMixin : BindableObjectMixinBase<DomainObject>, IBusinessObjectWithIdentity
  {
    protected override BindableObjectClass InitializeBindableObjectClass ()
    {
      return BindableObjectProvider.Current.GetBindableObjectClass (This.GetPublicDomainObjectType());
    }

    public string UniqueIdentifier
    {
      get { return This.ID.ToString(); }
    }
  }
}
