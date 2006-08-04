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

namespace $PROJECT_ROOTNAMESPACE$.UI
{
  // <WxePageFunction pageType="$PROJECT_ROOTNAMESPACE$.UI.SearchResult$DOMAIN_CLASSNAME$Form" aspxFile="UI/SearchResult$DOMAIN_CLASSNAME$Form.aspx" functionName="SearchResult$DOMAIN_CLASSNAME$Function"
  //      functionBaseType="Rubicon.Data.DomainObjects.Web.ExecutionEngine.WxeTransactedFunction">
  //   <Parameter name="p_query" type="Rubicon.Data.DomainObjects.Queries.IQuery" required="false" />
  //   <Variable name="v_$DOMAIN_CLASSNAME$s" type="Rubicon.Data.DomainObjects.DomainObjectCollection" />
  // </WxePageFunction>
  public partial class SearchResult$DOMAIN_CLASSNAME$Form : BasePage
  {
    private void Page_Load(object sender, System.EventArgs e)
    {
      if (!IsPostBack)
      {
        IQuery actualQuery = p_query;
        if (actualQuery == null)
          actualQuery = new Query ("All$DOMAIN_CLASSNAME$s");
        v_$DOMAIN_CLASSNAME$s = ClientTransaction.Current.QueryManager.GetCollection (actualQuery);
      }
      $DOMAIN_CLASSNAME$List.LoadUnboundValue (v_$DOMAIN_CLASSNAME$s, IsPostBack);
    }

    protected void $DOMAIN_CLASSNAME$List_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      if (e.Column.ItemID == "Edit")
      {
        // execute edit function in own transaction and commit on return
        if (! IsReturningPostBack)
        {
          string id = ((IBusinessObjectWithIdentity)e.BusinessObject).UniqueIdentifier;
          Edit$DOMAIN_CLASSNAME$Function function = new Edit$DOMAIN_CLASSNAME$Function (id);
          function.TransactionMode = WxeTransactionMode.None;
          ExecuteFunction (function);
        }
        else
        {
          ClientTransaction.Current.Commit ();
        }
      }
    }
  }
}
