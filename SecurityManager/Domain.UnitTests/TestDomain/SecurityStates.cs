using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;

namespace Rubicon.SecurityManager.Domain.UnitTests.TestDomain
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
}
