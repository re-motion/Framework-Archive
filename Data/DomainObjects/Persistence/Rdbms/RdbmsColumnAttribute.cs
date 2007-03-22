using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
{
  /// <summary>Overrides the name used as the column name in the <b>RDBMS</b>.</summary>
  [AttributeUsage (AttributeTargets.Property)]
  public class RdbmsColumnAttribute : StorageSpecificNameAttribute
  {
    /// <summary>Initializes a new instance of the <see cref="RdbmsColumnAttribute"/> class.</summary>
    /// <param name="name">The name. Must not be <see langword="null" /> or empty.</param>
    public RdbmsColumnAttribute (string name)
        : base (name)
    {
    }
  }
}