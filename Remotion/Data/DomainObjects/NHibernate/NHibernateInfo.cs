// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh 
// All rights reserved.
//  
using System;
using System.Reflection;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.NHibernate
{
  public class NHibernateInfo : IDatabaseInfo
  {
    #region Implementation of IDatabaseInfo

    public string GetTableName (FromClauseBase fromClause)
    {
      ArgumentUtility.CheckNotNull ("fromClause", fromClause);
      return fromClause.Identifier.Type.Name;
    }

    public string GetRelatedTableName (MemberInfo relationMember)
    {
      //Tuple<RelationDefinition, ClassDefinition, string> relationData = GetRelationData (relationMember);
      //if (relationData == null)
      //  return null;
      //RelationDefinition relationDefinition = relationData.A;
      //ClassDefinition classDefinition = relationData.B;
      //string propertyIdentifier = relationData.C;
      //return relationDefinition.GetOppositeClassDefinition (classDefinition.ID, propertyIdentifier).GetEntityName ();

      // TODO RELINQUING NHIBERNATE: Make sure return value is correct in all cases
      return relationMember.Name;
    }

    public string GetColumnName (MemberInfo member)
    {
      //ArgumentUtility.CheckNotNull ("member", member);
      //PropertyInfo property = member as PropertyInfo;
      //if (property == null)
      //  return null;
      //if (property.Name == "ID" && property.DeclaringType == typeof (DomainObject))
      //  return "ID";

      //ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[property.DeclaringType];
      //if (classDefinition == null)
      //  return null;
      //string propertyIdentifier = MappingConfiguration.Current.NameResolver.GetPropertyName (property);
      //PropertyDefinition propertyDefinition = classDefinition.GetPropertyDefinition (propertyIdentifier);
      //if (propertyDefinition != null)
      //  return propertyDefinition.StorageSpecificName;
      //else
      //  return null;

      ArgumentUtility.CheckNotNull ("member", member);
      PropertyInfo property = member as PropertyInfo;
      if (property == null)
      {
        return null;
      }
      // TODO RELINQUING NHIBERNATE: Make sure "Id" is correct in all cases (id, ID ?)
      else if (property.Name == "Id" && property.DeclaringType == typeof (DomainObject))
      {
        return "Id";
      }
      else
      {
        // TODO RELINQUING NHIBERNATE: Use MappingConfiguration as in DatabaseInfo class
        return property.Name;
      }
    }

    public Tuple<string, string> GetJoinColumnNames (MemberInfo relationMember)
    {
      //ArgumentUtility.CheckNotNull ("relationMember", relationMember);
      //Tuple<RelationDefinition, ClassDefinition, string> relationData = GetRelationData (relationMember);
      //if (relationData == null)
      //  return null;
      //RelationDefinition relationDefinition = relationData.A;
      //ClassDefinition classDefinition = relationData.B;
      //string propertyIdentifier = relationData.C;
      //IRelationEndPointDefinition leftEndPoint = relationDefinition.GetEndPointDefinition (classDefinition.ID, propertyIdentifier);
      //IRelationEndPointDefinition rightEndPoint = relationDefinition.GetOppositeEndPointDefinition (leftEndPoint);
      //string leftColumn = GetJoinColumn (leftEndPoint);
      //string rightColumn = GetJoinColumn (rightEndPoint);
      return Tuple.NewTuple (relationMember.Name, "DONT_KNOW_WHAT_TO_GIVE_HERE");
    }

    public object ProcessWhereParameter (object parameter)
    {
      //DomainObject domainObject = parameter as DomainObject;
      //if (domainObject != null)
      //  return domainObject.ID;
      //return parameter;

      // TODO RELINQUING NHIBERNATE: return DO Id  as in DatabaseInfo class
      return parameter;
    }

    public MemberInfo GetPrimaryKeyMember (Type entityType)
    {
      //ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[entityType];
      //if (classDefinition == null)
      //  return null;
      //else
      //  return typeof (DomainObject).GetProperty ("ID");

      //throw new System.NotImplementedException();
      // TODO RELINQUING NHIBERNATE: Return primary key if available
      return null;
    }

    public bool IsTableType (Type type)
    {
      throw new System.NotImplementedException();
    }

  

    #endregion
    
  }
}