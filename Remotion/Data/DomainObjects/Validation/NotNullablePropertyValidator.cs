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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Validation
{
  /// <summary>
  /// Validates that not-nullable properties are not assigned a <see langword="null" /> value.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public class NotNullablePropertyValidator : IPersistableDataValidator, IDataContainerValidator
  {
    public NotNullablePropertyValidator ()
    {
    }

    public void Validate (PersistableData data)
    {
      ArgumentUtility.CheckNotNull ("data", data);

      if (data.DomainObjectState == StateType.Deleted)
        return;

      foreach (var propertyDefinition in data.DomainObject.ID.ClassDefinition.GetPropertyDefinitions())
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
      if (propertyDefinition.IsNullable)
        return;

      object propertyValue = dataContainer.GetValueWithoutEvents (propertyDefinition, ValueAccess.Current);
      if (propertyValue == null)
      {
        throw new PropertyValueNotSetException (
            domainObject,
            propertyDefinition.PropertyName,
            string.Format (
                "Not-nullable property '{0}' of domain object '{1}' cannot be null.",
                propertyDefinition.PropertyName,
                dataContainer.ID));
      }
    }
  }
}