using System;
using System.Data;
using System.Data.SqlClient;

using Rubicon.Data.DomainObjects.Persistence.Configuration;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
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
    
    if (name.StartsWith ("@"))
      return name;
    else
      return "@" + name;
  }

  protected override IDbConnection CreateConnection ()
  {
    CheckDisposed ();
    
    return new SqlConnection ();
  }
}
}
