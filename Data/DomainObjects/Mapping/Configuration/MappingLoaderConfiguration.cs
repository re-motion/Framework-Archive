using System;
using System.Configuration;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Infrastructure;

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

    private readonly ConfigurationProperty _domainObjectFactoryProperty;
    private readonly DoubleCheckedLockingContainer<IDomainObjectFactory> _domainObjectFactory;

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

      _domainObjectFactory = new DoubleCheckedLockingContainer<IDomainObjectFactory> (
          delegate { return DomainObjectFactoryElement.CreateInstance (); });
      _domainObjectFactoryProperty = new ConfigurationProperty (
          "domainObjectFactory",
          typeof (TypeElement<IDomainObjectFactory, DomainObjectFactory>),
          null,
          ConfigurationPropertyOptions.None);

      _properties.Add (_mappingLoaderProperty);
      _properties.Add (_domainObjectFactoryProperty);
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

    public IDomainObjectFactory DomainObjectFactory
    {
      get { return _domainObjectFactory.Value; }
      set { _domainObjectFactory.Value = value; }
    }

    protected TypeElement<IDomainObjectFactory> DomainObjectFactoryElement
    {
      get { return (TypeElement<IDomainObjectFactory>) this[_domainObjectFactoryProperty]; }
    }
  }
}