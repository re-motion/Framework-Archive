using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using Rubicon.Utilities;

using Rubicon.ObjectBinding;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{

public class DomainObjectClass: IBusinessObjectClassWithIdentity
{
  Type _type;
  ClassDefinition _classDefinition;

  public DomainObjectClass (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    _type = type;

    _classDefinition = MappingConfiguration.Current.ClassDefinitions[_type];
  }

  public IBusinessObjectProperty GetPropertyDefinition (string propertyIdentifier)
  {
    PropertyInfo propertyInfo = _type.GetProperty (propertyIdentifier);
    return (propertyInfo == null) ? null : DomainObjectProperty.Create (
        propertyInfo, _classDefinition);
  }

  public IBusinessObjectProperty[] GetPropertyDefinitions()
  {
    PropertyInfo[] propertyInfos = _type.GetProperties ();
    if (propertyInfos == null)
      return new IBusinessObjectProperty[0];

    ArrayList properties = new ArrayList();
    for (int i = 0; i < propertyInfos.Length; ++i)
    {
      PropertyInfo propertyInfo = propertyInfos[i];
      EditorBrowsableAttribute[] editorBrowsableAttributes = (EditorBrowsableAttribute[]) propertyInfo.GetCustomAttributes (typeof (EditorBrowsableAttribute), true);
      if (editorBrowsableAttributes.Length == 1)
      {
        EditorBrowsableAttribute editorBrowsableAttribute = editorBrowsableAttributes[0];
        if (editorBrowsableAttribute.State == EditorBrowsableState.Never)
          continue;
      }

      //  Prevents the display of the indexers declared in BusinessObject.
      //  Adding "EditorBrowsable (EditorBrowsableState.Never)" to BusinessObject 
      //  might not be the best solution until the final way of hiding properties is established
      if (propertyInfo.Name == "Item")
        continue;

      properties.Add (DomainObjectProperty.Create (
          propertyInfo, _classDefinition));
    }

    return (IBusinessObjectProperty[]) properties.ToArray (typeof (IBusinessObjectProperty));
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
    get { return _type.FullName; }
  }
}

}
