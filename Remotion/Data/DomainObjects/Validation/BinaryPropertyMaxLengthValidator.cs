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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Validation
{
  /// <summary>
  /// Validates that a binary property's value does not exceed the maximum length defined for this property.
  /// </summary>
  public class BinaryPropertyMaxLengthValidator : IPersistableDataValidator, IDataContainerValidator
  {
    public BinaryPropertyMaxLengthValidator ()
    {
    }

    public void Validate (PersistableData data)
    {
      ArgumentUtility.CheckNotNull ("data", data);

      if (data.DomainObjectState == StateType.Deleted)
        return;

      foreach (var propertyDefinition in data.DataContainer.ID.ClassDefinition.GetPropertyDefinitions())
        ValidatePropertyDefinition (data.DomainObject, data.DataContainer, propertyDefinition);
    }

    public void Validate (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      foreach (var propertyDefinition in dataContainer.ID.ClassDefinition.GetPropertyDefinitions())
        ValidatePropertyDefinition (null, dataContainer, propertyDefinition);
    }

    private static void ValidatePropertyDefinition (DomainObject domainObject, DataContainer dataContainer, PropertyDefinition propertyDefinition)
    {
      var maxLength = propertyDefinition.MaxLength;
      if (maxLength == null)
        return;

      Type propertyType = propertyDefinition.PropertyType;
      if (propertyType != typeof (byte[]))
        return;

      object propertyValue = dataContainer.GetValueWithoutEvents (propertyDefinition, ValueAccess.Current);
      if (propertyValue == null)
        return;

      if (propertyType == typeof (byte[]) && ((byte[]) propertyValue).Length > maxLength.Value)
      {
        string message = string.Format (
            "Value for property '{0}' of domain object '{1}' is too large. Maximum size: {2}.",
            propertyDefinition.PropertyName,
            dataContainer.ID,
            maxLength.Value);

        throw new PropertyValueTooLongException (domainObject, propertyDefinition.PropertyName, maxLength.Value, message);
      }
    }
  }
}