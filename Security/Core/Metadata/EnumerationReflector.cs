using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Data;
using System.Reflection;

namespace Rubicon.Security.Metadata
{

  public class EnumerationReflector : IEnumerationReflector
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public EnumerationReflector ()
    {
    }

    // methods and properties

    public Dictionary<Enum, EnumValueInfo> GetValues (Type type, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      if (!type.IsEnum)
        throw new ArgumentException (string.Format ("The type '{0}' is not an enumerated type.", type.FullName), "type");
      ArgumentUtility.CheckNotNull ("cache", cache);

      System.Collections.IList values = Enum.GetValues (type);
      string[] names = Enum.GetNames (type);

      Dictionary<Enum, EnumValueInfo> enumValueInfos = new Dictionary<Enum, EnumValueInfo> ();
      for (int i = 0; i < values.Count; i++)
      {
        EnumValueInfo info = cache.GetEnumValueInfo ((Enum) values[i]);
        if (info == null)
        {
          info = new EnumValueInfo ((int) values[i], names[i]);
          FieldInfo fieldInfo = type.GetField (names[i], BindingFlags.Static | BindingFlags.Public);
          PermanentGuidAttribute attribute = (PermanentGuidAttribute) Attribute.GetCustomAttribute (fieldInfo, typeof (PermanentGuidAttribute), false);
          if (attribute != null)
            info.ID = attribute.Value;

          cache.AddEnumValueInfo ((Enum) values[i], info);
        }
        enumValueInfos.Add ((Enum) values[i], info);
      }

      return enumValueInfos;
    }
  }

}