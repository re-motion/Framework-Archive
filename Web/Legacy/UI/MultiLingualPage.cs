using System;
using System.Web.UI;

using Rubicon.Findit.Globalization.Classes;

namespace Rubicon.Findit.Globalization.UI
{
public class MultiLingualPage : Page
{
  // types

  // static members and constants

  // member fields

  private string _pageTitle;

  // construction and disposing

  // methods and properties

  public string PageTitle
  {
    get { return _pageTitle; }
    set { _pageTitle = value; }
  }

  protected override void OnInit(EventArgs e)
  {
    // TODO: Delete line below:
    if (this.Request.Url.AbsolutePath.IndexOf ("AdminEditContacts.aspx") >= 0 )
      ResourceDispatcher.Dispatch (this);

    base.OnInit (e);
  }

}
}
