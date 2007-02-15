using System;
using Rubicon.Security;

namespace Rubicon.SecurityManager.UnitTests.TestDomain
{
  public class Order : ISecurableObject
  {
    public OrderState State
    {
      get { return OrderState.Delivered; }
    }

    public PaymentState Payment
    {
      get { return PaymentState.Paid; }
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}
