using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Security.Configuration;
using Rubicon.Security.Data.DomainObjects;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.SecurityManager.Clients.Web.WxeFunctions
{
  public abstract class BaseTransactedFunction : WxeTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    protected BaseTransactedFunction ()
    {
      Initialize ();
    }

    protected BaseTransactedFunction (params object[] args)
      : base (args)
    {
      Initialize ();
    }

    protected BaseTransactedFunction (ObjectID ClientID)
      : base (ClientID)
    {
      Initialize ();
    }

    // methods and properties

    [WxeParameter (1, true, WxeParameterDirection.In)]
    public ObjectID ClientID
    {
      get { return (ObjectID) Variables["ClientID"]; }
      set { Variables["ClientID"] = value; }
    }

    public bool HasUserCancelled
    {
      get { return (Exception != null && Exception.GetType () == typeof (WxeUserCancelException)); }
    }

    public ClientTransaction CurrentTransaction
    {
      get
      {
        ClientTransaction currentTransaction = Transaction;
        if (currentTransaction == null)
        {
          WxeTransactedFunction transactedFunction = this;
          while (currentTransaction == null && transactedFunction != null)
          {
            transactedFunction = (WxeTransactedFunction) GetStepByType (transactedFunction.ParentFunction, typeof (WxeTransactedFunction));
            if (transactedFunction != null)
              currentTransaction = transactedFunction.Transaction;
          }
        }
        return currentTransaction;
      }
    }

    protected virtual void Initialize ()
    {
      SetCatchExceptionTypes (typeof (WxeUserCancelException));
    }

    protected override void OnTransactionCreated (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      base.OnTransactionCreated (transaction);

      if (!SecurityConfiguration.Current.SecurityService.IsNull)
        transaction.Extensions.Add (typeof (SecurityClientTransactionExtension).FullName, new SecurityClientTransactionExtension ());
    }
  }
}
