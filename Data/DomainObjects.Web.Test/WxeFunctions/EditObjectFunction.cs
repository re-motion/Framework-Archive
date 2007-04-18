using System;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Data.DomainObjects.Web.Test.Domain;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Data.DomainObjects.Web.Test.WxeFunctions
{
  [Serializable]
  public class EditObjectFunction: WxeTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditObjectFunction()
    {
      ReturnUrl = "default.aspx";
    }

    // methods and properties

    public ClassWithAllDataTypes ObjectWithAllDataTypes
    {
      get { return (ClassWithAllDataTypes) Variables["ObjectWithAllDataTypes"]; }
      set { Variables["ObjectWithAllDataTypes"] = value; }
    }

    private void Step1()
    {
      ObjectWithAllDataTypes = DomainObject.GetObject<ClassWithAllDataTypes> (DomainObjectIDs.ObjectWithAllDataTypes1);
    }

    private WxePageStep Step2 = new WxePageStep ("EditObject.aspx");
  }
}