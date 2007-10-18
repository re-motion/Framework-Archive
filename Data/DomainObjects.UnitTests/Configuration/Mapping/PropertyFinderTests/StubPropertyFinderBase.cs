using System;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyFinderTests
{
  public class StubPropertyFinderBase : PropertyFinderBase
  {
    public StubPropertyFinderBase (Type type, bool includeBaseProperties)
      : this (type, includeBaseProperties, PersistentMixinFinder.GetPersistentMixins (type))
    {
    }

    public StubPropertyFinderBase (Type type, bool includeBaseProperties, IEnumerable<Type> persistentMixins)
      : base (type, includeBaseProperties, persistentMixins)
    {
    }
  }
}