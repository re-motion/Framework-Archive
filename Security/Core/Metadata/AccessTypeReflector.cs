using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Rubicon.Security;
using Rubicon.Utilities;

namespace Rubicon.Security.Metadata
{

  public class AccessTypeReflector : Rubicon.Security.Metadata.IAccessTypeReflector
  {
    // types

    // static members

    // member fields
    private IEnumerationReflector _enumerationReflector;
    private PermissionReflector _permissionReflector = new PermissionReflector();

    // construction and disposing

    public AccessTypeReflector ()
      : this (new EnumerationReflector ())
    {
    }

    public AccessTypeReflector (IEnumerationReflector enumerationReflector)
    {
      ArgumentUtility.CheckNotNull ("enumerationReflector", enumerationReflector);
      _enumerationReflector = enumerationReflector;
    }

    // methods and properties

    public IEnumerationReflector EnumerationTypeReflector
    {
      get { return _enumerationReflector; }
    }

    public List<EnumValueInfo> GetAccessTypesFromAssembly (Assembly assembly, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      ArgumentUtility.CheckNotNull ("cache", cache);

      List<EnumValueInfo> accessTypes = new List<EnumValueInfo> ();
      foreach (Type type in assembly.GetTypes ())
      {
        if (type.IsEnum && Attribute.IsDefined (type, typeof (AccessTypeAttribute), false))
        {
          Dictionary<Enum, EnumValueInfo> values = _enumerationReflector.GetValues (type, cache);
          foreach (KeyValuePair<Enum, EnumValueInfo> entry in values)
          {
            if (!cache.ContainsAccessType (entry.Key))
              cache.AddAccessType (entry.Key, entry.Value);
            accessTypes.Add (entry.Value);
          }
        }
      }

      return accessTypes;
    }

    public List<EnumValueInfo> GetAccessTypesFromType (Type type, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("cache", cache);

      Dictionary<Enum, EnumValueInfo> accessTypes = _enumerationReflector.GetValues (typeof (GeneralAccessType), cache);
      foreach (KeyValuePair<Enum, EnumValueInfo> entry in accessTypes)
      {
        if (!cache.ContainsAccessType (entry.Key))
          cache.AddAccessType (entry.Key, entry.Value);
      }

      foreach (KeyValuePair<Enum, EnumValueInfo> entry in GetAccessTypesForMethods (type, cache))
      {
        if (!accessTypes.ContainsKey (entry.Key))
          accessTypes.Add (entry.Key, entry.Value);
      }

      return new List<EnumValueInfo> (accessTypes.Values);
    }


    private Dictionary<Enum, EnumValueInfo> GetAccessTypesForMethods (Type type, MetadataCache cache)
    {
      MemberInfo[] instanceMethods = GetInstanceMethods (type);
      MemberInfo[] staticMethods = GetStaticMethods (type);
      MemberInfo[] constructors = GetConstructors (type);

      MemberInfo[] memberInfos = (MemberInfo[]) ArrayUtility.Combine (instanceMethods, staticMethods, constructors);
      return GetAccessTypesFromRequiredMethodPermissions ((MethodBase[]) ArrayUtility.Convert (memberInfos, typeof (MethodBase)), cache);
    }

    private MemberInfo[] GetConstructors (Type type)
    {
      MemberInfo[] constructors = type.FindMembers (
          MemberTypes.Constructor,
          BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public,
          FindRequriedPermissionsFilter,
          null);
      return constructors;
    }

    private MemberInfo[] GetStaticMethods (Type type)
    {
      MemberInfo[] staticMethods = type.FindMembers (
          MemberTypes.Method,
          BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy,
          FindRequriedPermissionsFilter,
          null);
      return staticMethods;
    }

    private MemberInfo[] GetInstanceMethods (Type type)
    {
      MemberInfo[] instanceMethods = type.FindMembers (
          MemberTypes.Method,
          BindingFlags.Instance | BindingFlags.Public,
          FindRequriedPermissionsFilter,
          null);
      return instanceMethods;
    }

    private bool FindRequriedPermissionsFilter (MemberInfo member, object filterCriteria)
    {
      return Attribute.IsDefined (member, typeof (DemandMethodPermissionAttribute), true);
    }  

    private Dictionary<Enum, EnumValueInfo> GetAccessTypesFromRequiredMethodPermissions (MethodBase[] methodBases, MetadataCache cache)
    {
      Dictionary<Enum, EnumValueInfo> accessTypes = new Dictionary<Enum, EnumValueInfo> ();
      foreach (MethodBase methodbase in methodBases)
      {
        Enum[] values = _permissionReflector.GetRequiredMethodPermissions (methodbase);
        for (int i = 0; i < values.Length; i++)
        {
          Enum value = values[i];
          EnumValueInfo accessType = _enumerationReflector.GetValue (value, cache);

          if (!cache.ContainsAccessType (value))
            cache.AddAccessType (value, accessType);

          if (!accessTypes.ContainsKey (value))
            accessTypes.Add (value, accessType);
        }
      }
      return accessTypes;
    }
  }
}