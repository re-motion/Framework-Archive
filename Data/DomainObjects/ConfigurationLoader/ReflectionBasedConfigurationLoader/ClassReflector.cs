using System;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public class ClassReflector
  {
    public ClassReflector()
    {
    }

    public ClassDefinition GetMetadata (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (DomainObject));
      ClassDefinition classDefinition = new ClassDefinition (
          type.FullName,
          type.Name,
          DomainObjectsConfiguration.Current.Storage.StorageProviderDefinition.Name,
          type,
          null);

      return classDefinition;
    }
  }
}