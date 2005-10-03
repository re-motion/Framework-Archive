using System;

using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Data.DomainObjects.Web.Test.Domain;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
 
namespace Rubicon.Data.DomainObjects.Web.Test.WxeFunctions
{
public class EditObjectFunction : WxeTransactedFunction
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public EditObjectFunction ()
  {
    ReturnUrl = "default.aspx";
  }

  // methods and properties

  public ClassWithAllDataTypes ObjectWithAllDataTypes
  {
    get { return (ClassWithAllDataTypes) Variables["ObjectWithAllDataTypes"]; }
    set { Variables["ObjectWithAllDataTypes"] = value;}
  }

  private void Step1 ()
  {
    ObjectID id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));
    ObjectWithAllDataTypes = ClassWithAllDataTypes.GetObject (id);
  }

  private WxePageStep Step2 = new WxePageStep ("EditObject.aspx");
}
}
