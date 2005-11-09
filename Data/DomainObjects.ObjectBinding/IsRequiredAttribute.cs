using System;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
/// <summary>
/// Specifies if a property of fiels is required.
/// </summary>
[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property)]
[Serializable]
public sealed class IsRequiredAttribute : Attribute
{
  private bool _isRequired;

  /// <summary>
  /// Instantiates a new object.
  /// </summary>
  /// <param name="isRequired">A value indicating if the property or field is required.</param>
	public IsRequiredAttribute (bool isRequired)
	{
    _isRequired = isRequired;
	}

  /// <summary>
  /// Gets a value indicating if the property or field is required.
  /// </summary>
  public bool IsRequired
  {
    get { return _isRequired; }
  }
}
}
