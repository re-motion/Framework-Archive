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
using System.Web;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public class RenderingContext<TControl> : IRenderingContext
    where TControl : IControl
  {
    private readonly HttpContextBase _httpContext;
    private readonly HtmlTextWriter _writer;
    private readonly IStyledControl _control;

    public RenderingContext (HttpContextBase httpContext, HtmlTextWriter writer, IStyledControl control)
    {
      ArgumentUtility.CheckNotNull ("httpContext", httpContext);
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("control", control);

      _httpContext = httpContext;
      _writer = writer;
      _control = control;
    }

    public HttpContextBase HttpContext
    {
      get { return _httpContext;  }
    }

    public HtmlTextWriter Writer
    {
      get { return _writer; }
    }

    IStyledControl IRenderingContext.Control
    {
      get { return _control; }
    }

    public TControl Control { get { return (TControl) _control; } }
  }
}