using System;
using System.Collections;

using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Data.DomainObjects.Web.Test.Domain;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
 
namespace Rubicon.Data.DomainObjects.Web.Test.WxeFunctions
{
public class SearchFunction : WxeTransactedFunction
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public SearchFunction ()
  {
    ReturnUrl = "default.aspx";
  }

  // methods and properties

  public ClassWithAllDataTypesSearch SearchObject
  {
    get { return (ClassWithAllDataTypesSearch) Variables["SearchObject"]; }
    set { Variables["SearchObject"] = value;}
  }

  public IList Result 
  {
    get { return (IList) Variables["Result"]; }
    set { Variables["Result"] = value;}
  }

  public void Requery ()
  {
    Result = ClientTransaction.Current.QueryManager.GetCollection (SearchObject.CreateQuery ());
  }

  private void Step1 ()
  {
    SearchObject = new ClassWithAllDataTypesSearch ();
    Requery ();
  }

  private WxePageStep Step2 = new WxePageStep ("SearchObject.aspx");
}
}
