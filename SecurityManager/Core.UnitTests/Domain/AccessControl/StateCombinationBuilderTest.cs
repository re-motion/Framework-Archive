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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class StateCombinationBuilderTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;
    private SecurableClassDefinition _orderClass;
    private StateCombinationBuilder _stateCombinationBuilder;

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
      _orderClass = _testHelper.CreateOrderClassDefinition();
      _stateCombinationBuilder = new StateCombinationBuilder (_orderClass);
    }

    [Test]
    public void Create_WithoutStateProperty ()
    {
      Assert.That (_orderClass.StateProperties, Is.Empty);

      PropertyStateTuple[][] expected = new PropertyStateTuple[][] { };

      PropertyStateTuple[][] actual = _stateCombinationBuilder.CreatePropertyProduct();

      Check (actual, expected);
    }

    [Test]
    public void Create_WithSingleStateProperty ()
    {
      StatePropertyDefinition orderStateProperty = _testHelper.CreateOrderStateProperty (_orderClass);
      Assert.That (_orderClass.StateProperties.Count, Is.EqualTo (1));

      PropertyStateTuple[][] expected =
          new[]
          {
              new[] { CreateTuple (orderStateProperty, 0) },
              new[] { CreateTuple (orderStateProperty, 1) },
          };

      PropertyStateTuple[][] actual = _stateCombinationBuilder.CreatePropertyProduct();

      Check (actual, expected);
    }

    [Test]
    public void Create_WithTwoStateProperties ()
    {
      StatePropertyDefinition orderStateProperty = _testHelper.CreateOrderStateProperty (_orderClass);
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (_orderClass);
      Assert.That (_orderClass.StateProperties.Count, Is.EqualTo (2));

      PropertyStateTuple[][] expected =
          new[]
          {
              new[] { CreateTuple (orderStateProperty, 0), CreateTuple (paymentProperty, 0) },
              new[] { CreateTuple (orderStateProperty, 0), CreateTuple (paymentProperty, 1) },
              new[] { CreateTuple (orderStateProperty, 1), CreateTuple (paymentProperty, 0) },
              new[] { CreateTuple (orderStateProperty, 1), CreateTuple (paymentProperty, 1) },
          };

      PropertyStateTuple[][] actual = _stateCombinationBuilder.CreatePropertyProduct();

      Check (actual, expected);
    }

    [Test]
    public void Create_WithThreeStatePropertiesAndOneOfThemEmpty ()
    {
      StatePropertyDefinition orderStateProperty = _testHelper.CreateOrderStateProperty (_orderClass);
      StatePropertyDefinition emptyProperty = _testHelper.CreateStateProperty ("Empty");
      _orderClass.AddStateProperty (emptyProperty);
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (_orderClass);
      Assert.That (_orderClass.StateProperties.Count, Is.EqualTo (3));

      PropertyStateTuple[][] expected =
          new[]
          {
              new[] { CreateTuple (orderStateProperty, 0), new PropertyStateTuple (emptyProperty, null), CreateTuple (paymentProperty, 0) },
              new[] { CreateTuple (orderStateProperty, 0), new PropertyStateTuple (emptyProperty, null), CreateTuple (paymentProperty, 1) },
              new[] { CreateTuple (orderStateProperty, 1), new PropertyStateTuple (emptyProperty, null), CreateTuple (paymentProperty, 0) },
              new[] { CreateTuple (orderStateProperty, 1), new PropertyStateTuple (emptyProperty, null), CreateTuple (paymentProperty, 1) },
          };

      PropertyStateTuple[][] actual = _stateCombinationBuilder.CreatePropertyProduct();

      Check (actual, expected);
    }

    private PropertyStateTuple CreateTuple (StatePropertyDefinition stateProperty, int stateIndex)
    {
      return new PropertyStateTuple (stateProperty, stateProperty.DefinedStates[stateIndex]);
    }

    private void Check (PropertyStateTuple[][] actual, PropertyStateTuple[][] expected)
    {
      Assert.That (actual.Length, Is.EqualTo (expected.Length));
      for (int i = 0; i < actual.Length; i++)
        Assert.That (actual[0], Is.EquivalentTo (expected[0]));
    }
  }
}