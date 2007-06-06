using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Infrastructure.Interception;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class InterceptorsTest : StandardMappingTest
  {
    [Test]
    public void PropInterceptorShouldInterceptNonAutoPropertiesInMapping ()
    {
      IInterceptor<DomainObject> interceptor = (IInterceptor<DomainObject>) InstantiateInternalType (typeof (DomainObject).Assembly,
          "Rubicon.Data.DomainObjects.Infrastructure.Interception.DomainObjectPropertyInterceptor");
      IInterceptorSelector<DomainObject> selector = (IInterceptorSelector<DomainObject>) GetFieldValue(interceptor, "Selector");
      
      IInterceptorSelector<DomainObject> outerSelector = (IInterceptorSelector<DomainObject>) InstantiateInternalType (typeof (DomainObject).Assembly,
          "Rubicon.Data.DomainObjects.Infrastructure.Interception.DomainObjectInterceptorSelector");
      IInterceptor<DomainObject> interceptorOfOuter = (IInterceptor<DomainObject>) GetFieldValue (outerSelector, "_propertyInterceptor");

      MethodInfo valuePropertyGet = typeof (OrderWithNewPropertyAccess).GetMethod ("get_DeliveryDate");
      Assert.IsFalse (valuePropertyGet.IsAbstract);
      CheckInterception (true, selector, interceptor, valuePropertyGet);
      CheckInterception (true, outerSelector, interceptorOfOuter, valuePropertyGet);

      MethodInfo valuePropertySet = typeof (OrderWithNewPropertyAccess).GetMethod ("set_DeliveryDate");
      Assert.IsFalse (valuePropertySet.IsAbstract);
      CheckInterception (true, selector, interceptor, valuePropertySet);
      CheckInterception (true, outerSelector, interceptorOfOuter, valuePropertySet);

      MethodInfo relatedObjectGet = typeof (OrderWithNewPropertyAccess).GetMethod ("get_Customer");
      Assert.IsFalse (relatedObjectGet.IsAbstract);
      CheckInterception (true, selector, interceptor, relatedObjectGet);
      CheckInterception (true, outerSelector, interceptorOfOuter, relatedObjectGet);

      MethodInfo relatedObjectSet = typeof (OrderWithNewPropertyAccess).GetMethod ("set_Customer");
      Assert.IsFalse (relatedObjectSet.IsAbstract);
      CheckInterception (true, selector, interceptor, relatedObjectSet);
      CheckInterception (true, outerSelector, interceptorOfOuter, relatedObjectSet);

      MethodInfo relatedObjectsGet = typeof (OrderWithNewPropertyAccess).GetMethod ("get_OrderItems");
      Assert.IsFalse (relatedObjectsGet.IsAbstract);
      CheckInterception (true, selector, interceptor, relatedObjectsGet);
      CheckInterception (true, outerSelector, interceptorOfOuter, relatedObjectsGet);
    }

    private static object InstantiateInternalType (Assembly assembly, string typeName, params object[] ctorArgs)
    {
      ArgumentUtility.CheckNotNull ("typeName", typeName);
      ArgumentUtility.CheckNotNull ("ctorArgs", ctorArgs);
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      Type t = assembly.GetType (typeName);
      if (t == null)
        throw new ArgumentException (string.Format("Type {0} does not exist in assembly {1}.", typeName, assembly.FullName), "typename");
      return Activator.CreateInstance (t, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, ctorArgs, null);
    }

    private static object GetFieldValue (object o, string fieldName)
    {
      ArgumentUtility.CheckNotNull ("o", o);
      ArgumentUtility.CheckNotNull ("fieldName", fieldName);

      FieldInfo field = o.GetType().GetField (fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      if (field == null)
        throw new ArgumentException (string.Format ("Field {0} does not exist in type {1}.", fieldName, o.GetType().FullName), "fieldName");
      return field.GetValue (o);
    }

    [Test]
    [Ignore("Implement inheritence root first")]
    public void PropInterceptorShouldInterceptNonAutoPropertiesInMapping_ForBaseTypeNotInMapping ()
    {
      IInterceptor<DomainObject> interceptor = (IInterceptor<DomainObject>) InstantiateInternalType (typeof (DomainObject).Assembly,
          "Rubicon.Data.DomainObjects.Infrastructure.Interception.DomainObjectPropertyInterceptor");
      IInterceptorSelector<DomainObject> selector = (IInterceptorSelector<DomainObject>) GetFieldValue (interceptor, "Selector");

      IInterceptorSelector<DomainObject> outerSelector = (IInterceptorSelector<DomainObject>) InstantiateInternalType (typeof (DomainObject).Assembly,
          "Rubicon.Data.DomainObjects.Infrastructure.Interception.DomainObjectInterceptorSelector");
      IInterceptor<DomainObject> interceptorOfOuter = (IInterceptor<DomainObject>) GetFieldValue (outerSelector, "_propertyInterceptor");

      MethodInfo basePropertyGet = typeof (OrderWithNewPropertyAccess).GetMethod ("get_BaseProperty");
      Assert.IsFalse (basePropertyGet.IsAbstract);
      CheckInterception (true, selector, interceptor, basePropertyGet);
      CheckInterception (true, outerSelector, interceptorOfOuter, basePropertyGet);

      MethodInfo basePropertySet = typeof (OrderWithNewPropertyAccess).GetMethod ("set_BaseProperty");
      Assert.IsFalse (basePropertySet.IsAbstract);
      CheckInterception (true, selector, interceptor, basePropertySet);
      CheckInterception (true, outerSelector, interceptorOfOuter, basePropertySet);
    }

    [Test]
    public void PropInterceptorShouldInterceptAutoPropertiesInMapping ()
    {
      IInterceptor<DomainObject> interceptor = (IInterceptor<DomainObject>) InstantiateInternalType (typeof (DomainObject).Assembly,
          "Rubicon.Data.DomainObjects.Infrastructure.Interception.DomainObjectPropertyInterceptor");
      IInterceptorSelector<DomainObject> selector = (IInterceptorSelector<DomainObject>) GetFieldValue (interceptor, "Selector");
      
      IInterceptorSelector<DomainObject> outerSelector = (IInterceptorSelector<DomainObject>) InstantiateInternalType (typeof (DomainObject).Assembly,
          "Rubicon.Data.DomainObjects.Infrastructure.Interception.DomainObjectInterceptorSelector");
      IInterceptor<DomainObject> interceptorOfOuter = (IInterceptor<DomainObject>) GetFieldValue (outerSelector, "_propertyInterceptor");

      MethodInfo valuePropertyGet = typeof (Order).GetMethod ("get_DeliveryDate");
      Assert.IsTrue (valuePropertyGet.IsAbstract);
      CheckInterception (true, selector, interceptor, valuePropertyGet);
      CheckInterception (true, outerSelector, interceptorOfOuter, valuePropertyGet);

      MethodInfo valuePropertySet = typeof (Order).GetMethod ("set_DeliveryDate");
      Assert.IsTrue (valuePropertySet.IsAbstract);
      CheckInterception (true, selector, interceptor, valuePropertySet);
      CheckInterception (true, outerSelector, interceptorOfOuter, valuePropertySet);

      MethodInfo relatedObjectGet = typeof (Order).GetMethod ("get_Customer");
      Assert.IsTrue (relatedObjectGet.IsAbstract);
      CheckInterception (true, selector, interceptor, relatedObjectGet);
      CheckInterception (true, outerSelector, interceptorOfOuter, relatedObjectGet);

      MethodInfo relatedObjectSet = typeof (Order).GetMethod ("set_Customer");
      Assert.IsTrue (relatedObjectSet.IsAbstract);
      CheckInterception (true, selector, interceptor, relatedObjectSet);
      CheckInterception (true, outerSelector, interceptorOfOuter, relatedObjectSet);

      MethodInfo relatedObjectsGet = typeof (Order).GetMethod ("get_OrderItems");
      Assert.IsTrue (relatedObjectsGet.IsAbstract);
      CheckInterception (true, selector, interceptor, relatedObjectsGet);
      CheckInterception (true, outerSelector, interceptorOfOuter, relatedObjectsGet);
    }

    [Test]
    public void NoInterceptorShouldInterceptPropertiesNotInMapping ()
    {
      IInterceptor<DomainObject> interceptor = (IInterceptor<DomainObject>) InstantiateInternalType (typeof (DomainObject).Assembly,
          "Rubicon.Data.DomainObjects.Infrastructure.Interception.DomainObjectPropertyInterceptor");
      IInterceptorSelector<DomainObject> selector = (IInterceptorSelector<DomainObject>) GetFieldValue (interceptor, "Selector");
      
      IInterceptorSelector<DomainObject> outerSelector = (IInterceptorSelector<DomainObject>) InstantiateInternalType (typeof (DomainObject).Assembly,
          "Rubicon.Data.DomainObjects.Infrastructure.Interception.DomainObjectInterceptorSelector");

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
      IInterceptor<DomainObject> interceptor = (IInterceptor<DomainObject>) InstantiateInternalType (typeof (DomainObject).Assembly,
          "Rubicon.Data.DomainObjects.Infrastructure.Interception.DomainObjectPropertyInterceptor");
      IInterceptorSelector<DomainObject> selector = (IInterceptorSelector<DomainObject>) GetFieldValue (interceptor, "Selector");

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
      IInterceptor<DomainObject> interceptor = (IInterceptor<DomainObject>) InstantiateInternalType (typeof (DomainObject).Assembly,
          "Rubicon.Data.DomainObjects.Infrastructure.Interception.DomainObjectTypeInterceptor");
      IInterceptorSelector<DomainObject> selector = (IInterceptorSelector<DomainObject>) GetFieldValue (interceptor, "Selector");

      IInterceptorSelector<DomainObject> outerSelector = (IInterceptorSelector<DomainObject>) InstantiateInternalType (typeof (DomainObject).Assembly,
          "Rubicon.Data.DomainObjects.Infrastructure.Interception.DomainObjectInterceptorSelector");
      IInterceptor<DomainObject> interceptorOfOuter = (IInterceptor<DomainObject>) GetFieldValue (outerSelector, "_typeInterceptor");

      MethodInfo getPublicType = typeof (OrderWithNewPropertyAccess).GetMethod ("GetPublicDomainObjectType", 
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      CheckInterception (true, selector, interceptor, getPublicType);
      CheckInterception (true, outerSelector, interceptorOfOuter, getPublicType);
    }

    [Test]
    public void TypeInterceptorShouldNotInterceptAnythingElse()
    {
      IInterceptor<DomainObject> interceptor = (IInterceptor<DomainObject>) InstantiateInternalType (typeof (DomainObject).Assembly,
          "Rubicon.Data.DomainObjects.Infrastructure.Interception.DomainObjectTypeInterceptor");
      IInterceptorSelector<DomainObject> selector = (IInterceptorSelector<DomainObject>) GetFieldValue (interceptor, "Selector");

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
        + "DomainObjects.InterceptedPropertyTest+NonInstantiableAbstractClassWithProps, property Foo is abstract but not "
        + "defined in the mapping (assumed property id: Rubicon.Data.DomainObjects.UnitTests.DomainObjects."
        + "InterceptedPropertyTest+NonInstantiableAbstractClassWithProps.Foo).")]
    public void PropSelectionThrowsOnAbstractUndefined ()
    {
      IInterceptor<DomainObject> interceptor = (IInterceptor<DomainObject>) InstantiateInternalType (typeof (DomainObject).Assembly,
          "Rubicon.Data.DomainObjects.Infrastructure.Interception.DomainObjectPropertyInterceptor");
      IInterceptorSelector<DomainObject> selector = (IInterceptorSelector<DomainObject>) GetFieldValue (interceptor, "Selector");

      selector.ShouldInterceptMethod (typeof (InterceptedPropertyTest.NonInstantiableAbstractClassWithProps),
        typeof (InterceptedPropertyTest.NonInstantiableAbstractClassWithProps).GetMethod ("get_Foo"));
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests."
        + "DomainObjects.InterceptedPropertyTest+NonInstantiableClassWithAutomaticRelatedCollectionSetter, "
        + "automatic properties for related object collections cannot have setters: property 'RelatedObjects', property id 'Rubicon.Data."
        + "DomainObjects.UnitTests.DomainObjects.InterceptedPropertyTest+NonInstantiableClassWithAutomaticRelatedCollectionSetter."
        + "RelatedObjects'.")]
    public void PropSelectionThrowsOnAbstractRelatedCollectionSetter ()
    {
      IInterceptor<DomainObject> interceptor = (IInterceptor<DomainObject>) InstantiateInternalType (typeof (DomainObject).Assembly,
          "Rubicon.Data.DomainObjects.Infrastructure.Interception.DomainObjectPropertyInterceptor");
      IInterceptorSelector<DomainObject> selector = (IInterceptorSelector<DomainObject>) GetFieldValue (interceptor, "Selector");

      selector.ShouldInterceptMethod (typeof (InterceptedPropertyTest.NonInstantiableClassWithAutomaticRelatedCollectionSetter),
        typeof (InterceptedPropertyTest.NonInstantiableClassWithAutomaticRelatedCollectionSetter).GetMethod ("set_RelatedObjects"));
    }

    [Test]
    public void GetIdentifierFromProperty ()
    {
      Type t = typeof (DomainObject).Assembly.GetType ("Rubicon.Data.DomainObjects.Infrastructure.Interception.DomainObjectPropertyInterceptor");

      PropertyInfo property = typeof (OrderWithNewPropertyAccess).GetProperty ("Customer");
      Assert.AreEqual (property.DeclaringType.FullName + "." + property.Name,
          PrivateInvoke.InvokePublicStaticMethod(t, "GetIdentifierFromProperty", property));
    }

    [Test]
    public void GetClassDefinitionIfRelevantProperty ()
    {
      Type t = typeof (DomainObject).Assembly.GetType ("Rubicon.Data.DomainObjects.Infrastructure.Interception.DomainObjectPropertyInterceptorSelector");

      Assert.IsNotNull (PrivateInvoke.InvokePublicStaticMethod(t, "GetClassDefinitionIfRelevantProperty", typeof (OrderWithNewPropertyAccess),
        MakeID ("OrderItems")));
      Assert.IsNotNull (PrivateInvoke.InvokePublicStaticMethod (t, "GetClassDefinitionIfRelevantProperty", typeof (OrderWithNewPropertyAccess),
          MakeID ("OrderNumber")));
      Assert.IsNotNull (PrivateInvoke.InvokePublicStaticMethod (t, "GetClassDefinitionIfRelevantProperty", typeof (OrderWithNewPropertyAccess),
          MakeID ("Customer")));

      Assert.IsNull (PrivateInvoke.InvokePublicStaticMethod (t, "GetClassDefinitionIfRelevantProperty", typeof (OrderWithNewPropertyAccess),
          MakeID ("NotInMapping")));
      Assert.IsNull (PrivateInvoke.InvokePublicStaticMethod (t, "GetClassDefinitionIfRelevantProperty", typeof (OrderWithNewPropertyAccess),
          MakeID ("NotInMappingRelated")));
      Assert.IsNull (PrivateInvoke.InvokePublicStaticMethod (t, "GetClassDefinitionIfRelevantProperty", typeof (OrderWithNewPropertyAccess),
          MakeID ("NotInMappingRelatedObjects")));
      Assert.IsNull (PrivateInvoke.InvokePublicStaticMethod (t, "GetClassDefinitionIfRelevantProperty", typeof (OrderWithNewPropertyAccess),
          MakeID ("Bla")));
    }

    private string MakeID (string propertyName)
    {
      return typeof (OrderWithNewPropertyAccess).FullName + "." + propertyName;
    }


  }
}
