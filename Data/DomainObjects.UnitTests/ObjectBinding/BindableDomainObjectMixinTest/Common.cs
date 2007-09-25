using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Collections;
using Rubicon.Development.UnitTesting;
using Rubicon.ObjectBinding;
using Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.BindableDomainObjectMixinTest
{
  [TestFixture]
  public class Common : ObjectBindingBaseTest
  {
    [Test]
    public void InstantiateMixedType ()
    {
      Assert.That (BindableSampleDomainObject.NewObject(), Is.InstanceOfType (typeof (IBusinessObject)));
    }

    [Test]
    public void SerializeAndDeserialize ()
    {
      BindableSampleDomainObject value = BindableSampleDomainObject.NewObject ();
      Assert.AreNotEqual ("Earl", value.Name);
      value.Name = "Earl";
      Tuple<ClientTransactionMock, BindableSampleDomainObject> deserialized = Serializer.SerializeAndDeserialize (Tuple.NewTuple (ClientTransactionMock, value));

      using (deserialized.A.EnterScope ())
      {
        Assert.That (deserialized.B.Name, Is.EqualTo ("Earl"));
        Assert.That (((IBusinessObject) deserialized.B).BusinessObjectClass, Is.SameAs (((IBusinessObject) value).BusinessObjectClass));
      }
    }

    [Test]
    public void SerializeAndDeserialize_WithNewBindableObjectProvider ()
    {
      BindableSampleDomainObject value = BindableSampleDomainObject.NewObject ();
      byte[] serialized = Serializer.Serialize (Tuple.NewTuple (ClientTransactionMock, value));
      BindableObjectProvider.SetCurrent (null);
      Tuple<ClientTransactionMock, BindableSampleDomainObject> deserialized = (Tuple<ClientTransactionMock, BindableSampleDomainObject>) Serializer.Deserialize (serialized);

      using (deserialized.A.EnterScope ())
      {
        Assert.That (((IBusinessObject) deserialized.B).BusinessObjectClass, Is.Not.SameAs (((IBusinessObject) value).BusinessObjectClass));
      }
    }
  }
}