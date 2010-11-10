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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="RelationDefinition"/> from a <see cref="PropertyInfo"/>.</summary>
  public class RelationReflector : RelationReflectorBase
  {
    public static RelationReflector CreateRelationReflector (
        ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo, IMappingNameResolver nameResolver)
    {
      return new RelationReflector (classDefinition, propertyInfo, nameResolver);
    }

    public RelationReflector (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo, IMappingNameResolver nameResolver)
        : this (classDefinition, propertyInfo, typeof (BidirectionalRelationAttribute), nameResolver)
    {
    }

    protected RelationReflector (
        ReflectionBasedClassDefinition classDefinition,
        PropertyInfo propertyInfo,
        Type bidirectionalRelationAttributeType, IMappingNameResolver nameResolver)
        : base (classDefinition, propertyInfo, bidirectionalRelationAttributeType, nameResolver)
    {
    }

    public RelationDefinition GetMetadata (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      
      RelationEndPointReflector relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector (ClassDefinition, PropertyInfo, NameResolver);

      var firstEndPoint = relationEndPointReflector.GetMetadata ();
      var secondEndPoint = CreateOppositeEndPointDefinition (classDefinitions);
      var relationID = GetRelationID (firstEndPoint, secondEndPoint);
      return new RelationDefinition (relationID, firstEndPoint, secondEndPoint);
    }

    //TODO 3424: create rule
    protected void ValidateOppositePropertyInfo (PropertyInfo oppositePropertyInfo, ClassDefinitionCollection classDefintions)
    {
      ArgumentUtility.CheckNotNull ("oppositePropertyInfo", oppositePropertyInfo);
      ArgumentUtility.CheckNotNull ("classDefintions", classDefintions);

      Type oppositeDomainObjectType = GetDomainObjectTypeFromRelationProperty (oppositePropertyInfo);
      if (classDefintions.Contains (DeclaringDomainObjectTypeForProperty))
      {
        if (DeclaringDomainObjectTypeForProperty != oppositeDomainObjectType)
        {
          throw CreateMappingException (
              null,
              PropertyInfo,
              "The declaring type '{0}' does not match the type of the opposite relation propery '{1}' declared on type '{2}'.\r\n",
              DeclaringDomainObjectTypeForProperty.Name,
              BidirectionalRelationAttribute.OppositeProperty,
              oppositePropertyInfo.DeclaringType.Name);
        }
      }
      else
      {
        if (DeclaringDomainObjectTypeForProperty.IsAssignableFrom (oppositeDomainObjectType))
          return;

        throw CreateMappingException (
            null,
            PropertyInfo,
            "The declaring type '{0}' cannot be assigned to the type of the opposite relation propery '{1}' declared on type '{2}'.\r\n",
            DeclaringDomainObjectTypeForProperty.Name,
            BidirectionalRelationAttribute.OppositeProperty,
            oppositePropertyInfo.DeclaringType.Name);
      }
    }

    private string GetRelationID (IRelationEndPointDefinition first, IRelationEndPointDefinition second)
    {
      bool isFirstEndPointReal = !first.IsVirtual && !first.IsAnonymous;
      var endPoints = isFirstEndPointReal ? new { Left = first, Right = second } : new { Left = second, Right = first };
      var nameGivingEndPoint = endPoints.Left;
      var leftPropertyName = NameResolver.GetPropertyName (new PropertyInfoAdapter (nameGivingEndPoint.PropertyInfo));
      if (endPoints.Right.IsAnonymous)
      {
        return string.Format ("{0}:{1}", nameGivingEndPoint.ClassDefinition.ClassType.FullName, leftPropertyName);
      }
      else
      {
        var rightPropertyName = NameResolver.GetPropertyName (new PropertyInfoAdapter (endPoints.Right.PropertyInfo));
        return string.Format ("{0}:{1}->{2}", nameGivingEndPoint.ClassDefinition.ClassType.FullName, leftPropertyName, rightPropertyName);
      }
    }

    private IRelationEndPointDefinition CreateOppositeEndPointDefinition (ClassDefinitionCollection classDefinitions)
    {
      if (!IsBidirectionalRelation)
        return CreateOppositeAnonymousRelationEndPointDefinition (classDefinitions);

      RelationEndPointReflector oppositeRelationEndPointReflector = CreateOppositeRelationEndPointReflector (classDefinitions);
      return oppositeRelationEndPointReflector.GetMetadata ();
    }

    //TODO 3424: create rule ??
    private AnonymousRelationEndPointDefinition CreateOppositeAnonymousRelationEndPointDefinition (ClassDefinitionCollection classDefinitions)
    {
      try
      {
        return new AnonymousRelationEndPointDefinition (classDefinitions.GetMandatory (PropertyInfo.PropertyType));
      }
      catch (MappingException e)
      {
        throw CreateMappingException (null, PropertyInfo, e.Message);
      }
    }

    //TODO 3424: create rule ??
    private RelationEndPointReflector CreateOppositeRelationEndPointReflector (ClassDefinitionCollection classDefinitions)
    {
      PropertyInfo oppositePropertyInfo = GetOppositePropertyInfo();
      ValidateOppositePropertyInfo (oppositePropertyInfo, classDefinitions);

      ReflectionBasedClassDefinition classDefinition;
      try
      {
        classDefinition = (ReflectionBasedClassDefinition) classDefinitions.GetMandatory (GetDomainObjectTypeFromRelationProperty (PropertyInfo));
      }
      catch (MappingException e)
      {
        throw CreateMappingException (null, oppositePropertyInfo, e.Message);
      }

      return RelationEndPointReflector.CreateRelationEndPointReflector (classDefinition, oppositePropertyInfo, NameResolver);
    }
  }
}
