﻿using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Web.Contract.DiagnosticMetadata;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="T:Remotion.Web.UI.Controls.ListMenu"/>.
  /// </summary>
  public class ListMenuControlObject : RemotionControlObject, IClickableItemsControlObject
  {
    public ListMenuControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public UnspecifiedPageObject ClickItem (string itemID, IActionBehavior actionBehavior = null)
    {
      var item = Scope.FindDMA ("span.listMenuItem", DiagnosticMetadataAttributes.ItemID, itemID);
      return ClickItem (item);
    }

    public UnspecifiedPageObject ClickItem (int index, IActionBehavior actionBehavior = null)
    {
      var item = FindChild ((index - 1).ToString());
      return ClickItem (item, actionBehavior);
    }

    public UnspecifiedPageObject ClickItemByHtmlID (string htmlID, IActionBehavior actionBehavior = null)
    {
      var item = Scope.FindId (htmlID);
      return ClickItem (item, actionBehavior);
    }

    public UnspecifiedPageObject ClickItemByText (string text, IActionBehavior actionBehavior = null)
    {
      var item = Scope.FindDMA ("span.listMenuItem", DiagnosticMetadataAttributes.Text, text);
      return ClickItem (item, actionBehavior);
    }

    private UnspecifiedPageObject ClickItem (ElementScope item, IActionBehavior actionBehavior = null)
    {
      var anchorScope = item.FindLink();

      var actualActionBehavior = GetActualActionBehavior (anchorScope, actionBehavior);
      anchorScope.ClickAndWait (Context, actualActionBehavior);
      return UnspecifiedPage();
    }
  }
}