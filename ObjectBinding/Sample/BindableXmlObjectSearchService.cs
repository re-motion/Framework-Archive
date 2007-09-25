using System;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Sample
{
  public class BindableXmlObjectSearchService : ISearchAvailableObjectsService
  {
    public BindableXmlObjectSearchService ()
    {
    }

    public bool SupportsIdentity (IBusinessObjectReferenceProperty property)
    {
      return true;
    }

    public IBusinessObject[] Search (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property, string searchStatement)
    {
      ReferenceProperty referenceProperty = ArgumentUtility.CheckNotNullAndType<ReferenceProperty> ("property", property);
      BindableObjectClass bindableObjectClass = (BindableObjectClass) referenceProperty.ReferenceClass;

      return (IBusinessObject[]) ArrayUtility.Convert (
                                     XmlReflectionBusinessObjectStorageProvider.Current.GetObjects (bindableObjectClass.TargetType),
                                     typeof (IBusinessObject));
    }
  }
}