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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.UI.Controls.Rendering.WebTabStrip;

namespace Remotion.Web.UI.Controls.Rendering.TabbedMultiView
{
  public interface ITabbedMultiView : IStyledControl
  {
    bool IsDesignMode { get; }
    string CssClassBase { get; }
    string CssClassTabStrip { get; }
    string CssClassActiveView { get; }
    string ActiveViewClientID { get; }
    Style ActiveViewStyle { get; }
    string CssClassViewBody { get; }
    string CssClassContent { get; }
    Style TopControlsStyle { get; }
    string CssClassTopControls { get; }
    PlaceHolder TopControl { get; }
    Style BottomControlsStyle { get; }
    PlaceHolder BottomControl { get; }
    string CssClassBottomControls { get; }
    string CssClassEmpty { get; }
    IWebTabStrip TabStrip { get; }
    Control GetActiveView ();
  }
}