using System;
using System.Collections;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rubicon.Web.UI.Controls
{
public class CommandHandler : Control, IPostBackEventHandler
{
  // types
  public event CommandEventHandler Command;

  // static members and constants

  // member fields

  // construction and disposing

  // methods and properties
      
  protected virtual void OnCommand (CommandEventArgs e) 
  {     
      if (Command != null) 
      {
        Command (this, e);
      }  
  }
  
  public void RaisePostBackEvent (string eventArgument)
  {   
      CommandEventArgs e = new CommandEventArgs (this.UniqueID, eventArgument);

      OnCommand (e);
  }

  [Obsolete ("Use GetHref (...) instead.")]
  public string GetPostBackClientHyperlink (string eventArgument)
  { 
    return Page.GetPostBackClientHyperlink (this, eventArgument);
  }

  [Obsolete ("Use GetHref (...) instead.")]
  public string GetPostBackClientHyperlink (int eventArgument)
  { 
    return GetPostBackClientHyperlink (eventArgument.ToString ());
  }

  public string GetHref (int eventArgument)
  { 
    return GetHref (eventArgument.ToString ());
  }

  public string GetHref (string eventArgument)
  { 
    return Page.GetPostBackClientHyperlink (this, eventArgument);
  }


  public HyperLink CreateHyperLink (int eventArgument, string linkText)
  {
    return CreateHyperLink (eventArgument.ToString (), linkText);
  }

  public HyperLink CreateHyperLink (string eventArgument, string linkText)
  {
    return CreateHyperLink (eventArgument, linkText, null);
  }

  public HyperLink CreateHyperLink (int eventArgument, string linkText, string cssClass)
  {
    return CreateHyperLink (eventArgument.ToString (), linkText, cssClass);
  }

  public HyperLink CreateHyperLink (string eventArgument, string linkText, string cssClass)
  {
    HyperLink hyperLink = new HyperLink ();

    if (cssClass != null && cssClass != string.Empty)
      hyperLink.CssClass = "standardLink";

    hyperLink.Text = linkText;
    hyperLink.NavigateUrl = GetHref (eventArgument);

    return hyperLink;
  }

  public string GetLink (int eventArgument, string linkText)
  {
    return GetLink (eventArgument.ToString (), linkText);
  }

  public string GetLink (string eventArgument, string linkText)
  {
    return GetLink (eventArgument, linkText, null);
  }

  public string GetLink (int eventArgument, string linkText, string cssClass)
  {
    return GetLink (eventArgument.ToString (), linkText, cssClass);
  }

  public string GetLink (string eventArgument, string linkText, string cssClass)
  {
    if (cssClass != null && cssClass != string.Empty)
    {
      string link = "<a class=\"{0}\" href=\"{1}\">{2}</a>";
      return string.Format (link, cssClass, GetHref (eventArgument), linkText);
    }
    else
    {
      string link = "<a href=\"{0}\">{1}</a>";
      return string.Format (link, GetHref (eventArgument), linkText);
    }
  }

  public HyperLink CreateHyperLinkWithConfirmation (
      int eventArgument, 
      string linkText, 
      string confirmationText)
  {
    return CreateHyperLinkWithConfirmation (eventArgument.ToString (), linkText, confirmationText);
  }

  public HyperLink CreateHyperLinkWithConfirmation (
      string eventArgument, 
      string linkText, 
      string confirmationText)
  {
    return CreateHyperLinkWithConfirmation (eventArgument, linkText, confirmationText, null);
  }

  public HyperLink CreateHyperLinkWithConfirmation (
      int eventArgument, 
      string linkText, 
      string confirmationText,
      string cssClass)
  {
    return CreateHyperLinkWithConfirmation (eventArgument.ToString (), linkText, confirmationText, cssClass);
  }

  public HyperLink CreateHyperLinkWithConfirmation (
      string eventArgument, 
      string linkText, 
      string confirmationText,
      string cssClass)
  {
    HyperLink hyperLink = new HyperLink ();

    if (cssClass != null && cssClass != string.Empty)
      hyperLink.CssClass = "standardLink";

    hyperLink.Text = linkText;
    hyperLink.NavigateUrl = "#";
    hyperLink.Attributes["onClick"] = GetConfirmationJavaScript (confirmationText, eventArgument);

    return hyperLink;
  }

  private string GetConfirmationJavaScript (string confirmationText, string eventArgument)
  {
    return string.Format (
        @"if (window.confirm ('{0}')) {{{1}; return true;}}"
        + @"else {{return false;}}", 
        confirmationText, Page.GetPostBackEventReference (this, eventArgument));
  }

  public string GetLinkWithConfirmation (int eventArgument, string linkText, string confirmationText)
  {
    return GetLinkWithConfirmation (eventArgument.ToString (), linkText, confirmationText);
  }

  public string GetLinkWithConfirmation (string eventArgument, string linkText, string confirmationText)
  {
    return GetLinkWithConfirmation (eventArgument, linkText, confirmationText, null);
  }

  public string GetLinkWithConfirmation (
      int eventArgument, 
      string linkText, 
      string confirmationText, 
      string cssClass)
  {
    return GetLinkWithConfirmation (eventArgument.ToString (), linkText, confirmationText, cssClass);
  }

  public string GetLinkWithConfirmation (
      string eventArgument, 
      string linkText, 
      string confirmationText, 
      string cssClass)
  {
    if (cssClass != null && cssClass != string.Empty)
    {
      string link = "<a class=\"{0}\" href=\"#\" onClick=\"{1}\">{2}</a>";
      
      return string.Format (
          link, cssClass, GetConfirmationJavaScript (confirmationText, eventArgument), linkText);
    }
    else
    {
      string link = "<a href=\"#\" onClick=\"{0}\">{1}</a>";

      return string.Format (
          link, GetConfirmationJavaScript (confirmationText, eventArgument), linkText);
    }
  }
}
}
