using System;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace Rubicon.NullableValueTypes
{

/// <summary>
/// Provides utility methods for dealing with nullable runtime types.
/// </summary>
public sealed class NaTypeUtility
{
  private NaTypeUtility()
  {
  }

  private static Type[][] s_typeMappings = {
      new Type[] { typeof(Boolean), typeof (NaBoolean) },
      new Type[] { typeof(Byte), typeof (NaByte) },
      new Type[] { typeof(Int16), typeof (NaInt16) },
      new Type[] { typeof(Int32), typeof (NaInt32) },
      new Type[] { typeof(Int64), typeof (NaInt64) },
      new Type[] { typeof(Single), typeof (NaSingle) },
      new Type[] { typeof(Decimal), typeof (NaDecimal) },
      new Type[] { typeof(DateTime), typeof (NaDateTime) },
      new Type[] { typeof(Guid), typeof (NaGuid) },
      new Type[] { typeof(Double), typeof (NaDouble) }};


  /// <summary>
  /// Returns whether the specified type implements <see cref="INaNullable"/>.
  /// </summary>
  public static bool IsNaNullableType (Type type)
  {
    return typeof(INaNullable).IsAssignableFrom (type);
  }

  /// <summary>
  /// Gets the nullable type for a basic runtime type.
  /// </summary>
  /// <param name="basicType"></param>
  /// <returns>
  ///   Returns the nullable type registered for this basic type. If there is no associated nullable type, the basic type itself is returned.
  ///   <para>
  ///   Some examples:
  ///   </para>
  ///   <list type="table">
  ///     <listheader>
  ///       <term>Basic Type</term>
  ///       <description>Returned Type</description>
  ///     </listheader>
  ///     <item>
  ///       <term>Boolean</term>
  ///       <description>NaBoolean</description>
  ///     </item>
  ///     <item>
  ///       <term>Byte</term>
  ///       <description>NaByte</description>
  ///     </item>
  ///     <item>
  ///       <term>Int16</term>
  ///       <description>NaInt16</description>
  ///     </item>
  ///     <item>
  ///       <term>Int32</term>
  ///       <description>NaInt32</description>
  ///     </item>
  ///     <item>
  ///       <term>Int64</term>
  ///       <description>NaInt64</description>
  ///     </item>
  ///     <item>
  ///       <term>Single</term>
  ///       <description>NaSingle</description>
  ///     </item>
  ///     <item>
  ///       <term>Decimal</term>
  ///       <description>NaDecimal</description>
  ///     </item>
  ///     <item>
  ///       <term>Double</term>
  ///       <description>NaDouble</description>
  ///     </item>
  ///     <item>
  ///       <term>DateTime</term>
  ///       <description>NaDateTime</description>
  ///     </item>
  ///     <item>
  ///       <term>UserDefinedType</term>
  ///       <description>NaUserDefinedType</description>
  ///     </item>
  ///     <item>
  ///       <term>String</term>
  ///       <term>String</term>
  ///     </item>
  ///   </list>
  /// </returns>
  /// <remarks>
  /// This method depends on a user-extendable mapping table and can therefore handle user-defined types that 
  /// implement <see cref="INaNullable"/>too. Call <see cref="AddMapping"/> to add user-defined mappings first.
  /// </remarks>
  public static Type GetNullableType (Type basicType)
  {
    foreach (Type[] mapping in s_typeMappings)
    {
      if (mapping[0] == basicType)
        return mapping[1];
    }
    return basicType;
  }

  /// <summary>
  /// Registers a nullable type for use by <see cref="GetNullableType"/>.
  /// </summary>
  /// <exception cref="ArgumentException"><c>nullableType</c> does not implement <see cref="INaNullable"/>.</exception>
  public static void AddMapping (Type nullableType)
  {
    if (! IsNaNullableType (nullableType))
      throw new ArgumentException ("Specified type must implement Rubicon.INaNullable.", "nullableType");

    Type basicType = GetBasicType (nullableType);
    Type[][] newMappings = new Type[s_typeMappings.Length + 1][];
    s_typeMappings.CopyTo (newMappings, 0);
    newMappings[newMappings.Length - 1] = new Type[] {basicType, nullableType};
    s_typeMappings = newMappings;
  }

  /// <summary>
  ///   Gets the basic runtime-type for any nullable type.
  /// </summary>
  /// <param name="nullableType"></param>
  /// <returns>
  ///   Returns the basic type of the specified nullable type. If <c>nullableType</c> is not a nullable type, the specified type 
  ///   itself is returned.
  ///   <para>
  ///   Some examples:
  ///   </para>
  ///   <list type="table">
  ///     <listheader>
  ///       <term>Basic Type</term>
  ///       <description>Returned Type</description>
  ///     </listheader>
  ///     <item>
  ///       <term>NaBoolean</term>
  ///       <description>Boolean</description>
  ///     </item>
  ///     <item>
  ///       <term>NaByte</term>
  ///       <description>Byte</description>
  ///     </item>
  ///     <item>
  ///       <term>NaInt16</term>
  ///       <description>Int16</description>
  ///     </item>
  ///     <item>
  ///       <term>NaInt32</term>
  ///       <description>Int32</description>
  ///     </item>
  ///     <item>
  ///       <term>NaInt64</term>
  ///       <description>Int64</description>
  ///     </item>
  ///     <item>
  ///       <term>NaSingle</term>
  ///       <description>Single</description>
  ///     </item>
  ///     <item>
  ///       <term>NaDecimal</term>
  ///       <description>Decimal</description>
  ///     </item>
  ///     <item>
  ///       <term>NaDouble</term>
  ///       <description>Double</description>
  ///     </item>
  ///     <item>
  ///       <term>NaDateTime</term>
  ///       <description>DateTime</description>
  ///     </item>
  ///     <item>
  ///       <term>NaUserDefinedType</term>
  ///       <description>UserDefinedType</description>
  ///     </item>
  ///     <item>
  ///       <term>String</term>
  ///       <term>String</term>
  ///     </item>
  ///   </list>
  /// </returns>
  /// <remarks>
  ///   This method depends on the <see cref="NaBasicTypeAttribute"/> attribute of the nullable type and can therefore handle user-defined types 
  ///   that implement <see cref="INaNullable"/> too. There is no need to call <see cref="AddMapping"/> in order for this method to work.
  /// </remarks>
  /// <exception cref="NotSupportedException">
  ///   <c>nullableType</c> implements <see cref="INaNullable"/>, but does not specify the <see cref="NaBasicTypeAttribute"/> attribute.
  /// </exception>
  public static Type GetBasicType (Type nullableType)
  {
    if (! IsNaNullableType (nullableType))
      return nullableType;

    NaBasicTypeAttribute[] basicTypes = (NaBasicTypeAttribute[]) nullableType.GetCustomAttributes (typeof(NaBasicTypeAttribute), false);
    if (basicTypes != null && basicTypes.Length > 0)
      return basicTypes[0].BasicType;

    throw new NotSupportedException ("Types that implement Rubicon.INaNullable must specify a "
        + "Rubicon.NullableValueTypes.NaBasicTypeAttribute attribute. This attribute is missing from type " + nullableType.FullName + ".");
  }

  public static XmlSchema CreateXmlSchema (string name, string xmlType)
  {
    XmlSchema schema = new XmlSchema();
    schema.Id = name;
    schema.ElementFormDefault = XmlSchemaForm.Qualified;
    schema.Namespaces.Add ("xs", "http://www.w3.org/2001/XMLSchema");

    XmlSchemaElement element = new XmlSchemaElement();
    element.Name = name;
    element.SchemaTypeName = new XmlQualifiedName (xmlType, "http://www.w3.org/2001/XMLSchema");
    element.IsNillable = true;
    schema.Items.Add (element);

    return schema;
  }

  public static void WriteXml (XmlWriter writer, string value)
  {
    if (value == null)
      writer.WriteAttributeString ("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
    else
      writer.WriteString (value);
  }

  public static string ReadXml (XmlReader reader)
  {
    if (reader.IsEmptyElement)
      return null;
    else
      return reader.ReadElementString();
  }
}

internal enum DebuggingNull { Null };

}
