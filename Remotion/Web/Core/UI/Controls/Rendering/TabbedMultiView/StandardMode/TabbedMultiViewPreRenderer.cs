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
using Remotion.Web.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.Rendering.TabbedMultiView.StandardMode
{
  public class TabbedMultiViewPreRenderer : PreRendererBase<ITabbedMultiView>, ITabbedMultiViewPreRenderer
  {
    public TabbedMultiViewPreRenderer (IHttpContext context, ITabbedMultiView control)
        : base (context, control)
    {
    }

    public override void PreRender ()
    {
      if (Control is Control)
      {
        Control control = (Control) Control;
        ScriptUtility.RegisterElementForBorderSpans (control, Control.ActiveViewClientID, true);
        ScriptUtility.RegisterElementForBorderSpans (control, Control.TopControl.ClientID, true);
        ScriptUtility.RegisterElementForBorderSpans (control, Control.BottomControl.ClientID, true);
      }

      string keyStyle = typeof (ITabbedMultiView).FullName + "_Style";
      string keyScript = typeof (ITabbedMultiView).FullName + "_Script";
      if (!HtmlHeadAppender.Current.IsRegistered (keyStyle))
      {
        string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (ITabbedMultiView), ResourceType.Html, ResourceTheme.Standard, "TabbedMultiView.css");
        HtmlHeadAppender.Current.RegisterStylesheetLink (keyStyle, styleSheetUrl, HtmlHeadAppender.Priority.Library);

        string scriptFileUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (ITabbedMultiView), ResourceType.Html, ResourceTheme.Standard, "ViewLayout.js");
        HtmlHeadAppender.Current.RegisterJavaScriptInclude (keyScript, scriptFileUrl);

        string keyAdjust = Control.ClientID + "_AdjustView";
        string scriptAdjust =
            @"function adjustView_{0}()
{{
  var container = document.getElementById('{0}');
  var topControl = document.getElementById('{1}');
  var view = document.getElementById('{2}');
  ViewLayout.AdjustWidth(container);
  ViewLayout.AdjustTop(container, topControl); 
  ViewLayout.Adjust(container, view);
}}";
        scriptAdjust = string.Format (scriptAdjust, Control.ClientID, Control.TabStripContainerClientID, Control.ActiveViewClientID);
        HtmlHeadAppender.Current.RegisterJavaScriptBlock (keyAdjust, scriptAdjust);
      }

      HtmlHeadAppender.Current.RegisterJQueryJavaScriptInclude (Control.Page);

      string bindScript =
    @"$(document).ready( function(){{ 
  $(window).bind('resize', function(){{ 
    adjustView_{0}(); 
  }}); 
  setTimeout('adjustView_{0}();', 10); 
}} );";
      bindScript = string.Format (bindScript, Control.ClientID);
      Control.Page.ClientScript.RegisterStartupScriptBlock (Control, Control.ClientID + "_BindViewResize", bindScript);
    }
  }
}