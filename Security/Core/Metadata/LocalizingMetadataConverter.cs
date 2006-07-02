using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;
using System.Globalization;

namespace Rubicon.Security.Metadata
{
  public class LocalizingMetadataConverter : IMetadataConverter
  {
    private CultureInfo[] _cultures;
    private IMetadataConverter _metadataConverter;
    private IMetadataLocalizationConverter _localizationConverter;

    public LocalizingMetadataConverter (IMetadataLocalizationConverter localizationConverter, CultureInfo[] cultures)
    {
      ArgumentUtility.CheckNotNull ("localizationConverter", localizationConverter);
      ArgumentUtility.CheckNotNullOrItemsNull ("cultures", cultures);

      _localizationConverter = localizationConverter;
      _cultures = cultures;
    }

    public IMetadataConverter MetadataConverter
    {
      get { return _metadataConverter; }
      set { _metadataConverter = value; }
    }

    public void ConvertAndSave (MetadataCache cache, string filename)
    {
      if (_metadataConverter != null)
        _metadataConverter.ConvertAndSave (cache, filename);

      foreach (CultureInfo culture in _cultures)
        _localizationConverter.ConvertAndSave (GetLocalizedNames (cache, culture), culture, filename);
    }

    private LocalizedName[] GetLocalizedNames (MetadataCache cache, CultureInfo culture)
    {
      List<LocalizedName> localizedNames = new List<LocalizedName> ();

      AddNames (localizedNames, cache.GetSecurableClassInfos ());
      AddNames (localizedNames, cache.GetAbstractRoles ());
      AddNames (localizedNames, cache.GetAccessTypes ());
      AddStateNames (localizedNames, cache.GetStatePropertyInfos ());

      return localizedNames.ToArray ();
    }

    private void AddNames<T> (List<LocalizedName> localizedNames, List<T> items) where T : MetadataInfo
    {
      foreach (MetadataInfo item in items)
        localizedNames.Add (CreateLocalizedName (item, string.Empty));
    }

    private LocalizedName CreateLocalizedName (MetadataInfo metadataInfo, string text)
    {
      return new LocalizedName (metadataInfo.ID, metadataInfo.Description, metadataInfo.Description);
    }

    private LocalizedName CreateLocalizedNameFromStatePropertyInfo (StatePropertyInfo propertyInfo, string text)
    {
      return new LocalizedName (propertyInfo.ID, propertyInfo.Name, propertyInfo.Description);
    }

    private LocalizedName CreateLocalizedNameForState (StatePropertyInfo property, EnumValueInfo state, string text)
    {
      string description = property.Name + "|" + state.Name;
      return new LocalizedName (property.ID + "|" + state.Value, property.Name + "|" + state.Name, description);
    }

    private void AddStateNames (List<LocalizedName> localizedNames, List<StatePropertyInfo> properties)
    {
      foreach (StatePropertyInfo property in properties)
      {
        localizedNames.Add (CreateLocalizedNameFromStatePropertyInfo (property, string.Empty));

        foreach (EnumValueInfo stateInfo in property.Values)
          localizedNames.Add (CreateLocalizedNameForState (property, stateInfo, string.Empty));
      }
    }
  }
}
