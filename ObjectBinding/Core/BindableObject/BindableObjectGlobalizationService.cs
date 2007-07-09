using System;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public class BindableObjectGlobalizationService : IBindableObjectGlobalizationService
  {
    [ResourceIdentifiers]
    [MultiLingualResources ("Rubicon.ObjectBinding.Globalization.BindableObjectGlobalizationService")]
    private enum ResourceIdentifier
    {
      True,
      False
    }

    private readonly DoubleCheckedLockingContainer<IResourceManager> _resourceManager = new DoubleCheckedLockingContainer<IResourceManager> (
        delegate { return MultiLingualResourcesAttribute.GetResourceManager (typeof (ResourceIdentifier), false); });

    public BindableObjectGlobalizationService ()
    {
    }

    public string GetEnumerationValueDisplayName (Enum value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      return EnumDescription.GetDescription (value) ?? value.ToString();
    }

    public string GetBooleanValueDisplayName (bool value)
    {
      return _resourceManager.Value.GetString (value ? ResourceIdentifier.True : ResourceIdentifier.False);
    }
  }
}