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
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Web.Legacy.UI.Controls.BocListImplementation.Rendering
{
  public class StubRowQuirksModeRenderer : IBocRowRenderer
  {
    public StubRowQuirksModeRenderer ()
    {
    }

    public void RenderTitlesRow (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, "titleStub");
      renderingContext.Writer.RenderBeginTag ("tr");
      renderingContext.Writer.RenderEndTag ();
    }

    public void RenderEmptyListDataRow (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, "emptyStub");
      renderingContext.Writer.RenderBeginTag ("tr");
      renderingContext.Writer.RenderEndTag();
    }

    public void RenderDataRow (BocListRenderingContext renderingContext, IBusinessObject businessObject, int rowIndex, int absoluteRowIndex, int originalRowIndex)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, "dataStub");
      renderingContext.Writer.RenderBeginTag ("tr");
      renderingContext.Writer.RenderEndTag();
    }
  }
}