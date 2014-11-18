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
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="TextBoxControlObject"/>.
  /// </summary>
  public class TextBoxSelector
      : ControlSelectorBase<TextBoxControlObject>,
          IFirstControlSelector<TextBoxControlObject>,
          IIndexControlSelector<TextBoxControlObject>,
          ISingleControlSelector<TextBoxControlObject>
  {
    private const string c_htmlTextBoxTag = "input";

    /// <inheritdoc/>
    public TextBoxControlObject SelectFirst (ControlSelectionContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var scope = context.Scope.FindTagWithAttribute (c_htmlTextBoxTag, "type", "text");
      return CreateControlObject (context, scope);
    }

    /// <inheritdoc/>
    public TextBoxControlObject SelectSingle (ControlSelectionContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var scope = context.Scope.FindTagWithAttribute (c_htmlTextBoxTag, "type", "text");
      scope.EnsureSingle();
      return CreateControlObject (context, scope);
    }

    /// <inheritdoc/>
    public TextBoxControlObject SelectPerIndex (ControlSelectionContext context, int index)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var hasAttributeCheck = XPathUtils.CreateHasAttributeCheck ("type", "text");
      var xPathSelector = string.Format ("(.//{0}{1})[{2}]", c_htmlTextBoxTag, hasAttributeCheck, index);
      var scope = context.Scope.FindXPath (xPathSelector);
      return CreateControlObject (context, scope);
    }

    /// <inheritdoc/>
    protected override TextBoxControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new TextBoxControlObject (newControlObjectContext);
    }
  }
}