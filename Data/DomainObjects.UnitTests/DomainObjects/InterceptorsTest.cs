using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Infrastructure.Interception;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class InterceptorsTest : StandardMappingTest
  {
    [Test]
    public void PropInterceptorShouldInterceptNonAutoPropertiesInMapping ()
    {
      DomainObjectPropertyInterceptor interceptor = new DomainObjectPropertyInterceptor ();
      DomainObjectPropertyInterceptorSelector selector = new DomainObjectPropertyInterceptorSelector (interceptor);
      
      DomainObjectInterceptorSelector outerSelector = new DomainObjectInterceptorSelector ();

      MethodInfo valuePropertyGet = typeof (OrderWithNewPropertyAccess).GetMethod ("get_DeliveryDate");
      Assert.IsFalse (valuePropertyGet.IsAbstract);
      CheckInterception (true, selector, interceptor, valuePropertyGet);
      CheckInterception (true, outerSelector, outerSelector.PropertyInterceptor, valuePropertyGet);

      MethodInfo valuePropertySet = typeof (OrderWithNewPropertyAccess).GetMethod ("set_DeliveryDate");
      Assert.IsFalse (valuePropertySet.IsAbstract);
      CheckInterception (true, selector, interceptor, valuePropertySet);
      CheckInterception (true, outerSelector, outerSelector.PropertyInterceptor, valuePropertySet);

      MethodInfo relatedObjectGet = typeof (OrderWithNewPropertyAccess).GetMethod ("get_Customer");
      Assert.IsFalse (relatedObjectGet.IsAbstract);
      CheckInterception (true, selector, interceptor, relatedObjectGet);
      CheckInterception (true, outerSelector, outerSelector.PropertyInterceptor, relatedObjectGet);

      MethodInfo relatedObjectSet = typeof (OrderWithNewPropertyAccess).GetMethod ("set_Customer");
      Assert.IsFalse (relatedObjectSet.IsAbstract);
      CheckInterception (true, selector, interceptor, relatedObjectSet);
      CheckInterception (true, outerSelector, outerSelector.PropertyInterceptor, relatedObjectSet);

      MethodInfo relatedObjectsGet = typeof (OrderWithNewPropertyAccess).GetMethod ("get_OrderItems");
      Assert.IsFalse (relatedObjectsGet.IsAbstract);
      CheckInterception (true, selector, interceptor, relatedObjectsGet);
      CheckInterception (true, outerSelector, outerSelector.PropertyInterceptor, relatedObjectsGet);
    }
    [Test]
    [Ignore("Implement inheritence root first")]
    public void PropInterceptorShouldInterceptNonAutoPropertiesInMapping_ForBaseTypeNotInMapping ()
    {
      DomainObjectPropertyInterceptor interceptor = new DomainObjectPropertyInterceptor();
      DomainObjectPropertyInterceptorSelector selector = new DomainObjectPropertyInterceptorSelector (interceptor);

      DomainObjectInterceptorSelector outerSelector = new DomainObjectInterceptorSelector();

      MethodInfo basePropertyGet = typeof (OrderWithNewPropertyAccess).GetMethod ("get_BaseProperty");
      Assert.IsFalse (basePropertyGet.IsAbstract);
      CheckInterception (true, selector, interceptor, basePropertyGet);
      CheckInterception (true, outerSelector, outerSelector.PropertyInterceptor, basePropertyGet);

      MethodInfo basePropertySet = typeof (OrderWithNewPropertyAccess).GetMethod ("set_BaseProperty");
      Assert.IsFalse (basePropertySet.IsAbstract);
      CheckInterception (true, selector, interceptor, basePropertySet);
      CheckInterception (true, outerSelector, outerSelector.PropertyInterceptor, basePropertySet);
    }

    [Test]
    public void PropInterceptorShouldInterceptAutoPropertiesInMapping ()
    {
      DomainObjectPropertyInterceptor interceptor = new DomainObjectPropertyInterceptor ();
      DomainObjectPropertyInterceptorSelector selector = new DomainObjectPropertyInterceptorSelector (interceptor);
      DomainObjectInterceptorSelector outerSelector = new DomainObjectInterceptorSelector ();

      MethodInfo valuePropertyGet = typeof (Order).GetMethod ("get_DeliveryDate");
      Assert.IsTrue (valuePropertyGet.IsAbstract);
      CheckInterception (true, selector, interceptor, valuePropertyGet);
      CheckInterception (true, outerSelector, outerSelector.PropertyInterceptor, valuePropertyGet);

      MethodInfo valuePropertySet = typeof (Order).GetMethod ("set_DeliveryDate");
      Assert.IsTrue (valuePropertySet.IsAbstract);
      CheckInterception (true, selector, interceptor, valuePropertySet);
      CheckInterception (true, outerSelector, outerSelector.PropertyInterceptor, valuePropertySet);

      MethodInfo relatedObjectGet = typeof (Order).GetMethod ("get_Customer");
      Assert.IsTrue (relatedObjectGet.IsAbstract);
      CheckInterception (true, selector, interceptor, relatedObjectGet);
      CheckInterception (true, outerSelector, outerSelector.PropertyInterceptor, relatedObjectGet);

      MethodInfo relatedObjectSet = typeof (Order).GetMethod ("set_Customer");
      Assert.IsTrue (relatedObjectSet.IsAbstract);
      CheckInterception (true, selector, interceptor, relatedObjectSet);
      CheckInterception (true, outerSelector, outerSelector.PropertyInterceptor, relatedObjectSet);

      MethodInfo relatedObjectsGet = typeof (Order).GetMethod ("get_OrderItems");
      Assert.IsTrue (relatedObjectsGet.IsAbstract);
      CheckInterception (true, selector, interceptor, relatedObjectsGet);
      CheckInterception (true, outerSelector, outerSelector.PropertyInterceptor, relatedObjectsGet);
    }

    [Test]
    public void NoInterceptorShouldInterceptPropertiesNotInMapping ()
    {
      DomainObjectPropertyInterceptor interceptor = new DomainObjectPropertyInterceptor();
      DomainObjectPropertyInterceptorSelector selector = new DomainObjectPropertyInterceptorSelector (interceptor);
      DomainObjectInterceptorSelector outerSelector = new DomainObjectInterceptorSelector ();

      MethodInfo valuePropertyGet = typeof (OrderWithNewPropertyAccess).GetMethod ("get_NotInMapping");
      CheckInterception (false, selector, null, valuePropertyGet);
      CheckInterception (false, outerSelector, null, valuePropertyGet);

      MethodInfo valuePropertySet = typeof (OrderWithNewPropertyAccess).GetMethod ("set_NotInMapping");
      CheckInterception (false, selector, null, valuePropertySet);
      CheckInterception (false, outerSelector, null, valuePropertySet);

      MethodInfo relatedObjectGet = typeof (OrderWithNewPropertyAccess).GetMethod ("get_NotInMappingRelated");
      CheckInterception (false, selector, null, relatedObjectGet);
      CheckInterception (false, outerSelector, null, relatedObjectGet);

      MethodInfo relatedObjectSet = typeof (OrderWithNewPropertyAccess).GetMethod ("set_NotInMappingRelated");
      CheckInterception (false, selector, null, relatedObjectSet);
      CheckInterception (false, outerSelector, null, relatedObjectSet);

      MethodInfo relatedObjectsGet = typeof (OrderWithNewPropertyAccess).GetMethod ("get_NotInMappingRelatedObjects");
      CheckInterception (false, selector, null, relatedObjectsGet);
      CheckInterception (false, outerSelector, null, relatedObjectsGet);
    }

    [Test]
    public void PropInterceptorShouldNotInterceptAnythingButProperties ()
    {
      DomainObjectPropertyInterceptor interceptor = new DomainObjectPropertyInterceptor ();
      DomainObjectPropertyInterceptorSelector selector = new DomainObjectPropertyInterceptorSelector (interceptor);

      MethodInfo[] methods = typeof (OrderWithNewPropertyAccess).GetMethods (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      foreach (MethodInfo method in methods)
      {
        if (!ReflectionUtility.IsPropertyAccessor (method))
        {
          CheckInterception (false, selector, null, method);
        }
      }
    }

    [Test]
    public void TypeInterceptorShouldInterceptGetPublicType ()
    {
      DomainObjectTypeInterceptor interceptor = new DomainObjectTypeInterceptor ();
      DomainObjectTypeInterceptorSelector selector = new DomainObjectTypeInterceptorSelector (interceptor);
      DomainObjectInterceptorSelector outerSelector = new DomainObjectInterceptorSelector ();

      MethodInfo getPublicType = typeof (OrderWithNewPropertyAccess).GetMethod ("GetPublicDomainObjectType", 
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      CheckInterception (true, selector, interceptor, getPublicType);
      CheckInterception (true, outerSelector, outerSelector.TypeInterceptor, getPublicType);
    }

    [Test]
    public void TypeInterceptorShouldNotInterceptAnythingElse()
    {
      DomainObjectTypeInterceptor interceptor = new DomainObjectTypeInterceptor ();
      DomainObjectTypeInterceptorSelector selector = new DomainObjectTypeInterceptorSelector (interceptor);

      MethodInfo getPublicType = typeof (OrderWithNewPropertyAccess).GetMethod ("GetPublicDomainObjectType",
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

      MethodInfo[] methods = typeof (OrderWithNewPropertyAccess).GetMethods (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      foreach (MethodInfo method in methods)
      {
        if (!getPublicType.Equals (getPublicType))
        {
          CheckInterception (false, selector, null, method);
        }
      }
    }

    private void CheckInterception<T> (bool shouldIntercept, IInterceptorSelector<T> selector, IInterceptor<T> interceptor, MethodInfo methodToCheck)
    {
      Assert.AreEqual (shouldIntercept, selector.ShouldInterceptMethod (methodToCheck.DeclaringType, methodToCheck));
      if (shouldIntercept)
      {
        Assert.AreSame (interceptor, selector.SelectInterceptor (methodToCheck.DeclaringType, methodToCheck));
      }
    }

    [Test]
    [ExpectedException(typeof (NonInterceptableTypeException), ExpectedMessage =  "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests."
        + "DomainObjects.NewPropertyStyleDomainObjectsWithFactoryTest+NonInstantiableAbstractClassWithProps, property Foo is abstract but not "
        + "defined in the mapping (assumed property id: Rubicon.Data.DomainObjects.UnitTests.DomainObjects."
        + "NewPropertyStyleDomainObjectsWithFactoryTest+NonInstantiableAbstractClassWithProps.Foo).")]
    public void PropSelectionThrowsOnAbstractUndefined ()
    {
      DomainObjectPropertyInterceptor interceptor = new DomainObjectPropertyInterceptor ();
      DomainObjectPropertyInterceptorSelector selector = new DomainObjectPropertyInterceptorSelector (interceptor);

      selector.ShouldInterceptMethod (typeof (NewPropertyStyleDomainObjectsWithFactoryTest.NonInstantiableAbstractClassWithProps),
        typeof (NewPropertyStyleDomainObjectsWithFactoryTest.NonInstantiableAbstractClassWithProps).GetMethod ("get_Foo"));
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests."
        + "DomainObjects.NewPropertyStyleDomainObjectsWithFactoryTest+NonInstantiableClassWithAutomaticRelatedCollectionSetter, "
        + "automatic properties for related object collections cannot have setters: property 'RelatedObjects', property id 'Rubicon.Data."
        + "DomainObjects.UnitTests.DomainObjects.NewPropertyStyleDomainObjectsWithFactoryTest+NonInstantiableClassWithAutomaticRelatedCollectionSetter."
        + "RelatedObjects'.")]
    public void PropSelectionThrowsOnAbstractRelatedCollectionSetter ()
    {
      DomainObjectPropertyInterceptor interceptor = new DomainObjectPropertyInterceptor ();
      DomainObjectPropertyInterceptorSelector selector = new DomainObjectPropertyInterceptorSelector (interceptor);

      selector.ShouldInterceptMethod (typeof (NewPropertyStyleDomainObjectsWithFactoryTest.NonInstantiableClassWithAutomaticRelatedCollectionSetter),
        typeof (NewPropertyStyleDomainObjectsWithFactoryTest.NonInstantiableClassWithAutomaticRelatedCollectionSetter).GetMethod ("set_RelatedObjects"));
    }

    [Test]
    public void GetIdentifierFromProperty ()
    {
      PropertyInfo property = typeof (OrderWithNewPropertyAccess).GetProperty ("Customer");
      Assert.AreEqual (property.DeclaringType.FullName + "." + property.Name, DomainObjectPropertyInterceptor.GetIdentifierFromProperty (property));
    }

    [Test]
    public void GetClassDefinitionIfRelevantProperty ()
    {
      Assert.IsNotNull (DomainObjectPropertyInterceptorSelector.GetClassDefinitionIfRelevantProperty (typeof (OrderWithNewPropertyAccess),
          MakeID ("OrderItems")));
      Assert.IsNotNull (DomainObjectPropertyInterceptorSelector.GetClassDefinitionIfRelevantProperty (typeof (OrderWithNewPropertyAccess),
          MakeID ("OrderNumber")));
      Assert.IsNotNull (DomainObjectPropertyInterceptorSelector.GetClassDefinitionIfRelevantProperty (typeof (OrderWithNewPropertyAccess),
          MakeID ("Customer")));

      Assert.IsNull (DomainObjectPropertyInterceptorSelector.GetClassDefinitionIfRelevantProperty (typeof (OrderWithNewPropertyAccess),
          MakeID ("NotInMapping")));
      Assert.IsNull (DomainObjectPropertyInterceptorSelector.GetClassDefinitionIfRelevantProperty (typeof (OrderWithNewPropertyAccess),
          MakeID ("NotInMappingRelated")));
      Assert.IsNull (DomainObjectPropertyInterceptorSelector.GetClassDefinitionIfRelevantProperty (typeof (OrderWithNewPropertyAccess),
          MakeID ("NotInMappingRelatedObjects")));
      Assert.IsNull (DomainObjectPropertyInterceptorSelector.GetClassDefinitionIfRelevantProperty (typeof (OrderWithNewPropertyAccess),
          MakeID ("Bla")));
    }

    private string MakeID (string propertyName)
    {
      return typeof (OrderWithNewPropertyAccess).FullName + "." + propertyName;
    }


  }
}
