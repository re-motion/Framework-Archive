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
      throw new System.NotImplementedException();
    }

    public string GetColumnName (MemberInfo member)
    {
      throw new System.NotImplementedException();
    }

    public Tuple<string, string> GetJoinColumnNames (MemberInfo relationMember)
    {
      throw new System.NotImplementedException();
    }

    public object ProcessWhereParameter (object parameter)
    {
      throw new System.NotImplementedException();
    }

    public MemberInfo GetPrimaryKeyMember (Type entityType)
    {
      throw new System.NotImplementedException();
    }

    public bool IsTableType (Type type)
    {
      throw new System.NotImplementedException();
    }

  

    #endregion
    
  }
}