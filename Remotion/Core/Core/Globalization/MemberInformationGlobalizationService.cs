// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Collections.Generic;
using Remotion.ExtensibleEnums;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Globalization
{
  public class MemberInformationGlobalizationService : IMemberInformationGlobalizationService
  {
    private readonly IGlobalizationService _globalizationService;
    private readonly IMemberInfoNameResolver _memberInfoNameResolver;

    public MemberInformationGlobalizationService (IEnumerable<IGlobalizationService> globalizationServices, IMemberInfoNameResolver memberInfoNameResolver)
    {
      ArgumentUtility.CheckNotNull ("globalizationServices", globalizationServices);
      ArgumentUtility.CheckNotNull ("memberInfoNameResolver", memberInfoNameResolver);

      //TODO AO: RM-5508
      _globalizationService = new CompoundGlobalizationService (globalizationServices);
      _memberInfoNameResolver = memberInfoNameResolver;
    }

    public string GetPropertyDisplayName (IPropertyInformation propertyInformation, ITypeInformation typeInformation)
    {
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);
      ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);

      return GetString (typeInformation, propertyInformation.Name, _memberInfoNameResolver.GetPropertyName (propertyInformation), "property:");
    }

    public string GetTypeDisplayName (ITypeInformation typeInformation)
    {
      ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);

      return GetString (typeInformation, typeInformation.Name, _memberInfoNameResolver.GetTypeName (typeInformation), "type:");
    }

    public string GetEnumerationValueDisplayName (Enum value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      return EnumDescription.GetDescription (value);
    }

    public string GetExtensibleEnumerationValueDisplayName (IExtensibleEnum value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      return value.GetLocalizedName();
    }

    private string GetString (ITypeInformation typeInformation, string shortMemberName, string longMemberName, string resourcePrefix)
    {
      var resourceManager = _globalizationService.GetResourceManager (typeInformation);

      return GetResourceString (resourceManager, resourcePrefix + longMemberName)
             ?? GetResourceString (resourceManager, resourcePrefix + shortMemberName)
             ?? shortMemberName;
    }

    private string GetResourceString (IResourceManager resourceManager, string longID)
    {
      if (resourceManager.ContainsResource (longID))
        return resourceManager.GetString (longID);
      return null;
    }
  }
}