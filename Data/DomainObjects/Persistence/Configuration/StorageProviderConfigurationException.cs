using System;
using System.Runtime.Serialization;

namespace Rubicon.Data.DomainObjects.Configuration.StorageProviders
{
[Serializable]
public class StorageProviderConfigurationException : ConfigurationException
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public StorageProviderConfigurationException () {}

  public StorageProviderConfigurationException (string message) : base (message) {}
  public StorageProviderConfigurationException (string message, Exception inner) : base (message, inner) {}
  protected StorageProviderConfigurationException (SerializationInfo info, StreamingContext context) : base (info, context) {}

  // methods and properties

}
}
