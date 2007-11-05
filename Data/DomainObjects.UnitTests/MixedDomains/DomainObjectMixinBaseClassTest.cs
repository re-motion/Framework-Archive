using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Mixins;
using Rubicon.Mixins.Validation;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains
{
  [TestFixture]
  public class DomainObjectMixinBaseClassTest : ClientTransactionBaseTest
  {
    private ClassWithAllDataTypes _loadedClassWithAllDataTypes;
    private ClassWithAllDataTypes _newClassWithAllDataTypes;
    private MixinWithAccessToDomainObjectProperties<ClassWithAllDataTypes> _loadedClassWithAllDataTypesMixin;
    private MixinWithAccessToDomainObjectProperties<ClassWithAllDataTypes> _newClassWithAllDataTypesMixin;

    public override void SetUp ()
    {
      base.SetUp ();
      _loadedClassWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      _newClassWithAllDataTypes = ClassWithAllDataTypes.NewObject ();
      _loadedClassWithAllDataTypesMixin = Mixin.Get<MixinWithAccessToDomainObjectProperties<ClassWithAllDataTypes>> (_loadedClassWithAllDataTypes);
      _newClassWithAllDataTypesMixin = Mixin.Get<MixinWithAccessToDomainObjectProperties<ClassWithAllDataTypes>> (_newClassWithAllDataTypes);
    }

    [Test]
    public void MixinIsApplied ()
    {
      Assert.IsNotNull (_loadedClassWithAllDataTypesMixin);
      Assert.IsNotNull (_newClassWithAllDataTypesMixin);
    }

    [Test]
    [ExpectedException (typeof (ValidationException))]
    public void InvalidMixinConfiguration ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ClassWithAllDataTypes), typeof (MixinWithAccessToDomainObjectProperties<Official>)))
      {
        TypeFactory.GetActiveConfiguration (typeof (ClassWithAllDataTypes));
      }
    }

    [Test]
    public void This ()
    {
      Assert.AreSame (_loadedClassWithAllDataTypes, _loadedClassWithAllDataTypesMixin.This);
      Assert.AreSame (_newClassWithAllDataTypes, _newClassWithAllDataTypesMixin.This);
    }

    [Test]
    public void ID ()
    {
      Assert.AreSame (_loadedClassWithAllDataTypes.ID, _loadedClassWithAllDataTypesMixin.ID);
      Assert.AreSame (_newClassWithAllDataTypes.ID, _newClassWithAllDataTypesMixin.ID);
    }

    [Test]
    public void GetPublicDomainObjectType ()
    {
      Assert.AreSame (_loadedClassWithAllDataTypes.GetPublicDomainObjectType (), _loadedClassWithAllDataTypesMixin.GetPublicDomainObjectType ());
      Assert.AreSame (_newClassWithAllDataTypes.GetPublicDomainObjectType (), _newClassWithAllDataTypesMixin.GetPublicDomainObjectType ());
    }

    [Test]
    public void State ()
    {
      Assert.AreEqual (_loadedClassWithAllDataTypes.State, _loadedClassWithAllDataTypesMixin.State);
      Assert.AreEqual (_newClassWithAllDataTypes.State, _newClassWithAllDataTypesMixin.State);

      ++_loadedClassWithAllDataTypes.Int32Property;
      Assert.AreEqual (_loadedClassWithAllDataTypes.State, _loadedClassWithAllDataTypesMixin.State);

      _loadedClassWithAllDataTypes.Delete ();
      Assert.AreEqual (_loadedClassWithAllDataTypes.State, _loadedClassWithAllDataTypesMixin.State);
    }

    [Test]
    public void IsDiscarded()
    {
      Assert.AreEqual (_loadedClassWithAllDataTypes.IsDiscarded, _loadedClassWithAllDataTypesMixin.IsDiscarded);
      Assert.AreEqual (_newClassWithAllDataTypes.IsDiscarded, _newClassWithAllDataTypesMixin.IsDiscarded);

      _newClassWithAllDataTypes.Delete ();

      Assert.AreEqual (_newClassWithAllDataTypes.IsDiscarded, _newClassWithAllDataTypesMixin.IsDiscarded);
    }

    [Test]
    public void Properties ()
    {
      Assert.AreEqual (_loadedClassWithAllDataTypes.Properties, _loadedClassWithAllDataTypesMixin.Properties);
      Assert.AreEqual (_newClassWithAllDataTypes.Properties, _newClassWithAllDataTypesMixin.Properties);
    }

    [Test]
    public void OnDomainObjectCreated ()
    {
      Assert.IsFalse (_loadedClassWithAllDataTypesMixin.OnDomainObjectCreatedCalled);
      Assert.IsTrue (_newClassWithAllDataTypesMixin.OnDomainObjectCreatedCalled);
    }

    [Test]
    public void OnDomainObjectLoaded ()
    {
      Assert.IsTrue (_loadedClassWithAllDataTypesMixin.OnDomainObjectLoadedCalled);
      Assert.AreEqual (LoadMode.WholeDomainObjectInitialized, _loadedClassWithAllDataTypesMixin.OnDomainObjectLoadedLoadMode);

      _loadedClassWithAllDataTypesMixin.OnDomainObjectLoadedCalled = false;
      using (ClientTransaction.NewTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.EnlistDomainObject (_loadedClassWithAllDataTypes);
        ++_loadedClassWithAllDataTypes.Int32Property;
      }

      Assert.IsTrue (_loadedClassWithAllDataTypesMixin.OnDomainObjectLoadedCalled);
      Assert.AreEqual (LoadMode.DataContainerLoadedOnly, _loadedClassWithAllDataTypesMixin.OnDomainObjectLoadedLoadMode);

      Assert.IsFalse (_newClassWithAllDataTypesMixin.OnDomainObjectLoadedCalled);
    }

    [Test]
    public void Serializable ()
    {
      Serializer.SerializeAndDeserialize (_loadedClassWithAllDataTypesMixin);
      // no exception
    }
  }
}