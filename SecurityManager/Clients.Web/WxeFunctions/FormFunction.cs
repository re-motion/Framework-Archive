using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain;

namespace Rubicon.SecurityManager.Clients.Web.WxeFunctions
{
  public abstract class FormFunction : BaseTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public FormFunction ()
    {
    }

    protected FormFunction (params object[] args)
      : base (args)
    {
    }

    public FormFunction (ObjectID ClientID, ObjectID CurrentObjectID)
      : base (ClientID, CurrentObjectID)
    {
    }

    // methods and properties
    [WxeParameter (2, false, WxeParameterDirection.In)]
    public ObjectID CurrentObjectID
    {
      get { return (ObjectID) Variables["CurrentObjectID"]; }
      set { Variables["CurrentObjectID"] = value; }
    }

    public BaseSecurityManagerObject CurrentObject
    {
      get
      {
        if (CurrentObjectID != null)
          return (BaseSecurityManagerObject) ClientTransaction.Current.GetObject (CurrentObjectID);

        return null;
      }
      set
      {
        if (value != null)
          CurrentObjectID = value.ID;
        else
          CurrentObjectID = null;
      }
    }
  }
}
