using System;
using System.IO;
using System.Web.UI;

using Rubicon.Web.Utilities;
using System.Collections;

namespace Rubicon.Web.UI
{

/// <summary>
/// Provides a common implementation for web pages.
/// </summary>
public class StandardPage : NavigablePage
{
  // types

  // static members and constants

  // member fields

  private Control _focusControl = null;
  private bool _saveViewStateToSession = true;

  // construction and disposing

  public StandardPage ()
  {
  }

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);

    RegisterEventHandlers();

    this.PreRender += new EventHandler (this.Page_OnPreRender);

    Response.AppendHeader ("Expires", "-1");
    Response.AppendHeader ("Cache-Control", "max-age=0,no-cache");
    Response.AppendHeader ("Pragma", "no-cache");    
  }

  protected virtual void RegisterEventHandlers()
  {
  }


  // methods and properties

  public virtual void CloseBrowserWindow ()
  {
    CloseBrowserWindow (false);
  }

  public virtual void CloseBrowserWindow (bool refreshParent)
  {
    _saveViewStateToSession = false;
    CleanupSession ();

    StandardPageUtility.CloseBrowserWindow (this, refreshParent);
  }

  public string GetErrorImage (string errorMessage)
  {
    return ImageUtility.GetErrorImage (this, errorMessage);
  }

  public string GetRequiredFieldImage ()
  {
    return ImageUtility.GetRequiredFieldImage (this);
  }

  public string GetIconImage (string tooltip, string imagePath)
  {
    return ImageUtility.GetIconImage (this, tooltip, imagePath);
  }

  protected virtual bool SaveViewStateToSession 
  {
    get { return _saveViewStateToSession; }
  }

  protected virtual bool PreventClientCaching
  {
    get { return true; }
  }

  public void RegisterOpenReportScript (string reportUrl)
  {
    StandardPageUtility.RegisterOpenReportScript (this, reportUrl);
  }

  public Control FocusControl 
  {
    get { return _focusControl; }
    set { _focusControl = value; }
  }

  public override Control FindControl (string id)
  {
    if (id == this.ID)
      return this;
    return base.FindControl (id);
  }

  private void Page_OnPreRender (object sender, System.EventArgs e)
  {
    if (_focusControl != null)
    {
      string setFocusScript = string.Format (
        "var focusControl;"
        + "\n focusControl = document.all(\"{0}\");"
        + "\n if (focusControl != null)"
        + "\n   focusControl.focus ();", 
        _focusControl.ClientID);

      this.RegisterStartupScript ("SetPageFocus", 
        "<script language=\"javascript\" type=\"text/javascript\">\n" + setFocusScript + "\n</script>");
    }

    if (PreventClientCaching)
    {
      Response.AppendHeader ("Expires", "-1");
      Response.AppendHeader ("Cache-Control", "max-age=0,no-cache");
      Response.AppendHeader ("Pragma", "no-cache");
    }
  }

  /// <summary>
  /// Registers a Javascript method for opening an URL in a new window and 
  /// returns the required Javascript method call.
  /// </summary>
  /// <param name="url">The URL to open.</param>
  /// <param name="useScrollbars">Indicates whether scrollbars should be displayed in the new window.</param>
  /// <returns>The Javascript method call to open the URL in the new window.</returns>
  public string GetWindowOpenJavascriptWithRegistration (string url, bool useScrollbars)
  {
    StandardPageUtility.RegisterWindowOpenJavascript (this);
    return StandardPageUtility.GetWindowOpenJavascript (url, useScrollbars);
  }

  protected override void SavePageStateToPersistenceMedium (object state)
  {
    if (SaveViewStateToSession)
    {
#if NET11
      SaveViewStateToSessionInternal ((Triplet) state);
#else
      Pair pair = (Pair) state;
      SaveControlStateToSession ((IDictionary) pair.First);
      SaveViewStateToSessionInternal ((Pair) pair.Second);
#endif
    }
    
    base.SavePageStateToPersistenceMedium (state);
  }

  protected override object LoadPageStateFromPersistenceMedium ()
  {
    if (SaveViewStateToSession)
    {
#if NET11
      return LoadViewStateFromSession ();
#else
      IDictionary controlState = LoadControlStateFromSession ();
      Pair viewState = LoadViewStateFromSession ();
      return new Pair (controlState, viewState);
#endif
    }
    else
    {
      return base.LoadPageStateFromPersistenceMedium ();
    }
  }

#if NET11
  private void SaveViewStateToSessionInternal (Triplet viewState)
  {
    LosFormatter formatter = new LosFormatter();
    StringWriter writer = new StringWriter ();
    formatter.Serialize (writer, viewState.Second);
    SetSessionValue ("state.second", writer.ToString ());

    writer = new StringWriter ();

    formatter.Serialize (writer, viewState.Third);
    SetSessionValue ("state.third", writer.ToString ());

    viewState.Second = null;
    viewState.Third = null;
  }

  private Triplet LoadViewStateFromSession ()
  {
    Triplet viewState = (Triplet) base.LoadPageStateFromPersistenceMedium (); 
    LosFormatter formatter = new LosFormatter ();

    object obj= null;
    obj = this.GetSessionValue("state.second", false); 

    if (obj != null && obj.ToString() != string.Empty)
      viewState.Second = formatter.Deserialize((string) obj);
    else
      viewState.Second = null;

    obj = this.GetSessionValue("state.third", false);

    if (obj != null && obj.ToString() != string.Empty)
      viewState.Third = formatter.Deserialize ((string) obj);
    else
      viewState.Third = null;

    return viewState;
  }
#else
  private void SaveViewStateToSessionInternal (Pair viewState)
  {
    LosFormatter formatter = new LosFormatter ();
    StringWriter writer = new StringWriter ();
    formatter.Serialize (writer, viewState.Second);
    SetSessionValue ("viewState", writer.ToString ());

    viewState.Second = null;
  }

  private void SaveControlStateToSession (IDictionary controlState)
  {
    LosFormatter formatter = new LosFormatter ();
    StringWriter writer = new StringWriter ();
    formatter.Serialize (writer, controlState);
    SetSessionValue ("controlState", writer.ToString ());
    controlState.Clear ();
  }

  private Pair LoadViewStateFromSession ()
  {
    Pair state = (Pair) base.LoadPageStateFromPersistenceMedium ();
    Pair viewState = (Pair) state.Second;
    LosFormatter formatter = new LosFormatter ();

    object obj= null;
    obj = this.GetSessionValue ("viewState", false);

    if (obj != null && obj.ToString () != string.Empty)
      viewState.Second = formatter.Deserialize ((string) obj);
    else
      viewState.Second = null;

    return viewState;
  }

  private IDictionary LoadControlStateFromSession ()
  {
    IDictionary controlState = null;
    LosFormatter formatter = new LosFormatter ();

    object obj= null;
    obj = this.GetSessionValue ("controlState", false);
    if (obj != null && obj.ToString () != string.Empty)
      controlState = (IDictionary) formatter.Deserialize ((string) obj);

    return controlState;
  }
#endif
}

}
