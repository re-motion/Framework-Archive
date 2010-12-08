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
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// The <see cref="ClassDefinitionCollectionFactory"/> is used to get a class definition for the specified class type. If the class definition does
  /// not already exists in the passed <see cref="ClassDefinitionCollection"/>, a new class definition is created and added to the collection.
  /// </summary>
  public class ClassDefinitionCollectionFactory
  {
    private readonly IMappingNameResolver _nameResolver;
    private readonly ReflectionBasedMappingObjectFactory _mappingObjectFactory;

    public ClassDefinitionCollectionFactory (IMappingNameResolver nameResolver)
    {
      ArgumentUtility.CheckNotNull ("nameResolver", nameResolver);

      _nameResolver = nameResolver;
      _mappingObjectFactory = new ReflectionBasedMappingObjectFactory (_nameResolver);
    }

    public ClassDefinitionCollection CreateClassDefinitionCollection (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("types", types);

      var inheritanceHierarchyFilter = new InheritanceHierarchyFilter (types.ToArray());
      var leafTypes = inheritanceHierarchyFilter.GetLeafTypes();

      var classDefinitions = new ClassDefinitionCollection();
      foreach (var type in leafTypes)
        GetClassDefinition (classDefinitions, type);
      
      var classesByBaseClass = (from classDefinition in classDefinitions.Cast<ClassDefinition>()
                                where classDefinition.BaseClass != null
                                group classDefinition by classDefinition.BaseClass)
          .ToDictionary (grouping => grouping.Key, grouping => (IEnumerable<ClassDefinition>) grouping);

      foreach (ClassDefinition classDefinition in classDefinitions)
      {
        IEnumerable<ClassDefinition> derivedClasses;
        if (!classesByBaseClass.TryGetValue (classDefinition, out derivedClasses))
          derivedClasses = Enumerable.Empty<ClassDefinition>();

        classDefinition.SetDerivedClasses (new ClassDefinitionCollection (derivedClasses, true, true));
      }

      return classDefinitions;
    }

    public ReflectionBasedClassDefinition GetClassDefinition (
        ClassDefinitionCollection classDefinitions, Type classType)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      if (classDefinitions.Contains (classType))
        return (ReflectionBasedClassDefinition) classDefinitions.GetMandatory (classType);

      var classDefinition =
          (ReflectionBasedClassDefinition)
          _mappingObjectFactory.CreateClassDefinition (classType, GetBaseClassDefinition (classDefinitions, classType));
      classDefinitions.Add (classDefinition);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition GetBaseClassDefinition (
        ClassDefinitionCollection classDefinitions, Type type)
    {
      if (ReflectionUtility.IsInheritanceRoot (type))
        return null;

      return GetClassDefinition (classDefinitions, type.BaseType);
    }
  }
}