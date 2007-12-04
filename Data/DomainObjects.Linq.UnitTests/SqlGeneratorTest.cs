using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.Linq.Parsing;
using Rubicon.Collections;

namespace Rubicon.Data.DomainObjects.Linq.UnitTests
{
  [TestFixture]
  public class SqlGeneratorTest
  {
    private IDbConnection _connection;
    private IDatabaseInfo _databaseInfo;
    private IQueryable<Student> _source;

    [SetUp]
    public void SetUp()
    {
      MockRepository repository = new MockRepository();
      _connection = repository.CreateMock<IDbConnection>();

      IDataParameterCollection parameterCollection = new StubParameterCollection();
      
      IDbCommand command = repository.Stub<IDbCommand>();
      SetupResult.For (command.Parameters).Return (parameterCollection);

      Expect.Call (_connection.CreateCommand()).Return (command);
      repository.ReplayAll();

      _databaseInfo = new StubDatabaseInfo();

      _source = ExpressionHelper.CreateQuerySource();
    }

    [Test]
    public void SimpleQuery()
    {
      IQueryable<Student> query = TestQueryGenerator.CreateSimpleQuery (_source);
      QueryExpression parsedQuery = ExpressionHelper.ParseQuery<Student> (query);
      SqlGenerator sqlGenerator = new SqlGenerator(parsedQuery);
      IDbCommand command = sqlGenerator.GetCommand (_databaseInfo, _connection);

      Assert.AreEqual ("SELECT [s].* FROM [sourceTable] [s]", command.CommandText);
      Assert.AreEqual (CommandType.Text, command.CommandType);
      Assert.IsEmpty (command.Parameters);
    }

    [Test]
    public void MultiFromQueryWithProjection ()
    {
      IQueryable<Tuple<string, string, int>> query = TestQueryGenerator.CreateMultiFromQueryWithProjection (_source, _source, _source);
      QueryExpression parsedQuery = ExpressionHelper.ParseQuery (query);
      SqlGenerator sqlGenerator = new SqlGenerator (parsedQuery);
      IDbCommand command = sqlGenerator.GetCommand (_databaseInfo, _connection);

      Assert.AreEqual ("SELECT [s1].[FirstColumn], [s2].[LastColumn], [s3].[IDColumn] FROM [sourceTable] [s1], [sourceTable] [s2], [sourceTable] [s3]",
        command.CommandText);
      Assert.AreEqual (CommandType.Text, command.CommandType);
      Assert.IsEmpty (command.Parameters);
    }
  }
}