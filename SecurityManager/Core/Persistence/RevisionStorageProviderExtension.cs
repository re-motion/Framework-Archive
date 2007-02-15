using System;
using System.Data;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.SecurityManager.Domain;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Persistence
{
  public class RevisionStorageProviderExtension
  {
    // constants

    // types

    // static members

    // member fields

    // construction and disposing

    public RevisionStorageProviderExtension ()
    {
    }

    // methods and properties


    public virtual void Saving (IDbConnection connection, IDbTransaction transaction, DataContainerCollection dataContainers)
    {
      ArgumentUtility.CheckNotNull ("connection", connection);
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNullOrItemsNull ("datacContainers", dataContainers);

      Type securityManagerDomainLayerSuperType = typeof (BaseSecurityManagerObject);
      foreach (DataContainer dataContainer in dataContainers)
      {
        if (securityManagerDomainLayerSuperType.IsAssignableFrom (dataContainer.DomainObjectType))
        {
          IncrementRevision (connection, transaction);
          return;
        }
      }
    }

    private void IncrementRevision (IDbConnection connection, IDbTransaction transaction)
    {
      using (IDbCommand command = connection.CreateCommand ())
      {
        command.Transaction = transaction;
        QueryDefinition queryDefintion = QueryConfiguration.Current.QueryDefinitions.GetMandatory ("Rubicon.SecurityManager.Domain.Revision.IncrementRevision");
        command.CommandText = queryDefintion.Statement;

        command.ExecuteNonQuery ();
      }
    }
  }
}