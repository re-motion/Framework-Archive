using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.EventReceiver
{
  [Serializable]
  public class EventReceiverBase
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    protected EventReceiverBase ()
    {
    }

    // methods and properties

    protected void CancelOperation ()
    {
      throw new EventReceiverCancelException ();
    }
  }
}
