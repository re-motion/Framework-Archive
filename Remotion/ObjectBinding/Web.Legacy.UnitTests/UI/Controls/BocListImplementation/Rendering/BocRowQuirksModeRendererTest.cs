// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Legacy.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocRowQuirksModeRendererTest : BocListRendererTestBase
  {
    private BocListQuirksModeCssClassDefinition _bocListQuirksModeCssClassDefinition;
    private BocColumnRenderer[] _bocColumnRenderers;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      List.Stub (mock => mock.Selection).Return (RowSelection.Multiple);
      var stubColumnDefinition = new StubColumnDefinition();
      List.Stub (mock => mock.AreDataRowsClickSensitive()).Return (true);

      _bocColumnRenderers = new[]
                         {
                             new BocColumnRenderer (
                                 new StubColumnQuirksModeRenderer (),
                                 stubColumnDefinition,
                                 0,
                                 0,
                                 false,
                                 SortingDirection.Ascending,
                                 0)
                         };
      
      _bocListQuirksModeCssClassDefinition = new BocListQuirksModeCssClassDefinition();
    }

    [Test]
    public void RenderTitlesRow ()
    {
      IBocRowRenderer renderer = new BocRowQuirksModeRenderer (
          _bocListQuirksModeCssClassDefinition,
          new BocIndexColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition),
          new BocSelectorColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition));
      renderer.RenderTitlesRow (new BocListRenderingContext(HttpContext, Html.Writer, List, _bocColumnRenderers)); //use StubServiceLocator !??
      
      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement (document, "tr", 0);

      Html.GetAssertedChildElement (tr, "th", 0);
    }

    [Test]
    public void RenderTitlesRowWithIndex ()
    {
      List.Stub (mock => mock.IsIndexEnabled).Return (true);
      List.Stub (mock => mock.Index).Return (RowIndex.InitialOrder);

      IBocRowRenderer renderer = new BocRowQuirksModeRenderer (
          _bocListQuirksModeCssClassDefinition,
          new BocIndexColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition),
          new BocSelectorColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition));
      renderer.RenderTitlesRow (new BocListRenderingContext (HttpContext, Html.Writer, List, _bocColumnRenderers)); //use StubServiceLocator !??
      
      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement (document, "tr", 0);

      var thIndex = Html.GetAssertedChildElement (tr, "th", 0);
      Html.AssertAttribute (thIndex, "class", _bocListQuirksModeCssClassDefinition.TitleCell, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertAttribute (thIndex, "class", _bocListQuirksModeCssClassDefinition.TitleCellIndex, HtmlHelperBase.AttributeValueCompareMode.Contains);

      Html.GetAssertedChildElement (tr, "th", 1);
    }

    [Test]
    public void RenderTitlesRowWithSelector ()
    {
      List.Stub (mock => mock.IsSelectionEnabled).Return (true);
      List.Stub (mock => mock.Selection).Return (RowSelection.Multiple);

      IBocRowRenderer renderer = new BocRowQuirksModeRenderer (
          _bocListQuirksModeCssClassDefinition,
          new BocIndexColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition),
          new BocSelectorColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition));
      renderer.RenderTitlesRow (new BocListRenderingContext (HttpContext, Html.Writer, List, _bocColumnRenderers)); //use StubServiceLocator !??


      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement (document, "tr", 0);

      Html.GetAssertedChildElement (tr, "th", 0);

      Html.GetAssertedChildElement (tr, "th", 1);
    }

    [Test]
    public void RenderDataRow ()
    {
      IBocRowRenderer renderer = new BocRowQuirksModeRenderer (
          _bocListQuirksModeCssClassDefinition,
          new BocIndexColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition),
          new BocSelectorColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition));
      renderer.RenderDataRow (
          new BocListRenderingContext (HttpContext, Html.Writer, List, _bocColumnRenderers),
          new BocListRowRenderingContext (new BocListRow (0, BusinessObject), 1, true),
          0); //use StubServiceLocator !??

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement (document, "tr", 0);
      Html.AssertAttribute (tr, "class", _bocListQuirksModeCssClassDefinition.DataRowSelected);

      Html.GetAssertedChildElement (tr, "td", 0);
    }

    [Test]
    public void RenderDataRowSelected ()
    {
      IBocRowRenderer renderer = new BocRowQuirksModeRenderer (
          _bocListQuirksModeCssClassDefinition,
          new BocIndexColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition),
          new BocSelectorColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition));
      renderer.RenderDataRow (
          new BocListRenderingContext (HttpContext, Html.Writer, List, _bocColumnRenderers),
          new BocListRowRenderingContext (new BocListRow (0, BusinessObject), 1, true),
          0); //use StubServiceLocator !??

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement (document, "tr", 0);
      Html.AssertAttribute (tr, "class", _bocListQuirksModeCssClassDefinition.DataRowSelected);

      Html.GetAssertedChildElement (tr, "td", 0);
    }

    [Test]
    public void RenderEmptyDataRow ()
    {
      List.Stub (mock => mock.IsIndexEnabled).Return (true);
      List.Stub (mock => mock.IsSelectionEnabled).Return (true);

      IBocRowRenderer renderer = new BocRowQuirksModeRenderer (
          _bocListQuirksModeCssClassDefinition,
          new BocIndexColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition),
          new BocSelectorColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition));
      renderer.RenderEmptyListDataRow (new BocListRenderingContext (HttpContext, Html.Writer, List, _bocColumnRenderers));

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement (document, "tr", 0);

      Html.GetAssertedChildElement (tr, "td", 0);
    }
  }
}