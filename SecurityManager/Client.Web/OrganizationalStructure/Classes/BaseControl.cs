using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.Globalization;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.SecurityManager.Client.Web.OrganizationalStructure.Classes
{
  public abstract class BaseControl : DataEditUserControl, IObjectWithResources
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public BaseControl ()
    {
    }

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

    public virtual Control FocusControl
    {
      get { return null; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      RegisterEventHandlers ();
    }

    protected override void OnPreRender (EventArgs e)
    {
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

    protected virtual void RegisterEventHandlers ()
    {
    }
  }
}
