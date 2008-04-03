using System;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
  [Serializable]
  public class AutoCommitTestTransactedFunction: WxeTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public AutoCommitTestTransactedFunction (WxeTransactionMode transactionMode, ObjectID objectWithAllDataTypes)
        : base (transactionMode, objectWithAllDataTypes)
    {
    }

    // methods and properties

    [WxeParameter (1, true, WxeParameterDirection.In)]
    public ObjectID ObjectWithAllDataTypes
    {
      get { return (ObjectID) Variables["ObjectWithAllDataTypes"]; }
      set { Variables["ObjectWithAllDataTypes"] = value; }
    }

    private void Step1()
    {
      ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (ObjectWithAllDataTypes);

      objectWithAllDataTypes.Int32Property = 10;
    }
  }
}