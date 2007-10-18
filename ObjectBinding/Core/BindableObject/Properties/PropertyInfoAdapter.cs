using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject.Properties
{
  public class PropertyInfoAdapter : IPropertyInformation
  {
    public static IEnumerable<IPropertyInformation> AdaptCollection (IEnumerable<PropertyInfo> infos)
    {
      foreach (PropertyInfo info in infos)
        yield return new PropertyInfoAdapter (info);
    }

    public static IEnumerable<PropertyInfo> UnwrapCollection (IEnumerable<IPropertyInformation> adapters)
    {
      foreach (PropertyInfoAdapter adapter in adapters)
        yield return adapter.PropertyInfo;
    }

    // Checks whether the property has private virtual final accessors. This is used as a heuristic to find explicit interface properties without
    // having to check InterfaceMaps for every interface on info.DeclaringType. With C# and VB.NET, the heuristic should be alright.
    private static bool GuessIsExplicitInterfaceProperty (PropertyInfo info)
    {
      foreach (MethodInfo accessor in info.GetAccessors (true))
      {
        if (accessor.IsPrivate && accessor.IsVirtual && accessor.IsFinal)
          return true;
      }
      return false;
    }

    private readonly PropertyInfo _propertyInfo;
    private readonly bool _isExplicitInterfaceProperty;

    public PropertyInfoAdapter (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      _propertyInfo = propertyInfo;
      _isExplicitInterfaceProperty = GuessIsExplicitInterfaceProperty (propertyInfo);
    }

    public PropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
    }

    public Type PropertyType
    {
      get { return _propertyInfo.PropertyType; }
    }

    public string Name
    {
      get { return _propertyInfo.Name; }
    }

    public Type DeclaringType
    {
      get { return _propertyInfo.DeclaringType; }
    }

    public Type GetOriginalDeclaringType ()
    {
      return ReflectionUtility.GetOriginalDeclaringType (_propertyInfo);
    }

    public bool CanBeSetFromOutside
    {
      get
      {
        if (_isExplicitInterfaceProperty)
          return _propertyInfo.GetSetMethod (true) != null; // for explicit interface properties, we allow for private setters
        else
          return _propertyInfo.GetSetMethod (false) != null; // for normal properties, we want a public setter
      }
    }

    public T GetCustomAttribute<T> (bool inherited) where T: class
    {
      return AttributeUtility.GetCustomAttribute<T> (_propertyInfo, inherited);
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T: class
    {
      return AttributeUtility.GetCustomAttributes<T> (_propertyInfo, inherited);
    }

    public bool IsDefined<T> (bool inherited) where T: class
    {
      return AttributeUtility.IsDefined<T> (_propertyInfo, inherited);
    }

    public object GetValue (object instance, object[] indexParameters)
    {
      return _propertyInfo.GetValue (instance, indexParameters);
    }

    public void SetValue (object instance, object value, object[] indexParameters)
    {
      _propertyInfo.SetValue (instance, value, indexParameters);
    }

    public override bool Equals (object obj)
    {
      PropertyInfoAdapter other = obj as PropertyInfoAdapter;
      return other != null && _propertyInfo.Equals (other._propertyInfo);
    }

    public override int GetHashCode ()
    {
      return _propertyInfo.GetHashCode ();
    }
  }
}