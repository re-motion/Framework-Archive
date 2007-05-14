using System;
using Rubicon.Globalization;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.SecurityManager.Clients.Web.WxeFunctions;
using Rubicon.Web.UI.Controls;
using Rubicon.Data.DomainObjects;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.SecurityManager.Clients.Web.Classes
{
  public abstract class BaseControl : DataEditUserControl, IObjectWithResources
  {
    // types

    // static members and constants

    private static readonly string s_clientIDKey = typeof (BaseControl).FullName + "_ClientID";

    // member fields

    private bool _hasClientChanged;

    // construction and disposing

    // methods and properties
  
    public new BasePage Page
    {
      get { return (BasePage) base.Page; }
      set { base.Page = value; }
    }

    protected BaseTransactedFunction CurrentFunction
    {
      get { return (BaseTransactedFunction) Page.CurrentFunction; }
    }

    public virtual IFocusableControl InitialFocusControl
    {
      get { return null; }
    }

    protected ObjectID ClientID
    {
      get { return (ObjectID) ViewState[s_clientIDKey]; }
      set { ViewState[s_clientIDKey] = value; }
    }

    protected bool HasClientChanged
    {
      get { return _hasClientChanged; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
        ClientID = CurrentFunction.ClientID;
    }

    protected override void OnPreRender (EventArgs e)
    {
      if (ClientID != CurrentFunction.ClientID)
      {
        ClientID = CurrentFunction.ClientID;
        _hasClientChanged = true;
      }

      ResourceDispatcher.Dispatch (this, ResourceManagerUtility.GetResourceManager (this));

      base.OnPreRender (e);
    }

    IResourceManager IObjectWithResources.GetResourceManager ()
    {
      return this.GetResourceManager ();
    }

    protected virtual IResourceManager GetResourceManager ()
    {
      Type type = this.GetType ();

      if (MultiLingualResourcesAttribute.ExistsResource (type))
        return MultiLingualResourcesAttribute.GetResourceManager (type, true);
      else
        return null;
    }
  }
}
