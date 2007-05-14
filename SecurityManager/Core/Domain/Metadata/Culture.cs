using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  public class Culture : BaseSecurityManagerObject
  {
    // types

    // static members and constants

    public static new Culture GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Culture) DomainObject.GetObject (id, clientTransaction);
    }

    public static new Culture GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (Culture) DomainObject.GetObject (id, clientTransaction, includeDeleted);
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

    // member fields

    // construction and disposing

    public Culture (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    public Culture (ClientTransaction clientTransaction, string cultureName)
      : base (clientTransaction)
    {
      DataContainer["CultureName"] = cultureName;
    }

    protected Culture (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public string CultureName
    {
      get { return (string) DataContainer["CultureName"]; }
    }
  }
}
