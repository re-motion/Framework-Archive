using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using Rubicon.Collections;
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
    public class DOWithConstructors : DomainObject
    {
      public readonly string FirstArg;
      public readonly string SecondArg;

      public DOWithConstructors (string firstArg, string secondArg)
      {
        FirstArg = firstArg;
        SecondArg = secondArg;
      }

      public DOWithConstructors (int arg)
        : this (arg.ToString(), null)
      {
      }
    }

    [DBTable]
    [Serializable]
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

    [DBTable]
    [Instantiable]
    [Serializable]
    public abstract class DOImplementingISerializable : DomainObject, ISerializable
    {
      private string _memberHeldAsField;

      public DOImplementingISerializable (string memberHeldAsField)
      {
        _memberHeldAsField = memberHeldAsField;
      }

      protected DOImplementingISerializable (SerializationInfo info, StreamingContext context)
          : base (info, context)
      {
        _memberHeldAsField = info.GetString ("_memberHeldAsField") + "-Ctor";
      }

      public abstract int PropertyWithGetterAndSetter { get; set; }

      public string MemberHeldAsField
      {
        get { return _memberHeldAsField; }
        set { _memberHeldAsField = value; }
      }

      public void GetObjectData (SerializationInfo info, StreamingContext context)
      {
        info.AddValue ("_memberHeldAsField", _memberHeldAsField + "-GetObjectData");
      }
    }

    private ModuleManager _scope;

    public override void SetUp ()
    {
      base.SetUp ();
      string directory = Path.Combine (Environment.CurrentDirectory, "Interception.TypeGeneratorTest.Dlls");
      SetupAssemblyDirectory(directory);

      _scope = new ModuleManager (directory);
    }

    private void SetupAssemblyDirectory (string directory)
    {
      if (Directory.Exists (directory))
        Directory.Delete (directory, true);

      Directory.CreateDirectory (directory);

      Module unitTestAssemblyModule = Assembly.GetExecutingAssembly ().ManifestModule;
      File.Copy (unitTestAssemblyModule.FullyQualifiedName, Path.Combine (directory, unitTestAssemblyModule.Name));

      Module domainObjectAssemblyModule = typeof (DomainObject).Assembly.ManifestModule;
      File.Copy (domainObjectAssemblyModule.FullyQualifiedName, Path.Combine (directory, domainObjectAssemblyModule.Name));
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
      Type type2 = new InterceptedDomainObjectFactory(Environment.CurrentDirectory).GetConcreteDomainObjectType (typeof (DOWithVirtualProperties));
      Assert.AreNotSame (type1, type2);
      Assert.AreNotEqual (type1.Name, type2.Name);
    }

    [Test]
    public void ReplicatesConstructors ()
    {
      Type type = _scope.CreateTypeGenerator (typeof (DOWithConstructors)).BuildType ();
      Assert.IsNotNull (type.GetConstructor (new Type[] { typeof (string), typeof (string) }));
      Assert.IsNotNull (type.GetConstructor (new Type[] { typeof (int) }));
    }

    [Test]
    public void ReplicatedConstructorsDelegateToBase ()
    {
      Type type = _scope.CreateTypeGenerator (typeof (DOWithConstructors)).BuildType ();
      DOWithConstructors instance1 = (DOWithConstructors) Activator.CreateInstance (type, "Foo", "Bar");
      Assert.AreEqual ("Foo", instance1.FirstArg);
      Assert.AreEqual ("Bar", instance1.SecondArg);

      DOWithConstructors instance2 = (DOWithConstructors) Activator.CreateInstance (type, 7);
      Assert.AreEqual ("7", instance2.FirstArg);
      Assert.IsNull (instance2.SecondArg);
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
        + "Interception.InterceptedPropertyIntegrationTest+NonInstantiableAbstractClassWithProps, property Foo is abstract but not "
        + "defined in the mapping (assumed property id: Rubicon.Data.DomainObjects.UnitTests.Interception."
        + "InterceptedPropertyIntegrationTest+NonInstantiableAbstractClassWithProps.Foo).")]
    public void ThrowsOnAbstractPropertyNotInMapping ()
    {
      _scope.CreateTypeGenerator (typeof (InterceptedPropertyIntegrationTest.NonInstantiableAbstractClassWithProps)).BuildType();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests."
        + "Interception.InterceptedPropertyIntegrationTest+NonInstantiableClassWithAutomaticRelatedCollectionSetter, "
        + "automatic properties for related object collections cannot have setters: property 'RelatedObjects', property id 'Rubicon.Data."
        + "DomainObjects.UnitTests.Interception.InterceptedPropertyIntegrationTest+NonInstantiableClassWithAutomaticRelatedCollectionSetter."
        + "RelatedObjects'.")]
    public void ThrowsOnAbstractRelatedObjectCollectionSetter ()
    {
      _scope.CreateTypeGenerator (typeof (InterceptedPropertyIntegrationTest.NonInstantiableClassWithAutomaticRelatedCollectionSetter)).BuildType();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests."
        + "Interception.InterceptedPropertyIntegrationTest+NonInstantiableAbstractClass as its member Foo is abstract (and not an "
        + "automatic property).")]
    public void ThrowsOnAbstractMethod ()
    {
      _scope.CreateTypeGenerator (typeof (InterceptedPropertyIntegrationTest.NonInstantiableAbstractClass)).BuildType();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException),
        ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests.Interception."
        + "InterceptedPropertyIntegrationTest+NonInstantiableSealedClass as it is sealed.")]
    public void ThrowsOnSealedBaseType ()
    {
      _scope.CreateTypeGenerator (typeof (InterceptedPropertyIntegrationTest.NonInstantiableSealedClass)).BuildType();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests."
        + "Interception.InterceptedPropertyIntegrationTest+NonInstantiableNonDomainClass as it is not part of the mapping.")]
    public void ThrowsOnClassWithoutClassDefinition ()
    {
      _scope.CreateTypeGenerator (typeof (InterceptedPropertyIntegrationTest.NonInstantiableNonDomainClass)).BuildType();
    }

    [Test]
    public void GeneratedTypeImplementsISerializable ()
    {
      Type type = _scope.CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType ();
      Assert.IsTrue (typeof (ISerializable).IsAssignableFrom (type));
    }

    [Test]
    public void GeneratedTypeCanBeSerialized ()
    {
      Type type = _scope.CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType ();
      DOWithVirtualProperties instance = (DOWithVirtualProperties) Activator.CreateInstance (type);
      instance.PropertyWithGetterAndSetter = 17;

      Tuple<ClientTransaction, DOWithVirtualProperties> data =
          Serializer.SerializeAndDeserialize (
              new Tuple<ClientTransaction, DOWithVirtualProperties> (ClientTransactionScope.CurrentTransaction, instance));

      using (data.A.EnterScope ())
      {
        Assert.AreEqual (17, data.B.PropertyWithGetterAndSetter);
      }
    }

    [Test]
    public void GeneratedTypeCanBeSerializedWhenItImplementsISerializable ()
    {
      Type type = _scope.CreateTypeGenerator (typeof (DOImplementingISerializable)).BuildType ();
      DOImplementingISerializable instance = (DOImplementingISerializable) Activator.CreateInstance (type, "Start");
      instance.PropertyWithGetterAndSetter = 23;
      Assert.AreEqual ("Start", instance.MemberHeldAsField);

      Tuple<ClientTransaction, DOImplementingISerializable> data =
          Serializer.SerializeAndDeserialize (
              new Tuple<ClientTransaction, DOImplementingISerializable> (ClientTransactionScope.CurrentTransaction, instance));

      using (data.A.EnterScope ())
      {
        Assert.AreEqual (23, data.B.PropertyWithGetterAndSetter);
        Assert.AreEqual ("Start-GetObjectData-Ctor", data.B.MemberHeldAsField);
      }
    }
  }
}