using System;
using System.Runtime.Serialization;
using System.Data.SqlTypes;

namespace Rubicon.NullableValueTypes
{
/// <summary>
///   Base interface for value types that support the nullable semantics of the <c>Rubicon.NullableValues</c> namespace.
/// </summary>
/// <remarks>
///   <para>
///     Create a value type <c>NaT</c> for each value type <c>T</c> that you want to be able to contain null values.
///   </para>
///   <para>
///     A class that implements <c>INaNullable</c> must meet the following conditions:
///   </para>
///   <list type="bullet">
///     <item>
///       It must use the <see cref="NaBasicTypeAttribute"/> attribute to specify the basic type the wrap.
///       <code>
///         [NaBasicType (typeof(T))]
///         public struct NaT: INaNullable
///       </code>
///     </item>
///     <item>
///       It must contain an <c>IsNull</c> property (implementing the base interface <c>INullable</c>).
///       <code>
///         public bool IsNull { get; }
///       </code>
///     </item>
///     <item>
///       It must contain s static property <c>Null</c> of its own type to represent null values.
///       <code>
///         public static readonly NaT Null;
///       </code>
///     </item>
///     <item>
///       It must contain a property that provides the value of the basic type. This property must throw a <c>NaNullValueException</c>
///       exception if the current instance is <c>Null</c>.
///       <code>
///         public T Value {get;}
///       </code>
///     </item>
///     <item>
///       It must provide a serialization constructor and an <c>GetObjectData</c> method (implementing the base 
///       interface <c>ISerializable</c>).
///       <code>
///         public void GetObjectData (SerializationInfo info, StreamingContext context);
///         private NaT (SerializationInfo info, StreamingContext context);
///       </code>
///     </item>
///     <item>
///       It must contain a public static conversion method for converting boxed values to this type <c>NaT</c>. This method
///       must have the name <c>FromBoxed&lt;T&gt;</c> and accept a parameter of type <c>Object</c>.
///       It must return its <c>Null</c> value if the parameter is a null reference, and the according NaT value if
///       the parameter is an instance of T.
///       All other types may yield an <c>ArgumentException</c>.
///       <code>
///         public static NaT FromBoxed&lt;T&gt; (object value);
///       </code>
///     </item>
///     <item>
///       It must contain a public static conversion method for converting its values to boxed values of the wrapped type <c>T</c>. 
///       This method must have the name <c>ToBoxed&lt;T&gt;</c> and accept a parameter of type <c>NaT</c>.
///       It must return a null reference if the parameter is <c>Null</c>, and the according T value otherwise.
///       <code>
///         public static object ToBoxed&lt;T&gt; (NaT value);
///       </code>
///     </item>
///     <item>
///       The type may also contain converions methods that behave like the <c>FromBoxed&lt;T&gt;</c> and <c>ToBoxed&lt;T&gt;</c>
///       but use <c>DBNull.Value</c> instead of null references. They must be called <c>FromBoxed&lt;T&gt;DBNull</c> and 
///       <c>ToBoxed&lt;T&gt;DBNull</c>, respectively.
///     </item>
///     <item>
///       The type must implement equality such as that two <c>Null</c> values are considered equal.
///     </item>
///   </list>
/// </remarks>
public interface INaNullable: INullable, ISerializable
{
  /// <summary>
  /// Gets the underlying value.
  /// </summary>
  object Value { get; }
}

/// <summary>
/// Specify this attribute for types that implement INaNullable.
/// </summary>
[AttributeUsage (AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public class  NaBasicTypeAttribute: Attribute
{
  private Type _basicType;

  /// <summary>
  /// Specifies the basic type that is wrapped by the type that uses this attribute.
  /// </summary>
  public NaBasicTypeAttribute (Type basicType)
  {
    _basicType = basicType;
  }

  /// <summary>
  /// Gets the basic type that is wrapped by the type that uses this attribute.
  /// </summary>
  public Type BasicType
  {
    get { return _basicType; }
  }
}

}
