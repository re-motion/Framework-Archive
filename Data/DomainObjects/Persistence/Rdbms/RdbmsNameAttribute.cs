using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
{
  /// <summary>Overrides the name used as the type's (table) name in the <b>RDBMS</b>.</summary>
  [AttributeUsage (AttributeTargets.Class)]
  public class RdbmsNameAttribute: StorageSpecificNameAttribute
  {
    /// <summary>Initializes a new instance of the <see cref="RdbmsNameAttribute"/> class.</summary>
    /// <param name="name">The name. Must not be <see langword="null" /> or empty.</param>
    public RdbmsNameAttribute (string name)
        : base (name)
    {
    }
  }
}