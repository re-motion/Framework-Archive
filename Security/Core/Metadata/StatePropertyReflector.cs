using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Rubicon.Data;
using Rubicon.Utilities;

namespace Rubicon.Security.Metadata
{

  public class StatePropertyReflector : IStatePropertyReflector
  {
    // types

    // static members

    // member fields

    private IEnumerationReflector _enumeratedTypeReflector;

    // construction and disposing

    public StatePropertyReflector () : this (new EnumerationReflector())
    {
    }

    public StatePropertyReflector (IEnumerationReflector enumeratedTypeReflector)
    {
      ArgumentUtility.CheckNotNull ("enumeratedTypeReflector", enumeratedTypeReflector);
      _enumeratedTypeReflector = enumeratedTypeReflector;
    }

    // methods and properties

    public IEnumerationReflector EnumeratedTypeReflector
    {
      get { return _enumeratedTypeReflector; }
    }

    public StatePropertyInfo GetMetadata (PropertyInfo property, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      if (!property.PropertyType.IsEnum)
      {
        throw new ArgumentException (
            string.Format ("The type of the property '{0}' in type '{1}' is not an enumerated type.", property.Name, property.DeclaringType.FullName),
            "property");
      }

      if (!Attribute.IsDefined (property.PropertyType, typeof (SecurityStateAttribute), false))
      {
        throw new ArgumentException (string.Format ("The type of the property '{0}' in type '{1}' does not have the {2} applied.", 
                property.Name, property.DeclaringType.FullName, typeof (SecurityStateAttribute).FullName),
            "property");
      }

      ArgumentUtility.CheckNotNull ("cache", cache);

      StatePropertyInfo info = cache.GetStatePropertyInfo (property);
      if (info == null)
      {
        info = new StatePropertyInfo ();
        info.Name = property.Name;
        PermanentGuidAttribute guidAttribute = (PermanentGuidAttribute) Attribute.GetCustomAttribute (property, typeof (PermanentGuidAttribute), true);
        if (guidAttribute != null)
          info.ID = guidAttribute.Value;
        info.Values = _enumeratedTypeReflector.GetValues (property.PropertyType);

        cache.AddStatePropertyInfo (property, info);
      }
      return info;
    }
  }
}