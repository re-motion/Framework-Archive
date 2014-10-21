﻿using System;
using ActaNova.WebTesting.Infrastructure;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a node within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocTreeView"/>.
  /// </summary>
  public class ActaNovaTreeNodeControlObject : ActaNovaControlObject, IActaNovaTreeNodeNavigator
  {
    private readonly BocTreeViewNodeControlObject _bocTreeViewNode;

    [UsedImplicitly]
    public ActaNovaTreeNodeControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
      _bocTreeViewNode = new BocTreeViewNodeControlObject (id, context);
    }

    internal ActaNovaTreeNodeControlObject ([NotNull] BocTreeViewNodeControlObject bocTreeViewNode)
        : base (bocTreeViewNode.ID, bocTreeViewNode.Context)
    {
      _bocTreeViewNode = bocTreeViewNode;
    }

    public string Text
    {
      get { return _bocTreeViewNode.Text; }
    }

    public ActaNovaTreeNodeControlObject GetNode (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var bocTreeViewNode = _bocTreeViewNode.GetNode (itemID);
      return new ActaNovaTreeNodeControlObject (bocTreeViewNode);
    }

    public ActaNovaTreeNodeControlObject GetNode (int index)
    {
      var bocTreeViewNode = _bocTreeViewNode.GetNode (index);
      return new ActaNovaTreeNodeControlObject (bocTreeViewNode);
    }

    public ActaNovaTreeNodeControlObject GetNodeByHtmlID (string htmlID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var bocTreeViewNode = _bocTreeViewNode.GetNodeByHtmlID (htmlID);
      return new ActaNovaTreeNodeControlObject (bocTreeViewNode);
    }

    public ActaNovaTreeNodeControlObject GetNodeByText (string text)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      var bocTreeViewNode = _bocTreeViewNode.GetNodeByText (text);
      return new ActaNovaTreeNodeControlObject (bocTreeViewNode);
    }

    public ActaNovaTreeNodeControlObject Expand ()
    {
      var bocTreeViewNode = _bocTreeViewNode.Expand();
      return new ActaNovaTreeNodeControlObject (bocTreeViewNode);
    }

    public ActaNovaTreeNodeControlObject Collapse ()
    {
      var bocTreeViewNode = _bocTreeViewNode.Collapse();
      return new ActaNovaTreeNodeControlObject (bocTreeViewNode);
    }

    public UnspecifiedPageObject Select ([CanBeNull] IActionBehavior actionBehavior = null)
    {
      var actualActionBehavior = actionBehavior ?? Behavior.WaitFor (WaitForActaNova.OuterInnerOuterUpdate);
      _bocTreeViewNode.Select(actualActionBehavior);

      return UnspecifiedPage();
    }

    public ContextMenuControlObject OpenContextMenu ()
    {
      return _bocTreeViewNode.OpenContextMenu();
    }
  }
}