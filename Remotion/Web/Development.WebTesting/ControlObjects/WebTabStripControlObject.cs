﻿using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for form grids created with <see cref="T:Remotion.Web.UI.Controls.WebTabStrip"/>.
  /// </summary>
  public class WebTabStripControlObject : RemotionControlObject, ITabStripControlObject
  {
    public WebTabStripControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public UnspecifiedPageObject SwitchTo (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var itemScope = Scope.FindDMA ("span.tabStripTab", DiagnosticMetadataAttributes.ItemID, itemID);
      return SwitchTo (itemScope);
    }

    public UnspecifiedPageObject SwitchTo (int index)
    {
      var xPathSelector = string.Format (
          "(.//span{0})[{1}]",
          XPathUtils.CreateHasOneOfClassesCheck ("tabStripTab", "tabStripTabSelected"),
          index);
      var itemScope = Scope.FindXPath (xPathSelector);
      return SwitchTo (itemScope);
    }

    public UnspecifiedPageObject SwitchToByHtmlID (string htmlID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var itemScope = Scope.FindId (htmlID);
      return SwitchTo (itemScope);
    }

    public UnspecifiedPageObject SwitchToByText (string text)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      var itemScope = Scope.FindDMA ("span.tabStripTab", DiagnosticMetadataAttributes.Text, text);
      return SwitchTo (itemScope);
    }

    private UnspecifiedPageObject SwitchTo (ElementScope tabScope)
    {
      var commandScope = tabScope.FindLink();

      var commandContext = Context.CloneForControl (commandScope);
      var command = new CommandControlObject (commandContext);
      return command.Click (Continue.When (Wxe.PostBackCompleted));
    }
  }
}