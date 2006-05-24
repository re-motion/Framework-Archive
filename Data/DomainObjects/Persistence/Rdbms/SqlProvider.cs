using System;
using System.Data;
using System.Data.SqlClient;

using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Utilities;
using System.Text.RegularExpressions;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
{
public class SqlProvider : RdbmsProvider
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public SqlProvider (RdbmsProviderDefinition definition) : base (definition)
  {
  }

  // methods and properties

  public override string GetColumnsFromSortExpression (string sortExpression)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("sortExpression", sortExpression);

    string formattedSortExpression = Regex.Replace (sortExpression, @"\r|\n|\t", " ", RegexOptions.IgnoreCase);

    if (formattedSortExpression.IndexOf (" COLLATE", StringComparison.InvariantCultureIgnoreCase) >= 0)
      throw CreateArgumentException ("sortExpression", "Collations cannot be used in sort expressions. Sort expression: '{0}'.", sortExpression);

    return Regex.Replace (formattedSortExpression, @" asc| desc", string.Empty, RegexOptions.IgnoreCase);
  }

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
