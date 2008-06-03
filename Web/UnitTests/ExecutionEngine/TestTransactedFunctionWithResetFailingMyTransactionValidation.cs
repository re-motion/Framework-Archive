/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Threading;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;
using Remotion.Development.UnitTesting;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  public class TestTransactedFunctionWithResetFailingMyTransactionValidation : WxeTransactedFunctionBase<TestTransaction>
  {
    protected override WxeTransactionBase<TestTransaction> CreateWxeTransaction ()
    {
      return new TestWxeTransactionFailingResetValidation ();
    }

    protected override TestTransaction CreateRootTransaction ()
    {
      return new TestTransaction ();
    }

    private void Step1 ()
    {
      WxeTransactedFunctionBase<TestTransaction> parent = ParentFunction as WxeTransactedFunctionBase<TestTransaction>;
      Assert.IsNotNull (parent, "must be tested as a nested function");
      parent.ResetTransaction ();
    }
  }
}
