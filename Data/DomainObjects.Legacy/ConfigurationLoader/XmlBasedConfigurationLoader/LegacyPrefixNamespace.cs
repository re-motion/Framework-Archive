using System;
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Legacy.ConfigurationLoader.XmlBasedConfigurationLoader
{
  public sealed class LegacyPrefixNamespace
  {
    // types

    // static members and constants

    public static readonly PrefixNamespace MappingNamespace = new PrefixNamespace (
        "m", "http://www.rubicon-it.com/Data/DomainObjects/Mapping/1.0");

    public static readonly PrefixNamespace StorageProviderConfigurationNamespace = new PrefixNamespace (
        "sp", "http://www.rubicon-it.com/Data/DomainObjects/Persistence/1.0");

    // member fields

    // construction and disposing

    // methods and properties
  }
}
