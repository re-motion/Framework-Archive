using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Reflection
{

public class ReflectionBusinessObjectClass: IBusinessObjectClassWithIdentity
{
  Type _type;

  public ReflectionBusinessObjectClass (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    _type = type;
  }

  public IBusinessObjectProperty GetPropertyDefinition (string propertyIdentifier)
  {
    PropertyInfo propertyInfo = _type.GetProperty (propertyIdentifier);
    return (propertyInfo == null) ? null : ReflectionBusinessObjectProperty.Create (propertyInfo);
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

      properties.Add (ReflectionBusinessObjectProperty.Create (propertyInfo));
    }

    return (IBusinessObjectProperty[]) properties.ToArray (typeof (IBusinessObjectProperty));
  }

  public IBusinessObjectProvider BusinessObjectProvider 
  {
    get { return ReflectionBusinessObjectProvider.Instance; }
  }

  public IBusinessObjectWithIdentity GetObject (string identifier)
  {
    Guid id = Guid.Empty;
    if (! StringUtility.IsNullOrEmpty (identifier))
      id = new Guid (identifier);

    return ReflectionBusinessObjectStorage.GetObject (_type, id);
  }

  public bool RequiresWriteBack
  { 
    get { return false; }
  }
}

}
