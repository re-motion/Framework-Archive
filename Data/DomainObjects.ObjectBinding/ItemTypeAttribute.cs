using System;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
/// <summary>
/// Specifies the type of items for properties returning a <see cref="DomainObjectCollection"/>.
/// </summary>
/// <remarks>Use this attribute to specify the item type on computed properties or other properties, where the type cannot be detected in the mapping.</remarks>
[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property)]
public class ItemTypeAttribute : Attribute
{
  private Type _itemType;

  /// <summary>
  /// Instantiates a new object.
  /// </summary>
  /// <param name="itemType">The type of items returned by the property. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="itemType"/> is <see langword="null"/>.</exception>
	public ItemTypeAttribute (Type itemType)
	{
    ArgumentUtility.CheckNotNull ("itemType", itemType);

    _itemType = itemType;
	}

  /// <summary>
  /// The type of items returned by the property.
  /// </summary>
  //TODO: missing ArgumentUtility?
  public Type ItemType
  {
    get { return _itemType; }
    set { _itemType = value; }
  }
}
}
