using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Queries;

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