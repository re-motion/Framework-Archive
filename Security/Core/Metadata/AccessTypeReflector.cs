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

    public List<EnumValueInfo> GetAccessTypes (Type type, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("cache", cache);

      Dictionary<Enum, EnumValueInfo> accessTypes = _enumerationReflector.GetValues (typeof (GeneralAccessType), cache);
      foreach (KeyValuePair<Enum, EnumValueInfo> entry in accessTypes)
      {
        if (!cache.ContainsAccessType (entry.Key))
          cache.AddAccessType (entry.Key, entry.Value);
      }

      return new List<EnumValueInfo> (accessTypes.Values);
    }
  }
}