using System;

using System.ComponentModel;

using System.Web.UI;
using System.Web.UI.WebControls;

using System.Collections.Specialized;

namespace Rubicon.Findit.Client.Controls
{
/// <summary>
/// TextBox control with the ability to specify a default text to be displayed at the client.
/// This default text is cleared when control receives focus.
/// </summary>
/// <remarks>The IPostBackDataHandler interface has to be implemented to 
/// be able to raise the TextChanged event correctly. If this interface is not implemented
/// ASP.Net raises always TextChanged event even if text hasn't changed.</remarks>
public class DefaultTextControl : System.Web.UI.WebControls.TextBox, IPostBackDataHandler
{
  private string _defaultText = "";
  
  public DefaultTextControl() : base ()
  {

  }

  protected DefaultTextControl (string defaultText) : base()
  {
    DefaultText = defaultText;
  }


  /// <summary>
  /// Returns/Sets the default text to be displayed at the client.
  /// </summary>
  public string DefaultText
  {
    get { return _defaultText; }
    set { _defaultText = value; }
  }
  

  /// <summary>
  /// Returns/Sets the text for server-side processing.
  /// </summary>
  public override string Text
  {
    get 
    { 
      string text = string.Empty;

      if (ViewState["Text"] != null)
        text = (string) ViewState["Text"];
      
      if (text == DefaultText)
        return "";
      else
        return text; 
    }
     
    set 
    { 
      ViewState["Text"] = value;
      base.Text = value; 
    }
  }

  
  /// <summary>
  /// Returns the current text to be displayed at the client.
  /// </summary>
  /// <returns></returns>
  public string ClientText 
  {
    get 
    { 
      string text = Text;

      if (text == string.Empty)
        return DefaultText;
      else
        return text;
    }
  }

  
  /// <summary>
  /// Is called by the ASP.NET Framework and 
  /// decides if a TextChanged Event has to be raised.
  /// </summary>
  bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
  { 
    if (this.EnableViewState)
    {
      bool hasTextChanged = (ClientText != postCollection[postDataKey]);
      
      if (hasTextChanged)
        Text = postCollection[postDataKey];

      return hasTextChanged;
    }
    else
    {
      // If ViewState is disabled => ASP-TextBox keeps it's Text and 
      // raises TextChanged event only if Text <> "" => 
      // Implement same behaviour
      Text = postCollection[postDataKey];
      return (Text != "");
    }
  }
  
  /// <summary>
  /// Is called by the ASP.NET Framework when TextChanged event has to be raised.
  /// </summary>
  void IPostBackDataHandler.RaisePostDataChangedEvent()
  {    
    OnTextChanged (EventArgs.Empty);    
  }
  
  /// <summary>
  /// Raises the OnTextChanged event. Can be overridden.
  /// </summary>
  protected override void OnTextChanged (EventArgs e)
  {
    base.OnTextChanged (e);
  }

  
  /// <summary>
  /// Adds attributes to be rendered with the control. 
  /// </summary>
  protected override void AddAttributesToRender(HtmlTextWriter writer) 
  {
    
    string  commandTemplate=  "if (this.value == '{0}') this.value = '{1}';";
    
    string onClickJavaScript = String.Format ( commandTemplate, DefaultText, "");
    string onBlurJavaScript = String.Format ( commandTemplate,"",  DefaultText);
    
    writer.AddAttribute(HtmlTextWriterAttribute.Value, ClientText);

    writer.AddAttribute("onFocus", onClickJavaScript);
    writer.AddAttribute("onBlur", onBlurJavaScript);

    base.AddAttributesToRender(writer);
  }
}
}
