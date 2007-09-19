using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{
  public class TestWxeTransactionFailingResetValidation : TestWxeTransaction
  {
    protected override void CheckCurrentTransactionResettable()
    {
      throw new InvalidOperationException ("The current transaction cannot be reset.");
    }
  }
}