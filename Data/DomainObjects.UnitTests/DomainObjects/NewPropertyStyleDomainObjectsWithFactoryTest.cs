using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.Resources;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Development.UnitTesting;
using System.Reflection;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Infrastructure.Interception;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class NewPropertyStyleDomainObjectsWithFactoryTest : ClientTransactionBaseTest
  {
    [DBTable]
    [NotAbstract]
    public abstract class NonInstantiableAbstractClass : DomainObject
    {
      public static NonInstantiableAbstractClass NewObject()
      {
        return DomainObject.NewObject<NonInstantiableAbstractClass>().With();
      }

      protected NonInstantiableAbstractClass ()
      {
      }
  
      public abstract void Foo ();
    }

    [NotAbstract]
    [DBTable]
    public abstract class NonInstantiableAbstractClassWithProps : DomainObject
    {
      public static NonInstantiableAbstractClassWithProps NewObject ()
      {
        return DomainObject.NewObject<NonInstantiableAbstractClassWithProps> ().With();
      }

      protected NonInstantiableAbstractClassWithProps()
      {
      }

      [StorageClassNone]
      public abstract int Foo { get; }
    }

    [DBTable]
    public sealed class NonInstantiableSealedClass : DomainObject
    {
      public static NonInstantiableSealedClass NewObject ()
      {
        return DomainObject.NewObject<NonInstantiableSealedClass> ().With();
      }

      protected NonInstantiableSealedClass()
      {
      }
    }

    public class NonInstantiableNonDomainClass
    {
    }

    [DBTable]
    public class Throws : DomainObject
    {
      public static Throws NewObject ()
      {
        return DomainObject.NewObject<Throws> ().With();
      }

      public Throws ()
        : base (((int) (object) "this always throws before entering base constructor!") == 5 ? (DataContainer) null : (DataContainer) null)
      {
      }
    }

    [DBTable]
    public class ClassWithWrongConstructor : DomainObject
    {
      public static ClassWithWrongConstructor NewObject ()
      {
        return DomainObject.NewObject<ClassWithWrongConstructor> ().With();
      }

      public static ClassWithWrongConstructor NewObject (ClientTransaction clientTransaction)
      {
        return DomainObject.NewObject<ClassWithWrongConstructor> ().With (clientTransaction);
      }

      public ClassWithWrongConstructor (string s)
      {
        Assert.Fail ("Shouldn't be executed.");
      }
    }

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();
      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration (), DomainObjectsConfiguration.Current.Storage));
    }

    [TearDown]
    public override void TearDown ()
    {
      base.TearDown ();
    }

    private bool WasCreatedByFactory (object o)
    {
      return DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory.WasCreatedByFactory (o.GetType ());
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
      OrderWithNewPropertyAccess order = OrderWithNewPropertyAccess.NewObject ();
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
    public void GetPropertyValue_WithNullAndAbstractProperty ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject();
      Assert.That (classWithAllDataTypes.StringWithNullValueProperty, Is.Null);
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
    public void SetPropertyValue_WithNullAndAbstractProperty ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject ();
      classWithAllDataTypes.StringWithNullValueProperty = null;
      Assert.That (classWithAllDataTypes.StringWithNullValueProperty, Is.Null);
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
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests."
        + "DomainObjects.NewPropertyStyleDomainObjectsWithFactoryTest+NonInstantiableAbstractClass as its member Foo is abstract (and not an "
        + "automatic property).\r\nParameter name: type")]
    public void AbstractWithMethodCannotBeInstantiated ()
    {
      using (new FactoryInstantiationScope ())
      {
        NonInstantiableAbstractClass.NewObject ();
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot instantiate type "
        + "Rubicon.Data.DomainObjects.UnitTests.DomainObjects.NewPropertyStyleDomainObjectsWithFactoryTest+NonInstantiableAbstractClassWithProps, "
        + "property Foo is abstract but not defined in the mapping (assumed property id: "
        + "Rubicon.Data.DomainObjects.UnitTests.DomainObjects.NewPropertyStyleDomainObjectsWithFactoryTest+NonInstantiableAbstractClassWithProps.Foo)."
        + "\r\nParameter name: type")]
    public void AbstractWithNonAutoPropertiesCannotBeInstantiated ()
    {
      using (new FactoryInstantiationScope ())
      {
        NonInstantiableAbstractClassWithProps.NewObject ();
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests.DomainObjects."
        + "NewPropertyStyleDomainObjectsWithFactoryTest+NonInstantiableSealedClass as it is sealed.\r\nParameter name: baseType")]
    public void SealedCannotBeInstantiated ()
    {
      using (new FactoryInstantiationScope ())
      {
        NonInstantiableSealedClass.NewObject ();
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests.DomainObjects."
        + "NewPropertyStyleDomainObjectsWithFactoryTest+NonInstantiableNonDomainClass as it is not derived from DomainObject.\r\nParameter name: baseType")]
    public void NonDomainCannotBeInstantiated ()
    {
      DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory.GetConcreteDomainObjectType (typeof (NonInstantiableNonDomainClass));
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Rubicon.Data.DomainObjects.UnitTests.TestDomain."
        + "OrderWithNewPropertyAccess does not support the requested constructor with signature (System.String, System.String, System.String, "
        + "System.Object).")]
    public void WrongConstructorCannotBeInstantiated ()
    {
      Type t = DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory.GetConcreteDomainObjectType(typeof(OrderWithNewPropertyAccess));;
      DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory.GetTypesafeConstructorInvoker<object> (t)
        .With ("foo", "bar", "foobar", (object)null);
    }

    [Test]
    [ExpectedException (typeof (InvalidCastException))]
    public void OldConstructorThrowIsPropagated ()
    {
      Throws.NewObject ();
    }

    [Test]
    [ExpectedException (typeof (InvalidCastException))]
    public void NewConstructorThrowIsPropagated ()
    {
      using (new FactoryInstantiationScope ())
      {
        Throws.NewObject ();
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = 
        "Type Rubicon.Data.DomainObjects.UnitTests.DomainObjects.NewPropertyStyleDomainObjectsWithFactoryTest+ClassWithWrongConstructor does not "
        + "support the requested constructor with signature ().")]
    public void OldConstructorMismatch1()
    {
      ClassWithWrongConstructor.NewObject();
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Rubicon.Data.DomainObjects.UnitTests.DomainObjects."
        + "NewPropertyStyleDomainObjectsWithFactoryTest+ClassWithWrongConstructor does not support the requested constructor with signature "
        + "().")]
    public void NewConstructorMismatch1 ()
    {
      using (new FactoryInstantiationScope ())
      {
        ClassWithWrongConstructor.NewObject();
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage =
        "Type Rubicon.Data.DomainObjects.UnitTests.DomainObjects.NewPropertyStyleDomainObjectsWithFactoryTest+ClassWithWrongConstructor does not "
        + "support the requested constructor with signature (Rubicon.Data.DomainObjects.ClientTransaction).")]
    public void OldConstructorMismatch2 ()
    {
      ClassWithWrongConstructor.NewObject (ClientTransaction.Current);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Rubicon.Data.DomainObjects.UnitTests.DomainObjects."
        + "NewPropertyStyleDomainObjectsWithFactoryTest+ClassWithWrongConstructor does not support the requested constructor with signature "
        + "(Rubicon.Data.DomainObjects.ClientTransaction).")]
    public void NewConstructorMismatch2 ()
    {
      using (new FactoryInstantiationScope ())
      {
        ClassWithWrongConstructor.NewObject (ClientTransaction.Current);
      }
    }

    [Test]
    public void GetSetRelatedObjectAndOriginal ()
    {
      OrderWithNewPropertyAccess order = DomainObject.GetObject<OrderWithNewPropertyAccess> (DomainObjectIDs.OrderWithNewPropertyAccess1);
      Customer customer = order.Customer;
      Assert.IsNotNull (customer);
      Assert.AreSame (DomainObject.GetObject<Customer> (DomainObjectIDs.Customer1), customer);

      Customer newCustomer = Customer.NewObject ();
      Assert.IsNotNull (newCustomer);
      order.Customer = newCustomer;
      Assert.AreSame (newCustomer, order.Customer);

      Assert.AreSame (customer, order.OriginalCustomer);
    }

    [Test]
    public void GetSetRelatedObjectAndOriginal_WithNullAndAutomaticProperty ()
    {
      OrderWithNewPropertyAccess order = DomainObject.GetObject<OrderWithNewPropertyAccess> (DomainObjectIDs.OrderWithNewPropertyAccess1);
      Assert.IsNotEmpty (order.OrderItems);
      OrderItemWithNewPropertyAccess orderItem = order.OrderItems[0];

      orderItem.Order = null;
      Assert.IsNull (orderItem.Order);

      Assert.AreSame (order, orderItem.OriginalOrder);
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

      OrderItemWithNewPropertyAccess newItem = OrderItemWithNewPropertyAccess.NewObject ();
      order.OrderItems.Add (newItem);

      Assert.IsTrue (order.OrderItems.ContainsObject (newItem));
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "There is no current property or it hasn't been properly initialized.")]
    public void PropertyAccessWithoutBeingInMappingThrows ()
    {
      OrderWithNewPropertyAccess order = OrderWithNewPropertyAccess.NewObject ();
      Dev.Null = order.NotInMapping;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property or it hasn't been properly initialized.")]
    public void RelatedAccessWithoutBeingInMappingThrows ()
    {
      OrderWithNewPropertyAccess order = OrderWithNewPropertyAccess.NewObject ();
      Dev.Null = order.NotInMappingRelated;
    }

    [Test]
    public void DefaultRelatedObject ()
    {
      OrderWithNewPropertyAccess order = DomainObject.GetObject<OrderWithNewPropertyAccess> (DomainObjectIDs.OrderWithNewPropertyAccess1);
      OrderItemWithNewPropertyAccess item = order.OrderItems[0];
      Assert.AreSame (order, item.Order);

      OrderWithNewPropertyAccess newOrder = OrderWithNewPropertyAccess.NewObject ();
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

    [Test][ExpectedException(typeof(ArgumentException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests.TestDomain."
        + "AbstractClassNotInMapping as it is abstract; for classes with automatic properties, NotAbstractAttribute must be used.\r\nParameter name: baseType")]
    public void CannotInstantiateReallyAbstractClass ()
    {
      using (new FactoryInstantiationScope ())
      {
        AbstractClassNotInMapping.NewObject ();
      }
    }
  }
}
