// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections.Generic;
using System.Web.UI;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.Web;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  public abstract class RendererTestBase
  {
    protected IHttpContext HttpContext { get; set; }
    protected HtmlHelper Html { get; set; }
    protected IBocList List { get; set; }
    protected IBusinessObject BusinessObject { get; set; }
    protected BocListDataRowRenderEventArgs EventArgs { get; set; }

    [SetUp]
    public virtual void SetUp ()
    {
      TypeWithReference businessObject = TypeWithReference.Create (
          TypeWithReference.Create ("referencedObject1"),
          TypeWithReference.Create ("referencedObject2"));
      businessObject.ReferenceList = new[] { businessObject.FirstValue, businessObject.SecondValue };
      BusinessObject = (IBusinessObject) businessObject;
      BusinessObject.BusinessObjectClass.BusinessObjectProvider.AddService<IBusinessObjectWebUIService> (new ReflectionBusinessObjectWebUIService());

      EventArgs = new BocListDataRowRenderEventArgs (0, (IBusinessObject) businessObject.FirstValue);
      EventArgs.IsEditableRow = false;

      Html = new HtmlHelper();
      Html.InitializeStream();

      HttpContext = MockRepository.GenerateMock<IHttpContext>();
      IHttpResponse response = MockRepository.GenerateMock<IHttpResponse>();
      HttpContext.Stub (mock => mock.Response).Return (response);
      response.Stub (mock => mock.ContentType).Return ("text/html");
    }

    protected void InitializeMockList ()
    {
      List = MockRepository.GenerateMock<IBocList>();

      List.Stub (list => list.HasClientScript).Return (true);
      List.Stub (list => list.DataSource).Return (MockRepository.GenerateStub<IBusinessObjectDataSource>());
      List.DataSource.BusinessObject = BusinessObject;
      List.Stub (list => list.Property).Return (BusinessObject.BusinessObjectClass.GetPropertyDefinition ("ReferenceList"));

      List.Stub (list => list.Value).Return (((TypeWithReference) BusinessObject).ReferenceList);

      List.Stub (list => list.FixedColumns).Return (new BocColumnDefinitionCollection (List));
      List.Stub (list => list.IsColumnVisible (null)).IgnoreArguments().Return (true);

      List.Stub (list => list.SelectorControlCheckedState).Return (new List<int> ());

      List.Stub (list => list.CssClassTitleCell).Return ("cssClassTitleCell");
      List.Stub (list => list.CssClassTitleCellIndex).Return ("cssClassTitleCellIndex");
      List.Stub (list => list.CssClassSortingOrder).Return ("cssClassSortingOrder");
      List.Stub (list => list.CssClassContent).Return ("cssClassContent");
      List.Stub (list => list.CssClassDataCellOdd).Return ("cssClassDataCellOdd");
      List.Stub (list => list.CssClassDataCellEven).Return ("cssClassDataCellEven");
      List.Stub (list => list.CssClassDataRow).Return ("cssClassDataRow");
      
      var page = MockRepository.GenerateMock<IPage>();
      List.Stub (list => list.Page).Return (page);

      var clientScriptManager = MockRepository.GenerateMock<IClientScriptManager>();
      page.Stub (pageMock => pageMock.ClientScript).Return (clientScriptManager);

      clientScriptManager.Stub (scriptManagerMock => scriptManagerMock.GetPostBackEventReference (null, ""))
          .IgnoreArguments().Return ("postBackEventReference");

      var editModeController = MockRepository.GenerateMock<IEditModeController>();
      List.Stub (list => list.EditModeController).Return (editModeController);

      List.Stub (list => list.GetResourceManager()).Return (
          MultiLingualResources.GetResourceManager (typeof (ObjectBinding.Web.UI.Controls.BocList.ResourceIdentifier)));
    }

    protected void InitializeBocList ()
    {
      Page page = new Page();
      List = new BocListMock();
      page.Controls.Add (((BocListMock) List));

      List.DataSource = MockRepository.GenerateStub<IBusinessObjectDataSource>();
      List.DataSource.BusinessObject = BusinessObject;
      List.Property = BusinessObject.BusinessObjectClass.GetPropertyDefinition ("ReferenceList");

      ((BocListMock) List).Value = ((TypeWithReference) BusinessObject).ReferenceList;
    }
  }
}