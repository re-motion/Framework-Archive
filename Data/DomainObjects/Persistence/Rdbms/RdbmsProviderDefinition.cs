using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
{
  public class RdbmsProviderDefinition: StorageProviderDefinition
  {
    // types

    // static members and constants

    private static string c_rdbmsConfigurationNamespace = "http://www.rubicon-it.com/Data/DomainObjects/Persistence/Rdbms";

    // member fields

    private string _connectionString;

    // construction and disposing

    public RdbmsProviderDefinition (
        string name,
        Type storageProviderType,
        string connectionString)
        : base (name, storageProviderType)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("connectionString", connectionString);
      _connectionString = connectionString;
    }

    public RdbmsProviderDefinition (string name, NameValueCollection config)
        : base (name, config)
    {
      ArgumentUtility.CheckNotNull ("config", config);

      string connectionStringName = GetAndRemoveNonEmptyStringAttribute (config, "connectionString", name, true);
      _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
    }

    // methods and properties

    public string ConnectionString
    {
      get { return _connectionString; }
    }

    public override bool IsIdentityTypeSupported (Type identityType)
    {
      ArgumentUtility.CheckNotNull ("identityType", identityType);

      return (identityType == typeof (Guid));
    }
  }
}