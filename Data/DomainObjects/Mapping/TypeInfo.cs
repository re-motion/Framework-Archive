using System;
using System.Collections;

using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
// TODO: Document how new types can be added. DBValueConverter methods must be changed too
public class TypeInfo
{
  // types

  // static members and constants

  private static readonly Hashtable s_types;
  private static readonly Hashtable s_mappingTypes;

  public static void AddInstance (TypeInfo typeInfo)
  {
    ArgumentUtility.CheckNotNull ("typeInfo", typeInfo);

    lock (typeof (TypeInfo))
    {
      s_mappingTypes.Add (GetMappingTypeKey (typeInfo), typeInfo);
      s_types.Add (typeInfo.Type, typeInfo);
    }
  }

  public static TypeInfo GetInstance (string mappingType, bool isNullable)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("mappingType", mappingType);

    return (TypeInfo) s_mappingTypes[GetMappingTypeKey (mappingType, isNullable)];
  }

  public static TypeInfo GetInstance (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    return (TypeInfo) s_types[type];
  }

  public static TypeInfo GetMandatory (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    TypeInfo typeInfo = GetInstance (type);
    
    if (typeInfo == null)
      throw new MandatoryTypeNotFoundException (type);

    return typeInfo;
  }

  public static TypeInfo GetMandatory (string mappingType, bool isNullable)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("mappingType", mappingType);

    TypeInfo typeInfo = GetInstance (mappingType, isNullable);

    if (typeInfo == null)
    {
      string message;
      if (isNullable)
        message = string.Format ("The nullable mapping type '{0}' could not be found.", mappingType);
      else
        message = string.Format ("The not-nullable mapping type '{0}' could not be found.", mappingType);

      throw new MandatoryMappingTypeNotFoundException (message, mappingType, isNullable);
    }

    return typeInfo;
  }

  public static object GetDefaultEnumValue (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    Array enumValues = Enum.GetValues (type);

    if (enumValues.Length > 0)
      return enumValues.GetValue (0);
  
    throw new InvalidEnumDefinitionException (type);
  }

  private static int GetMappingTypeKey (TypeInfo typeInfo)
  {
    return GetMappingTypeKey (typeInfo.MappingType, typeInfo.IsNullable); 
  }

  private static int GetMappingTypeKey (string mappingType, bool isNullable)
  {
    return mappingType.GetHashCode () ^ isNullable.GetHashCode (); 
  }

  // member fields

  private Type _type;
  private string _mappingType;
  private bool _isNullable;
  private object _defaultValue;

  // construction and disposing

  static TypeInfo ()
  {
    s_types = new Hashtable ();
    s_mappingTypes = new Hashtable ();

    foreach (TypeInfo typeInfo in GetAllKnownTypeInfos ())
    {
      if (!s_types.Contains (typeInfo.Type))
        s_types.Add (typeInfo.Type, typeInfo);

      s_mappingTypes.Add (GetMappingTypeKey (typeInfo), typeInfo);
    }
  }

  private static TypeInfo[] GetAllKnownTypeInfos ()
  {
    TypeInfo[] allTypeInfos = new TypeInfo[26];

    // Note: Nullable types must be added first to ensure hashtable s_types contains the nullable version

    allTypeInfos[0] = new TypeInfo (typeof (NaBoolean), "boolean", true, NaBoolean.Null);
    allTypeInfos[1] = new TypeInfo (typeof (NaByte), "byte", true, NaByte.Null);

    // Note: mappingType "dateTime" must be before mappingType "date" to ensure hashtable s_types contains the "NaDateTime" version
    allTypeInfos[2] = new TypeInfo (typeof (NaDateTime), "dateTime", true, NaDateTime.Null);
    allTypeInfos[3] = new TypeInfo (typeof (NaDateTime), "date", true, NaDateTime.Null);
    
    allTypeInfos[4] = new TypeInfo (typeof (NaDecimal), "decimal", true, NaDecimal.Null);
    allTypeInfos[5] = new TypeInfo (typeof (NaDouble), "double", true, NaDouble.Null);
    allTypeInfos[6] = new TypeInfo (typeof (NaGuid), "guid", true, NaGuid.Null);
    allTypeInfos[7] = new TypeInfo (typeof (NaInt16), "int16", true, NaInt16.Null);
    allTypeInfos[8] = new TypeInfo (typeof (NaInt32), "int32", true, NaInt32.Null);
    allTypeInfos[9] = new TypeInfo (typeof (NaInt64), "int64", true, NaInt64.Null);
    allTypeInfos[10] = new TypeInfo (typeof (NaSingle), "single", true, NaSingle.Null);
    allTypeInfos[11] = new TypeInfo (typeof (string), "string", true, null);
    allTypeInfos[12] = new TypeInfo (typeof (ObjectID), "objectID", true, null);

    allTypeInfos[13] = new TypeInfo (typeof (bool), "boolean", false, false);
    allTypeInfos[14] = new TypeInfo (typeof (byte), "byte", false, (byte) 0);

    // Note: mappingType "dateTime" must be before mappingType "date" to ensure hashtable s_types contains the "dateTime" version
    allTypeInfos[15] = new TypeInfo (typeof (DateTime), "dateTime", false, DateTime.MinValue);
    allTypeInfos[16] = new TypeInfo (typeof (DateTime), "date", false, DateTime.MinValue);

    allTypeInfos[17] = new TypeInfo (typeof (decimal), "decimal", false, (decimal) 0);
    allTypeInfos[18] = new TypeInfo (typeof (double), "double", false, (double) 0);
    allTypeInfos[19] = new TypeInfo (typeof (Guid), "guid", false, Guid.Empty);
    allTypeInfos[20] = new TypeInfo (typeof (short), "int16", false, (short) 0);
    allTypeInfos[21] = new TypeInfo (typeof (int), "int32", false, (int) 0);
    allTypeInfos[22] = new TypeInfo (typeof (long), "int64", false, (long) 0);
    allTypeInfos[23] = new TypeInfo (typeof (float), "single", false, (float) 0);
    allTypeInfos[24] = new TypeInfo (typeof (string), "string", false, string.Empty);
    allTypeInfos[25] = new TypeInfo (typeof (ObjectID), "objectID", false, null);

    return allTypeInfos;
  }

  public TypeInfo (Type type, string mappingType, bool isNullable, object defaultValue)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    ArgumentUtility.CheckNotNullOrEmpty ("mappingType", mappingType);

    _type = type;       
    _mappingType = mappingType;
    _isNullable = isNullable;
    _defaultValue = defaultValue;
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

  public object DefaultValue
  {
    get { return _defaultValue; }
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
