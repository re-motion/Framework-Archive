namespace Rubicon.ObjectBinding
{
  //TODO: Type property for concrete numeric type
  /// <summary> 
  ///   The <b>IBusinessObjectNumericProperty</b> interface provides additional meta data for numeric values.
  /// </summary>
  /// <remarks>
  ///   This interface is used as a base for the specific numeric data type interfaces 
  ///   (e.g. <see cref="IBusinessObjectInt32Property"/>, <see cref="IBusinessObjectDoubleProperty"/>).
  /// </remarks>
  public interface IBusinessObjectNumericProperty : IBusinessObjectProperty
  {
    /// <summary> Gets a flag specifying whether negative numbers are valid for the property. </summary>
    /// <value> <see langword="true"/> if this property can be assigned a negative value. </value>
    bool AllowNegative { get; }
  }

  /// <summary> The <b>IBusinessObjectDoubleProperty</b> interface is used for accessing <see cref="double"/> values. </summary>
  public interface IBusinessObjectDoubleProperty : IBusinessObjectNumericProperty
  {
  }

  /// <summary> The <b>IBusinessObjectInt32Property</b> interface is used for accessing <see cref="int"/> values. </summary>
  public interface IBusinessObjectInt32Property : IBusinessObjectNumericProperty
  {
  }
}