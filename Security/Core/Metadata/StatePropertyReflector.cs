using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Rubicon.Data;
using Rubicon.Utilities;

namespace Rubicon.Security.Metadata
{

  public class StatePropertyReflector
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public StatePropertyReflector ()
    {
    }

    // methods and properties

    public StatePropertyInfo GetMetadata (PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      if (!property.PropertyType.IsEnum)
      {
        throw new ArgumentException (string.Format ("The property '{0}' of type '{1}' is not of an enumerated type.", property.Name, property.DeclaringType.FullName),
            "property");
      }

      if (!Attribute.IsDefined (property.PropertyType, typeof (SecurityStateAttribute), false))
      {
        throw new ArgumentException (string.Format ("The property '{0}' of type '{1}' does not have the {2} applied.", 
                property.Name, property.DeclaringType.FullName, typeof (SecurityStateAttribute).FullName),
            "property");
      }

      StatePropertyInfo info = new StatePropertyInfo ();
      info.Name = property.Name;
      PermanentGuidAttribute guidAttribute = (PermanentGuidAttribute) Attribute.GetCustomAttribute (property, typeof (PermanentGuidAttribute), true);
      if (guidAttribute != null)
        info.ID = guidAttribute.Value;
      info.Values = GetValues (property.PropertyType);

      return info;
    }

    private List<EnumValueInfo> GetValues (Type enumType)
    {
      System.Collections.IList values = Enum.GetValues (enumType);
      string[] names = Enum.GetNames (enumType);

      List<EnumValueInfo> enumValueInfos = new List<EnumValueInfo> ();
      for (int i = 0; i < names.Length; i++)
        enumValueInfos.Add (new EnumValueInfo ((int) values[i], names[i]));
      
      return enumValueInfos;
    }
  }
}