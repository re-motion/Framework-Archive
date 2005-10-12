using System;
using System.Globalization;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence
{
public class ValueConverterBase
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  protected ValueConverterBase ()
  {
  }

  // methods and properties

  public virtual ObjectID GetObjectID (ClassDefinition classDefinition, object value)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

    if (value == null)
      return null;

    if (value.GetType () == typeof (string))
    {
      ObjectID id = null;
      try
      {
        id = ObjectID.Parse ((string) value);
      }
      catch (ArgumentException)
      {
      }
      catch (FormatException)
      {
      }

      if (id != null)
        return id;
    }

    return new ObjectID (classDefinition.ID, value);
  }

  public virtual object GetValue (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, object dataValue)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

    if (dataValue == null)
    {
      if (!propertyDefinition.IsNullable)
      {
        throw CreateConverterException (
            "Invalid null value for not-nullable property '{0}' encountered, class '{1}'.", 
            propertyDefinition.PropertyName, classDefinition.ID);
      }
      else
      {
        // For null values use Null singletons (e.g. NaBoolean.Null) instead
        return propertyDefinition.DefaultValue;
      }
    }

    if (propertyDefinition.PropertyType.IsEnum)
      return GetEnumValue (propertyDefinition, dataValue);

    if (propertyDefinition.PropertyType == typeof (string))
      return (string) dataValue;

    if (propertyDefinition.IsNullable)
    {
      if (dataValue.GetType () != typeof (string))
        return GetNullableObjectFromObject (propertyDefinition, dataValue);
      else
        return GetNullableObjectFromString (propertyDefinition, (string) dataValue);
    }
    else
    {
      if (dataValue.GetType () != typeof (string))
        return dataValue;
      else 
        return GetNativeObjectFromString (propertyDefinition, (string) dataValue);
    }
  }

  // TODO: Remove this method after Rubicon.Core exposes TypeConverterFactory.
  protected virtual object GetNullableObjectFromObject (PropertyDefinition propertyDefinition, object dataValue)
  {
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
    ArgumentUtility.CheckNotNull ("dataValue", dataValue);

    if (propertyDefinition.PropertyType == typeof (NaBoolean))
      return new NaBoolean ((bool) dataValue);

    if (propertyDefinition.PropertyType == typeof (NaByte))
      return new NaByte ((byte) dataValue);

    if (propertyDefinition.PropertyType == typeof (NaDateTime))
      return new NaDateTime ((DateTime) dataValue);

    if (propertyDefinition.PropertyType == typeof (NaDouble))
      return new NaDouble ((double) dataValue);

    if (propertyDefinition.PropertyType == typeof (NaDecimal))
      return new NaDecimal ((decimal) dataValue);

    if (propertyDefinition.PropertyType == typeof (NaGuid))
      return new NaGuid ((Guid) dataValue);

    if (propertyDefinition.PropertyType == typeof (NaInt16))
      return new NaInt16 ((short) dataValue);

    if (propertyDefinition.PropertyType == typeof (NaInt32))
      return new NaInt32 ((int) dataValue);

    if (propertyDefinition.PropertyType == typeof (NaInt64))
      return new NaInt64 ((long) dataValue);

    if (propertyDefinition.PropertyType == typeof (NaSingle))
      return new NaSingle ((float) dataValue);

    return dataValue;
  }

  // TODO: Remove this method after Rubicon.Core exposes TypeConverterFactory.
  protected virtual object GetNullableObjectFromString (PropertyDefinition propertyDefinition, string dataValue)
  {
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

    if (propertyDefinition.PropertyType == typeof (NaBoolean))
      return NaBoolean.Parse (dataValue);

    if (propertyDefinition.PropertyType == typeof (NaByte))
      return NaByte.Parse (dataValue, CultureInfo.InvariantCulture);

    if (propertyDefinition.PropertyType == typeof (NaDateTime))
      return NaDateTime.Parse (dataValue, CultureInfo.InvariantCulture);

    if (propertyDefinition.PropertyType == typeof (NaDouble))
      return NaDouble.Parse (dataValue, CultureInfo.InvariantCulture);

    if (propertyDefinition.PropertyType == typeof (NaDecimal))
      return NaDecimal.Parse (dataValue, CultureInfo.InvariantCulture);

    if (propertyDefinition.PropertyType == typeof (NaGuid))
      return NaGuid.Parse (dataValue);

    if (propertyDefinition.PropertyType == typeof (NaInt16))
      return NaInt16.Parse (dataValue, CultureInfo.InvariantCulture);

    if (propertyDefinition.PropertyType == typeof (NaInt32))
      return NaInt32.Parse (dataValue, CultureInfo.InvariantCulture);

    if (propertyDefinition.PropertyType == typeof (NaInt64))
      return NaInt64.Parse (dataValue, CultureInfo.InvariantCulture);

    if (propertyDefinition.PropertyType == typeof (NaSingle))
      return NaSingle.Parse (dataValue, CultureInfo.InvariantCulture);

    return dataValue;
  }

  // TODO: Remove this method after Rubicon.Core exposes TypeConverterFactory.
  protected virtual object GetNativeObjectFromString (PropertyDefinition propertyDefinition, string dataValue)
  {
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
    ArgumentUtility.CheckNotNullOrEmpty ("dataValue", dataValue);

    if (propertyDefinition.PropertyType == typeof (bool))
      return bool.Parse (dataValue);

    if (propertyDefinition.PropertyType == typeof (byte))
      return byte.Parse (dataValue, CultureInfo.InvariantCulture);

    if (propertyDefinition.PropertyType == typeof (DateTime))
      return DateTime.Parse (dataValue, CultureInfo.InvariantCulture);

    if (propertyDefinition.PropertyType == typeof (double))
      return double.Parse (dataValue, CultureInfo.InvariantCulture);

    if (propertyDefinition.PropertyType == typeof (decimal))
      return decimal.Parse (dataValue, CultureInfo.InvariantCulture);

    if (propertyDefinition.PropertyType == typeof (Guid))
      return new Guid (dataValue);

    if (propertyDefinition.PropertyType == typeof (short))
      return short.Parse (dataValue, CultureInfo.InvariantCulture);

    if (propertyDefinition.PropertyType == typeof (int))
      return int.Parse (dataValue, CultureInfo.InvariantCulture);

    if (propertyDefinition.PropertyType == typeof (long))
      return long.Parse (dataValue, CultureInfo.InvariantCulture);

    if (propertyDefinition.PropertyType == typeof (float))
      return float.Parse (dataValue, CultureInfo.InvariantCulture);

    return dataValue;
  }

  protected virtual object GetEnumValue (PropertyDefinition propertyDefinition, object dataValue)
  {
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
    ArgumentUtility.CheckNotNull ("dataValue", dataValue);

    if (Enum.IsDefined (propertyDefinition.PropertyType, dataValue))
      return Enum.ToObject (propertyDefinition.PropertyType, dataValue);

    throw CreateConverterException (
        "Enumeration '{0}' does not define the value '{1}', property '{2}'.",
        propertyDefinition.PropertyType.FullName, dataValue, propertyDefinition.PropertyName);
  }

  protected ClassDefinition GetOppositeClassDefinition (ClassDefinition classDefinition, PropertyDefinition propertyDefinition)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

    ClassDefinition relatedClassDefinition = classDefinition.GetOppositeClassDefinition (propertyDefinition.PropertyName);

    if (relatedClassDefinition == null)
    {
      throw CreateConverterException (
          "Property '{0}' of class '{1}' has no relations assigned.", propertyDefinition.PropertyName, classDefinition.ID);  
    }

    return relatedClassDefinition;
  }

  protected ArgumentException CreateArgumentException (string argumentName, string message, params object[] args)
  {
    return new ArgumentException (string.Format (message, args), argumentName);
  }

  protected ConverterException CreateConverterException (string formatString, params object[] args)
  {
    return CreateConverterException (null, formatString, args);
  }

  protected ConverterException CreateConverterException (Exception innerException, string formatString, params object[] args)
  {
    return new ConverterException (string.Format (formatString, args), innerException);
  }
}
}
