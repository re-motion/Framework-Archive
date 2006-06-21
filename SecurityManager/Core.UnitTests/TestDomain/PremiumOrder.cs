using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.SecurityManager.UnitTests.TestDomain
{
  public class PremiumOrder : Order
  {
    public Delivery Delivery
    {
      get { return Delivery.Dhl; }
    }
  }
}
