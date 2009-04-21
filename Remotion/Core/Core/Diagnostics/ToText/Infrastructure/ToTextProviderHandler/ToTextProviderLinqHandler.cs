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
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler
{
  /// <summary>
  /// Handles types typically encountered when using Linq (e.g. <see cref="IGrouping{TKey,TElement}"/>.
  /// </summary>
  public class ToTextProviderLinqHandler : ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ToTextProviderHandler.CheckNotNull (toTextParameters, toTextProviderHandlerFeedback);

      Object obj = toTextParameters.Object;
      IToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;

      Type specificGroupingInterface = obj.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof (IGrouping<,>)).FirstOrDefault();
      if (specificGroupingInterface != null)
      {
        PropertyInfo keyProperty = specificGroupingInterface.GetProperty ("Key");
        var key = keyProperty.GetValue (obj, null);

        toTextBuilder.sb().e(key);
        toTextBuilder.WriteEnumerable ((IEnumerable) obj);
        toTextBuilder.se();
        toTextProviderHandlerFeedback.Handled = true;
      }
    }
  }
}