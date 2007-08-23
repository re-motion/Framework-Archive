using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes;
using Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests
{
  [TestFixture]
  public class DomainObjectClassTest : DatabaseTest
  {
    // types

    // static members and constants

    // member fields

    private DomainObjectClass _domainObjectClass;
    private ClientTransactionScope _transactionScope;

    // construction and disposing

    public DomainObjectClassTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp();
      _transactionScope = ClientTransaction.NewTransaction ().EnterNonReturningScope ();
      _domainObjectClass = new DomainObjectClass (typeof (Order));
    }

    public override void TearDown ()
    {
      _transactionScope.Leave ();
      base.TearDown ();
    }

    [Test]
    public void GetPropertyDefinitionForValueTypeProperty()
    {
      IBusinessObjectProperty orderNumberProperty = _domainObjectClass.GetPropertyDefinition ("OrderNumber");
      Assert.That (orderNumberProperty, Is.InstanceOfType (typeof (Int32Property)));
      Assert.That (orderNumberProperty.Identifier, Is.EqualTo ("OrderNumber"));
      Assert.That (orderNumberProperty.PropertyType, Is.SameAs (typeof (int)));
      Assert.That (((BaseProperty) orderNumberProperty).UnderlyingType, Is.SameAs (typeof (int)));
    }

    [Test]
    public void GetPropertyDefinitionForOneSideRelationProperty ()
    {
      IBusinessObjectProperty orderItemsProperty = _domainObjectClass.GetPropertyDefinition ("OrderItems");
      Assert.That (orderItemsProperty, Is.InstanceOfType (typeof (ReferenceProperty)));
      Assert.That (orderItemsProperty.Identifier, Is.EqualTo ("OrderItems"));
      Assert.That (orderItemsProperty.ListInfo.ItemType, Is.SameAs (typeof (OrderItem)));
    }

    [Test]
    public void GetProperty ()
    {
      Order order = new Order();
      order.OrderNumber = 1;
      
      Assert.That (((IBusinessObject) order).GetProperty("OrderNumber"), Is.EqualTo (1));
    }

    [Test]
    public void SetProperty ()
    {
      Order order = new Order ();
      order.OrderNumber = 0;
      
      ((IBusinessObject) order).SetProperty ("OrderNumber", 1);

      Assert.That (order.OrderNumber, Is.EqualTo (1));
    }

    [Test]
    public void GetPropertyDefinitionsExpected ()
    {
      IBusinessObjectProperty[] propertyDefinitions = _domainObjectClass.GetPropertyDefinitions ();
      Assert.That (propertyDefinitions.Length, Is.EqualTo (4));
      AssertPropertyIsContained ("OrderNumber", propertyDefinitions);
      AssertPropertyIsContained ("DeliveryDate", propertyDefinitions);
      AssertPropertyIsContained ("OrderTicket", propertyDefinitions);
      AssertPropertyIsContained ("OrderItems", propertyDefinitions);
    }

    private void AssertPropertyIsContained (string propertyName, IBusinessObjectProperty[] properties)
    {
      foreach (IBusinessObjectProperty property in properties)
      {
        if (propertyName == property.Identifier)
          return;
      }
      Assert.Fail ("Provided array must contain property '{0}'.", propertyName);
    }
  }
}
