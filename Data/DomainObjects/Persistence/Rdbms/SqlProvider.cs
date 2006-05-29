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
    if (StringUtility.IsNullOrEmpty (sortExpression))
      return sortExpression;

    // Collapse all whitespaces (space, tab, carrriage return, ...) to a single space
    string formattedSortExpression = Regex.Replace (sortExpression, @"\s+", " ");

    // Remove any leading or trailing whitespace
    formattedSortExpression = formattedSortExpression.Trim ();

    // Collate is not supported. If collate should be supported later, UnionSelectCommandBuilder must 
    // add collate columns to select list (because it uses a UNION).
    if (formattedSortExpression.IndexOf (" COLLATE", StringComparison.InvariantCultureIgnoreCase) >= 0)
      throw CreateArgumentException ("sortExpression", "Collations cannot be used in sort expressions. Sort expression: '{0}'.", sortExpression);

    // Space after "," must be removed to avoid the following regex " asc| desc" matching a column beginning with "asc" or "desc"
    formattedSortExpression = Regex.Replace (formattedSortExpression, @" ?, ?", ",");
    
    // Remove sort orders
    formattedSortExpression = Regex.Replace (formattedSortExpression, @" asc| desc", string.Empty, RegexOptions.IgnoreCase);
    
    // Add space after each "," for better readability of resulting SQL expression
    formattedSortExpression = Regex.Replace (formattedSortExpression, @" ?, ?", ", ");

    // Remove any leading or trailing whitespace
    return formattedSortExpression.Trim ();
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
