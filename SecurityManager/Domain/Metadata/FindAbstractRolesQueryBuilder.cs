using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Security;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Queries.Configuration;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  public class FindAbstractRolesQueryBuilder : QueryBuilder
  {
    public FindAbstractRolesQueryBuilder ()
      : base ("FindAbstractRoles", typeof (AbstractRoleDefinition))
    {
    }

    public Query CreateQuery (EnumWrapper[] abstractRoles)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("abstractRoles", abstractRoles);

      return CreateQuery (GetStatement (abstractRoles));
    }

    protected string GetStatement (EnumWrapper[] abstractRoles)
    {
      string sql = "SELECT * FROM [EnumValueDefinition] WHERE ClassID='AbstractRoleDefinition' AND ({0});";
      StringBuilder whereClauseBuilder = new StringBuilder (abstractRoles.Length * 50);

      for (int i = 0; i < abstractRoles.Length; i++)
      {
        EnumWrapper roleWrapper = abstractRoles[i];

        if (whereClauseBuilder.Length > 0)
          whereClauseBuilder.Append (" OR ");

        string parameterName = "@p" + i.ToString ();
        whereClauseBuilder.Append ("Name = ");
        whereClauseBuilder.Append (parameterName);

        Parameters.Add (parameterName, roleWrapper.Value);
      }

      return string.Format (sql, whereClauseBuilder.ToString ());
    }
  }
}
