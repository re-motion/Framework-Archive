using System;
using System.Web.UI;

using Rubicon.Findit.Globalization.Classes;
namespace Rubicon.Findit.Globalization.UI
{
public class MultiLingualUserControl : UserControl
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  // methods and properties

  protected override void OnInit(EventArgs e)
  {
    // TODO: delete TRY
    try
    {
      ResourceDispatcher.Dispatch (this);
    }
    catch
    {
    }

    base.OnInit (e);
  }

}
}
