using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;
using System.Globalization;

namespace Rubicon.Security.Metadata
{
  public class LocalizingMetadataConverter : IMetadataConverter
  {
    private delegate LocalizedName CreateLocalizedName<T> (T item, string text);

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

      AddNames (localizedNames, cache.GetSecurableClassInfos (), CreateLocalizedNameFromClassInfo);
      AddNames (localizedNames, cache.GetAbstractRoles (), CreateLocalizedNameFromEnumValueInfo);
      AddNames (localizedNames, cache.GetAccessTypes (), CreateLocalizedNameFromEnumValueInfo);
      AddStateNames (localizedNames, cache.GetStatePropertyInfos ());

      return localizedNames.ToArray ();
    }

    private void AddNames<T> (List<LocalizedName> localizedNames, List<T> items, CreateLocalizedName<T> createLocalizedNameDelegate)
    {
      foreach (T item in items)
        localizedNames.Add (createLocalizedNameDelegate (item, string.Empty));
    }

    private LocalizedName CreateLocalizedNameFromClassInfo (SecurableClassInfo classInfo, string text)
    {
      return new LocalizedName (classInfo.ID, classInfo.Name, text);
    }

    private LocalizedName CreateLocalizedNameFromEnumValueInfo (EnumValueInfo enumValueInfo, string text)
    {
      return new LocalizedName (enumValueInfo.ID, enumValueInfo.TypeName, text);
    }

    private LocalizedName CreateLocalizedNameFromStatePropertyInfo (StatePropertyInfo propertyInfo, string text)
    {
      return new LocalizedName (propertyInfo.ID, propertyInfo.Name, text);
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

    private LocalizedName CreateLocalizedNameForState (StatePropertyInfo property, EnumValueInfo state, string text)
    {
      return new LocalizedName (property.ID + "|" + state.Value, property.Name + "|" + state.Name, text);
    }
  }
}
