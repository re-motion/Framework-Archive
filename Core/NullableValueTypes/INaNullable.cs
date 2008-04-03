using System;
using System.Data.SqlTypes;

namespace Remotion.NullableValueTypes
{
/// <summary>
///   Base interface for value types that support the nullable semantics of the 
///   <see cref="Remotion.NullableValueTypes"/> namespace.
/// </summary>
/// <include file='doc\include\NullableValueTypes\include.xml' path='Comments/INaNullable/remarks' />
public interface INaNullable: INullable
{
  /// <summary>
  /// Gets the underlying value.
  /// </summary>
  object Value { get; }
}

/// <summary>
/// Specify this attribute for types that implement <see cref="INaNullable"/>.
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
