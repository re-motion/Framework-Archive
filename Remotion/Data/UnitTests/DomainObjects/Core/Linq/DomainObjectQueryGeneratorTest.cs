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
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.EagerFetching;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  public class DomainObjectQueryGeneratorTest : StandardMappingTest
  {
    private ISqlQueryGenerator _sqlQueryGeneratorMock;
    private TypeConversionProvider _typeConversionProvider;

    private DomainObjectQueryGenerator _generator;

    private ClassDefinition _customerClassDefinition;
    private QueryModel _customerQueryModel;

    public override void SetUp ()
    {
      base.SetUp();

      _sqlQueryGeneratorMock = MockRepository.GenerateStrictMock<ISqlQueryGenerator> ();
      _typeConversionProvider = TypeConversionProvider.Create();

      _generator = new DomainObjectQueryGenerator (_sqlQueryGeneratorMock, _typeConversionProvider);

      _customerClassDefinition = GetTypeDefinition (typeof (Customer));
      _customerQueryModel = QueryModelObjectMother.Create (Expression.Constant (null, typeof (Customer)));
    }

    [Test]
    public void CreateQuery ()
    {
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult ("SELECT x");
      _sqlQueryGeneratorMock.Expect (mock => mock.CreateSqlQuery (_customerQueryModel)).Return (fakeSqlQueryResult);

      var id = "id";
      var fetchQueryModelBuilders = Enumerable.Empty<FetchQueryModelBuilder> ();
      var result = _generator.CreateQuery (id, _customerClassDefinition, _customerQueryModel, fetchQueryModelBuilders, QueryType.Scalar);

      _sqlQueryGeneratorMock.VerifyAllExpectations();
      Assert.That (result.ID, Is.EqualTo (id));
      Assert.That (result.StorageProviderDefinition, Is.EqualTo (_customerClassDefinition.StorageEntityDefinition.StorageProviderDefinition));
      Assert.That (result.Statement, Is.EqualTo ("SELECT x"));
      Assert.That (result.Parameters, Is.Empty);
      Assert.That (result.QueryType, Is.EqualTo (QueryType.Scalar));
      Assert.That (result.EagerFetchQueries, Is.Empty);
    }

    [Test]
    public void CreateQuery_WithParameters ()
    {
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult (parameters: new[] { new CommandParameter ("p0", "paramval") });

      _sqlQueryGeneratorMock.Stub (stub => stub.CreateSqlQuery (_customerQueryModel)).Return (fakeSqlQueryResult);

      var result = _generator.CreateQuery ("id", _customerClassDefinition, _customerQueryModel, Enumerable.Empty<FetchQueryModelBuilder> (), QueryType.Scalar);

      Assert.That (result.Parameters, Is.EqualTo (new[] { new QueryParameter ("p0", "paramval", QueryParameterType.Value) }));
    }

    [Test]
    public void CreateQuery_QueryTypeCollection_WithEntityQuery ()
    {
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult (queryKind: SqlQueryGeneratorResult.QueryKind.EntityQuery);
      _sqlQueryGeneratorMock.Stub (stub => stub.CreateSqlQuery (_customerQueryModel)).Return (fakeSqlQueryResult);

      var result = _generator.CreateQuery ("id", _customerClassDefinition, _customerQueryModel, Enumerable.Empty<FetchQueryModelBuilder> (), QueryType.Collection);

      Assert.That (result.QueryType, Is.EqualTo (QueryType.Collection));
    }

    [Test]
    public void CreateQuery_QueryTypeCollection_WithGroupQuery ()
    {
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult (queryKind: SqlQueryGeneratorResult.QueryKind.GroupQuery);
      _sqlQueryGeneratorMock.Stub (stub => stub.CreateSqlQuery (_customerQueryModel)).Return (fakeSqlQueryResult);

      var expectedMessage = "This query provider does not support the given query ('from Order o in null select null'). "
          + "re-store only supports queries selecting a scalar value, a single DomainObject, or a collection of DomainObjects."
          + " GroupBy must be executed in memory, for example by issuing AsEnumerable() before performing the grouping operation.";
      Assert.That (
          () => _generator.CreateQuery ("id", _customerClassDefinition, _customerQueryModel, Enumerable.Empty<FetchQueryModelBuilder> (), QueryType.Collection),
          Throws.TypeOf<NotSupportedException> ().With.Message.EqualTo (expectedMessage));
    }

    [Test]
    public void CreateQuery_QueryTypeCollection_WithOtherQuery ()
    {
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult (queryKind: SqlQueryGeneratorResult.QueryKind.Other);
      _sqlQueryGeneratorMock.Stub (stub => stub.CreateSqlQuery (_customerQueryModel)).Return (fakeSqlQueryResult);

      var expectedMessage = "This query provider does not support the given query ('from Order o in null select null'). "
          + "re-store only supports queries selecting a scalar value, a single DomainObject, or a collection of DomainObjects.";
      Assert.That (
          () => _generator.CreateQuery ("id", _customerClassDefinition, _customerQueryModel, Enumerable.Empty<FetchQueryModelBuilder> (), QueryType.Collection),
          Throws.TypeOf<NotSupportedException> ().With.Message.EqualTo (expectedMessage));
    }

    [Test]
    public void CreateQuery_WithFetchRequests ()
    {
      var fakeSqlQuery = CreateSqlQueryGeneratorResult ();
      _sqlQueryGeneratorMock.Stub (stub => stub.CreateSqlQuery (_customerQueryModel)).Return (fakeSqlQuery);

      var fetchQueryModelBuilder = CreateFetchOneQueryModelBuilder((Customer o) => o.Ceo);
      var fakeFetchSqlQueryResult = CreateSqlQueryGeneratorResult (commandText: "FETCH");

      _sqlQueryGeneratorMock
          .Expect (mock => mock.CreateSqlQuery (Arg<QueryModel>.Is.Anything))
          .Return (fakeFetchSqlQueryResult)
          .WhenCalled (mi =>
          {
            var actualQueryModel = (QueryModel) mi.Arguments[0];
            var fetchQueryModel = fetchQueryModelBuilder.GetOrCreateFetchQueryModel ();
            CheckActualFetchQueryModel (actualQueryModel, fetchQueryModel);
          });

      var result = _generator.CreateQuery ("id", _customerClassDefinition, _customerQueryModel, new[] { fetchQueryModelBuilder }, QueryType.Collection);

      _sqlQueryGeneratorMock.VerifyAllExpectations ();
      CheckSingleFetchRequest (result.EagerFetchQueries, typeof (Company), "Ceo", "FETCH");
    }

    [Test]
    public void CreateQuery_WithFetchRequestWithSortExpression ()
    {
      var fakeSqlQuery = CreateSqlQueryGeneratorResult ();
      _sqlQueryGeneratorMock.Stub (stub => stub.CreateSqlQuery (_customerQueryModel)).Return (fakeSqlQuery);

      var fetchQueryModelBuilder = CreateFetchManyQueryModelBuilder ((Customer o) => o.Orders);
      var fakeFetchSqlQueryResult = CreateSqlQueryGeneratorResult (commandText: "FETCH");

      _sqlQueryGeneratorMock
          .Expect (mock => mock.CreateSqlQuery (Arg<QueryModel>.Is.Anything))
          .Return (fakeFetchSqlQueryResult)
          .WhenCalled (mi =>
          {
            var actualQueryModel = (QueryModel) mi.Arguments[0];
            var fetchQueryModel = fetchQueryModelBuilder.GetOrCreateFetchQueryModel ();
            
            Assert.That (actualQueryModel.MainFromClause.FromExpression, Is.TypeOf<SubQueryExpression> ());
            CheckActualFetchQueryModel (((SubQueryExpression) actualQueryModel.MainFromClause.FromExpression).QueryModel, fetchQueryModel);

            Assert.That (actualQueryModel.BodyClauses, Has.Some.TypeOf<OrderByClause>());
            var orderByClause = (OrderByClause) actualQueryModel.BodyClauses.Single();
            var endPointDefinition = ((VirtualRelationEndPointDefinition) GetEndPointDefinition (typeof (Customer), "Orders"));
            Assert.That (endPointDefinition.SortExpressionText, Is.EqualTo ("OrderNumber asc"));
            var orderNumberMember = MemberInfoFromExpressionUtility.GetProperty ((Order o) => o.OrderNumber);
            Assert.That (((MemberExpression) orderByClause.Orderings[0].Expression).Member, Is.SameAs (orderNumberMember));
            Assert.That (orderByClause.Orderings[0].OrderingDirection, Is.EqualTo (OrderingDirection.Asc));
          });

      _generator.CreateQuery ("id", _customerClassDefinition, _customerQueryModel, new[] { fetchQueryModelBuilder }, QueryType.Collection);

      _sqlQueryGeneratorMock.VerifyAllExpectations ();
    }

    [Test]
    public void CreateQuery_WithNestedFetchRequests ()
    {
      var fakeSqlQuery = CreateSqlQueryGeneratorResult();
      _sqlQueryGeneratorMock.Stub (stub => stub.CreateSqlQuery (_customerQueryModel)).Return (fakeSqlQuery);

      var fetchQueryModelBuilder = CreateFetchOneQueryModelBuilder ((Customer c) => c.Ceo);
      var fakeFetchSqlQueryResult = CreateSqlQueryGeneratorResult (commandText: "FETCH");
      _sqlQueryGeneratorMock.Expect (mock => mock.CreateSqlQuery (Arg<QueryModel>.Is.Anything)).Return (fakeFetchSqlQueryResult).Repeat.Once();

      var innerFetchRequest = CreateFetchOneRequest ((Ceo c) => c.Company);
      fetchQueryModelBuilder.FetchRequest.GetOrAddInnerFetchRequest (innerFetchRequest);

      var fakeInnerFetchSqlQueryResult = CreateSqlQueryGeneratorResult ("INNER FETCH");
      _sqlQueryGeneratorMock
          .Expect (mock => mock.CreateSqlQuery (Arg<QueryModel>.Is.Anything))
          .Return (fakeInnerFetchSqlQueryResult)
          .WhenCalled (mi =>
          {
            var actualQueryModel = (QueryModel) mi.Arguments[0];
            Assert.That (((StreamedSequenceInfo) actualQueryModel.GetOutputDataInfo()).ItemExpression.Type, Is.SameAs (typeof (Company)));
          });

      var result = _generator.CreateQuery ("id", _customerClassDefinition, _customerQueryModel, new[] { fetchQueryModelBuilder }, QueryType.Collection);

      _sqlQueryGeneratorMock.VerifyAllExpectations();
      
      var fetchQuery = result.EagerFetchQueries.Single ();
      CheckSingleFetchRequest (fetchQuery.Value.EagerFetchQueries, typeof (Ceo), "Company", "INNER FETCH");
    }

    private SqlQueryGeneratorResult CreateSqlQueryGeneratorResult (
        string commandText = null, 
        CommandParameter[] parameters = null,
        SqlQueryGeneratorResult.QueryKind queryKind = SqlQueryGeneratorResult.QueryKind.EntityQuery)
    {
      return new SqlQueryGeneratorResult (CreateSqlCommandData (commandText, parameters), queryKind);
    }

    private SqlCommandData CreateSqlCommandData (string commandText = null, CommandParameter[] parameters = null)
    {
      return new SqlCommandData (
          commandText ?? "bla", parameters ?? new CommandParameter[0], Expression.Parameter (typeof (Order), "o"), Expression.Constant (null));
    }

    private void CheckActualFetchQueryModel (QueryModel actualQueryModel, QueryModel fetchQueryModel)
    {
      Assert.That (actualQueryModel, Is.Not.SameAs (fetchQueryModel));
      Assert.That (fetchQueryModel.ResultOperators, Has.No.TypeOf<DistinctResultOperator> ());
      Assert.That (actualQueryModel.ResultOperators, Has.Some.TypeOf<DistinctResultOperator> ());
      Assert.That (actualQueryModel.MainFromClause.ToString (), Is.EqualTo (fetchQueryModel.MainFromClause.ToString ()));
      Assert.That (actualQueryModel.SelectClause.ToString (), Is.EqualTo (fetchQueryModel.SelectClause.ToString ()));
    }

    private FetchQueryModelBuilder CreateFetchOneQueryModelBuilder<TSource, TDest> (Expression<Func<TSource, TDest>> memberExpression)
    {
      var fetchRequest = CreateFetchOneRequest(memberExpression);
      return new FetchQueryModelBuilder (fetchRequest, _customerQueryModel, 0);
    }

    private FetchOneRequest CreateFetchOneRequest<TSource, TDest> (Expression<Func<TSource, TDest>> memberExpression)
    {
      var relationMember = MemberInfoFromExpressionUtility.GetProperty (memberExpression);
      return new FetchOneRequest (relationMember);
    }

    private FetchQueryModelBuilder CreateFetchManyQueryModelBuilder<TSource, TDest> (Expression<Func<TSource, TDest>> memberExpression)
    {
      var fetchRequest = CreateFetchManyRequest (memberExpression);
      return new FetchQueryModelBuilder (fetchRequest, _customerQueryModel, 0);
    }

    private FetchManyRequest CreateFetchManyRequest<TSource, TDest> (Expression<Func<TSource, TDest>> memberExpression)
    {
      var relationMember = MemberInfoFromExpressionUtility.GetProperty (memberExpression);
      return new FetchManyRequest (relationMember);
    }

    private void CheckSingleFetchRequest (EagerFetchQueryCollection fetchQueryCollection, Type sourceType, string fetchedProperty, string expectedFetchQueryText)
    {
      Assert.That (fetchQueryCollection.Count, Is.EqualTo (1));
      var fetchQuery = fetchQueryCollection.Single ();
      Assert.That (fetchQuery.Key, Is.EqualTo (GetEndPointDefinition (sourceType, fetchedProperty)));
      Assert.That (fetchQuery.Value.Statement, Is.EqualTo (expectedFetchQueryText));
    }

  }
}