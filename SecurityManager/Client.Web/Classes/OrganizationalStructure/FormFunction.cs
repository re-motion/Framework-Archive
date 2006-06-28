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
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.Client.Web.Classes.OrganizationalStructure
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

    public FormFunction (ObjectID ClientID, ObjectID OrganizationalStructureObjectID)
      : base (ClientID, OrganizationalStructureObjectID)
    {
    }

    // methods and properties
    [WxeParameter (2, false, WxeParameterDirection.In)]
    public ObjectID OrganizationalStructureObjectID
    {
      get { return (ObjectID) Variables["OrganizationalStructureObjectID"]; }
      set { Variables["OrganizationalStructureObjectID"] = value; }
    }

    public OrganizationalStructureObject OrganizationalStructureObject
    {
      get
      {
        if (OrganizationalStructureObjectID != null)
          return (OrganizationalStructureObject) ClientTransaction.Current.GetObject (OrganizationalStructureObjectID);

        return null;
      }
      set
      {
        if (value != null)
          OrganizationalStructureObjectID = value.ID;
        else
          OrganizationalStructureObjectID = null;
      }
    }
  }
}
