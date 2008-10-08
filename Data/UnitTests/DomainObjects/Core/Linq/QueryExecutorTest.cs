/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Data.Linq.SqlGeneration.SqlServer;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class QueryExecutorTest : ClientTransactionBaseTest
  {
    private SqlServerGenerator _sqlGenerator;

    public override void SetUp ()
    {
      base.SetUp ();
      _sqlGenerator = new SqlServerGenerator (DatabaseInfo.Instance);
    }
    
    [Test]
    public void ExecuteSingle()
    {
      SetDatabaseModifyable();

      Computer.GetObject (DomainObjectIDs.Computer1).Delete ();
      Computer.GetObject (DomainObjectIDs.Computer2).Delete ();
      Computer.GetObject (DomainObjectIDs.Computer3).Delete ();
      Computer.GetObject (DomainObjectIDs.Computer4).Delete ();

      ClientTransaction.Current.Commit();

      var executor = new QueryExecutor<Computer> (_sqlGenerator);
      QueryModel model = GetParsedSimpleQuery ();

      object instance = executor.ExecuteSingle (model);
      Assert.IsNotNull (instance);
      Assert.AreSame (Computer.GetObject (DomainObjectIDs.Computer5), instance);
    }

    [Test]
    [ExpectedException (ExpectedMessage = "ExecuteSingle must return a single object, but the query returned 5 objects.")]
    public void ExecuteSingle_TooManyObjects()
    {
      var executor = new QueryExecutor<Computer> (_sqlGenerator);
      executor.ExecuteSingle (GetParsedSimpleQuery());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void QueryExecutor_ExecuteSingle_NoCurrentTransaction ()
    {
      var executor = new QueryExecutor<Computer> (_sqlGenerator);
      QueryModel model = GetParsedSimpleQuery ();

      using (ClientTransactionScope.EnterNullScope ())
      {
        executor.ExecuteSingle (model);
      }
    }

    [Test]
    public void ExecuteCollection ()
    {
      var executor = new QueryExecutor<Computer> (_sqlGenerator);
      QueryModel model = GetParsedSimpleQuery();

      IEnumerable computers = executor.ExecuteCollection (model);

      var computerList = new ArrayList();
      foreach (Computer computer in computers)
        computerList.Add (computer);

      var expected = new[]
                            {
                                Computer.GetObject (DomainObjectIDs.Computer1),
                                Computer.GetObject (DomainObjectIDs.Computer2),
                                Computer.GetObject (DomainObjectIDs.Computer3),
                                Computer.GetObject (DomainObjectIDs.Computer4),
                                Computer.GetObject (DomainObjectIDs.Computer5),
                            };
      Assert.That (computerList, Is.EquivalentTo (expected));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Mapping does not contain class 'Remotion.Data.DomainObjects.DomainObject'.")]
    public void ExecuteCollection_WrongType ()
    {
      var executor = new QueryExecutor<DomainObject> (_sqlGenerator);
      executor.ExecuteCollection (GetParsedSimpleQuery());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void QueryExecutor_ExecuteCollection_NoCurrentTransaction ()
    {
      var executor = new QueryExecutor<Computer> (_sqlGenerator);
      QueryModel model = GetParsedSimpleQuery ();

      using (ClientTransactionScope.EnterNullScope ())
      {
        executor.ExecuteCollection (model);
      }
    }

    [Test]
    public void ExecuteSingle_WithParameters ()
    {
      var executor = new QueryExecutor<Order> (_sqlGenerator);
      QueryModel model = GetParsedSimpleWhereQuery ();

      var order = (Order) executor.ExecuteSingle (model);

      Order expected = Order.GetObject (DomainObjectIDs.Order1);
      Assert.AreSame (expected, order);
    }

    [Test]
    public void ExecuteCollection_WithParameters()
    {
      var executor = new QueryExecutor<Order> (_sqlGenerator);
      QueryModel model = GetParsedSimpleWhereQuery ();

      IEnumerable orders = executor.ExecuteCollection (model);

      var orderList = new ArrayList ();
      foreach (Order order in orders) 
        orderList.Add (order);

      var expected = new[]
                         {
                             Order.GetObject (DomainObjectIDs.Order1),

                         };
      Assert.That (orderList, Is.EquivalentTo (expected));
    }

    public class TestMixin : Mixin<object, TestMixin.IBaseCallRequirements>
    {
      public interface IBaseCallRequirements
      {
        ClassDefinition GetClassDefinition ();
        IQuery CreateQuery (ClassDefinition classDefinition, string statement, CommandParameter[] commandParameters);
        CommandData CreateStatement (QueryModel queryModel);
      }

      public bool GetClassDefinitionCalled = false;
      public bool CreateQueryCalled = false;
      public bool GetStatementCalled = false;

      [OverrideTarget]
      public ClassDefinition GetClassDefinition ()
      {
        GetClassDefinitionCalled = true;
        return Base.GetClassDefinition();
      }

      [OverrideTarget]
      public IQuery CreateQuery (ClassDefinition classDefinition, string statement, CommandParameter[] commandParameters)
      {
        CreateQueryCalled = true;
        return Base.CreateQuery (classDefinition, statement, commandParameters);
      }

      [OverrideTarget]
      public CommandData CreateStatement (QueryModel queryModel)
      {
        GetStatementCalled = true;
        return Base.CreateStatement (queryModel);
      }
    }
    
    [Test]
    public void QueryExecutor_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (QueryExecutor<>)).AddMixin<TestMixin> ().EnterScope ())
      {
        var queryable = new DomainObjectQueryable<Order> (new SqlServerGenerator (DatabaseInfo.Instance));
        Assert.That (Mixin.Get<TestMixin> (queryable.Provider.Executor), Is.Not.Null);
      }
    }

    [Test]
    public void GetClassDefinition_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (QueryExecutor<>)).AddMixin<TestMixin> ().EnterScope ())
      {
        var queryable = new DomainObjectQueryable<Order> (new SqlServerGenerator (DatabaseInfo.Instance));
        var executor = queryable.GetExecutor();

        executor.GetClassDefinition();
        Assert.That (Mixin.Get<TestMixin> (executor).GetClassDefinitionCalled, Is.True);
      }
    }

    [Test]
    public void GetStatement_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (QueryExecutor<>)).AddMixin<TestMixin> ().EnterScope ())
      {
        var queryable = new DomainObjectQueryable<Computer> (new SqlServerGenerator (DatabaseInfo.Instance));
        var executor = queryable.GetExecutor();

        executor.CreateStatement (GetParsedSimpleQuery ());
        Assert.That (Mixin.Get<TestMixin> (executor).GetStatementCalled, Is.True);
      }
    }

    [Test]
    public void CreateQuery_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (QueryExecutor<>)).AddMixin<TestMixin> ().EnterScope ())
      {
        var queryable = new DomainObjectQueryable<Order> (new SqlServerGenerator (DatabaseInfo.Instance));
        var executor = queryable.GetExecutor();

        ClassDefinition classDefinition = executor.GetClassDefinition();
        CommandData statement = executor.CreateStatement(GetParsedSimpleQuery());

        executor.CreateQuery (classDefinition, statement.Statement, statement.Parameters);
        Assert.That (Mixin.Get<TestMixin> (executor).CreateQueryCalled, Is.True);
      }
    }

    private QueryModel GetParsedSimpleQuery ()
    {
      IQueryable<Computer> query = from computer in QueryFactory.CreateQueryable<Computer>() select computer;
      return new QueryParser (query.Expression).GetParsedQuery ();
    }

    private QueryModel GetParsedSimpleWhereQuery ()
    {
      IQueryable<Order> query = from order in QueryFactory.CreateQueryable<Order>() where order.OrderNumber == 1 select order;
      return new QueryParser (query.Expression).GetParsedQuery ();
    }

  }
}