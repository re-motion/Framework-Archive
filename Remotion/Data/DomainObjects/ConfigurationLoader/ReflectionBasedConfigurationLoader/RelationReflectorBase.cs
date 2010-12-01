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
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Base class for reflecting on the relations of a class.</summary>
  public abstract class RelationReflectorBase : MemberReflectorBase
  {
    protected RelationReflectorBase (
        ReflectionBasedClassDefinition classDefinition,
        PropertyInfo propertyInfo,
        Type bidirectionalRelationAttributeType,
        IMappingNameResolver nameResolver)
        : base (propertyInfo, nameResolver)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom (
          "bidirectionalRelationAttributeType", bidirectionalRelationAttributeType, typeof (BidirectionalRelationAttribute));

      ClassDefinition = classDefinition;
      BidirectionalRelationAttribute =
          (BidirectionalRelationAttribute) AttributeUtility.GetCustomAttribute (PropertyInfo, bidirectionalRelationAttributeType, true);
      DeclaringMixin = ClassDefinition.GetPersistentMixin (PropertyInfo.DeclaringType);
      DeclaringDomainObjectTypeForProperty = ReflectionUtility.GetDeclaringDomainObjectTypeForProperty (PropertyInfo, ClassDefinition);

      CheckClassDefinitionType();
    }

    public ReflectionBasedClassDefinition ClassDefinition { get; private set; }
    public BidirectionalRelationAttribute BidirectionalRelationAttribute { get; private set; }
    public Type DeclaringMixin { get; private set; }

    /// <summary>
    /// Gives the type of DomainObject that originally declared the property.
    /// There are four cases:
    /// - If the <see cref="ClassDefinition"/>'s type itself declares the property, this returns the 
    ///   <see cref="ClassDefinition"/>'s type.
    /// - If a base class of the <see cref="ClassDefinition"/>'s type declares the property, this returns the base class. This can only 
    ///   happen if the <see cref="ClassDefinition"/> is the inheritance root and the base class is above the inheritance root. (The 
    ///   <see cref="ClassReflectorForRelations"/> will only create <see cref="RelationReflectorBase"/> objects for properties declared above the
    ///   <see cref="ClassDefinition"/>'s type if the <see cref="ClassDefinition"/> is an inheritance root.)
    /// - If a mixin applied to the <see cref="ClassDefinition"/>'s type declares the property, this returns the <see cref="ClassDefinition"/>'s type.
    /// - If a mixin applied to a base class of the <see cref="ClassDefinition"/>'s type declares the property, this returns the base class. Again,
    ///   this can only happen if the <see cref="ClassDefinition"/> is an inheritance root.
    /// </summary>
    public Type DeclaringDomainObjectTypeForProperty { get; private set; }

    protected bool IsBidirectionalRelation
    {
      get { return BidirectionalRelationAttribute != null; }
    }

    public bool IsMixedProperty
    {
      get { return DeclaringMixin != null; }
    }

    protected PropertyInfo GetOppositePropertyInfo ()
    {
      var type = ReflectionUtility.GetRelatedObjectTypeFromRelationProperty (PropertyInfo);
      var propertyFinder = new OppositePropertyFinder (
          BidirectionalRelationAttribute.OppositeProperty, 
          type, 
          true, 
          true, 
          NameResolver, 
          new PersistentMixinFinder (type, true));
      
      var properties = propertyFinder.FindPropertyInfos();
      // TODO Review 3484: Write a test for RelationReflector where the opposite property has the same name as a property in a base class. (The opposite property uses "new" to hide the base property.) The RelationDefinition should use the derived property, not the base property. The test should fail; use LastOrDefault to fix it.
      if (properties.Length > 0)
        return properties[0];
      else
        return null;
    }

    private void CheckClassDefinitionType ()
    {
      if (!PropertyInfo.DeclaringType.IsAssignableFrom (ClassDefinition.ClassType) && !IsMixedProperty)
      {
        string message = string.Format (
            "The classDefinition's class type '{0}' is not assignable to the property's declaring type.\r\nDeclaring type: {1}, property: {2}",
            ClassDefinition.ClassType,
            PropertyInfo.DeclaringType,
            PropertyInfo.Name);
        throw new ArgumentTypeException (message, null, ClassDefinition.ClassType, PropertyInfo.DeclaringType);
      }
    }
  }
}