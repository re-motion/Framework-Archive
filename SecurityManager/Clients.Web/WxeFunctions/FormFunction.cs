using System;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain;
using Rubicon.Web.ExecutionEngine;

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

    public FormFunction (ObjectID CurrentObjectID)
      : base (CurrentObjectID)
    {
    }

    // methods and properties
    [WxeParameter (1, false, WxeParameterDirection.In)]
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
          return BaseSecurityManagerObject.GetObject (CurrentObjectID, ClientTransaction.Current);

        return null;
      }
      set
      {
        CurrentObjectID = (value != null) ? value.ID : null;
      }
    }
  }
}
