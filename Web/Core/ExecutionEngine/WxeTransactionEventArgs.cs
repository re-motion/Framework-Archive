using System;
using Rubicon.Data;
using Rubicon.Utilities;

namespace Rubicon.Web.ExecutionEngine
{
  public class WxeTransactionEventArgs<TTransaction> : EventArgs
    where TTransaction : class, ITransaction
  {
    // types

    // static members

    // member fields

    private TTransaction _transaction;

    // construction and disposing



    public WxeTransactionEventArgs (TTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      _transaction = transaction;
    }

    // methods and properties

    public TTransaction Transaction
    {
      get { return _transaction; }
    }
  }
}