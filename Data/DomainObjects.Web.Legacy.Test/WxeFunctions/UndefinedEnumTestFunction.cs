using System;

using Remotion.Data.DomainObjects.Web.ExecutionEngine;
using Remotion.Data.DomainObjects.Web.Legacy.Test.Domain;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
 
namespace Remotion.Data.DomainObjects.Web.Legacy.Test.WxeFunctions
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
    ExistingObjectWithUndefinedEnum = ClassWithUndefinedEnum.GetObject (DomainObjectIDs.ObjectWithUndefinedEnum);
    NewObjectWithUndefinedEnum = new ClassWithUndefinedEnum ();
    SearchObjectWithUndefinedEnum = new SearchObjectWithUndefinedEnum ();
  }

  private WxePageStep Step2 = new WxePageStep ("UndefinedEnumTest.aspx");
}
}
