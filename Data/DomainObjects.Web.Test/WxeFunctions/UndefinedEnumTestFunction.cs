using System;

using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Data.DomainObjects.Web.Test.Domain;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
 
namespace Rubicon.Data.DomainObjects.Web.Test.WxeFunctions
{
[Serializable]
public class UndefinedEnumTestFunction : WxeTransactedFunction
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public UndefinedEnumTestFunction ()
  {
    ReturnUrl = "default.aspx";
  }

  // methods and properties

  public SearchObjectWithUndefinedEnum SearchObjectWithUndefinedEnum
  {
    get { return (SearchObjectWithUndefinedEnum) Variables["SearchObjectWithUndefinedEnum"]; }
    set { Variables["SearchObjectWithUndefinedEnum"] = value;}
  }

  public ClassWithUndefinedEnum ExistingObjectWithUndefinedEnum
  {
    get { return (ClassWithUndefinedEnum) Variables["ExistingObjectWithUndefinedEnum"]; }
    set { Variables["ExistingObjectWithUndefinedEnum"] = value;}
  }

  public ClassWithUndefinedEnum NewObjectWithUndefinedEnum
  {
    get { return (ClassWithUndefinedEnum) Variables["NewObjectWithUndefinedEnum"]; }
    set { Variables["NewObjectWithUndefinedEnum"] = value;}
  }

  private void Step1 ()
  {
    ExistingObjectWithUndefinedEnum = DomainObject.GetObject<ClassWithUndefinedEnum> (DomainObjectIDs.ObjectWithUndefinedEnum);
    NewObjectWithUndefinedEnum = ClassWithUndefinedEnum.NewObject ();
    SearchObjectWithUndefinedEnum = new SearchObjectWithUndefinedEnum ();
  }

  private WxePageStep Step2 = new WxePageStep ("UndefinedEnumTest.aspx");
}
}
