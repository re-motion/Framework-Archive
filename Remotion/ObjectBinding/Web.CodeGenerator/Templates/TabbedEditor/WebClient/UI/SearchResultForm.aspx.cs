// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Globalization;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Data.DomainObjects.ObjectBinding;
using $PROJECT_ROOTNAMESPACE$;
using $PROJECT_ROOTNAMESPACE$.Classes;
using $DOMAIN_ROOTNAMESPACE$;

namespace $PROJECT_ROOTNAMESPACE$.UI
{
  // <WxeFunction>
  //   <Parameter name="query" type="IQuery" />
  //   <Variable name="searchResult" type="DomainObjectCollection" />
  // </WxeFunction>
  public partial class SearchResult$DOMAIN_CLASSNAME$Form : BasePage
  {
    private void Page_Load(object sender, System.EventArgs e)
    {
      Title = ResourceManagerUtility.GetResourceManager(this).GetString("Search~$DOMAIN_CLASSNAME$");
      var searchAllService = new BindableDomainObjectSearchAllService ();
      var list$DOMAIN_CLASSNAME$s = searchAllService.GetAllObjects (ClientTransaction.Current, typeof ($DOMAIN_CLASSNAME$));
      $DOMAIN_CLASSNAME$List.LoadUnboundValue (list$DOMAIN_CLASSNAME$s, IsPostBack);
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
