using System;
using System.Runtime.Serialization;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
// TODO documentation:
[Serializable]
public class MandatoryMappingTypeNotFoundException : DomainObjectException
{
  // types

  // static members and constants

  // member fields

  private string _mappingType;
  private bool _isNullable;

  // construction and disposing

  public MandatoryMappingTypeNotFoundException () : this ("A mandatory mapping type could not be found.") 
  {
  }

  public MandatoryMappingTypeNotFoundException (string message) : base (message) 
  {
  }
  
  public MandatoryMappingTypeNotFoundException (string message, Exception inner) : base (message, inner) 
  {
  }

  protected MandatoryMappingTypeNotFoundException (SerializationInfo info, StreamingContext context) : base (info, context) 
  {
    _mappingType = info.GetString ("MappingType");
    _isNullable = info.GetBoolean ("IsNullable");
  }

  public MandatoryMappingTypeNotFoundException (string message, string mappingType, bool isNullable) : base (message) 
  {
    ArgumentUtility.CheckNotNullOrEmpty ("mappingType", mappingType);

    _mappingType = mappingType;
    _isNullable = isNullable;
  }

  // methods and properties

  public string MappingType
  {
    get { return _mappingType; }
  }

  public bool IsNullable
  {
    get { return _isNullable; }
  }

  /// <summary>
  /// Sets the SerializationInfo object with the parameter name and additional exception information.
  /// </summary>
  /// <param name="info">The object that holds the serialized object data.</param>
  /// <param name="context">The contextual information about the source or destination.</param>
  public override void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    base.GetObjectData (info, context);

    info.AddValue ("MappingType", _mappingType);
    info.AddValue ("IsNullable", _isNullable);
  }
}
}
