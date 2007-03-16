using System;
using System.Configuration;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.Mapping;

namespace Rubicon.Data.DomainObjects.Mapping.Configuration
{
  public class MappingLoaderConfiguration: ExtendedConfigurationSection
  {
    // constants

    // types

    // static members

    // member fields

    private readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

    private readonly ConfigurationProperty _mappingLoaderProperty;
    private readonly DoubleCheckedLockingContainer<IMappingLoader> _mappingLoader;

    // construction and disposing

    public MappingLoaderConfiguration()
    {
      _mappingLoader = new DoubleCheckedLockingContainer<IMappingLoader> (
          delegate { return MappingLoaderElement.CreateInstance(); });
      _mappingLoaderProperty = new ConfigurationProperty (
          "loader",
          typeof (TypeElement<IMappingLoader, MappingReflector>),
          null,
          ConfigurationPropertyOptions.None);

      _properties.Add (_mappingLoaderProperty);
    }

    // methods and properties

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    public IMappingLoader MappingLoader
    {
      get { return _mappingLoader.Value; }
      set { _mappingLoader.Value = value; }
    }

    protected TypeElement<IMappingLoader> MappingLoaderElement
    {
      get { return (TypeElement<IMappingLoader>) this[_mappingLoaderProperty]; }
    }
  }
}