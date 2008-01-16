using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI.Globalization;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.UI.Controls;
using $PROJECT_ROOTNAMESPACE$;
using $PROJECT_ROOTNAMESPACE$.Classes;
using $DOMAIN_ROOTNAMESPACE$;

namespace $PROJECT_ROOTNAMESPACE$.UI
{
  // <WxePageFunction>
  //   <Parameter name="query" type="IQuery" />
  //   <Variable name="searchResult" type="DomainObjectCollection" />
  // </WxePageFunction>
  public partial class SearchResult$DOMAIN_CLASSNAME$Form : BasePage
  {
    private void Page_Load(object sender, System.EventArgs e)
    {
      if (!IsPostBack)
      {
        if (query == null)
          query = new Query ("All$DOMAIN_CLASSNAME$s");
        searchResult = ClientTransaction.Current.QueryManager.GetCollection (query);
      }
      $DOMAIN_CLASSNAME$List.LoadUnboundValue (searchResult, IsPostBack);
    }

    protected void $DOMAIN_CLASSNAME$List_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      if (e.Column.ItemID == "Edit")
      {
        try
        {
          Edit$DOMAIN_CLASSNAME$Form.Call (this, ($DOMAIN_CLASSNAME$)e.BusinessObject);
          ClientTransaction.Current.Commit ();
        }
        catch (WxeUserCancelException)
        {
        }
      }
    }
  }
}
