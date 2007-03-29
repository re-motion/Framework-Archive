using System;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Overrides the name used to represent the type or property in the storage layer.</summary>
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public abstract class StorageSpecificNameAttribute : Attribute, IMappingAttribute
  {
    private string _name;

    protected StorageSpecificNameAttribute (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      _name = name;
    }

    public string Name
    {
      get { return _name; }
    }
  }
}