using System;

using System.ComponentModel;

using System.Web.UI;
using System.Web.UI.WebControls;

using System.Collections.Specialized;

namespace Rubicon.Web.UI.Controls
{
/// <summary>
/// TextBox control with the ability to specify a default text to be displayed at the client.
/// This default text is cleared when control receives focus.
/// </summary>
/// <remarks>The IPostBackDataHandler interface has to be implemented to 
/// be able to raise the TextChanged event correctly. If this interface is not implemented
/// ASP.Net raises always TextChanged event even if text hasn't changed.</remarks>
public class ExtendedTextBox: System.Web.UI.WebControls.TextBox, IPostBackDataHandler
{
  private string _defaultText = string.Empty;
  
  public ExtendedTextBox()
  {
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
      string text = base.Text;
      if (text == DefaultText)
        return string.Empty;
      else
        return text;
    }
     
    set 
    { 
      if (value == DefaultText)
        base.Text = string.Empty;
      else
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
      string text = base.Text;

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
    string newText = postCollection[postDataKey];
    if (this.EnableViewState)
    {
      bool hasTextChanged = (ClientText != newText);
      
      if (hasTextChanged)
        Text = newText;

      return hasTextChanged;
    }
    else
    {
      // If ViewState is disabled => ASP-TextBox keeps its Text and 
      // raises TextChanged event only if Text <> "" => 
      // Implement same behaviour
      Text = newText;
      return (Text != string.Empty);
    }
  }

  /// <summary>
  /// Adds attributes to be rendered with the control. 
  /// </summary>
  protected override void AddAttributesToRender(HtmlTextWriter writer) 
  {
    string onFocusJavaScript = string.Format (
        "if (this.value == '{0}') {{ "
            + " this.value = ''; "
            + " this.select(); }}", 
        DefaultText);
    
    string onBlurJavaScript = string.Format ( 
        "if (this.value == '') this.value = '{0}';",
        DefaultText);
    
    writer.AddAttribute(HtmlTextWriterAttribute.Value, ClientText);

    writer.AddAttribute("onFocus", onFocusJavaScript);
    writer.AddAttribute("onBlur", onBlurJavaScript);

    base.AddAttributesToRender(writer);   
  }
}

}
