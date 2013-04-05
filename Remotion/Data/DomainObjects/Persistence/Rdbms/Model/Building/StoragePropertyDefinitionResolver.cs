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
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// The <see cref="StoragePropertyDefinitionResolver"/> is responsible to get all <see cref="IRdbmsStoragePropertyDefinition"/> instances for a 
  /// <see cref="ClassDefinition"/>.
  /// </summary>
  public class StoragePropertyDefinitionResolver : IStoragePropertyDefinitionResolver
  {
    private readonly IRdbmsPersistenceModelProvider _persistenceModelProvider;

    public StoragePropertyDefinitionResolver (IRdbmsPersistenceModelProvider persistenceModelProvider)
    {
      ArgumentUtility.CheckNotNull ("persistenceModelProvider", persistenceModelProvider);
      _persistenceModelProvider = persistenceModelProvider;
    }

    public IRdbmsPersistenceModelProvider PersistenceModelProvider
    {
      get { return _persistenceModelProvider; }
    }

    public IEnumerable<IRdbmsStoragePropertyDefinition> GetStoragePropertiesForHierarchy (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var allClassesInHierarchy = classDefinition
          .CreateSequence (cd => cd.BaseClass)
          .Reverse ()
          .Concat (classDefinition.GetAllDerivedClasses ());

      var storageProperties =
          from cd in allClassesInHierarchy
          from PropertyDefinition pd in cd.MyPropertyDefinitions
          where pd.StorageClass == StorageClass.Persistent
          group _persistenceModelProvider.GetStoragePropertyDefinition (pd) by pd.PropertyInfo into storagePropertyGroup
          select Unify (storagePropertyGroup);

      return storageProperties;
    }

    private IRdbmsStoragePropertyDefinition Unify (IGrouping<IPropertyInformation, IRdbmsStoragePropertyDefinition> storagePropertyGroup)
    {
      var numberOfStorageProperties = storagePropertyGroup.Count();
      Assertion.IsTrue (numberOfStorageProperties > 0);
      if (numberOfStorageProperties == 1)
        return storagePropertyGroup.Single();
      else
      {
        // It is possible to have multiple StoragePropertyDefinitions for the same C# property when a mixin adds the same property to different
        // places within an inheritance hierarchy. In that case, we now need to choose one of those. We can assume that all storage properties 
        // are equivalent (same storage property type, equivalent columns, etc.). The only difference could be that non-nullable properties might be 
        // nullable due to mapping constraints. (See ValueStoragePropertyDefinitionFactory.MustBeNullable.)
        // TODO 5512: Choosing the first one works for now because the nullability of properties resulting from such a property group with different 
        // TODO 5512: inheritance requirements is not evaluated. But it would be better to generate a "combined" property.
        // TODO 5512: Therefore, add an IRdbmsStoragePropertyDefinition.CombineEquivalent method that combines multiple storage properties into a single one.

        return storagePropertyGroup.First();
      }
    }
  }
}