using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain
{
  public static class Revision
  {
    public static int GetRevision ()
    {
      Query query = new Query ("Rubicon.SecurityManager.Domain.Revision.GetRevision");
      return (int) ClientTransactionScope.CurrentTransaction.QueryManager.GetScalar (query);
    }

    public static void IncrementRevision ()
    {
      Query query = new Query ("Rubicon.SecurityManager.Domain.Revision.IncrementRevision");
      ClientTransactionScope.CurrentTransaction.QueryManager.GetScalar (query);
    }
  }
}