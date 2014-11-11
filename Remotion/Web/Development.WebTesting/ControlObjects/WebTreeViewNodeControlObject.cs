﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a node within a <see cref="T:Remotion.Web.UI.Controls.WebTreeView"/>.
  /// </summary>
  public class WebTreeViewNodeControlObject : WebFormsControlObject, IControlObjectWithNodes<WebTreeViewNodeControlObject>
  {
    public WebTreeViewNodeControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public string GetText ()
    {
      return Scope[DiagnosticMetadataAttributes.Text];
    }

    public bool IsSelected ()
    {
      return Scope[DiagnosticMetadataAttributes.WebTreeViewIsSelectedNode] != null;
    }

    public int GetNumberOfChildren ()
    {
      return int.Parse (Scope[DiagnosticMetadataAttributes.WebTreeViewChildren]);
    }

    public IControlObjectWithNodes<WebTreeViewNodeControlObject> GetNode ()
    {
      return this;
    }

    public WebTreeViewNodeControlObject GetNode (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return GetNode().WithItemID (itemID);
    }

    WebTreeViewNodeControlObject IControlObjectWithNodes<WebTreeViewNodeControlObject>.WithItemID (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var nodeScope = Scope.FindDMA ("ul li", DiagnosticMetadataAttributes.ItemID, itemID);
      return new WebTreeViewNodeControlObject (Context.CloneForControl (nodeScope));
    }

    WebTreeViewNodeControlObject IControlObjectWithNodes<WebTreeViewNodeControlObject>.WithIndex (int index)
    {
      var nodeScope = Scope.FindDMA ("ul li", DiagnosticMetadataAttributes.IndexInCollection, index.ToString());
      return new WebTreeViewNodeControlObject (Context.CloneForControl (nodeScope));
    }

    WebTreeViewNodeControlObject IControlObjectWithNodes<WebTreeViewNodeControlObject>.WithText (string text)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      var nodeScope = Scope.FindDMA ("ul li", DiagnosticMetadataAttributes.Text, text);
      return new WebTreeViewNodeControlObject (Context.CloneForControl (nodeScope));
    }

    public WebTreeViewNodeControlObject Expand ()
    {
      var actualCompletionDetector = GetActualCompletionDetector (null);

      var expandAnchorScope = Scope.FindDMA (
          "span a",
          DiagnosticMetadataAttributes.WebTreeViewWellKnownAnchor,
          DiagnosticMetadataAttributeValues.WebTreeViewWellKnownExpandAnchor);
      expandAnchorScope.ClickAndWait (Context, actualCompletionDetector);
      return this;
    }

    public WebTreeViewNodeControlObject Collapse ()
    {
      var actualCompletionDetector = GetActualCompletionDetector (null);

      var collapseAnchorScope = Scope.FindDMA (
          "span a",
          DiagnosticMetadataAttributes.WebTreeViewWellKnownAnchor,
          DiagnosticMetadataAttributeValues.WebTreeViewWellKnownCollapseAnchor);
      collapseAnchorScope.ClickAndWait (Context, actualCompletionDetector);
      return this;
    }

    public WebTreeViewNodeControlObject Select ([CanBeNull] ICompletionDetection completionDetection = null)
    {
      ClickNode (completionDetection);
      return this;
    }

    public UnspecifiedPageObject Click ([CanBeNull] ICompletionDetection completionDetection = null)
    {
      ClickNode (completionDetection);
      return UnspecifiedPage();
    }

    private void ClickNode (ICompletionDetection completionDetection)
    {
      var actualCompletionDetector = GetActualCompletionDetector (completionDetection);

      var selectAnchorScope = GetWellKnownSelectAnchorScope();
      selectAnchorScope.ClickAndWait (Context, actualCompletionDetector);
    }

    public ContextMenuControlObject OpenContextMenu ()
    {
      var selectAnchorScope = GetWellKnownSelectAnchorScope();
      return new ContextMenuControlObject (Context.CloneForControl (selectAnchorScope));
    }

    private ElementScope GetWellKnownSelectAnchorScope ()
    {
      return Scope.FindDMA (
          "span a",
          DiagnosticMetadataAttributes.WebTreeViewWellKnownAnchor,
          DiagnosticMetadataAttributeValues.WebTreeViewWellKnownSelectAnchor);
    }

    protected override ICompletionDetection GetDefaultCompletionDetection (ElementScope scope)
    {
      return Continue.When (Wxe.PostBackCompleted);
    }
  }
}