// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe.Expressions;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.Expressions
{
  [TestFixture]
  public class ConstrainedMethodCallExpressionTest
  {
    private MethodCallExpression _methodCall;

    private ConstrainedMethodCallExpression _expression;

    [SetUp]
    public void SetUp ()
    {
      _methodCall = Expression.Call (Expression.Default (typeof (ConstrainedMethodCallExpressionTest)), "ToString", Type.EmptyTypes);

      _expression = new ConstrainedMethodCallExpression (_methodCall);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_expression.Type, Is.SameAs (_methodCall.Type));
      Assert.That (_expression.MethodCall, Is.SameAs (_methodCall));
      Assert.That (_expression.ConstrainingType, Is.SameAs (typeof (ConstrainedMethodCallExpressionTest)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot constrain calls to static methods.\r\nParameter name: methodCall")]
    public void Initialization_StaticMethod ()
    {
      var expression = Expression.Call (typeof (string), "Intern", Type.EmptyTypes, Expression.Default (typeof (string)));
      Dev.Null = new ConstrainedMethodCallExpression (expression);
    }

    [Test]
    public virtual void Accept ()
    {
      ExpressionTestHelper.CheckAccept (_expression, mock => mock.VisitConstrainedMethodCall (_expression));
    }

    [Test]
    public void VisitChildren_NoChanges ()
    {
      ExpressionTestHelper.CheckVisitChildren_NoChanges (_expression, _expression.MethodCall);
    }

    [Test]
    public void VisitChildren_WithChanges ()
    {
      var newMethodCall = Expression.Call (Expression.Default (typeof (object)), "ToString", Type.EmptyTypes);

      var expressionVisitorMock = MockRepository.GenerateStrictMock<ExpressionVisitor>();
      expressionVisitorMock.Expect (mock => mock.Visit (_expression.MethodCall)).Return (newMethodCall);

      var result = _expression.Invoke<Expression> ("VisitChildren", expressionVisitorMock);

      Assert.That (result, Is.Not.SameAs (_expression));
      Assert.That (result.Type, Is.SameAs (_expression.Type));
      Assert.That (result, Is.TypeOf<ConstrainedMethodCallExpression>());
      Assert.That (result.As<ConstrainedMethodCallExpression>().MethodCall, Is.SameAs (newMethodCall));
    }
  }
}