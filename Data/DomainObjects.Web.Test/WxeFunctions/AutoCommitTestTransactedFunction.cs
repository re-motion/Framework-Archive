using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;

using Rubicon.Data.DomainObjects.Web.Test.Domain;

namespace Rubicon.Data.DomainObjects.Web.Test.WxeFunctions
{
public class AutoCommitTestTransactedFunction : WxeTransactedFunction
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public AutoCommitTestTransactedFunction (TransactionMode transactionMode, ObjectID objectWithAllDataTypes) 
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

  private void Step1 ()
  {
    ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (ObjectWithAllDataTypes);

    objectWithAllDataTypes.Int32Property = 10;
  }

}
}
