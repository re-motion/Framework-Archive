using System;

using Rubicon.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
// TODO Doc: 
public class DomainObjectClass: IBusinessObjectClassWithIdentity
{
  private BusinessObjectClassReflector _classReflector;

  // TODO Doc: I do not understand this code
  public DomainObjectClass (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    ReflectionPropertyFactory propertyFactory = null;
    if (type == typeof (BindableDomainObject))
      propertyFactory = new ReflectionPropertyFactory ();
    else
      propertyFactory = new DomainObjectPropertyFactory (MappingConfiguration.Current.ClassDefinitions.GetMandatory (type));

    _classReflector = new BusinessObjectClassReflector (type, propertyFactory);
  }

  public IBusinessObjectProperty GetPropertyDefinition (string propertyIdentifier)
  {
    return _classReflector.GetPropertyDefinition (propertyIdentifier);
  }

  public IBusinessObjectProperty[] GetPropertyDefinitions()
  {
    return _classReflector.GetPropertyDefinitions ();
  }

  public IBusinessObjectProvider BusinessObjectProvider 
  {
    get { return DomainObjectProvider.Instance; }
  }

  public IBusinessObjectWithIdentity GetObject (string identifier)
  {
    return BindableDomainObject.GetObject (ObjectID.Parse (identifier));
  }

  public bool RequiresWriteBack
  { 
    get { return false; }
  }

  public string Identifier
  {
    get { return _classReflector.Identifier; }
  }
}
}
