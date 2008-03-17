using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Transport;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Transport
{
  [TestFixture]
  public class TransporterListenerTest : ClientTransactionBaseTest
  {
    private DomainObjectTransporter _transporter;
    private TransporterListener _listener;

    public override void SetUp ()
    {
      base.SetUp ();
      _transporter = new DomainObjectTransporter();
      _listener = new TransporterListener (_transporter);
    }

    [Test]
    public void Serializable ()
    {
      Serializer.SerializeAndDeserialize (_listener);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot use the transported transaction for changing properties after " 
        + "it has been deserialized.")]
    public void Serialization_AndMethodCalled ()
    {
      TransporterListener listener = Serializer.SerializeAndDeserialize (_listener);
      listener.PropertyValueChanging (null, null, null, null);
    }

    [Test]
    public void ModifyingProperty_Loaded ()
    {
      _transporter.Load (DomainObjectIDs.Computer1);

      Computer source = (Computer) _transporter.GetTransportedObject(DomainObjectIDs.Computer1);
      _listener.PropertyValueChanging (source.InternalDataContainer,
          source.InternalDataContainer.PropertyValues[ReflectionUtility.GetPropertyName (typeof (Computer), "SerialNumber")], null, null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Object 'Computer|c7c26bf5-871d-48c7-822a-e9b05aac4e5a|System.Guid' " 
        + "cannot be modified for transportation because it hasn't been loaded yet. Load it before manipulating it.")]
    public void ModifyingProperty_NotLoaded ()
    {
      _transporter.Load (DomainObjectIDs.Computer2);

      Computer source = _transporter.GetTransportedObject (DomainObjectIDs.Computer2)
          .ClientTransaction.GetObjects<Computer> (DomainObjectIDs.Computer1)[0];
      _listener.PropertyValueChanging (source.InternalDataContainer,
          source.InternalDataContainer.PropertyValues[ReflectionUtility.GetPropertyName (typeof (Computer), "SerialNumber")], null, null);
    }
  }
}