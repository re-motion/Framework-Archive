using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain
{
  public static class Revision
  {
    public static int GetRevision (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      Query query = new Query ("Rubicon.SecurityManager.Domain.Revision.GetRevision");
      return (int) clientTransaction.QueryManager.GetScalar (query);
    }

    public static void IncrementRevision (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      Query query = new Query ("Rubicon.SecurityManager.Domain.Revision.IncrementRevision");
      clientTransaction.QueryManager.GetScalar (query);
    }
  }
}