using System;
using System.Data;
using System.Data.SqlClient;

using Rubicon.Data.DomainObjects.Configuration.StorageProviders;

namespace Rubicon.Data.DomainObjects.Persistence
{
public class SqlProvider : RdbmsProvider
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public SqlProvider (RdbmsProviderDefinition rdbmsProviderDefinition) : base (rdbmsProviderDefinition)
  {
  }

  // methods and properties

  public override string GetParameterName (string name)
  {
    CheckDisposed ();
    
    return "@" + name;
  }

  protected override IDbConnection CreateConnection ()
  {
    CheckDisposed ();
    
    return new SqlConnection ();
  }
}
}
