using System;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
  [Serializable]
  [GetObjectServiceType (typeof (BindableDomainObjectGetObjectService))]
  [SearchAvailableObjectsServiceType (typeof (BindableDomainObjectSearchService))]
  [UseBindableDomainObjectMetadataFactory]
  public class BindableDomainObjectMixin : BindableObjectMixinBase<BindableDomainObjectMixin.IDomainObject>, IBusinessObjectWithIdentity
  {
    public interface IDomainObject
    {
      Type GetPublicDomainObjectType ();
      ObjectID ID { get; }
      PropertyIndexer Properties { get; }
      StateType State { get; }
    }

    protected override BindableObjectClass InitializeBindableObjectClass ()
    {
      return BindableObjectProvider.Current.GetBindableObjectClass (This.GetPublicDomainObjectType());
    }

    public string UniqueIdentifier
    {
      get { return This.ID.ToString(); }
    }

    public string BaseDisplayName
    {
      get { return base.DisplayName; }
    }

    protected override bool IsDefaultValue (PropertyBase property, object nativeValue)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      if (This.State != StateType.New)
        return false;
      else
      {
        string propertyIdentifier = ReflectionUtility.GetPropertyName (property.PropertyInfo.GetOriginalDeclaringType(), property.PropertyInfo.Name);
        if (This.Properties.Contains (propertyIdentifier))
          return !This.Properties[propertyIdentifier].HasBeenTouched;
        else
          return base.IsDefaultValue (property, nativeValue);
      }
    }
  }
}
