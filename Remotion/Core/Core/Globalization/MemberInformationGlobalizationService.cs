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
using Remotion.ExtensibleEnums;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Globalization
{
  public class MemberInformationGlobalizationService : IMemberInformationGlobalizationService
  {
    private readonly IGlobalizationService _globalizationService;
    private readonly IMemberInfoNameResolver _memberInfoNameResolver;

    public MemberInformationGlobalizationService (ICompoundGlobalizationService globalizationService, IMemberInfoNameResolver memberInfoNameResolver)
    {
      ArgumentUtility.CheckNotNull ("globalizationService", globalizationService);
      ArgumentUtility.CheckNotNull ("memberInfoNameResolver", memberInfoNameResolver);

      _globalizationService = globalizationService;
      _memberInfoNameResolver = memberInfoNameResolver;
    }

    public string GetPropertyDisplayName (IPropertyInformation propertyInformation, ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      return GetString (typeInformationForResourceResolution, propertyInformation.Name, _memberInfoNameResolver.GetPropertyName (propertyInformation), "property:");
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
      //TODO AO: GetStringOrDefault
      if (resourceManager.ContainsResource (longID))
        return resourceManager.GetString (longID);
      return null;
    }
  }
}