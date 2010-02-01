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
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.Linq.Backend;
using Remotion.Data.Linq.Backend.DataObjectModel;
using Remotion.Data.Linq.Clauses;
using ArgumentUtility=Remotion.Utilities.ArgumentUtility;


namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// The implementation of <see cref="IDatabaseInfo"/>  for getting detailed information from re-store which is the underlying data source.
  /// </summary>
  public class DatabaseInfo : IDatabaseInfo
  {
    public static readonly DatabaseInfo Instance = new DatabaseInfo();

    private DatabaseInfo ()
    {
    }

    public Table GetTableForFromClause (FromClauseBase fromClause)
    {
      ArgumentUtility.CheckNotNull ("fromClause", fromClause);
      
      Type domainObjectType = fromClause.ItemType;
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[domainObjectType];
      if (classDefinition == null)
      {
        string message = string.Format (
            "The from clause with identifier '{0}' and item type '{1}' does not identify a queryable table.",
            fromClause.ItemName,
            fromClause.ItemType.FullName);
        throw new UnmappedItemException (message);
      }
      else
      {
        var viewName = classDefinition.GetViewName();
        return new Table (viewName, fromClause.ItemName);
      }
    }

    public bool IsTableType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[type];
      if (classDefinition == null)
        return false;
      else
        return true;
    }

    public string GetRelatedTableName (MemberInfo relationMember)
    {
      ArgumentUtility.CheckNotNull ("relationMember", relationMember);

      Tuple<RelationDefinition, ClassDefinition, string> relationData = GetRelationData (relationMember);
      if (relationData == null)
        return null;

      RelationDefinition relationDefinition = relationData.A;
      ClassDefinition classDefinition = relationData.B;
      string propertyIdentifier = relationData.C;

      return relationDefinition.GetOppositeClassDefinition (classDefinition.ID, propertyIdentifier).GetViewName();
    }

    public string GetColumnName (MemberInfo member)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      PropertyInfo property = member as PropertyInfo;
      if (property == null)
        return null;

      if (property.Name == "ID" && property.DeclaringType == typeof (DomainObject))
        return "ID";

      //fix this if something is not in the MappingConfiguration -> go to derived classes (entity <T>) and look for property
      //new parameter needed to get type (<T>) of entity<T>
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[property.DeclaringType];
      if (classDefinition == null)
        return null;

      string propertyIdentifier = MappingConfiguration.Current.NameResolver.GetPropertyName (property);
      PropertyDefinition propertyDefinition = classDefinition.GetPropertyDefinition (propertyIdentifier);

      if (propertyDefinition != null)
        return propertyDefinition.StorageSpecificName;
      else
        return null;
    }

    public JoinColumnNames? GetJoinColumnNames (MemberInfo relationMember)
    {
      ArgumentUtility.CheckNotNull ("relationMember", relationMember);

      Tuple<RelationDefinition, ClassDefinition, string> relationData = GetRelationData (relationMember);
      if (relationData == null)
        return null;

      RelationDefinition relationDefinition = relationData.A;
      ClassDefinition classDefinition = relationData.B;
      string propertyIdentifier = relationData.C;

      IRelationEndPointDefinition leftEndPoint = relationDefinition.GetEndPointDefinition (classDefinition.ID, propertyIdentifier);
      IRelationEndPointDefinition rightEndPoint = relationDefinition.GetOppositeEndPointDefinition (leftEndPoint);
     
      string leftColumn = GetJoinColumn (leftEndPoint);
      string rightColumn = GetJoinColumn (rightEndPoint);

      return new JoinColumnNames (leftColumn, rightColumn);
    }

    public object ProcessWhereParameter (object parameter)
    {
      DomainObject domainObject = parameter as DomainObject;
      if (domainObject != null)
        return domainObject.ID;
      return parameter;
    }

    public MemberInfo GetPrimaryKeyMember (Type entityType)
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[entityType];
      if (classDefinition == null)
        return null;
      else
        return typeof (DomainObject).GetProperty ("ID");
    }
    

    private Tuple<RelationDefinition, ClassDefinition, string> GetRelationData (MemberInfo relationMember)
    {
      ArgumentUtility.CheckNotNull ("relationMember", relationMember);
      PropertyInfo property = relationMember as PropertyInfo;
      if (property == null)
        return null;

      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[property.DeclaringType];
      if (classDefinition == null)
        return null;

      string propertyIdentifier = MappingConfiguration.Current.NameResolver.GetPropertyName (property);
      RelationDefinition relationDefinition = classDefinition.GetRelationDefinition (propertyIdentifier);
      if (relationDefinition == null)
        return null;
      else
        return Tuple.NewTuple (relationDefinition, classDefinition, propertyIdentifier);
    }

    private string GetJoinColumn (IRelationEndPointDefinition endPoint)
    {
      ClassDefinition classDefinition = endPoint.ClassDefinition;
      return endPoint.IsVirtual ? "ID" : classDefinition.GetMandatoryPropertyDefinition (endPoint.PropertyName).StorageSpecificName;
    }
  }
}
