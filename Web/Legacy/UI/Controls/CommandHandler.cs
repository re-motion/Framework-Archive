using System;

using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rubicon.Findit.Client.Controls
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
      CommandEventArgs e = new CommandEventArgs ("CommandHandler", eventArgument);

      OnCommand (e);
  }

  public string GetPostBackClientHyperlink (string eventArgument)
  { 
    return Page.GetPostBackClientHyperlink (this, eventArgument);
  }

  public string GetPostBackClientHyperlink (int eventArgument)
  { 
    return GetPostBackClientHyperlink (eventArgument.ToString ());
  }
}
}
