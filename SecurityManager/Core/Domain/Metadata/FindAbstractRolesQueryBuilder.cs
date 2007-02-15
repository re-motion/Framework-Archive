using System.Text;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Security;
using Rubicon.Utilities;

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

      return CreateQueryFromStatement (GetStatement (abstractRoles));
    }

    protected string GetStatement (EnumWrapper[] abstractRoles)
    {
      using (StorageProviderManager storageProviderManager = new StorageProviderManager ())
      {
        RdbmsProvider storageProvider = (RdbmsProvider) storageProviderManager.GetMandatory (GetStorageProviderID ());

        StringBuilder whereClauseBuilder = new StringBuilder (abstractRoles.Length * 50);
        for (int i = 0; i < abstractRoles.Length; i++)
        {
          EnumWrapper roleWrapper = abstractRoles[i];

          if (whereClauseBuilder.Length > 0)
            whereClauseBuilder.Append (" OR ");

          string parameterName = storageProvider.GetParameterName ("p" + i);
          whereClauseBuilder.Append (storageProvider.DelimitIdentifier ("Name"));
          whereClauseBuilder.Append (" = ");
          whereClauseBuilder.Append (parameterName);

          Parameters.Add (parameterName, roleWrapper.ToString ());
        }

        return string.Format ("SELECT * FROM {0} WHERE {1}", storageProvider.DelimitIdentifier ("AbstractRoleDefinitionView"), whereClauseBuilder);
      }
    }
  }
}
