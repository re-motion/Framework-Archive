using System;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Utilities.Serialization
{
  [Serializable]
  public class PropertyInfoFixupData
  {
    private const BindingFlags _bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    private string _typeName;
    private string _propertyName;
    private string _returnTypeName;
    private string[] _argumentTypeNames;

    public PropertyInfoFixupData (PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      _typeName = property.DeclaringType.AssemblyQualifiedName;
      _propertyName = property.Name;
      ParameterInfo[] parameters = property.GetIndexParameters();
      _argumentTypeNames = new string[parameters.Length];
      for (int i = 0; i < parameters.Length; ++i)
        _argumentTypeNames[i] = parameters[i].ParameterType.AssemblyQualifiedName;
      _returnTypeName = property.PropertyType.AssemblyQualifiedName;
    }

    public PropertyInfo GetPropertyInfo ()
    {
      Type declaringType = Type.GetType (_typeName);
      Type[] argumentTypes = new Type[_argumentTypeNames.Length];
      for (int i = 0; i < _argumentTypeNames.Length; ++i)
      {
        argumentTypes[i] = Type.GetType (_argumentTypeNames[i]);
      }
      Type returnType = Type.GetType (_returnTypeName);
      PropertyInfo property = declaringType.GetProperty (_propertyName, _bindingFlags, null, returnType, argumentTypes, null);
      Assertion.Assert (property != null);
      return property;
    }

    internal static object PreparePropertyInfo (object data)
    {
      PropertyInfo propertyInfo = data as PropertyInfo;
      if (propertyInfo == null)
        throw new ArgumentException ("Invalid data object - PropertyInfo expected.", "data");
      return new PropertyInfoFixupData (propertyInfo);
    }

    internal static object FixupPropertyInfo (object data)
    {
      PropertyInfoFixupData fixupData = data as PropertyInfoFixupData;
      if (fixupData == null)
        throw new ArgumentException ("Invalid data object - PropertyInfoFixupData expected.", "data");
      return fixupData.GetPropertyInfo ();
    }

  }
}