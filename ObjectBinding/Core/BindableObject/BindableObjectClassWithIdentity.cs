using System;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public class BindableObjectClassWithIdentity : BindableObjectClass, IBusinessObjectClassWithIdentity
  {
    internal static bool HasMixin (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);

      return BindableObjectMixin.HasMixin (targetType, typeof (BindableObjectWithIdentityMixin));
    }

    internal static bool IncludesMixin (Type concreteType)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);

      return BindableObjectMixin.IncludesMixin (concreteType, typeof (BindableObjectWithIdentityMixin));
    }
    
    private readonly Type _getObjectServiceType;

    public BindableObjectClassWithIdentity (Type type, BindableObjectProvider businessObjectProvider)
        : base (type, businessObjectProvider)
    {
      _getObjectServiceType = GetGetObjectServiceType();
    }

    public IBusinessObjectWithIdentity GetObject (string uniqueIdentifier)
    {
      IGetObjectService service = GetGetObjectService();
      return service.GetObject (this, uniqueIdentifier);
    }

    private IGetObjectService GetGetObjectService ()
    {
      IGetObjectService service = (IGetObjectService) BusinessObjectProvider.GetService (_getObjectServiceType);
      if (service == null)
      {
        throw new InvalidOperationException (
            string.Format (
                "The '{0}' required for loading objectes of type '{1}' is not registered with the '{2}'.",
                _getObjectServiceType.FullName,
                Type.FullName,
                BusinessObjectProvider.GetType().FullName));
      }
      return service;
    }

    private Type GetGetObjectServiceType ()
    {
      GetObjectServiceTypeAttribute attribute = AttributeUtility.GetCustomAttribute<GetObjectServiceTypeAttribute> (Type, true);
      if (attribute == null)
        return typeof (IGetObjectService);
      return attribute.Type;
    }
  }
}