using System;
using System.Reflection;


namespace Rubicon.Data.NullableValueTypes
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
      new Type[] { typeof(Int32), typeof (NaInt32) },
      new Type[] { typeof(Boolean), typeof (NaBoolean) },
      new Type[] { typeof(DateTime), typeof (NaDateTime) },
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
  ///       <term>Int32</term>
  ///       <description>NaInt32</description>
  ///     </item>
  ///     <item>
  ///       <term>Boolean</term>
  ///       <description>NaBoolean</description>
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
      throw new ArgumentException ("Specified type must implement Rubicon.Data.INaNullable.", "nullableType");

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
  ///       <term>NaInt32</term>
  ///       <description>Int32</description>
  ///     </item>
  ///     <item>
  ///       <term>NaBoolean</term>
  ///       <description>Boolean</description>
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

    throw new NotSupportedException ("Types that implement Rubicon.Data.INaNullable must specify a "
        + "Rubicon.Data.NaBasicTypeAttribute attribute. This attribute is missing from type " + nullableType.FullName + ".");
  }

}
}
