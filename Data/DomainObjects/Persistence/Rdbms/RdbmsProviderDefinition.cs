using System;
using System.Xml;

using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
{
public class RdbmsProviderDefinition : StorageProviderDefinition
{
  // types

  // static members and constants

  private static string c_rdbmsConfigurationNamespace = 
      "http://www.rubicon-it.com/Data/DomainObjects/Persistence/Rdbms";

  // member fields

  private string _connectionString;

  // construction and disposing

  public RdbmsProviderDefinition (
      string storageProviderID, 
      Type storageProviderType, 
      string connectionString)
      : base (storageProviderID, storageProviderType)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("connectionString", connectionString);
    _connectionString = connectionString;
  }

  public RdbmsProviderDefinition (
      string storageProviderID, 
      Type storageProviderType, 
      XmlNode configurationNode)
      : base (storageProviderID, storageProviderType)
  {
    ArgumentUtility.CheckNotNull ("configurationNode", configurationNode);

    XmlNamespaceManager namespaceManager = new XmlNamespaceManager (configurationNode.OwnerDocument.NameTable);
    namespaceManager.AddNamespace ("r", c_rdbmsConfigurationNamespace);

    XmlNode connectionStringNode = configurationNode.SelectSingleNode ("r:connectionString", namespaceManager);
    if (connectionStringNode != null)        
    {
      _connectionString = connectionStringNode.InnerText;
    }
    else
    {
      throw new StorageProviderConfigurationException (string.Format (
          "Configuration node must contain a 'connectionString' element from namespace '{0}'.",
          c_rdbmsConfigurationNamespace));
    }
  }

  // methods and properties

  public string ConnectionString
  {
    get { return _connectionString; }
  }
}
}
