using System;

namespace Rubicon.Web.UI.Controls
{
public class StandardControl : MultiLingualUserControl
{
	public StandardControl ()
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
}
}
