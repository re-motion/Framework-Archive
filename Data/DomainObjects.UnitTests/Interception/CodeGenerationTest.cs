using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;
using File=System.IO.File;

namespace Rubicon.Data.DomainObjects.UnitTests.Interception
{
  [TestFixture]
  public class CodeGenerationTest : ClientTransactionBaseTest
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

    private InterceptedDomainObjectFactory _factory;

    public override void SetUp ()
    {
      base.SetUp ();
      _factory = new InterceptedDomainObjectFactory ();
    }

    public override void TearDown ()
    {
      base.TearDown ();
      string[] paths = _factory.SaveGeneratedAssemblies ();
      foreach (string path in paths)
        PEVerifier.VerifyPEFile (path);
    }

    [Test]
    public void GetConcreteDomainObjectTypeReturnsAssignableType ()
    {
      Type concreteType = _factory.GetConcreteDomainObjectType (typeof (Order));
      Assert.IsTrue (typeof (Order).IsAssignableFrom (concreteType));
    }

    [Test]
    public void GetConcreteDomainObjectTypeReturnsDifferentType ()
    {
      Type concreteType = _factory.GetConcreteDomainObjectType (typeof (Order));
      Assert.AreNotEqual (typeof (Order), concreteType);
    }

    [Test]
    public void SaveReturnsPathOfGeneratedAssemblySigned ()
    {
      _factory.GetConcreteDomainObjectType (typeof (Order));
      string[] paths = _factory.SaveGeneratedAssemblies ();
      Assert.AreEqual (1, paths.Length);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, "Rubicon.Data.DomainObjects.Generated.Signed.dll"), paths[0]);
      Assert.IsTrue (File.Exists (paths[0]));
    }

    [Test]
    public void CanContinueToGenerateTypesAfterSave ()
    {
      _factory.GetConcreteDomainObjectType (typeof (Order));
      _factory.SaveGeneratedAssemblies ();
      _factory.GetConcreteDomainObjectType (typeof (OrderItem));
      _factory.SaveGeneratedAssemblies ();
      _factory.GetConcreteDomainObjectType (typeof (ClassWithAllDataTypes));
    }

    [Test]
    public void GeneratedTypeHasOtherNameThanBaseType ()
    {
      Type type = _factory.GetConcreteDomainObjectType (typeof (DOWithVirtualProperties));
      Assert.AreNotEqual (typeof (DOWithVirtualProperties).Name, type.Name);
      Assert.AreNotEqual (typeof (DOWithVirtualProperties).FullName, type.Name);
    }

    [Test]
    public void EachGeneratedTypeHasDifferentName ()
    {
      Type type1 = _factory.GetConcreteDomainObjectType (typeof (DOWithVirtualProperties));
      Type type2 = new InterceptedDomainObjectFactory().GetConcreteDomainObjectType (typeof (DOWithVirtualProperties));
      Assert.AreNotSame (type1, type2);
      Assert.AreNotEqual (type1.Name, type2.Name);
    }

    [Test]
    public void OverridesGetPublicDomainObjectType ()
    {
      Type type = _factory.GetConcreteDomainObjectType (typeof (DOWithVirtualProperties));
      Assert.IsNotNull (type.GetMethod ("GetPublicDomainObjectType", _declaredPublicInstanceFlags));
    }

    [Test]
    public void OverridesGetPublicDomainObjectTypeToReturnBaseType ()
    {
      Type type = _factory.GetConcreteDomainObjectType (typeof (DOWithVirtualProperties));
      DOWithVirtualProperties instance = (DOWithVirtualProperties) Activator.CreateInstance (type);
      Assert.AreEqual (typeof (DOWithVirtualProperties), instance.GetPublicDomainObjectType ());
      Assert.IsNotNull (type.GetMethod ("GetPublicDomainObjectType", _declaredPublicInstanceFlags));
    }

    [Test]
    public void OverridesVirtualProperties ()
    {
      Type type = _factory.GetConcreteDomainObjectType (typeof (DOWithVirtualProperties));
      Assert.IsNotNull (type.GetProperty ("PropertyWithGetterOnly", _declaredPublicInstanceFlags));
      Assert.IsNotNull (type.GetProperty ("PropertyWithSetterOnly", _declaredPublicInstanceFlags));
      Assert.IsNotNull (type.GetProperty ("PropertyWithGetterAndSetter", _declaredPublicInstanceFlags));
    }

    [Test]
    public void OverridesVirtualPropertiesSoThatCurrentPropertyWorks ()
    {
      Type type = _factory.GetConcreteDomainObjectType (typeof (DOWithVirtualProperties));
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
      Type type = _factory.GetConcreteDomainObjectType (typeof (DOWithVirtualProperties));
      DOWithVirtualProperties instance = (DOWithVirtualProperties) Activator.CreateInstance (type);

      Assert.AreEqual (0, instance.PropertyWithGetterAndSetter);
      instance.GetAndCheckCurrentPropertyName();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property", MatchType = MessageMatch.Contains)]
    public void OverriddenPropertiesCleanUpCurrentPropertyNameEvenOnExceptionInGetter ()
    {
      Type type = _factory.GetConcreteDomainObjectType (typeof (DOWithVirtualProperties));
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
      Type type = _factory.GetConcreteDomainObjectType (typeof (DOWithVirtualProperties));
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
    public void ImplementsAbstractProperties ()
    {
      Type type = _factory.GetConcreteDomainObjectType (typeof (DOWithAbstractProperties));
      Assert.IsNotNull (type.GetProperty ("PropertyWithGetterOnly", _declaredPublicInstanceFlags));
      Assert.IsNotNull (type.GetProperty ("PropertyWithSetterOnly", _declaredPublicInstanceFlags));
      Assert.IsNotNull (type.GetProperty ("PropertyWithGetterAndSetter", _declaredPublicInstanceFlags));
      Assert.IsFalse (type.IsAbstract);
    }

    [Test]
    public void ImplementsAbstractPropertiesSoThatCurrentPropertyWorks ()
    {
      Type type = _factory.GetConcreteDomainObjectType (typeof (DOWithAbstractProperties));
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
      Type type = _factory.GetConcreteDomainObjectType (typeof (DOWithAbstractProperties));
      DOWithAbstractProperties instance = (DOWithAbstractProperties) Activator.CreateInstance (type);

      Assert.AreEqual (0, instance.PropertyWithGetterAndSetter);
      instance.GetAndCheckCurrentPropertyName ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property", MatchType = MessageMatch.Contains)]
    public void ImplementedAbstractPropertySettersCleanUpCurrentPropertyName ()
    {
      Type type = _factory.GetConcreteDomainObjectType (typeof (DOWithAbstractProperties));
      DOWithAbstractProperties instance = (DOWithAbstractProperties) Activator.CreateInstance (type);

      instance.PropertyWithGetterAndSetter = 17;
      instance.GetAndCheckCurrentPropertyName ();
    }
  }
}