using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.Resources;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Development.UnitTesting;
using System.Reflection;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class NewPropertyStyleDomainObjectsWithFactoryTest : ClientTransactionBaseTest
  {
    // types

    public abstract class NonInstantiableAbstractClass : DomainObject
    {
      public NonInstantiableAbstractClass () { }
      public abstract void Foo ();
    }

    public abstract class NonInstantiableAbstractClassWithProps : DomainObject
    {
      public abstract int Foo { get; }
    }

    public sealed class NonInstantiableSealedClass : DomainObject
    {
    }

    public class NonInstantiableNonDomainClass
    {
    }

    // static members and constants

    // member fields

    // construction and disposing

    public NewPropertyStyleDomainObjectsWithFactoryTest ()
    {
    }

    // methods and properties

    [Test]
    public void LoadOfSimpleObjectWorks ()
    {
      OrderWithNewPropertyAccess order = OrderWithNewPropertyAccess.GetObject (DomainObjectIDs.OrderWithNewPropertyAccess1);
      Assert.IsTrue (DomainObjectFactory.WasCreatedByFactory (order));
    }

    [Test]
    public void ConstructionOfSimpleObjectWorks ()
    {
      OrderWithNewPropertyAccess order = DomainObjectFactory.Create<OrderWithNewPropertyAccess> ();
      Assert.IsTrue (DomainObjectFactory.WasCreatedByFactory (order));
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
      DomainObjectFactory.Create<NonInstantiableAbstractClass> ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void AbstractWithNonAutoPropertiesCannotBeInstantiated ()
    {
      DomainObjectFactory.Create<NonInstantiableAbstractClass> ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void SealedCannotBeInstantiated ()
    {
      DomainObjectFactory.Create<NonInstantiableSealedClass> ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void NonDomainCannotBeInstantiated ()
    {
      DomainObjectFactory.Create (typeof(NonInstantiableNonDomainClass));
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException))]
    public void WrongConstructorCannotBeInstantiated ()
    {
      PrivateInvoke.InvokeNonPublicStaticMethod (typeof (DomainObjectFactory), "Create", typeof(OrderWithNewPropertyAccess),
        new object[] { "foo", "bar", "foobar", null });
    }
  }
}
