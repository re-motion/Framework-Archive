using System;
using Rubicon.Security;

namespace Rubicon.SecurityManager.UnitTests.TestDomain
{
  [SecurityState]
  public enum OrderState
  {
    Received = 0,
    Delivered
  }

  [SecurityState]
  public enum PaymentState
  {
    None,
    Paid
  }

  [SecurityState]
  public enum Delivery
  {
    Dhl,
    Post
  }
}
