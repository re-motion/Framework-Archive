using System;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding
{
  /// <summary>
  /// Indicates an value of an enum that should be the undefined value.
  /// </summary>
  /// <remarks>
  ///   Use this Attribute if you need to have one value of an enum that represents the undefined value.
  ///   This value is then mapped the undefined value for displaying in Business Object Controls controls.
  /// </remarks>
  [AttributeUsage (AttributeTargets.Enum, AllowMultiple = false, Inherited = true)]
  public sealed class UndefinedEnumValueAttribute : Attribute
  {
    private readonly Enum _value;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value">The enum value that should be the undefined value. Must not be <see langword="null"/>.</param>
    // Note: Constructor parameter 'value' should be of type Enum, but in this case the C# compiler raises the following 
    // error when the attribute is applied to an enum:
    // "An attribute argument must be a constant expression, typeof expression or array creation expression."
    // => Use object instead, because this does not produce a compiler error.
    public UndefinedEnumValueAttribute (object value)
    {
      ArgumentUtility.CheckNotNullAndType<Enum> ("value", value);
      ArgumentUtility.CheckValidEnumValue ("value", (Enum) value);

      _value = (Enum) value;
    }

    /// <summary>
    /// Gets the undefined value of the enum.
    /// </summary>
    public Enum Value
    {
      get { return _value; }
    }
  }
}