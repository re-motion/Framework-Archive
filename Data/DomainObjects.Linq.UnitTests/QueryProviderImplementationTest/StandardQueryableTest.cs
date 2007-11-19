using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.Linq.QueryProviderImplementation;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Linq.UnitTests.QueryProviderImplementationTest
{
  [TestFixture]
  public class StandardQueryableTest
  {
    private IQueryProvider _provider;
    private MockRepository _mockRepository;

    [SetUp]
    public void SetUp()
    {
      _mockRepository = new MockRepository();
      _provider = _mockRepository.CreateMock<IQueryProvider> ();
    }

    [Test]
    public void Initialize ()
    {
      Expression expression = ExpressionHelper.CreateNewIntArrayExpression();
      StandardQueryable<int> queryable = new StandardQueryable<int> (_provider, expression);

      Assert.AreSame (_provider, queryable.Provider);
      Assert.AreSame (expression, queryable.Expression);

      Assert.AreEqual (typeof (int), queryable.ElementType);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void ConstructorThrowsTypeException ()
    {
      Expression expression = ExpressionHelper.CreateNewIntArrayExpression();
      new StandardQueryable<string> (_provider, expression);
    }

    [Test]
    public void GenericGetEnumerator ()
    {
      Expression expression = ExpressionHelper.CreateNewIntArrayExpression();
      Expect.Call (_provider.Execute<IEnumerable<int>> (expression)).Return(new List<int>());

      _mockRepository.ReplayAll ();
      StandardQueryable<int> queryable = new StandardQueryable<int> (_provider, expression);
      queryable.GetEnumerator();
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetEnumerator()
    {
      Expression expression = ExpressionHelper.CreateNewIntArrayExpression ();
      Expect.Call (_provider.Execute (expression)).Return(new List<int>());

      _mockRepository.ReplayAll ();
      StandardQueryable<int> queryable = new StandardQueryable<int> (_provider, expression);
      ((IEnumerable)queryable).GetEnumerator ();
      _mockRepository.VerifyAll ();
    }
  }
}