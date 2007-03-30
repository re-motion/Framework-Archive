using System;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  //TODO: Doc
  public interface IStorageSpecificIdentifierAttribute : IMappingAttribute
  {
    string Identifier { get; }
  }
}