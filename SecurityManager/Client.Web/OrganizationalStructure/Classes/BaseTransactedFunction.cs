using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.Client.Web.OrganizationalStructure.Classes
{
  public abstract class BaseTransactedFunction : WxeTransactedFunction
  {
    // types

    // static members and constants
    private const string c_ClientID = "00000001-0000-0000-0000-000000000001";
    // member fields

    // construction and disposing

    public BaseTransactedFunction ()
    {
      Initialize ();
    }

    public BaseTransactedFunction (params object[] args)
      : base (args)
    {
      Initialize ();
    }

    public BaseTransactedFunction (ObjectID ClientID)
      : base (ClientID)
    {
      Initialize ();
    }

    // methods and properties
    [WxeParameter (1, true, WxeParameterDirection.In)]
    public ObjectID ClientID
    {
      get { return new ObjectID (typeof (Rubicon.SecurityManager.Domain.OrganizationalStructure.Client), new Guid (c_ClientID)); }
      set { Variables["ClientID"] = value; }
    }

    protected virtual void Initialize ()
    {
      SetCatchExceptionTypes (typeof (WxeUserCancelException));
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
  }
}
