using System;
using Rubicon.Data;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{

  /// <summary> Provides a test implementation of the <see langword="abstract"/> <see cref="WxeTransactedFunctionBase{TTransaction}"/> type. </summary>
  [Serializable]
  public class WxeTransactedFunctionMock : WxeTransactedFunctionBase<ITransaction>
  {
    private ProxyWxeTransaction _storedWxeTransaction;

    public WxeTransactedFunctionMock (ProxyWxeTransaction wxeTransaction)
      : base ()
    {
      _storedWxeTransaction = wxeTransaction;
    }

    protected override WxeTransactionBase<ITransaction> CreateWxeTransaction ()
    {
      return _storedWxeTransaction;
    }

    public new ITransaction OwnTransaction
    {
      get { return base.OwnTransaction; }
    }

    public new ITransaction ExecutionTransaction
    {
      get { return base.ExecutionTransaction; }
    }

    public void InitiateCreateTransaction ()
    {
      PrivateInvoke.SetNonPublicField (this, "_wxeTransaction", CreateWxeTransaction ());
    }
  }
}