using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

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

    public List<EnumValueInfo> GetValues (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      if (!type.IsEnum)
        throw new ArgumentException (string.Format ("The type '{0}' is not an enumerated type.", type.FullName), "type");

      System.Collections.IList values = Enum.GetValues (type);
      string[] names = Enum.GetNames (type);

      List<EnumValueInfo> enumValueInfos = new List<EnumValueInfo> ();
      for (int i = 0; i < names.Length; i++)
        enumValueInfos.Add (new EnumValueInfo ((int) values[i], names[i]));

      return enumValueInfos;
    }
  }

}