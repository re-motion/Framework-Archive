using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Rubicon.Utilities;
using Rubicon;

namespace Rubicon.Security.Metadata
{

  public class ClassReflector : IClassReflector
  {
    // types

    // static members

    // member fields

    private IStatePropertyReflector _statePropertyReflector;
    private IAccessTypeReflector _accessTypeReflector;

    // construction and disposing

    public ClassReflector ()
      : this (new StatePropertyReflector (), new AccessTypeReflector ())
    {
    }

    public ClassReflector (IStatePropertyReflector statePropertyReflector, IAccessTypeReflector accessTypeReflector)
    {
      ArgumentUtility.CheckNotNull ("statePropertyReflector", statePropertyReflector);
      ArgumentUtility.CheckNotNull ("accessTypeReflector", accessTypeReflector);

      _statePropertyReflector = statePropertyReflector;
      _accessTypeReflector = accessTypeReflector;
    }

    // methods and properties

    public IStatePropertyReflector StatePropertyReflector
    {
      get { return _statePropertyReflector; }
    }

    public IAccessTypeReflector AccessTypeReflector
    {
      get { return _accessTypeReflector; }
    }

    public SecurableClassInfo GetMetadata (Type type, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (ISecurableObject));
      if (type.IsValueType)
        throw new ArgumentException ("Value types are not supported.", "type");
      ArgumentUtility.CheckNotNull ("cache", cache);

      SecurableClassInfo info = cache.GetSecurableClassInfo (type);
      if (info == null)
      {
        info = new SecurableClassInfo ();
        info.Name = TypeUtility.GetPartialAssemblyQualifiedName (type);
        PermanentGuidAttribute guidAttribute = (PermanentGuidAttribute) Attribute.GetCustomAttribute (type, typeof (PermanentGuidAttribute), true);
        if (guidAttribute != null)
          info.ID = guidAttribute.Value.ToString ();
        info.Properties.AddRange (GetProperties (type, cache));
        info.AccessTypes.AddRange (_accessTypeReflector.GetAccessTypesFromType (type, cache));

        cache.AddSecurableClassInfo (type, info);

        if (typeof (ISecurableObject).IsAssignableFrom (type.BaseType))
        {
          info.BaseClass = GetMetadata (type.BaseType, cache);
          info.BaseClass.DerivedClasses.Add (info);
        }
      }

      return info;
    }

    protected virtual List<StatePropertyInfo> GetProperties (Type type, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (ISecurableObject));
      ArgumentUtility.CheckNotNull ("cache", cache);

      MemberInfo[] propertyInfos = type.FindMembers (
          MemberTypes.Property,
          BindingFlags.Instance | BindingFlags.Public,
          FindStatePropertiesFilter,
          null);

      List<StatePropertyInfo> statePropertyInfos = new List<StatePropertyInfo> ();
      foreach (PropertyInfo propertyInfo in propertyInfos)
        statePropertyInfos.Add (_statePropertyReflector.GetMetadata (propertyInfo, cache));

      return statePropertyInfos;
    }

    protected bool FindStatePropertiesFilter (MemberInfo member, object filterCriteria)
    {
      PropertyInfo property = ArgumentUtility.CheckNotNullAndType<PropertyInfo> ("member", member);
      return property.PropertyType.IsEnum && Attribute.IsDefined (property.PropertyType, typeof (SecurityStateAttribute), false);
    }
  }
}