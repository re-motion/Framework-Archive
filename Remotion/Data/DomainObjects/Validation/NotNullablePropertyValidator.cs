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

using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Validation
{
  public class NotNullablePropertyValidator : IPersistableDataValidator
  {
    public void Validate (PersistableData data)
    {
      ArgumentUtility.CheckNotNull ("data", data);

      if (data.DomainObjectState == StateType.Deleted)
        return;

      foreach (var propertyDefinition in data.DomainObject.ID.ClassDefinition.GetPropertyDefinitions())
      {
        if (!propertyDefinition.IsNullable && data.DataContainer.GetValueWithoutEvents (propertyDefinition, ValueAccess.Current) == null)
        {
          throw new NotNullablePropertyValueNotSetException (
              data.DomainObject,
              propertyDefinition.PropertyName,
              string.Format (
                  "Not-nullable property '{0}' of domain object '{1}' cannot be null.",
                  propertyDefinition.PropertyName,
                  data.DomainObject.ID));
        }
      }
    }
  }
}