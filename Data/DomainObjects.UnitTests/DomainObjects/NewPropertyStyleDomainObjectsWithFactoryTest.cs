using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.Resources;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Development.UnitTesting;
using System.Reflection;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Interception;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class NewPropertyStyleDomainObjectsWithFactoryTest : ClientTransactionBaseTest
  {
    // types

    public abstract class NonInstantiableAbstractClass : DomainObject
    {
      public NonInstantiableAbstractClass () : base (null, null) { }
      public abstract void Foo ();
    }

    public abstract class NonInstantiableAbstractClassWithProps : DomainObject
    {
      public NonInstantiableAbstractClassWithProps () : base (null, null) { }
      public abstract int Foo { get; }
    }

    public sealed class NonInstantiableSealedClass : DomainObject
    {
      public NonInstantiableSealedClass () : base (null, null) { }
    }

    public class NonInstantiableNonDomainClass
    {
    }

    public class Throws : DomainObject
    {
      public Throws ()
        : this (null, null)
      {
      }

      public Throws (ClientTransaction tx, ObjectID id)
        : base (((int) (object) "this always throws before entering base constructor!") == 5 ? ClientTransaction.Current : ClientTransaction.Current, null)
      {
      }
    }

    public class ClassWithWrongConstructor : DomainObject
    {
      public ClassWithWrongConstructor (string s)
      {
        Assert.Fail ("Shouldn't be executed.");
      }
    }

    // static members and constants

    // member fields

    // construction and disposing

    public NewPropertyStyleDomainObjectsWithFactoryTest ()
    {
    }

    // methods and properties

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();
      DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory = null;
    }

    [TearDown]
    public override void TearDown ()
    {
      DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory = null;
      base.TearDown ();
    }

    private bool WasCreatedByFactory (object o)
    {
      return DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory.WasCreatedByFactory (o);
    }

    [Test]
    public void LoadOfSimpleObjectWorks ()
    {
      OrderWithNewPropertyAccess order = DomainObject.GetObject<OrderWithNewPropertyAccess> (DomainObjectIDs.OrderWithNewPropertyAccess1);
      Assert.IsTrue (WasCreatedByFactory (order));
    }

    [Test]
    public void ConstructionOfSimpleObjectWorks ()
    {
      OrderWithNewPropertyAccess order = DomainObject.Create<OrderWithNewPropertyAccess> ();
      Assert.IsTrue (WasCreatedByFactory (order));
    }

    [Test]
    public void GetPropertyValueWorks ()
    {
      OrderWithNewPropertyAccess order = DomainObject.GetObject<OrderWithNewPropertyAccess> (DomainObjectIDs.OrderWithNewPropertyAccess1);
      Assert.AreEqual (1, order.OrderNumber);
      Assert.AreEqual (new DateTime (2005, 01, 01), order.DeliveryDate);
      Assert.AreEqual (1, order.OrderNumber);
    }

    [Test]
    public void SetPropertyValueWorks ()
    {
      OrderWithNewPropertyAccess order = DomainObject.GetObject<OrderWithNewPropertyAccess> (DomainObjectIDs.OrderWithNewPropertyAccess1);
      
      order.OrderNumber = 15;
      Assert.AreEqual (15, order.OrderNumber);

      order.DeliveryDate = new DateTime (2007, 02, 03);
      Assert.AreEqual (new DateTime (2007, 02, 03), order.DeliveryDate);

      Assert.AreEqual (15, order.OrderNumber);
    }

    [Test]
    public void IsPropertyAccessor ()
    {
      Assert.IsFalse (ReflectionUtility.IsPropertyAccessor (typeof (NewPropertyStyleDomainObjectsWithFactoryTest).GetConstructor(Type.EmptyTypes)));
      Assert.IsFalse (ReflectionUtility.IsPropertyGetter (typeof (NewPropertyStyleDomainObjectsWithFactoryTest).GetConstructor (Type.EmptyTypes)));
      Assert.IsFalse (ReflectionUtility.IsPropertySetter (typeof (NewPropertyStyleDomainObjectsWithFactoryTest).GetConstructor (Type.EmptyTypes)));

      Assert.IsFalse (ReflectionUtility.IsPropertyAccessor (typeof (object).GetMethod ("ToString")));
      Assert.IsFalse (ReflectionUtility.IsPropertyGetter (typeof (object).GetMethod ("ToString")));
      Assert.IsFalse (ReflectionUtility.IsPropertySetter (typeof (object).GetMethod ("ToString")));

      Assert.IsTrue (ReflectionUtility.IsPropertyAccessor (typeof (OrderWithNewPropertyAccess).GetMethod ("get_OrderNumber")));
      Assert.IsTrue (ReflectionUtility.IsPropertyAccessor (typeof (OrderWithNewPropertyAccess).GetMethod ("set_OrderNumber")));
      Assert.IsTrue (ReflectionUtility.IsPropertyGetter (typeof (OrderWithNewPropertyAccess).GetMethod ("get_OrderNumber")));
      Assert.IsFalse (ReflectionUtility.IsPropertySetter (typeof (OrderWithNewPropertyAccess).GetMethod ("get_OrderNumber")));
      Assert.IsFalse (ReflectionUtility.IsPropertyGetter (typeof (OrderWithNewPropertyAccess).GetMethod ("set_OrderNumber")));
      Assert.IsTrue (ReflectionUtility.IsPropertySetter (typeof (OrderWithNewPropertyAccess).GetMethod ("set_OrderNumber")));
    }

    private int TestProperty
    {
      get { return 0; }
      set { }
    }

    private static int StaticTestProperty
    {
      get { return 0; }
      set { }
    }

    [Test]
    public void GetPropertyFromMethod ()
    {
      Assert.IsNull (ReflectionUtility.GetPropertyNameForMethodName (""));
      Assert.IsNull (ReflectionUtility.GetPropertyNameForMethodName ("bla"));
      Assert.IsNull (ReflectionUtility.GetPropertyNameForMethodName ("MethodWithLongName"));
      Assert.IsNull (ReflectionUtility.GetPropertyNameForMethodName ("get_"));
      Assert.IsNull (ReflectionUtility.GetPropertyNameForMethodName ("set_"));

      Assert.AreEqual ("Prop", ReflectionUtility.GetPropertyNameForMethodName ("get_Prop"));
      Assert.AreEqual ("Prop", ReflectionUtility.GetPropertyNameForMethodName ("set_Prop"));

      Assert.IsNull (ReflectionUtility.GetPropertyForMethod (typeof (object).GetMethod ("ToString")));
      
      Assert.AreEqual (typeof (OrderWithNewPropertyAccess).GetProperty ("OrderNumber"),
          ReflectionUtility.GetPropertyForMethod(typeof (OrderWithNewPropertyAccess).GetMethod ("get_OrderNumber")));
      Assert.AreEqual (typeof (OrderWithNewPropertyAccess).GetProperty ("OrderNumber"),
          ReflectionUtility.GetPropertyForMethod (typeof (OrderWithNewPropertyAccess).GetMethod ("set_OrderNumber")));

      PropertyInfo privateProperty = typeof (NewPropertyStyleDomainObjectsWithFactoryTest).GetProperty ("TestProperty", BindingFlags.NonPublic
          | BindingFlags.Instance);
      Assert.IsNotNull (privateProperty);
      Assert.AreEqual (privateProperty, ReflectionUtility.GetPropertyForMethod(privateProperty.GetGetMethod(true)));
      Assert.AreEqual (privateProperty, ReflectionUtility.GetPropertyForMethod(privateProperty.GetSetMethod(true)));

      privateProperty = typeof (NewPropertyStyleDomainObjectsWithFactoryTest).GetProperty ("StaticTestProperty", BindingFlags.NonPublic
          | BindingFlags.Static);
      Assert.IsNotNull (privateProperty);
      Assert.AreEqual (privateProperty, ReflectionUtility.GetPropertyForMethod (privateProperty.GetGetMethod (true)));
      Assert.AreEqual (privateProperty, ReflectionUtility.GetPropertyForMethod (privateProperty.GetSetMethod (true)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void AbstractWithMethodCannotBeInstantiated ()
    {
      using (new FactoryInstantiationScope ())
      {
        DomainObject.Create<NonInstantiableAbstractClass> ();
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void AbstractWithNonAutoPropertiesCannotBeInstantiated ()
    {
      using (new FactoryInstantiationScope ())
      {
        DomainObject.Create<NonInstantiableAbstractClass> ();
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void SealedCannotBeInstantiated ()
    {
      using (new FactoryInstantiationScope ())
      {
        DomainObject.Create<NonInstantiableSealedClass> ();
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void NonDomainCannotBeInstantiated ()
    {
      DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory.Create (typeof (NonInstantiableNonDomainClass),
          new object[0]);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException))]
    public void WrongConstructorCannotBeInstantiated ()
    {
      DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory.Create(typeof(OrderWithNewPropertyAccess),
          new object[] { "foo", "bar", "foobar", null });
    }

    [Test]
    [ExpectedException (typeof (InvalidCastException))]
    public void OldConstructorThrowIsPropagated ()
    {
      DomainObject.Create<Throws> ();
    }

    [Test]
    [ExpectedException (typeof (InvalidCastException))]
    public void NewConstructorThrowIsPropagated ()
    {
      using (new FactoryInstantiationScope ())
      {
        DomainObject.Create<Throws> ();
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException))]
    public void OldConstructorMismatch1 ()
    {
      DomainObject.Create<ClassWithWrongConstructor> ();
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException))]
    public void NewConstructorMismatch1 ()
    {
      using (new FactoryInstantiationScope ())
      {
        DomainObject.Create<ClassWithWrongConstructor> ();
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException))]
    public void OldConstructorMismatch2 ()
    {
      DomainObject.Create<ClassWithWrongConstructor> (ClientTransaction.Current);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException))]
    public void NewConstructorMismatch2 ()
    {
      using (new FactoryInstantiationScope ())
      {
        DomainObject.Create<ClassWithWrongConstructor> (ClientTransaction.Current);
      }
    }

    [Test]
    public void GetSetRelatedObjectAndOriginal ()
    {
      OrderWithNewPropertyAccess order = DomainObject.GetObject<OrderWithNewPropertyAccess> (DomainObjectIDs.OrderWithNewPropertyAccess1);
      Customer customer = order.Customer;
      Assert.IsNotNull (customer);
      Assert.AreSame (DomainObject.GetObject<Customer> (DomainObjectIDs.Customer1), customer);
      
      Customer newCustomer = DomainObject.Create<Customer> ();
      Assert.IsNotNull (newCustomer);
      order.Customer = newCustomer;
      Assert.AreSame (newCustomer, order.Customer);

      Assert.AreSame (customer, order.OriginalCustomer);
    }

    [Test]
    public void GetRelatedObjects()
    {
      OrderWithNewPropertyAccess order = DomainObject.GetObject<OrderWithNewPropertyAccess> (DomainObjectIDs.OrderWithNewPropertyAccess1);
      DomainObjectCollection orderItems = order.OrderItems;
      Assert.IsNotNull (orderItems);
      Assert.AreEqual (2, orderItems.Count);

      Assert.IsTrue (orderItems.Contains (DomainObjectIDs.OrderItemWithNewPropertyAccess1));
      Assert.IsTrue (orderItems.Contains (DomainObjectIDs.OrderItemWithNewPropertyAccess2));
      
      OrderItemWithNewPropertyAccess newItem = DomainObject.Create<OrderItemWithNewPropertyAccess> ();
      order.OrderItems.Add (newItem);

      Assert.IsTrue (order.OrderItems.ContainsObject (newItem));
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException))]
    public void PropertyAccessWithoutBeingInMappingThrows ()
    {
      OrderWithNewPropertyAccess order = DomainObject.Create<OrderWithNewPropertyAccess> ();
      int i = order.NotInMapping;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void RelatedAccessWithoutBeingInMappingThrows ()
    {
      OrderWithNewPropertyAccess order = DomainObject.Create<OrderWithNewPropertyAccess> ();
      DomainObject o = order.NotInMappingRelated;
    }

    [Test]
    public void DefaultRelatedObject ()
    {
      OrderWithNewPropertyAccess order = DomainObject.GetObject<OrderWithNewPropertyAccess> (DomainObjectIDs.OrderWithNewPropertyAccess1);
      OrderItemWithNewPropertyAccess item = (OrderItemWithNewPropertyAccess) order.OrderItems[0];
      Assert.AreSame (order, item.Order);
      
      OrderWithNewPropertyAccess newOrder = DomainObject.Create<OrderWithNewPropertyAccess> ();
      Assert.IsNotNull (newOrder);
      item.Order = newOrder;
      Assert.AreNotSame (order, item.Order);
      Assert.AreSame (newOrder, item.Order);
    }

    [Test]
    public void IsRelatedObject ()
    {
      Assert.IsTrue (DomainObjectPropertyInterceptor.IsRelatedObject (typeof (OrderWithNewPropertyAccess),
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderWithNewPropertyAccess.Customer"));
      Assert.IsTrue (DomainObjectPropertyInterceptor.IsRelatedObject (typeof (OrderWithNewPropertyAccess),
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderWithNewPropertyAccess.OrderItems"));
      Assert.IsFalse (DomainObjectPropertyInterceptor.IsRelatedObject (typeof (OrderWithNewPropertyAccess),
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderWithNewPropertyAccess.OrderNumber"));
      Assert.IsFalse (DomainObjectPropertyInterceptor.IsRelatedObject (typeof (OrderWithNewPropertyAccess),
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderWithNewPropertyAccess.DeliveryDate"));
    }

    [Test]
    public void IsPropertyValue ()
    {
      Assert.IsFalse (DomainObjectPropertyInterceptor.IsPropertyValue (typeof (OrderWithNewPropertyAccess),
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderWithNewPropertyAccess.Customer"));
      Assert.IsFalse (DomainObjectPropertyInterceptor.IsPropertyValue (typeof (OrderWithNewPropertyAccess),
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderWithNewPropertyAccess.OrderItems"));
      Assert.IsTrue (DomainObjectPropertyInterceptor.IsPropertyValue (typeof (OrderWithNewPropertyAccess),
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderWithNewPropertyAccess.OrderNumber"));
      Assert.IsTrue (DomainObjectPropertyInterceptor.IsPropertyValue (typeof (OrderWithNewPropertyAccess),
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderWithNewPropertyAccess.DeliveryDate"));
    }
  }
}
