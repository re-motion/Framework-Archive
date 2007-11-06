using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class SerializationCallbackTest : ClientTransactionBaseTest
  {
    [Test]
    public void SerializationEvents ()
    {
      ClassWithSerializationCallbacks instance =
          (ClassWithSerializationCallbacks) DomainObject.NewObject (typeof (ClassWithSerializationCallbacks));

      Assert.AreNotSame (typeof (ClassWithSerializationCallbacks), ((object)instance).GetType ());

      new SerializationCallbackTester<ClassWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter (), instance, ClassWithSerializationCallbacks.SetReceiver)
          .Test_SerializationCallbacks ();
    }

    [Test]
    public void DeserializationEvents ()
    {
      ClassWithSerializationCallbacks instance =
          (ClassWithSerializationCallbacks) DomainObject.NewObject (typeof (ClassWithSerializationCallbacks));

      Assert.AreNotSame (typeof (ClassWithSerializationCallbacks), ((object) instance).GetType ());

      new SerializationCallbackTester<ClassWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter (), instance, ClassWithSerializationCallbacks.SetReceiver)
          .Test_DeserializationCallbacks ();
    }
  }
}