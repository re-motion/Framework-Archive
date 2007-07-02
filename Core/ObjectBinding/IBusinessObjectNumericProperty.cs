using System;

namespace Rubicon.ObjectBinding
{
  //TODO: Type property for concrete numeric type
  /// <summary>The <see cref="IBusinessObjectNumericProperty"/> interface provides additional meta data for numeric values.</summary>
  public interface IBusinessObjectNumericProperty : IBusinessObjectProperty
  {
    /// <summary> Gets a flag specifying whether negative numbers are valid for the property. </summary>
    /// <value> <see langword="true"/> if this property can be assigned a negative value. </value>
    bool AllowNegative { get; }

    /// <summary>Gets the numeric type associated with this <see cref="IBusinessObjectNumericProperty"/>.</summary>
    Type Type { get; }
  }
}