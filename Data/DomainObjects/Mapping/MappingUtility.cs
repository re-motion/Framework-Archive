using System;
using System.Collections.Specialized;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
public sealed class MappingUtility
{
  // types

  // static members and constants

  private static readonly NameValueCollection s_typeMapping;

  public static Type MapType (string mappingType)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("mappingType", mappingType);

    if (Array.IndexOf (s_typeMapping.AllKeys, mappingType) >= 0)
    {
      return Type.GetType (s_typeMapping[mappingType], true);
    }
    else
    {
      try
      {
        return Type.GetType (mappingType, true);
      }
      catch (TypeLoadException e)
      {
        throw new MappingException (string.Format ("Cannot map unknown type '{0}'.", mappingType), e);
      }
    }
  }

  // member fields

  // construction and disposing


  // TODO: Move to TypeConversion.
  static MappingUtility ()
  {
    s_typeMapping = new NameValueCollection ();
    s_typeMapping.Add ("boolean", "System.Boolean, Mscorlib");
    s_typeMapping.Add ("byte", "System.Byte, Mscorlib");
    s_typeMapping.Add ("date", "System.DateTime, Mscorlib");
    s_typeMapping.Add ("dateTime", "System.DateTime, Mscorlib");
    s_typeMapping.Add ("decimal", "System.Decimal, Mscorlib");
    s_typeMapping.Add ("double", "System.Double, Mscorlib");
    s_typeMapping.Add ("guid", "System.Guid, Mscorlib");
    s_typeMapping.Add ("int16", "System.Int16, Mscorlib");
    s_typeMapping.Add ("int32", "System.Int32, Mscorlib");
    s_typeMapping.Add ("int64", "System.Int64, Mscorlib");
    s_typeMapping.Add ("single", "System.Single, Mscorlib");
    s_typeMapping.Add ("string", "System.String, Mscorlib");
    s_typeMapping.Add ("char", "System.Char, Mscorlib");
    s_typeMapping.Add ("objectID", "Rubicon.Data.DomainObjects.ObjectID, Rubicon.Data.DomainObjects");
  }

  private MappingUtility ()
  {
  }

  // methods and properties

}
}
