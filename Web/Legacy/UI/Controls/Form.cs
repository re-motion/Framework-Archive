using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.XPath;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Rubicon.Web.UI.Controls
{
/// <summary>
/// Extends the standard ASP.NET HtmlForm. 
/// Rubicon.Web.UI.Controls.Form overcomes 2 ASP.NET limitations:
/// Set the action attribute of a server-side html form.
/// Place multiple html forms on a single ASP.NET page (Note: Set property 'AllowMultipleForms' to true).
/// </summary>
public class Form : HtmlForm
{

  // types

  // static members and constants

  // member fields

  private string _action;
  private bool _allowMultipeForms;

  // construction and disposing

  public Form ()
  {
    _action = string.Empty;
    _allowMultipeForms = false;
  }

  // abstract methods

  // methods and properties

  /// <summary>
  /// Override Render to add citizen card environment form fields.
  /// </summary>
	protected override void Render (HtmlTextWriter output)
	{
    CheckMembers ();

    output.WriteBeginTag ("form");
    RenderAttributes (output);
    output.Write (HtmlTextWriter.TagRightChar);
    
    RenderChildren (output);

    output.WriteEndTag ("form");
	}

  /// <remarks>
  /// Override RenderAttributes to prevent base class HtmlForm from using hardcoded "action" attribute. 
  /// </remarks>
  protected override void RenderAttributes (System.Web.UI.HtmlTextWriter output) 
  {
    output.WriteAttribute ("action", _action);
    Attributes.Remove ("action");

    RenderAttributesLikeBaseClass (output);
  }

  /// <summary>
  /// Implements RenderAttributes from base class HtmlForm.
  /// </summary>
  /// <remarks>
  /// RenderAttributes from base class cannot be called, because base class would write 
  /// hardcoded "action" attribute.
  /// </remarks>
  private void RenderAttributesLikeBaseClass (HtmlTextWriter output)
  {
    output.WriteAttribute ("name", Name);
    output.WriteAttribute ("method", Method);
    
    // Remove written attributes
    Attributes.Remove ("name");
    Attributes.Remove ("method");
			
    string submitEvent = Page_ClientOnSubmitEvent;
    if (submitEvent != null && submitEvent.Length > 0) 
    {
      if (this.Attributes["onsubmit"] != null) 
      {
        submitEvent = submitEvent + Attributes["onsubmit"];
        Attributes.Remove ("onsubmit");
      }
      output.WriteAttribute ("language", "javascript");
      output.WriteAttribute ("onsubmit", submitEvent);
    }
    
    output.WriteAttribute ("id", ClientID);

    ViewState.Remove ("innerhtml");
    Attributes.Render (output);
  }

  protected override void RenderChildren (HtmlTextWriter output) 
  {
    if (_allowMultipeForms)
    {
      // If multiple forms should be allowed the complete RenderChildren method has
      // to be implemented by ourselves.
      Page_OnFormRender (output);
      RenderChildrenLikeBaseClass (output);    
      Page_OnFormPostRender (output);
    }
    else
    {
      // Multiple forms are not allowed
      base.RenderChildren (output);
    }
  }

  private void RenderChildrenLikeBaseClass (HtmlTextWriter output)
  {
    if (Page_RenderMethod != null) 
    {
      Page_RenderMethod.Invoke(output, this);
      return;
    }

    if (this.Controls != null) 
    {
      foreach (Control control in this.Controls)
        control.RenderControl (output);
    }
  }

  private void Page_OnFormRender (HtmlTextWriter output) 
  {
    SetPrivateFieldValue (typeof (Page), "_inOnFormRender", Page, true);
    Page_RenderHiddenFields (output);

    if (Page_ViewStateToPersist != null)
    {
      if (Page_Formatter == null)
        Page_CreateLosFormatter ();

      output.WriteLine();
      output.Write("<input type=\"hidden\" name=\"");
      output.Write("__VIEWSTATE");
      output.Write("\" value=\"");
      Page_Formatter.Serialize(output, Page_ViewStateToPersist);
      output.WriteLine("\" />");
    }
    else
    {
      output.WriteLine();
      output.Write("<input type=\"hidden\" name=\"");
      output.Write("__VIEWSTATE");
      output.Write("\" value=\"\" />");
    }

    if (Page_FRequirePostBackScript && !IsPostBackScriptRendered)
      Page_RenderPostBackScript (output, UniqueID);

    if (!AreRegisteredClientScriptBlocksRendered) 
    {
      Page_RenderScriptBlock (output, Page_RegisteredClientScriptBlocks);
      AreRegisteredClientScriptBlocksRendered = true;
    }
  }

  private bool AreRegisteredClientScriptBlocksRendered
  {
    get 
    { 
      return (ViewState["Rubicon.Web.UI.Controls.Form.AreRegisteredClientScriptBlocksRendered"] != null 
          && (bool) ViewState["Rubicon.Web.UI.Controls.Form.AreRegisteredClientScriptBlocksRendered"]);
    }
    set
    {
      ViewState["Rubicon.Web.UI.Controls.Form.AreRegisteredClientScriptBlocksRendered"] = value;
    }
  }

  private bool IsPostBackScriptRendered
  {
    get 
    { 
      return (ViewState["Rubicon.Web.UI.Controls.Form.IsPostBackScriptRendered"] != null 
        && (bool) ViewState["Rubicon.Web.UI.Controls.Form.IsPostBackScriptRendered"]);
    }
    set
    {
      ViewState["Rubicon.Web.UI.Controls.Form.IsPostBackScriptRendered"] = value;
    }
  }
  private bool AreRegisteredClientStartupScriptsRendered
  {
    get 
    { 
      return (ViewState["Rubicon.Web.UI.Controls.Form.AreRegisteredClientStartupScriptsRendered"] != null 
        && (bool) ViewState["Rubicon.Web.UI.Controls.Form.AreRegisteredClientStartupScriptsRendered"]);
    }
    set
    {
      ViewState["Rubicon.Web.UI.Controls.Form.AreRegisteredClientStartupScriptsRendered"] = value;
    }
  }

  
  private void Page_RenderScriptBlock (HtmlTextWriter output, IDictionary scriptBlocks) 
  {
    Type pageType = typeof(Page);

    MethodInfo renderScriptBlockMethod = pageType.GetMethod (
      "RenderScriptBlock", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);

    renderScriptBlockMethod.Invoke (Page, new object[] {output});     
  }

  private void Page_OnFormPostRender (HtmlTextWriter output) 
  {
    if (Page_RegisteredArrayDeclares != null)
    {
      output.WriteLine ();
      output.WriteLine ("<script language=\"javascript\">\r\n<!--");
      ++output.Indent;

      IDictionaryEnumerator registeredArrayDeclaresEnumerator = 
          Page_RegisteredArrayDeclares.GetEnumerator();

      while (registeredArrayDeclaresEnumerator.MoveNext())
      {
        output.Write ("var ");
        output.Write(registeredArrayDeclaresEnumerator.Key);
        output.Write(" =  new Array(");
        IEnumerator valueEnumerator = ((ArrayList) registeredArrayDeclaresEnumerator.Value).GetEnumerator();
        bool isFirstRun = true;
        while (valueEnumerator.MoveNext())
        {
          if (isFirstRun)
            isFirstRun = false;
          else
            output.Write(", ");

          output.Write(valueEnumerator.Current);
        }

        output.WriteLine(");");
      }

      ++output.Indent;
      output.WriteLine("// -->\r\n</script>");
      output.WriteLine();
    }

    Page_RenderHiddenFields (output);

    if (Page_FRequirePostBackScript && !IsPostBackScriptRendered)
      Page_RenderPostBackScript (output, UniqueID);

    if (!AreRegisteredClientStartupScriptsRendered) 
    {
      Page_RenderScriptBlock (output, Page_RegisteredClientStartupScripts);
      AreRegisteredClientStartupScriptsRendered = true;
    }

    SetPrivateFieldValue (typeof (Page), "_inOnFormRender", Page, false);
  }

  private void Page_RenderPostBackScript (HtmlTextWriter output, string formUniqueID)
  {
    string postBackScript = @"
        <script language=""javascript"">
        <!--
	        function __doPostBack(eventTarget, eventArgument) {
		        var controlID = eventTarget.split(""$"").join(""_"");
        		
		        var control = document[controlID];

		        if (control == null && document.all != null)
		          control = document.all[controlID];
        		  
		        if (control == null && document.getElementById != null)
		          control = document.getElementById (controlID);
        		
		        var theform = control.form;
		        theform.__EVENTTARGET.value = controlID;
		        theform.__EVENTARGUMENT.value = eventArgument;
		        theform.submit();
	        }
        // -->";
    
    output.Write(postBackScript);
    IsPostBackScriptRendered = true;
  }
 
  private void Page_RenderHiddenFields (HtmlTextWriter output) 
  {
    Type pageType = typeof(Page);

    MethodInfo renderHiddenFieldsMethod = pageType.GetMethod (
      "RenderHiddenFields", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);

    renderHiddenFieldsMethod.Invoke (Page, new object[] {output});     
  }

  private void Page_CreateLosFormatter () 
  {
    Type pageType = typeof (Page);

    MethodInfo _createLosFormatterMethod = pageType.GetMethod (
      "CreateLosFormatter", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);

    _createLosFormatterMethod.Invoke (Page, new object[] {});     
  }  

  /// <summary>
  /// Uses reflection to access the ClientOnSubmitEvent property of the Page class
  /// </summary>
  private string Page_ClientOnSubmitEvent 
  {
    get 
    {
      return (string) GetPrivatePropertyValue (typeof(Page), "ClientOnSubmitEvent", Page);
    }
  }

  /// <summary>
  /// Uses reflection to access the _viewStateToPersist field of the Page class
  /// </summary>
  private object Page_ViewStateToPersist 
  {
    get 
    {
      return GetPrivateFieldValue (typeof(Page), "_viewStateToPersist", this);
    }
  }

  /// <summary>
  /// Uses reflection to access the _registeredClientScriptBlocks field of the Page class
  /// </summary>
  private IDictionary Page_RegisteredClientScriptBlocks 
  {
    get 
    {
      return (IDictionary) GetPrivateFieldValue (typeof(Page), "_registeredClientScriptBlocks", this);
    }
  }

  /// <summary>
  /// Uses reflection to access the _registeredClientStartupScripts field of the Page class
  /// </summary>
  private IDictionary Page_RegisteredClientStartupScripts 
  {
    get 
    {
      return (IDictionary) GetPrivateFieldValue (typeof(Page), "_registeredClientStartupScripts", this);
    }
  }

  /// <summary>
  /// Uses reflection to access the _registeredArrayDeclares field of the Page class
  /// </summary>
  private IDictionary Page_RegisteredArrayDeclares
  {
    get 
    {
      return (IDictionary) GetPrivateFieldValue (typeof(Page), "_registeredArrayDeclares", this);
    }
  }
  /// <summary>
  /// Uses reflection to access the _fRequirePostBackScript field of the Page class
  /// </summary>
  private bool Page_FRequirePostBackScript 
  {
    get 
    {
      return (bool) GetPrivateFieldValue (typeof(Page), "_fRequirePostBackScript", this);
    }
    set
    {
      SetPrivateFieldValue (typeof (Page), "_fRequirePostBackScript", Page, value);
    }
  }

  /// <summary>
  /// Uses reflection to access the _formatter field of the Page class
  /// </summary>
  private LosFormatter Page_Formatter 
  {
    get 
    {
      return (LosFormatter) GetPrivateFieldValue (typeof (Page), "_formatter", this);
    }
  }
  /// <summary>
  /// Uses reflection to access the _renderMethod field of the Page class
  /// </summary>
  private RenderMethod Page_RenderMethod
  {
    get 
    {
      return (RenderMethod) GetPrivateFieldValue (typeof(Page), "_renderMethod", this);
    }
  }

  private Object GetPrivatePropertyValue (Type targetType, String propertyName, Object target) 
  {
    PropertyInfo property = GetPrivateProperty (targetType, propertyName);

    if (property != null) 
      return property.GetValue (target, null);		
    else 
      return null; 
  }

  private PropertyInfo GetPrivateProperty (Type targetType, String propertyName) 
  {
    return targetType.GetProperty (
        propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);
  }

  private Object GetPrivateFieldValue (Type targetType, String fieldName, Object target) 
  {
    FieldInfo field = GetPrivateField (targetType, fieldName);

    if (field != null) 
      return field.GetValue (target);		
    else 
      return null; 
  }

  private void SetPrivateFieldValue (Type targetType, String fieldName, Object target, object value) 
  {
    FieldInfo field = GetPrivateField (targetType, fieldName);

    if (field != null) 
    {
      field.SetValue (target, value);		
    }
    else 
    {
      throw new InvalidOperationException (string.Format (
          "Type '{0}' doesn't contain a field '{1}'.", targetType.ToString (), fieldName));
    }
  }

  private FieldInfo GetPrivateField (Type targetType, String fieldName) 
  {
    return targetType.GetField (
        fieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);
  }

  private void CheckMembers ()
  {
    if (_action == null || _action == string.Empty) throw new InvalidOperationException ("'Action' attribute must not be empty string.");
  }

  /// <summary>
  /// Gets or sets the action attribute of the html form.
  /// </summary>
  public string Action
  {
    get { return _action; }
    set { _action = value; }
  }

  /// <summary>
  /// Gets or sets whether multiple html forms are allowed on a single page.
  /// </summary>
  public bool AllowMultipeForms
  {
    get { return _allowMultipeForms; }
    set { _allowMultipeForms = value; }
  }
}
}

