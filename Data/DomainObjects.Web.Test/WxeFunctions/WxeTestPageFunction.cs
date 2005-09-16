using System;

using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Data.DomainObjects.Web.Test.WxeFunctions
{
public class WxeTestPageFunction : WxeFunction
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public WxeTestPageFunction (ObjectID classWithAllDataTypesID, ClientTransaction clientTransaction) 
      : base (classWithAllDataTypesID, clientTransaction)
  {
  }

  // methods and properties

  [WxeParameter (1, true, WxeParameterDirection.In)]
  public ObjectID ClassWithAllDataTypesID
  {
    get { return (ObjectID) Variables["ClassWithAllDataTypesID"]; }
    set { Variables["ClassWithAllDataTypesID"] = value; }
  }

  [WxeParameter (2, false, WxeParameterDirection.In)]
  public ClientTransaction ClientTransaction
  {
    get { return (ClientTransaction) Variables["ClientTransaction"]; }
    set { Variables["ClientTransaction"] = value; }
  }
}
}
