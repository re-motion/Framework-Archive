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
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.EagerFetching;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// Generates <see cref="IQuery"/> objects from LINQ queries (parsed by re-linq into <see cref="QueryModel"/> instances).
  /// </summary>
  public class DomainObjectQueryGenerator : IDomainObjectQueryGenerator
  {
    private readonly ISqlQueryGenerator _sqlQueryGenerator;
    private readonly TypeConversionProvider _typeConversionProvider;
    private readonly IStorageTypeInformationProvider _storageTypeInformationProvider;

    public DomainObjectQueryGenerator (ISqlQueryGenerator sqlQueryGenerator, TypeConversionProvider typeConversionProvider, IStorageTypeInformationProvider storageTypeInformationProvider)
    {
      ArgumentUtility.CheckNotNull ("sqlQueryGenerator", sqlQueryGenerator);
      ArgumentUtility.CheckNotNull ("typeConversionProvider", typeConversionProvider);
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);

      _sqlQueryGenerator = sqlQueryGenerator;
      _typeConversionProvider = typeConversionProvider;
      _storageTypeInformationProvider = storageTypeInformationProvider;
    }

    public ISqlQueryGenerator SqlQueryGenerator
    {
      get { return _sqlQueryGenerator; }
    }

    public TypeConversionProvider TypeConversionProvider
    {
      get { return _typeConversionProvider; }
    }

    public IStorageTypeInformationProvider StorageTypeInformationProvider
    {
      get { return _storageTypeInformationProvider; }
    }

    public virtual IExecutableQuery<T> CreateScalarQuery<T> (string id, StorageProviderDefinition storageProviderDefinition, QueryModel queryModel)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      var sqlQuery = _sqlQueryGenerator.CreateSqlQuery (queryModel);
      var command = sqlQuery.SqlCommand;
      var statement = command.CommandText;

      var query = CreateQuery (id, storageProviderDefinition, statement, command.Parameters, QueryType.Scalar);

      var projection = sqlQuery.SqlCommand.GetInMemoryProjection<T>().Compile();
      return new ScalarQueryAdapter<T> (query, o => projection (new ScalarResultRowAdapter (o, _storageTypeInformationProvider)));
    }

    public virtual IExecutableQuery<IEnumerable<T>> CreateSequenceQuery<T> (
        string id,
        ClassDefinition classDefinition,
        QueryModel queryModel,
        IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);
      ArgumentUtility.CheckNotNull ("fetchQueryModelBuilders", fetchQueryModelBuilders);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var sqlQuery = _sqlQueryGenerator.CreateSqlQuery (queryModel);
      var command = sqlQuery.SqlCommand;
      var statement = command.CommandText;

      var queryType = sqlQuery.Kind == SqlQueryGeneratorResult.QueryKind.EntityQuery ? QueryType.Collection : QueryType.Custom;
      var query = CreateQuery (id, classDefinition.StorageEntityDefinition.StorageProviderDefinition, statement, command.Parameters , queryType);
      CreateEagerFetchQueries<T> (query, classDefinition, fetchQueryModelBuilders);
      
      if (sqlQuery.Kind == SqlQueryGeneratorResult.QueryKind.EntityQuery)
      {
        return new DomainObjectSequenceQueryAdapter<T> (query);
      }
      else
      {
        var projection = sqlQuery.SqlCommand.GetInMemoryProjection<T>().Compile();
        return new CustomSequenceQueryAdapter<T> (query, qrr=> projection(new QueryResultRowAdapter(qrr)));
      }
    }

    protected virtual IQuery CreateQuery (
        string id, StorageProviderDefinition storageProviderDefinition, string statement, CommandParameter[] commandParameters, QueryType queryType)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("statement", statement);
      ArgumentUtility.CheckNotNull ("commandParameters", commandParameters);

      var queryParameters = new QueryParameterCollection();
      foreach (CommandParameter commandParameter in commandParameters)
        queryParameters.Add (commandParameter.Name, commandParameter.Value, QueryParameterType.Value);

      if (queryType == QueryType.Scalar)
        return QueryFactory.CreateScalarQuery (id, storageProviderDefinition, statement, queryParameters);
      else if (queryType == QueryType.Collection)
        return QueryFactory.CreateCollectionQuery (id, storageProviderDefinition, statement, queryParameters, typeof (DomainObjectCollection));
      else
        return QueryFactory.CreateCustomQuery (id, storageProviderDefinition, statement, queryParameters);
    }

    private void CreateEagerFetchQueries<T> (IQuery query, ClassDefinition classDefinition, IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders)
    {
      foreach (var fetchQueryModelBuilder in fetchQueryModelBuilders)
      {
        var relationEndPointDefinition = GetEagerFetchRelationEndPointDefinition (fetchQueryModelBuilder.FetchRequest, classDefinition);

        // clone the fetch query model because we don't want to modify the source model of all inner requests
        var fetchQueryModel = fetchQueryModelBuilder.GetOrCreateFetchQueryModel().Clone();

        // for re-store, fetch queries must always be distinct even when the query would return duplicates
        // e.g., for (from o in Orders select o).FetchOne (o => o.Customer)
        // when two orders have the same customer, re-store gives an error, unless we add a DISTINCT clause
        fetchQueryModel.ResultOperators.Add (new DistinctResultOperator());

        var sortExpression = GetSortExpressionForRelation (relationEndPointDefinition);
        if (sortExpression != null)
        {
          // If we have a SortExpression, we need to add the ORDER BY clause _after_ the DISTINCT (because re-linq strips out ORDER BY clauses when
          // seeing a DISTINCT operator); therefore, we put the DISTINCT query into a sub-query, then append the ORDER BY clause
          fetchQueryModel = fetchQueryModel.ConvertToSubQuery ("#fetch");

          var orderByClause = new OrderByClause();
          foreach (var sortedPropertySpecification in sortExpression.SortedProperties)
            orderByClause.Orderings.Add (GetOrdering (fetchQueryModel, sortedPropertySpecification));

          fetchQueryModel.BodyClauses.Add (orderByClause);
        }

        var fetchQuery = CreateSequenceQuery<T> (
            "<fetch query for " + fetchQueryModelBuilder.FetchRequest.RelationMember.Name + ">",
            relationEndPointDefinition.GetOppositeClassDefinition(),
            fetchQueryModel,
            fetchQueryModelBuilder.CreateInnerBuilders()
           );

        query.EagerFetchQueries.Add (relationEndPointDefinition, fetchQuery);
      }
    }

    private SortExpressionDefinition GetSortExpressionForRelation (IRelationEndPointDefinition relationEndPointDefinition)
    {
      var virtualEndPointDefinition = relationEndPointDefinition as VirtualRelationEndPointDefinition;
      if (virtualEndPointDefinition != null)
        return virtualEndPointDefinition.GetSortExpression();
      else
        return null;
    }

    private IRelationEndPointDefinition GetEagerFetchRelationEndPointDefinition (FetchRequestBase fetchRequest, ClassDefinition classDefinition)
    {
      var propertyInfo = fetchRequest.RelationMember as PropertyInfo;
      if (propertyInfo == null)
      {
        var message = string.Format (
            "The member '{0}' is a '{1}', which cannot be fetched by this LINQ provider. Only properties can be fetched.",
            fetchRequest.RelationMember.Name,
            fetchRequest.RelationMember.MemberType);
        throw new NotSupportedException (message);
      }

      var propertyName = MappingConfiguration.Current.NameResolver.GetPropertyName (PropertyInfoAdapter.Create (propertyInfo));
      try
      {
        return classDefinition.GetMandatoryRelationEndPointDefinition (propertyName);
      }
      catch (MappingException ex)
      {
        var message = string.Format (
            "The property '{0}' is not a relation end point. Fetching it is not supported by this LINQ provider.",
            propertyName);
        throw new NotSupportedException (message, ex);
      }
    }

    private Ordering GetOrdering (QueryModel queryModel, SortedPropertySpecification sortedPropertySpecification)
    {
      var propertyInfo = (PropertyInfo) _typeConversionProvider.Convert (
          sortedPropertySpecification.PropertyDefinition.PropertyInfo.GetType(),
          typeof (PropertyInfo),
          sortedPropertySpecification.PropertyDefinition.PropertyInfo);

      var memberExpression = Expression.MakeMemberAccess (queryModel.SelectClause.Selector, propertyInfo);
      var orderingDirection = sortedPropertySpecification.Order == SortOrder.Ascending ? OrderingDirection.Asc : OrderingDirection.Desc;
      return new Ordering (memberExpression, orderingDirection);
    }
  }
}