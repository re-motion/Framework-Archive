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
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Retrieves the human-readable localized representation of reflection objects based on the <see cref="MultiLingualNameAttribute"/> 
  /// applied to the respective reflection object.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  public class MultiLingualNameBasedMemberInformationGlobalizationService : IMemberInformationGlobalizationService
  {
    public MultiLingualNameBasedMemberInformationGlobalizationService ()
    {
    }

    public bool TryGetTypeDisplayName (ITypeInformation typeInformation, ITypeInformation typeInformationForResourceResolution, out string result)
    {
      ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      var multLingualAttribute = GetLocalizedNameForCurrentUICulture (typeInformation);

      if (multLingualAttribute == null)
      {
        result = null;
        return false;
      }
      else
      {
        result = multLingualAttribute;
        return true;
      }
    }

    public bool TryGetPropertyDisplayName (
        IPropertyInformation propertyInformation,
        ITypeInformation typeInformationForResourceResolution,
        out string result)
    {
      throw new NotImplementedException();
    }

    [CanBeNull]
    private string GetLocalizedNameForCurrentUICulture (ITypeInformation typeInformation)
    {
      var attributes = typeInformation.GetCustomAttributes<MultiLingualNameAttribute> (false).ToDictionary (a => a.Culture, a => a.LocalizedName);

      foreach (var cultureInfo in CultureInfo.CurrentUICulture.GetCultureHierarchy())
      {
        string localizedName;
        if (attributes.TryGetValue (cultureInfo, out localizedName))
          return localizedName;
      }
      return null;
    }
  }
}