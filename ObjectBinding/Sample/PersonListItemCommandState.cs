using System;
using System.Web.UI;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.UI.Controls;

namespace Rubicon.ObjectBinding.Sample
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