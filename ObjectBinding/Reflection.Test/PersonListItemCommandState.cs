using System;
using System.Web.UI;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.Controls;

namespace OBRTest
{
public class PersonListItemCommandState: IBocListItemCommandState
{
  public PersonListItemCommandState ()
  {
  }

  public bool IsEnabled(
      BocList list, 
      IBusinessObject businessObject, 
      BocCommandEnabledColumnDefinition columnDefiniton)
  {
    return true;
  }
}
}
