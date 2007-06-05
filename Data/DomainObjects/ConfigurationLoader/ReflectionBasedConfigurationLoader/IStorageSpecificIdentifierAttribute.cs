using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// The <see cref="IStorageSpecificIdentifierAttribute"/> interface is used as a storage provider indifferent marker interface for more 
  /// conrete attributes such as the <see cref="DBColumnAttribute"/>.
  /// </summary>
  public interface IStorageSpecificIdentifierAttribute : IMappingAttribute
  {
    /// <summary>
    /// Gets the <see cref="string"/> used as the identifier.
    /// </summary>
    //TODO: Always allow null values.
    string Identifier { get; }
  }
}