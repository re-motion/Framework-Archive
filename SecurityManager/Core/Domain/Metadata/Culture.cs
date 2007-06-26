using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class Culture : BaseSecurityManagerObject
  {
    public static Culture NewObject (ClientTransaction clientTransaction, string cultureName)
    {
      using (new ClientTransactionScope (clientTransaction))
      {
        return NewObject<Culture> ().With (cultureName);
      }
    }

    public static Culture Find (string name, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      Query query = new Query ("Rubicon.SecurityManager.Domain.Metadata.Culture.Find");
      query.Parameters.Add ("@cultureName", name);

      DomainObjectCollection result = clientTransaction.QueryManager.GetCollection (query);
      if (result.Count == 0)
        return null;

      return (Culture) result[0];
    }

    protected Culture (string cultureName)
    {
      ArgumentUtility.CheckNotNull ("cultureName", cultureName);
      
      CultureName = cultureName;
    }

    [StringProperty (IsNullable = false, MaximumLength = 10)]
    public abstract string CultureName { get; protected set; }
  }
}
