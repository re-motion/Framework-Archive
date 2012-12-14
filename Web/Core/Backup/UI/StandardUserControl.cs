using System;

namespace Rubicon.Web.UI
{

public class StandardUserControl : MultiLingualUserControl
{
	public StandardUserControl ()
	{
	}

  protected object GetSessionValue (string key)
  {
    NavigablePage navigablePage = Page as NavigablePage;

    if (navigablePage == null)
      throw new InvalidOperationException ("GetSessionValue can only be used on a NavigablePage.");

    return navigablePage.GetSessionValue (key);
  }

  protected object GetSessionValue (string key, bool required)
  {
    NavigablePage navigablePage = Page as NavigablePage;

    if (navigablePage == null)
      throw new InvalidOperationException ("GetSessionValue can only be used on a NavigablePage.");

    return navigablePage.GetSessionValue (key, required);
  }

  protected void SetSessionValue (string key, object sessionValue)
  {
    NavigablePage navigablePage = Page as NavigablePage;

    if (navigablePage == null)
      throw new InvalidOperationException ("SetSessionValue can only be used on a NavigablePage.");

    navigablePage.SetSessionValue (key, sessionValue);
  }

  protected void ClearSessionValue (string key)
  {
    NavigablePage navigablePage = Page as NavigablePage;

    if (navigablePage == null)
      throw new InvalidOperationException ("ClearSessionValue can only be used on a NavigablePage.");

    navigablePage.ClearSessionValue (key);
  }

  protected StandardPage StandardPage
  {
    get 
    {
      return Page as StandardPage;
    }
  }

  protected override void OnInit(EventArgs e)
  {
    RegisterEventHandlers ();
    base.OnInit (e);
  }

  protected virtual void RegisterEventHandlers ()
  {
  }
}
}
