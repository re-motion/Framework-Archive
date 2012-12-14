using System;
using System.Runtime.Serialization;

namespace Rubicon.Data.DomainObjects.Queries.Configuration
{
[Serializable]
public class QueryConfigurationException : ConfigurationException
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public QueryConfigurationException () : this ("Error in query configuration.") {}
  public QueryConfigurationException (string message) : base (message) {}
  public QueryConfigurationException (string message, Exception inner) : base (message, inner) {}
  protected QueryConfigurationException (SerializationInfo info, StreamingContext context) : base (info, context) {}

  // methods and properties

}
}
