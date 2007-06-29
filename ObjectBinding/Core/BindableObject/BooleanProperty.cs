using System;
using System.Reflection;
using Rubicon.Globalization;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public class BooleanProperty : PropertyBase, IBusinessObjectBooleanProperty
  {
    [ResourceIdentifiers]
    [MultiLingualResources ("Rubicon.ObjectBinding.Globalization.BooleanProperty")]
    private enum ResourceIdentifier
    {
      True,
      False
    }

    private readonly DoubleCheckedLockingContainer<IResourceManager> _resourceManager = new DoubleCheckedLockingContainer<IResourceManager> (
        delegate { return MultiLingualResourcesAttribute.GetResourceManager (typeof (ResourceIdentifier), false); });

    public BooleanProperty (BindableObjectProvider businessObjectProvider, PropertyInfo propertyInfo, IListInfo listInfo, bool isRequired)
        : base (businessObjectProvider, propertyInfo, listInfo, isRequired)
    {
    }

    /// <summary> Returns the human readable value of the boolean property. </summary>
    /// <param name="value"> The <see cref="bool"/> value to be formatted. </param>
    /// <returns> The human readable string value of the boolean property. </returns>
    /// <remarks> The value of this property may depend on the current culture. </remarks>
    public string GetDisplayName (bool value)
    {
      return _resourceManager.Value.GetString (value ? ResourceIdentifier.True : ResourceIdentifier.False);
    }

    /// <summary> Returns the default value to be assumed if the boolean property returns <see langword="null"/>. </summary>
    /// <param name="objectClass"> The <see cref="IBusinessObjectClass"/> for which to get the property's default value. </param>
    /// <remarks> 
    ///   If <see langword="null"/> is returned, the object model does not define a default value. In case the 
    ///   caller requires a default value, the selection of the appropriate value is left to the caller.
    /// </remarks>
    public bool? GetDefaultValue (IBusinessObjectClass objectClass)
    {
      if (IsNullable())
        return null;
      return false;
    }

    private bool IsNullable ()
    {
      return Nullable.GetUnderlyingType (IsList ? ListInfo.ItemType : PropertyType) != null;
    }
  }
}