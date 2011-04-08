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
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Represents the non-anonymous, foreign-key side of a bidirectional or unidirectional relationship.
  /// </summary>
  [Serializable]
  [DebuggerDisplay ("{GetType().Name}: {PropertyName}, Cardinality: {Cardinality}")]
  public class RelationEndPointDefinition : SerializableMappingObject, IRelationEndPointDefinition
  {
    // serialized member fields
    // Note: RelationEndPointDefinitions can only be serialized if they are part of the current mapping configuration. Only the fields listed below
    // will be serialized; these are used to retrieve the "real" object at deserialization time.

    private readonly string _serializedClassDefinitionID;
    private readonly string _serializedPropertyName;

    // nonserialized member fields

    [NonSerialized]
    private RelationDefinition _relationDefinition;

    [NonSerialized]
    private readonly ClassDefinition _classDefinition;

    [NonSerialized]
    private readonly PropertyDefinition _propertyDefinition;

    [NonSerialized]
    private readonly bool _isMandatory;

    public RelationEndPointDefinition (PropertyDefinition propertyDefinition, bool isMandatory)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      
      if (!propertyDefinition.IsObjectID)
      {
        throw CreateMappingException (
            "Relation definition error: Property '{0}' of class '{1}' is of type '{2}', but non-virtual properties must be of type '{3}'.",
            propertyDefinition.PropertyName,
            propertyDefinition.ClassDefinition.ID,
            propertyDefinition.PropertyType,
            typeof (ObjectID));
      }

      _classDefinition = propertyDefinition.ClassDefinition;
      _serializedClassDefinitionID = _classDefinition.ID;
      _isMandatory = isMandatory;
      _propertyDefinition = propertyDefinition;
      _serializedPropertyName = _propertyDefinition.PropertyName;
    }

    public bool CorrespondsTo (string classID, string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

      return (_classDefinition.ID == classID && PropertyName == propertyName);
    }

    public void SetRelationDefinition (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

      _relationDefinition = relationDefinition;
    }

    public RelationDefinition RelationDefinition
    {
      get { return _relationDefinition; }
    }

    public ClassDefinition ClassDefinition
    {
      get { return _classDefinition; }
    }

    public string PropertyName
    {
      get { return _propertyDefinition.PropertyName; }
    }

    public bool IsMandatory
    {
      get { return _isMandatory; }
    }

    public CardinalityType Cardinality
    {
      get { return CardinalityType.One; }
    }

    public Type PropertyType
    {
      get { return _propertyDefinition.PropertyType; }
    }

    public PropertyInfo PropertyInfo 
    {
      get { return _propertyDefinition.PropertyInfo; }
    }

    public bool IsPropertyInfoResolved 
    {
      get { return true; }
    }

    public bool IsVirtual
    {
      get { return false; }
    }

    public bool IsAnonymous
    {
      get { return false; }
    }

    public PropertyDefinition PropertyDefinition
    {
      get { return _propertyDefinition; }
    }

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (string.Format (message, args));
    }

    #region Serialization

    public override object GetRealObject (StreamingContext context)
    {
      // Note: A EndPointDefinition knows its ClassDefinition and a ClassDefinition implicitly knows 
      // its RelationEndPointDefinitions via its RelationDefinitions. For bi-directional relationships 
      // with two classes implementing IObjectReference.GetRealObject the order of calling this method is unpredictable.
      // Therefore the members _classDefinition and _propertyDefinition cannot be used here, because they could point to the wrong instances. 
      return MappingConfiguration.Current.ClassDefinitions.GetMandatory (_serializedClassDefinitionID).GetMandatoryRelationEndPointDefinition (_serializedPropertyName);
    }

    protected override bool IsPartOfMapping
    {
      get { return MappingConfiguration.Current.Contains (this); }
    }

    protected override string IDForExceptions
    {
      get { return PropertyName; }
    }

    #endregion
  }
}
