using System;
using System.Collections;

using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
// TODO Documentation:
public class TypeInfo
{
  // types

  // static members and constants

  private static readonly Hashtable s_mappingToType;
  private static readonly Hashtable s_typeToMapping;
  private static readonly Hashtable s_typeToNullableType;
  private static readonly Hashtable s_isNullableType;

//  public static Type MapType (string mappingType)
//  {
//    ArgumentUtility.CheckNotNullOrEmpty ("mappingType", mappingType);
//
//    if (Array.IndexOf (s_typeMapping.AllKeys, mappingType) >= 0)
//    {
//      return Type.GetType (s_typeMapping[mappingType], true);
//    }
//    else
//    {
//      try
//      {
//        return Type.GetType (mappingType, true);
//      }
//      catch (TypeLoadException e)
//      {
//        throw new MappingException (string.Format ("Cannot map unknown type '{0}'.", mappingType), e);
//      }
//    }
//  }

  // member fields

  private bool _isNullable;
  private string _mappingType;
  private Type _type;

  // construction and disposing
  static TypeInfo ()
  {
    s_mappingToType = new Hashtable ();
    s_typeToMapping = new Hashtable ();
    s_typeToNullableType = new Hashtable ();
    s_isNullableType = new Hashtable ();

    s_mappingToType.Add ("boolean", typeof (bool));
    s_mappingToType.Add ("byte", typeof (byte));
    s_mappingToType.Add ("date", typeof (DateTime));
    s_mappingToType.Add ("dateTime", typeof (DateTime));
    s_mappingToType.Add ("decimal", typeof (decimal));
    s_mappingToType.Add ("double", typeof (double));
    s_mappingToType.Add ("guid", typeof (Guid));
    s_mappingToType.Add ("int16", typeof (short));
    s_mappingToType.Add ("int32", typeof (int));
    s_mappingToType.Add ("int64", typeof (long));
    s_mappingToType.Add ("single", typeof (float));
    s_mappingToType.Add ("string", typeof (string));
    s_mappingToType.Add ("char", typeof (char));
    s_mappingToType.Add ("objectID", typeof (ObjectID));

    s_typeToMapping.Add (typeof (bool), "boolean");
    s_typeToMapping.Add (typeof (byte), "byte");
    s_typeToMapping.Add (typeof (DateTime), "dateTime");
    s_typeToMapping.Add (typeof (decimal), "decimal");
    s_typeToMapping.Add (typeof (double), "double");
    s_typeToMapping.Add (typeof (Guid), "guid");
    s_typeToMapping.Add (typeof (short), "int16");
    s_typeToMapping.Add (typeof (int), "int32");
    s_typeToMapping.Add (typeof (long), "int64");
    s_typeToMapping.Add (typeof (float), "single");
    s_typeToMapping.Add (typeof (string), "string");
    s_typeToMapping.Add (typeof (char), "char");
    s_typeToMapping.Add (typeof (ObjectID), "objectID");  
    s_typeToMapping.Add (typeof (NaBoolean), "boolean");
    s_typeToMapping.Add (typeof (NaDateTime), "dateTime");
    s_typeToMapping.Add (typeof (NaDouble), "double");
    s_typeToMapping.Add (typeof (NaInt32), "int32");

    s_isNullableType.Add (typeof (bool), false);
    s_isNullableType.Add (typeof (byte), false);
    s_isNullableType.Add (typeof (DateTime), false);
    s_isNullableType.Add (typeof (decimal), false);
    s_isNullableType.Add (typeof (double), false);
    s_isNullableType.Add (typeof (Guid), false);
    s_isNullableType.Add (typeof (short), false);
    s_isNullableType.Add (typeof (int), false);
    s_isNullableType.Add (typeof (long), false);
    s_isNullableType.Add (typeof (float), false);
    s_isNullableType.Add (typeof (string), true);
    s_isNullableType.Add (typeof (char), false);
    s_isNullableType.Add (typeof (ObjectID), true);  
    s_isNullableType.Add (typeof (NaBoolean), true);
    s_isNullableType.Add (typeof (NaDateTime), true);
    s_isNullableType.Add (typeof (NaDouble), true);
    s_isNullableType.Add (typeof (NaInt32), true);

    s_typeToNullableType.Add (typeof (bool), typeof (NaBoolean));
    s_typeToNullableType.Add (typeof (DateTime), typeof (NaDateTime));
    s_typeToNullableType.Add (typeof (double), typeof (NaDouble));
    s_typeToNullableType.Add (typeof (int), typeof (NaInt32));
    s_typeToNullableType.Add (typeof (string), typeof (string));
    s_typeToNullableType.Add (typeof (ObjectID), typeof (ObjectID));
  }

  public TypeInfo (string mappingType) : this (mappingType, false)
  {
  }

  public TypeInfo (string mappingType, bool isNullable)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("mappingType", mappingType);

    _type = MapType (mappingType, isNullable);
    _mappingType = mappingType;
    _isNullable = isNullable;
  }

  public TypeInfo (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    
    _mappingType = MapType (type);
    _type = type;   
    _isNullable = (bool) s_isNullableType[type];
  }

  private Type MapType (string mappingType, bool isNullable)
  {
    if (!s_mappingToType.Contains (mappingType))
    {
      try
      {
        return Type.GetType (mappingType, true);
      }
      catch (TypeLoadException e)
      {
        throw CreateArgumentException (e, "Cannot map unknown type '{0}'.", mappingType);
      }
    }

    Type type = (Type) s_mappingToType[mappingType];

    if (!isNullable)
      return type;

    if (!s_typeToNullableType.Contains (type))
      throw CreateArgumentException ("mappingType", "MappingType '{0}' cannot be nullable.", mappingType);

    return (Type) s_typeToNullableType[type];  
  }

  private string MapType (Type type)
  {
    if (!s_typeToMapping.Contains (type))
      throw CreateArgumentException ("type", "Cannot map unknown type '{0}'.", type);

    return (string) s_typeToMapping[type];
  }

  // methods and properties

  public Type Type
  {
    get { return _type; }
  }

  public string MappingType
  {
    get { return _mappingType; }
  }

  public bool IsNullable
  {
    get { return _isNullable; }
  }

  protected ArgumentException CreateArgumentException (string argumentName, string message, params object[] args)
  {
    return CreateArgumentException (null, argumentName, message, args);
  }

  protected ArgumentException CreateArgumentException (Exception innerException, string argumentName, string message, params object[] args)
  {
    return new ArgumentException (string.Format (message, args), argumentName, innerException);
  }
}
}
