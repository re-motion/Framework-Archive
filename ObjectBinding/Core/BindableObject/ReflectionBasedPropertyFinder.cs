using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  public class ReflectionBasedPropertyFinder : IPropertyFinder
  {
    private readonly Type _concreteType;
    private readonly Dictionary<MethodInfo, MethodInfo> _interfaceMethodImplementations; // from implementation to declaration

    public ReflectionBasedPropertyFinder (Type concreteType)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      _concreteType = concreteType;

      _interfaceMethodImplementations = GetInterfaceMethodImplementationCache ();
    }

    private Dictionary<MethodInfo, MethodInfo> GetInterfaceMethodImplementationCache ()
    {
      Dictionary<MethodInfo, MethodInfo> cache = new Dictionary<MethodInfo, MethodInfo> ();
      foreach (Type currentType in GetInheritanceHierarchy ())
      {
        foreach (Type interfaceType in currentType.GetInterfaces ())
        {
          InterfaceMapping mapping = currentType.GetInterfaceMap (interfaceType);
          for (int i = 0; i < mapping.TargetMethods.Length; ++i)
            cache.Add (mapping.TargetMethods[i], mapping.InterfaceMethods[i]);
        }
      }
      return cache;
    }

    public IEnumerable<PropertyInfo> GetPropertyInfos ()
    {
      PropertyInfoCollection propertyInfos = new PropertyInfoCollection ();
      
      foreach (Type currentType in GetInheritanceHierarchy ())
      {
        foreach (PropertyInfo propertyInfo in currentType.FindMembers (MemberTypes.Property,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly, PropertyFilter, null))
        {
          if (!propertyInfos.Contains (propertyInfo.Name))
            propertyInfos.Add (propertyInfo);
        }
      }
      return propertyInfos;
    }

    private IEnumerable<Type> GetInheritanceHierarchy ()
    {
      for (Type currentType = _concreteType; currentType != null; currentType = currentType.BaseType)
        yield return currentType;
    }

    //OPF Mapping
    protected virtual bool PropertyFilter (MemberInfo memberInfo, object filterCriteria)
    {
      ObjectBindingAttribute attribute = AttributeUtility.GetCustomAttribute<ObjectBindingAttribute> (memberInfo, true);
      if (attribute != null && !attribute.Visible)
        return false;

      PropertyInfo propertyInfo = (PropertyInfo) memberInfo;

      // property can be an explicit interface implementation or property must have a public getter
      if (HasPublicGetter (propertyInfo))
        return true;
      else return 
        IsNonInfrastructureInterfaceProperty (propertyInfo);
    }

    private bool IsNonInfrastructureInterfaceProperty (PropertyInfo propertyInfo)
    {
      MethodInfo accessor = propertyInfo.GetGetMethod (true) ?? propertyInfo.GetSetMethod (true);
      if (accessor == null || !_interfaceMethodImplementations.ContainsKey (accessor))
        return false;
      else
        return !IsInfrastructureProperty (propertyInfo, accessor, _interfaceMethodImplementations[accessor]);
    }

    protected virtual bool IsInfrastructureProperty (PropertyInfo propertyInfo, MethodInfo accessor, MethodInfo interfaceAccessorDeclaration)
    {
      return interfaceAccessorDeclaration.DeclaringType.Assembly == typeof (IBusinessObject).Assembly
          || interfaceAccessorDeclaration.DeclaringType.Assembly == typeof (BindableObjectClass).Assembly
          || interfaceAccessorDeclaration.DeclaringType.Assembly == typeof (Mixins.IMixinTarget).Assembly;
    }

    private bool HasPublicGetter (PropertyInfo propertyInfo)
    {
      return propertyInfo.GetGetMethod (false) != null;
    }
  }
}