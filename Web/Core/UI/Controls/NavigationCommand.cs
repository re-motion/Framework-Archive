using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.Collections;
using Rubicon.Globalization;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.Utilities;
using Rubicon.Web.UI.Design;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.Web.UI.Controls
{

//  TODO: Command: Move long comment blocks to xml-file
/// <summary> A <see cref="NavigationCommand"/> defines an action the user can invoke when navigating between pages. </summary>
public class NavigationCommand: Command
{
  public NavigationCommand ()
    : this (CommandType.Href)
  {
  }

  public NavigationCommand (CommandType defaultType)
    : base (defaultType)
  {
  }

  /// <summary> Adds the attributes for the Wxe Function command to the anchor tag. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. Must not be <see langword="null"/>. </param>
  /// <param name="postBackEvent">
  ///   The string executed upon the click on a command of types
  ///   <see cref="CommandType.Event"/> or <see cref="CommandType.WxeFunction"/>.
  ///   This string is usually the call to the <c>__doPostBack</c> script function used by ASP.net
  ///   to force a post back. Must not be <see langword="null"/>.
  /// </param>
  /// <param name="onClick"> 
  ///   The string always rendered in the <c>onClick</c> tag of the anchor element. 
  /// </param>
  /// <param name="additionalUrlParameters">
  ///   The <see cref="NameValueCollection"/> containing additional url parameters.
  ///   Must not be <see langword="null"/>.
  /// </param>
  /// <exception cref="InvalidOperationException">
  ///   If called while the <see cref="Type"/> is not set to <see cref="CommandType.WxeFunction"/>.
  /// </exception> 
  protected override void AddAttributesToRenderForWxeFunctionCommand (
      HtmlTextWriter writer, 
      string postBackEvent,
      string onClick,
      NameValueCollection additionalUrlParameters)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);
    ArgumentUtility.CheckNotNull ("postBackEvent", postBackEvent); 
    ArgumentUtility.CheckNotNull ("additionalUrlParameters", additionalUrlParameters); 
    if (Type != CommandType.WxeFunction)
      throw new InvalidOperationException ("Call to AddAttributesToRenderForWxeFunctionCommand not allowed unless Type is set to CommandType.WxeFunction.");
       
    string href = "#";
    if (System.Web.HttpContext.Current != null)
      href = GetWxeFunctionPermanentUrl (additionalUrlParameters);
    writer.AddAttribute (HtmlTextWriterAttribute.Href, href);
    if (! StringUtility.IsNullOrEmpty (WxeFunctionCommand.Target))
      writer.AddAttribute (HtmlTextWriterAttribute.Target, WxeFunctionCommand.Target);
    if (! StringUtility.IsNullOrEmpty (onClick))
      writer.AddAttribute (HtmlTextWriterAttribute.Onclick, onClick);
    if (! StringUtility.IsNullOrEmpty (ToolTip))
      writer.AddAttribute (HtmlTextWriterAttribute.Title, ToolTip);
  }

  /// <summary> 
  ///   Gets the permanent URL for the <see cref="WxeFunction"/> defined by the <see cref="WxeFunctionCommandInfo"/>. 
  /// </summary>
  /// <param name="additionalUrlParameters">
  ///   The <see cref="NameValueCollection"/> containing additional url parameters. 
  ///   Must not be <see langword="null"/>.
  /// </param>
  /// <exception cref="InvalidOperationException">
  ///   If called while the <see cref="Type"/> is not set to <see cref="CommandType.WxeFunction"/>.
  /// </exception> 
  public virtual string GetWxeFunctionPermanentUrl (NameValueCollection additionalUrlParameters)
  {
    ArgumentUtility.CheckNotNull ("additionalUrlParameters", additionalUrlParameters);

    if (Type != CommandType.WxeFunction)
      throw new InvalidOperationException ("Call to ExecuteWxeFunction not allowed unless Type is set to CommandType.WxeFunction.");

    Type functionType = TypeUtility.GetType (WxeFunctionCommand.TypeName, true, false);
    WxeParameterDeclaration[] parameterDeclarations = WxeFunction.GetParamaterDeclarations (functionType);
    object[] parameterValues = WxeFunction.ParseActualParameters (
        parameterDeclarations, WxeFunctionCommand.Parameters, System.Globalization.CultureInfo.InvariantCulture);   
    NameValueCollection queryString = 
        WxeFunction.SerializeParametersForQueryString (parameterDeclarations, parameterValues);
    queryString.Add (additionalUrlParameters);
    return WxeContext.GetPermanentUrl (HttpContext.Current, functionType, queryString);
  }

  /// <summary> 
  ///   Gets the permanent URL for the <see cref="WxeFunction"/> defined by the <see cref="WxeFunctionCommandInfo"/>. 
  /// </summary>
  /// <exception cref="InvalidOperationException">
  ///   If called while the <see cref="Type"/> is not set to <see cref="CommandType.WxeFunction"/>.
  /// </exception> 
  public string GetWxeFunctionPermanentUrl ()
  {
    return GetWxeFunctionPermanentUrl (new NameValueCollection (0));
  }
}
}
