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
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Rendering;
using Remotion.Utilities;
using Remotion.Web;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.Factories
{
  /// <summary>
  /// Responsible for creating the <see cref="BocBooleanValue"/> <see cref="BocBooleanValueResourceSet"/> for quirks mode rendering.
  /// </summary>
  public class BocBooleanValueQuirksModeResourceSetFactory : IBocBooleanValueResourceSetFactory
  {
    private const string c_trueIcon = "CheckBoxTrue.gif";
    private const string c_falseIcon = "CheckBoxFalse.gif";
    private const string c_nullIcon = "CheckBoxNull.gif";
    private const string c_defaultResourceGroup = "default";

    public BocBooleanValueResourceSet CreateResourceSet (HttpContextBase context, IBocBooleanValue control, IResourceUrlFactory resourceUrlFactory)
    {
      ArgumentUtility.CheckNotNull ("httpContext", context);
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNull ("resourceUrlFactory", resourceUrlFactory);

      return control.CreateResourceSet() ?? CreateDefaultResourceSet (context, control);
    }

    private BocBooleanValueResourceSet CreateDefaultResourceSet (HttpContextBase context, IBocBooleanValue control)
    {
      IResourceManager resourceManager = control.GetResourceManager();

      BocBooleanValueResourceSet resourceSet = new BocBooleanValueResourceSet (
          c_defaultResourceGroup,
          GetResourceUrl (control, context, c_trueIcon),
          GetResourceUrl (control, context, c_falseIcon),
          GetResourceUrl (control, context, c_nullIcon),
          resourceManager.GetString (BocBooleanValue.ResourceIdentifier.TrueDescription),
          resourceManager.GetString (BocBooleanValue.ResourceIdentifier.FalseDescription),
          resourceManager.GetString (BocBooleanValue.ResourceIdentifier.NullDescription)
          );

      return resourceSet;
    }

    private string GetResourceUrl (IBocBooleanValue control, HttpContextBase context, string icon)
    {
      return ResourceUrlResolver.GetResourceUrl (control, context, typeof (BocBooleanValueQuirksModeResourceSetFactory), ResourceType.Image, icon);
    }
  }
}