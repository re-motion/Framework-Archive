using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.Infrastructure.Interception;
using Rubicon.Development.UnitTesting;
using System.Threading;

namespace Rubicon.Data.DomainObjects.UnitTests.Interception
{
  [TestFixture]
  public class TypeGeneratorTest : ClientTransactionBaseTest
  {
    private const BindingFlags _declaredPublicInstanceFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;

    [DBTable]
    public class DOWithVirtualProperties : DomainObject
    {
      public virtual int PropertyWithGetterAndSetter
      {
        get { return CurrentProperty.GetValue<int> (); }
        set { CurrentProperty.SetValue (value); }
      }

      public virtual string PropertyWithGetterOnly
      {
        get { return CurrentProperty.GetValue<string> (); }
      }

      public virtual DateTime PropertyWithSetterOnly
      {
        set { CurrentProperty.SetValue (value); }
      }

      public virtual DateTime PropertyThrowing
      {
        get { throw new Exception (); }
        set { throw new Exception (); }
      }

      [StorageClassNone]
      public virtual DateTime PropertyNotInMapping
      {
        get { return CurrentProperty.GetValue<DateTime>(); }
      }

      [StorageClassNone]
      public new PropertyIndexer Properties
      {
        get { return base.Properties; }
      }

      public new string GetAndCheckCurrentPropertyName()
      {
        return base.GetAndCheckCurrentPropertyName();
      }
    }

    [DBTable]
    [Instantiable]
    public abstract class DOWithAbstractProperties : DomainObject
    {
      public abstract int PropertyWithGetterAndSetter { get; set; }
      public abstract string PropertyWithGetterOnly { get; }
      public abstract DateTime PropertyWithSetterOnly { set; }

      [StorageClassNone]
      public new PropertyIndexer Properties
      {
        get { return base.Properties; }
      }

      public new string GetAndCheckCurrentPropertyName ()
      {
        return base.GetAndCheckCurrentPropertyName();
      }
    }

    private ModuleManager _scope;

    public override void SetUp ()
    {
      base.SetUp ();
      _scope = new ModuleManager ();
    }

    public override void TearDown ()
    {
      string[] paths = _scope.SaveAssemblies ();
      foreach (string path in paths)
        PEVerifier.VerifyPEFile (path);
    }

    [Test]
    public void GeneratedTypeHasOtherNameThanBaseType ()
    {
      Type type = _scope.CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      Assert.AreNotEqual (typeof (DOWithVirtualProperties).Name, type.Name);
      Assert.AreNotEqual (typeof (DOWithVirtualProperties).FullName, type.Name);
    }

    [Test]
    public void EachGeneratedTypeHasDifferentName ()
    {
      Type type1 = _scope.CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      Type type2 = new InterceptedDomainObjectFactory().GetConcreteDomainObjectType (typeof (DOWithVirtualProperties));
      Assert.AreNotSame (type1, type2);
      Assert.AreNotEqual (type1.Name, type2.Name);
    }

    [Test]
    public void OverridesGetPublicDomainObjectType ()
    {
      Type type = _scope.CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      Assert.IsNotNull (type.GetMethod ("GetPublicDomainObjectType", _declaredPublicInstanceFlags));
    }

    [Test]
    public void OverridesGetPublicDomainObjectTypeToReturnBaseType ()
    {
      Type type = _scope.CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      DOWithVirtualProperties instance = (DOWithVirtualProperties) Activator.CreateInstance (type);
      Assert.AreEqual (typeof (DOWithVirtualProperties), instance.GetPublicDomainObjectType ());
      Assert.IsNotNull (type.GetMethod ("GetPublicDomainObjectType", _declaredPublicInstanceFlags));
    }

    [Test]
    public void OverridesVirtualProperties ()
    {
      Type type = _scope.CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      Assert.IsNotNull (type.GetProperty ("PropertyWithGetterOnly", _declaredPublicInstanceFlags));
      Assert.IsNotNull (type.GetProperty ("PropertyWithSetterOnly", _declaredPublicInstanceFlags));
      Assert.IsNotNull (type.GetProperty ("PropertyWithGetterAndSetter", _declaredPublicInstanceFlags));
    }

    [Test]
    public void OverridesVirtualPropertiesSoThatCurrentPropertyWorks ()
    {
      Type type = _scope.CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      DOWithVirtualProperties instance = (DOWithVirtualProperties) Activator.CreateInstance (type);

      Assert.AreEqual (0, instance.PropertyWithGetterAndSetter);
      instance.PropertyWithGetterAndSetter = 17;
      Assert.AreEqual (17, instance.PropertyWithGetterAndSetter);

      Assert.IsNull (instance.PropertyWithGetterOnly);
      instance.Properties[typeof (DOWithVirtualProperties), "PropertyWithGetterOnly"].SetValue ("hear, hear");
      Assert.AreEqual ("hear, hear", instance.PropertyWithGetterOnly);

      Assert.AreEqual (new DateTime(), instance.Properties[typeof (DOWithVirtualProperties), "PropertyWithSetterOnly"].GetValue<DateTime>());
      instance.PropertyWithSetterOnly = new DateTime (2260, 1, 2);
      Assert.AreEqual (new DateTime (2260, 1, 2), instance.Properties[typeof (DOWithVirtualProperties), "PropertyWithSetterOnly"].GetValue<DateTime> ());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property", MatchType = MessageMatch.Contains)]
    public void OverriddenPropertiesCleanUpCurrentPropertyName ()
    {
      Type type = _scope.CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      DOWithVirtualProperties instance = (DOWithVirtualProperties) Activator.CreateInstance (type);

      Assert.AreEqual (0, instance.PropertyWithGetterAndSetter);
      instance.GetAndCheckCurrentPropertyName();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property", MatchType = MessageMatch.Contains)]
    public void OverriddenPropertiesCleanUpCurrentPropertyNameEvenOnExceptionInGetter ()
    {
      Type type = _scope.CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      DOWithVirtualProperties instance = (DOWithVirtualProperties) Activator.CreateInstance (type);

      try
      {
        Dev.Null = instance.PropertyThrowing;
      }
      catch
      {
      }
      instance.GetAndCheckCurrentPropertyName ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property", MatchType = MessageMatch.Contains)]
    public void OverriddenPropertiesCleanUpCurrentPropertyNameEvenOnExceptionInSetter ()
    {
      Type type = _scope.CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      DOWithVirtualProperties instance = (DOWithVirtualProperties) Activator.CreateInstance (type);

      try
      {
        instance.PropertyThrowing = DateTime.Now;
      }
      catch
      {
      }
      instance.GetAndCheckCurrentPropertyName ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property", MatchType = MessageMatch.Contains)]
    public void DoesNotOverridePropertiesNotInMapping ()
    {
      Type type = _scope.CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      DOWithVirtualProperties instance = (DOWithVirtualProperties) Activator.CreateInstance (type);
      Dev.Null = instance.PropertyNotInMapping;
    }

    [Test]
    public void ImplementsAbstractProperties ()
    {
      Type type = _scope.CreateTypeGenerator (typeof (DOWithAbstractProperties)).BuildType();
      Assert.IsNotNull (type.GetProperty ("PropertyWithGetterOnly", _declaredPublicInstanceFlags));
      Assert.IsNotNull (type.GetProperty ("PropertyWithSetterOnly", _declaredPublicInstanceFlags));
      Assert.IsNotNull (type.GetProperty ("PropertyWithGetterAndSetter", _declaredPublicInstanceFlags));
      Assert.IsFalse (type.IsAbstract);
    }

    [Test]
    public void ImplementsAbstractPropertiesSoThatCurrentPropertyWorks ()
    {
      Type type = _scope.CreateTypeGenerator (typeof (DOWithAbstractProperties)).BuildType();
      DOWithAbstractProperties instance = (DOWithAbstractProperties) Activator.CreateInstance (type);

      Assert.AreEqual (0, instance.PropertyWithGetterAndSetter);
      instance.PropertyWithGetterAndSetter = 17;
      Assert.AreEqual (17, instance.PropertyWithGetterAndSetter);

      Assert.IsNull (instance.PropertyWithGetterOnly);
      instance.Properties[typeof (DOWithAbstractProperties), "PropertyWithGetterOnly"].SetValue ("hear, hear");
      Assert.AreEqual ("hear, hear", instance.PropertyWithGetterOnly);

      Assert.AreEqual (new DateTime (), instance.Properties[typeof (DOWithAbstractProperties), "PropertyWithSetterOnly"].GetValue<DateTime> ());
      instance.PropertyWithSetterOnly = new DateTime (2260, 1, 2);
      Assert.AreEqual (new DateTime (2260, 1, 2), instance.Properties[typeof (DOWithAbstractProperties), "PropertyWithSetterOnly"].GetValue<DateTime> ());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property", MatchType = MessageMatch.Contains)]
    public void ImplementedAbstractPropertyGettersCleanUpCurrentPropertyName ()
    {
      Type type = _scope.CreateTypeGenerator (typeof (DOWithAbstractProperties)).BuildType();
      DOWithAbstractProperties instance = (DOWithAbstractProperties) Activator.CreateInstance (type);

      Assert.AreEqual (0, instance.PropertyWithGetterAndSetter);
      instance.GetAndCheckCurrentPropertyName ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property", MatchType = MessageMatch.Contains)]
    public void ImplementedAbstractPropertySettersCleanUpCurrentPropertyName ()
    {
      Type type = _scope.CreateTypeGenerator (typeof (DOWithAbstractProperties)).BuildType();
      DOWithAbstractProperties instance = (DOWithAbstractProperties) Activator.CreateInstance (type);

      instance.PropertyWithGetterAndSetter = 17;
      instance.GetAndCheckCurrentPropertyName ();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests."
        + "Interception.InterceptedPropertyTest+NonInstantiableAbstractClassWithProps, property Foo is abstract but not "
        + "defined in the mapping (assumed property id: Rubicon.Data.DomainObjects.UnitTests.Interception."
        + "InterceptedPropertyTest+NonInstantiableAbstractClassWithProps.Foo).")]
    public void ThrowsOnAbstractPropertyNotInMapping ()
    {
      _scope.CreateTypeGenerator (typeof (InterceptedPropertyTest.NonInstantiableAbstractClassWithProps)).BuildType();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests."
        + "Interception.InterceptedPropertyTest+NonInstantiableClassWithAutomaticRelatedCollectionSetter, "
        + "automatic properties for related object collections cannot have setters: property 'RelatedObjects', property id 'Rubicon.Data."
        + "DomainObjects.UnitTests.Interception.InterceptedPropertyTest+NonInstantiableClassWithAutomaticRelatedCollectionSetter."
        + "RelatedObjects'.")]
    public void ThrowsOnAbstractRelatedObjectCollectionSetter ()
    {
      _scope.CreateTypeGenerator (typeof (InterceptedPropertyTest.NonInstantiableClassWithAutomaticRelatedCollectionSetter)).BuildType();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests."
        + "Interception.InterceptedPropertyTest+NonInstantiableAbstractClass as its member Foo is abstract (and not an "
        + "automatic property).")]
    public void ThrowsOnAbstractMethod ()
    {
      _scope.CreateTypeGenerator (typeof (InterceptedPropertyTest.NonInstantiableAbstractClass)).BuildType();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException),
        ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests.Interception."
        + "InterceptedPropertyTest+NonInstantiableSealedClass as it is sealed.")]
    public void ThrowsOnSealedBaseType ()
    {
      _scope.CreateTypeGenerator (typeof (InterceptedPropertyTest.NonInstantiableSealedClass)).BuildType();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests."
        + "Interception.InterceptedPropertyTest+NonInstantiableNonDomainClass as it is not part of the mapping.")]
    public void ThrowsOnClassWithoutClassDefinition ()
    {
      _scope.CreateTypeGenerator (typeof (InterceptedPropertyTest.NonInstantiableNonDomainClass)).BuildType();
    }
  }
}