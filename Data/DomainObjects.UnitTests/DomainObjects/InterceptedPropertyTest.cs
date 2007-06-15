using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class InterceptedPropertyTest : ClientTransactionBaseTest
  {
    [DBTable]
    [Instantiable]
    public abstract class NonInstantiableAbstractClass : DomainObject
    {
      public static NonInstantiableAbstractClass NewObject()
      {
        return NewObject<NonInstantiableAbstractClass>().With();
      }

      protected NonInstantiableAbstractClass ()
      {
      }
  
      public abstract void Foo ();
    }

    [Instantiable]
    [DBTable]
    public abstract class NonInstantiableAbstractClassWithProps : DomainObject
    {
      public static NonInstantiableAbstractClassWithProps NewObject ()
      {
        return NewObject<NonInstantiableAbstractClassWithProps> ().With();
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
        return NewObject<NonInstantiableSealedClass> ().With();
      }

      public NonInstantiableSealedClass()
      {
      }
    }

    [DBTable]
    [Instantiable]
    public abstract class NonInstantiableClassWithAutomaticRelatedCollectionSetter : DomainObject
    {
      public static NonInstantiableClassWithAutomaticRelatedCollectionSetter NewObject ()
      {
        return NewObject<NonInstantiableClassWithAutomaticRelatedCollectionSetter> ().With ();
      }

      protected NonInstantiableClassWithAutomaticRelatedCollectionSetter()
      {
      }

      [DBBidirectionalRelation ("RelatedObjects")]
      public abstract NonInstantiableClassWithAutomaticRelatedCollectionSetter Parent { get; }

      [DBBidirectionalRelation ("Parent")]
      public abstract ObjectList<NonInstantiableClassWithAutomaticRelatedCollectionSetter> RelatedObjects { get; set; }
    }

    public class NonInstantiableNonDomainClass
    {
    }

    [DBTable]
    public class Throws : DomainObject
    {
      public static Throws NewObject ()
      {
        return NewObject<Throws> ().With();
      }

      public Throws ()
        : base (ThrowException())
      {
      }

      private static DataContainer ThrowException ()
      {
        throw new Exception ("Thrown in ThrowException()");
      }
    }

    [DBTable]
    public class ClassWithWrongConstructor : DomainObject
    {
      public static ClassWithWrongConstructor NewObject ()
      {
        return NewObject<ClassWithWrongConstructor> ().With();
      }

      public static ClassWithWrongConstructor NewObject (ClientTransaction clientTransaction)
      {
        return NewObject<ClassWithWrongConstructor> ().With (clientTransaction);
      }

      public ClassWithWrongConstructor (string s)
      {
        Assert.Fail ("Shouldn't be executed.");
      }
    }

    public interface IPropertyInterface
    {
      int Property { get; set; }
    }

    [DBTable]
    public class ClassWithExplicitInterfaceProperty : DomainObject, IPropertyInterface
    {
      public static ClassWithExplicitInterfaceProperty NewObject()
      {
        return DomainObject.NewObject<ClassWithExplicitInterfaceProperty>().With();
      }

      protected ClassWithExplicitInterfaceProperty ()
      {
      }

      int IPropertyInterface.Property
      {
        get { return CurrentProperty.GetValue<int> (); }
        set { CurrentProperty.SetValue (value); }
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
      OrderWithNewPropertyAccess order = OrderWithNewPropertyAccess.GetObject (DomainObjectIDs.OrderWithNewPropertyAccess1);
      Assert.IsTrue (WasCreatedByFactory (order));
    }

    [Test]
    public void ConstructionOfSimpleObjectWorks ()
    {
      OrderWithNewPropertyAccess order = OrderWithNewPropertyAccess.NewObject ();
      Assert.IsTrue (WasCreatedByFactory (order));

      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject ();
      Assert.IsNotNull (classWithAllDataTypes);
      Assert.IsTrue (WasCreatedByFactory (classWithAllDataTypes));
    }

    [Test]
    public void ConstructedObjectIsDerived ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject ();
      Assert.IsTrue (classWithAllDataTypes is ClassWithAllDataTypes);
      Assert.IsFalse (classWithAllDataTypes.GetType ().Equals (typeof (ClassWithAllDataTypes)));
    }

    private bool ShouldUseFactoryForInstantiation (Type type)
    {
      return (bool) PrivateInvoke.InvokeNonPublicStaticMethod (typeof (DomainObject), "ShouldUseFactoryForInstantiation", type);
    }

    [Test]
    public void ShouldUseFactoryForInstantiation ()
    {
      Assert.IsTrue (ShouldUseFactoryForInstantiation (typeof (OrderItem)));

      using (new FactoryInstantiationScope ())
      {
        Assert.IsTrue (ShouldUseFactoryForInstantiation (typeof (ClassWithAllDataTypes)));
      }
      Assert.IsTrue (ShouldUseFactoryForInstantiation (typeof (ClassWithAllDataTypes)));
    }

    [Test]
    public void GetPropertyValueWorks ()
    {
      OrderWithNewPropertyAccess order = OrderWithNewPropertyAccess.GetObject (DomainObjectIDs.OrderWithNewPropertyAccess1);
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
      OrderWithNewPropertyAccess order = OrderWithNewPropertyAccess.GetObject (DomainObjectIDs.OrderWithNewPropertyAccess1);
      
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
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests."
        + "DomainObjects.InterceptedPropertyTest+NonInstantiableAbstractClass as its member Foo is abstract (and not an "
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
        + "Rubicon.Data.DomainObjects.UnitTests.DomainObjects.InterceptedPropertyTest+NonInstantiableAbstractClassWithProps, "
        + "property Foo is abstract but not defined in the mapping (assumed property id: "
        + "Rubicon.Data.DomainObjects.UnitTests.DomainObjects.InterceptedPropertyTest+NonInstantiableAbstractClassWithProps.Foo)."
        + "\r\nParameter name: type")]
    public void AbstractWithNonAutoPropertiesCannotBeInstantiated ()
    {
      using (new FactoryInstantiationScope ())
      {
        NonInstantiableAbstractClassWithProps.NewObject ();
      }
    }

    [Test]
    [ExpectedException(typeof (ArgumentException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests."
        + "DomainObjects.InterceptedPropertyTest+NonInstantiableClassWithAutomaticRelatedCollectionSetter, automatic "
        + "properties for related object collections cannot have setters: property 'RelatedObjects', property id 'Rubicon.Data.DomainObjects."
        + "UnitTests.DomainObjects.InterceptedPropertyTest+NonInstantiableClassWithAutomaticRelatedCollectionSetter."
        + "RelatedObjects'.\r\nParameter name: type")]
    public void AbstractWithAutoCollectionSetterCannotBeInstantiated ()
    {
      NonInstantiableClassWithAutomaticRelatedCollectionSetter.NewObject();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests.DomainObjects."
        + "InterceptedPropertyTest+NonInstantiableSealedClass as it is sealed.\r\nParameter name: baseType")]
    public void SealedCannotBeInstantiated ()
    {
      using (new FactoryInstantiationScope ())
      {
        NonInstantiableSealedClass.NewObject ();
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
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
    [ExpectedException (typeof (Exception), ExpectedMessage = "Thrown in ThrowException()")]
    public void OldConstructorThrowIsPropagated ()
    {
      Throws.NewObject ();
    }

    [Test]
    [ExpectedException (typeof (Exception), ExpectedMessage = "Thrown in ThrowException()")]
    public void NewConstructorThrowIsPropagated ()
    {
      using (new FactoryInstantiationScope ())
      {
        Throws.NewObject ();
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = 
        "Type Rubicon.Data.DomainObjects.UnitTests.DomainObjects.InterceptedPropertyTest+ClassWithWrongConstructor does not "
        + "support the requested constructor with signature ().")]
    public void OldConstructorMismatch1()
    {
      ClassWithWrongConstructor.NewObject();
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Rubicon.Data.DomainObjects.UnitTests.DomainObjects."
        + "InterceptedPropertyTest+ClassWithWrongConstructor does not support the requested constructor with signature "
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
        "Type Rubicon.Data.DomainObjects.UnitTests.DomainObjects.InterceptedPropertyTest+ClassWithWrongConstructor does not "
        + "support the requested constructor with signature (Rubicon.Data.DomainObjects.ClientTransaction).")]
    public void OldConstructorMismatch2 ()
    {
      ClassWithWrongConstructor.NewObject (ClientTransaction.Current);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Rubicon.Data.DomainObjects.UnitTests.DomainObjects."
        + "InterceptedPropertyTest+ClassWithWrongConstructor does not support the requested constructor with signature "
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
      OrderWithNewPropertyAccess order = OrderWithNewPropertyAccess.GetObject (DomainObjectIDs.OrderWithNewPropertyAccess1);
      Customer customer = order.Customer;
      Assert.IsNotNull (customer);
      Assert.AreSame (Customer.GetObject (DomainObjectIDs.Customer1), customer);

      Customer newCustomer = Customer.NewObject ();
      Assert.IsNotNull (newCustomer);
      order.Customer = newCustomer;
      Assert.AreSame (newCustomer, order.Customer);

      Assert.AreSame (customer, order.OriginalCustomer);
    }

    [Test]
    public void GetSetRelatedObjectAndOriginal_WithNullAndAutomaticProperty ()
    {
      OrderWithNewPropertyAccess order = OrderWithNewPropertyAccess.GetObject (DomainObjectIDs.OrderWithNewPropertyAccess1);
      Assert.IsNotEmpty (order.OrderItems);
      OrderItemWithNewPropertyAccess orderItem = order.OrderItems[0];

      orderItem.Order = null;
      Assert.IsNull (orderItem.Order);

      Assert.AreSame (order, orderItem.OriginalOrder);
    }

    [Test]
    public void GetRelatedObjects()
    {
      OrderWithNewPropertyAccess order = OrderWithNewPropertyAccess.GetObject (DomainObjectIDs.OrderWithNewPropertyAccess1);
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
    public void GetRelatedObjects_WithAutomaticProperties ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.IsNotNull (order.OrderItems);
      Assert.AreEqual (2, order.OrderItems.Count);

      Assert.IsTrue (order.OrderItems.Contains (DomainObjectIDs.OrderItem1));
      Assert.IsTrue (order.OrderItems.Contains (DomainObjectIDs.OrderItem2));

      OrderItem newItem = OrderItem.NewObject ();
      order.OrderItems.Add (newItem);

      Assert.IsTrue (order.OrderItems.ContainsObject (newItem));
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "There is no current property or it hasn't been properly initialized.",
        MatchType = MessageMatch.Contains)]
    public void PropertyAccessWithoutBeingInMappingThrows ()
    {
      OrderWithNewPropertyAccess order = OrderWithNewPropertyAccess.NewObject ();
      Dev.Null = order.NotInMapping;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property or it hasn't been properly initialized.",
        MatchType = MessageMatch.Contains)]
    public void RelatedAccessWithoutBeingInMappingThrows ()
    {
      OrderWithNewPropertyAccess order = OrderWithNewPropertyAccess.NewObject ();
      Dev.Null = order.NotInMappingRelated;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property or it hasn't been properly initialized.",
        MatchType = MessageMatch.Contains)]
    public void RelatedObjectsAccessWithoutBeingInMappingThrows ()
    {
      OrderWithNewPropertyAccess order = OrderWithNewPropertyAccess.NewObject ();
      Dev.Null = order.NotInMappingRelatedObjects;
    }


    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property or it hasn't been properly initialized.",
        MatchType = MessageMatch.Contains)]
    public void PropertySetAccessWithoutBeingInMappingThrows ()
    {
      OrderWithNewPropertyAccess order = OrderWithNewPropertyAccess.NewObject ();
      order.NotInMapping = 0;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property or it hasn't been properly initialized.",
        MatchType = MessageMatch.Contains)]
    public void RelatedSetAccessWithoutBeingInMappingThrows ()
    {
      OrderWithNewPropertyAccess order = OrderWithNewPropertyAccess.NewObject ();
      order.NotInMappingRelated = null;
    }

    [Test]
    public void DefaultRelatedObject ()
    {
      OrderWithNewPropertyAccess order = OrderWithNewPropertyAccess.GetObject (DomainObjectIDs.OrderWithNewPropertyAccess1);
      OrderItemWithNewPropertyAccess item = order.OrderItems[0];
      Assert.AreSame (order, item.Order);

      OrderWithNewPropertyAccess newOrder = OrderWithNewPropertyAccess.NewObject ();
      Assert.IsNotNull (newOrder);
      item.Order = newOrder;
      Assert.AreNotSame (order, item.Order);
      Assert.AreSame (newOrder, item.Order);
    }

    [Test][ExpectedException(typeof(ArgumentException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests.TestDomain."
        + "AbstractClass as it is abstract; for classes with automatic properties, InstantiableAttribute must be used.\r\nParameter name: baseType")]
    public void CannotInstantiateReallyAbstractClass ()
    {
      using (new FactoryInstantiationScope ())
      {
        AbstractClass.NewObject ();
      }
    }

    [Test]
    [ExpectedException(typeof (InvalidOperationException), ExpectedMessage = "There is no current property or it hasn't been properly initialized.",
        MatchType = MessageMatch.Contains)]
    public void ExplicitInterfaceProperty ()
    {
      IPropertyInterface domainObject = ClassWithExplicitInterfaceProperty.NewObject();
      domainObject.Property = 5;
      Assert.AreEqual (5, domainObject.Property);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "no current property", MatchType = MessageMatch.Contains)]
    public void CurrentPropertyThrowsWhenNotInitializes()
    {
      Order order = Order.NewObject();
      PropertyAccessor accessor = order.CurrentProperty;
      Assert.Fail ("Expected exception");
    }

    [Test]
    public void PreparePropertyAccessCorrectlySetsCurrentProperty()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.PreparePropertyAccess ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber");
      int orderNumber;
      try
      {
        orderNumber = order.CurrentProperty.GetValue<int>();
      }
      finally
      {
        order.PropertyAccessFinished();
      }
      Assert.AreEqual (order.OrderNumber, orderNumber);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "is not a valid property", MatchType = MessageMatch.Contains)]
    public void PreparePropertyAccessThrowsOnInvalidPropertyName ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.PreparePropertyAccess ("Bla");
    }
  }
}
