using System;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property)]
public class ItemTypeAttribute: Attribute
{
  private Type _itemType;

	public ItemTypeAttribute (Type itemType)
	{
    ArgumentUtility.CheckNotNull ("itemType", itemType);

    _itemType = itemType;
	}

  public Type ItemType
  {
    get { return _itemType; }
    set { _itemType = value; }
  }
}
}
