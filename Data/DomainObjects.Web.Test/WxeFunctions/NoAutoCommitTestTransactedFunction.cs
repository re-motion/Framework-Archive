using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;

using Rubicon.Data.DomainObjects.Web.Test.Domain;

namespace Rubicon.Data.DomainObjects.Web.Test.WxeFunctions
{
public class NoAutoCommitTestTransactedFunction : WxeTransactedFunction
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public NoAutoCommitTestTransactedFunction (TransactionMode transactionMode, ObjectID objectWithAllDataTypes) 
      : base (transactionMode, objectWithAllDataTypes)
  {
  }

  // methods and properties

  protected override WxeTransactionBase CreateWxeTransaction ()
  {
    // TODO: check this with ML: (true, true) should be (false, true)
    return new WxeTransaction (false, true);
  }


  [WxeParameter (1, true, WxeParameterDirection.In)]
  public ObjectID ObjectWithAllDataTypes
  {
    get { return (ObjectID) Variables["ObjectWithAllDataTypes"]; }
    set { Variables["ObjectWithAllDataTypes"] = value; }
  }

  private void Step1 ()
  {
    ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (ObjectWithAllDataTypes);

    objectWithAllDataTypes.Int32Property = 10;
  }

}
}
