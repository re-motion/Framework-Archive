using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Reflection;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Development.UnitTesting;
using Rubicon.Data;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{

  /// <summary> Provides a test implementation of the <see langword="abstract"/> <see cref="WxeTransactedFunctionBase{TTransaction}"/> type. </summary>
  [Serializable]
  public class WxeTransactedFunctionMock : WxeTransactedFunctionBase<ITransaction>
  {
    private ProxyWxeTransaction _wxeTransaction;

    public WxeTransactedFunctionMock (ProxyWxeTransaction wxeTransaction)
      : base ()
    {
      _wxeTransaction = wxeTransaction;
    }

    protected override WxeTransactionBase<ITransaction> CreateWxeTransaction ()
    {
      return _wxeTransaction;
    }
  }
}