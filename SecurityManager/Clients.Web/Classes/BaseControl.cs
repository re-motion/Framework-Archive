using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Globalization;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.SecurityManager.Clients.Web.WxeFunctions;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.SecurityManager.Clients.Web.Classes
{
  public abstract class BaseControl : DataEditUserControl, IObjectWithResources
  {
    // types

    // static members and constants

    private static readonly string s_currentTenantIDKey = typeof (BaseControl).FullName + "_CurrentTenantID";

    // member fields

    private bool _hasTenantChanged;

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

    protected ObjectID CurrentTenantID
    {
      get { return (ObjectID) ViewState[s_currentTenantIDKey]; }
      set { ViewState[s_currentTenantIDKey] = value; }
    }

    protected bool HasTenantChanged
    {
      get { return _hasTenantChanged; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
        CurrentTenantID = CurrentFunction.TenantID;
    }

    protected override void OnPreRender (EventArgs e)
    {
      if (CurrentTenantID != CurrentFunction.TenantID)
      {
        CurrentTenantID = CurrentFunction.TenantID;
        _hasTenantChanged = true;
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

      if (MultiLingualResources.ExistsResource (type))
        return MultiLingualResources.GetResourceManager (type, true);
      else
        return null;
    }
  }
}
