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
      Assert.That (BindableDomainObject.NewObject(), Is.InstanceOfType (typeof (IBusinessObject)));
    }

    [Test]
    public void SerializeAndDeserialize ()
    {
      BindableDomainObject value = BindableDomainObject.NewObject ();
      Assert.AreNotEqual ("Earl", value.Name);
      value.Name = "Earl";
      Tuple<ClientTransactionMock, BindableDomainObject> deserialized = Serializer.SerializeAndDeserialize (Tuple.NewTuple (ClientTransactionMock, value));

      using (deserialized.A.EnterScope ())
      {
        Assert.That (deserialized.B.Name, Is.EqualTo ("Earl"));
        Assert.That (((IBusinessObject) deserialized.B).BusinessObjectClass, Is.SameAs (((IBusinessObject) value).BusinessObjectClass));
      }
    }

    [Test]
    public void SerializeAndDeserialize_WithNewBindableObjectProvider ()
    {
      BindableDomainObject value = BindableDomainObject.NewObject ();
      byte[] serialized = Serializer.Serialize (Tuple.NewTuple (ClientTransactionMock, value));
      BindableObjectProvider.SetCurrent (null);
      Tuple<ClientTransactionMock, BindableDomainObject> deserialized = (Tuple<ClientTransactionMock, BindableDomainObject>) Serializer.Deserialize (serialized);

      using (deserialized.A.EnterScope ())
      {
        Assert.That (((IBusinessObject) deserialized.B).BusinessObjectClass, Is.Not.SameAs (((IBusinessObject) value).BusinessObjectClass));
      }
    }
  }
}