using System;

using Rubicon.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
public class DomainObjectClass: IBusinessObjectClassWithIdentity
{
  private BusinessObjectClassReflector _classReflector;

  public DomainObjectClass (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    DomainObjectPropertyFactory propertyFactory = new DomainObjectPropertyFactory (MappingConfiguration.Current.ClassDefinitions.GetMandatory [type]);
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
