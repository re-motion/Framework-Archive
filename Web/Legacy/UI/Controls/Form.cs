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

using Rubicon.Web.UI.Utilities;

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

  // construction and disposing

  public Form ()
  {
    _action = string.Empty;
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
    output.WriteAttribute ("action", Page.ResolveUrl (_action));
    Attributes.Remove ("action");

    RenderAttributesLikeBaseClass (output);
  }

  protected override void RenderChildren (HtmlTextWriter output) 
  {
    // Cache the hidden fields (EVENTTARGET, EVENTARGUMENT, ...) for another form on the same page
    if (CachedRegisteredHiddenFields != null)
      Page_RegisteredHiddenFields = CachedRegisteredHiddenFields;
    else
      CachedRegisteredHiddenFields = Page_RegisteredHiddenFields;

    Page_OnFormRender (output);
    RenderChildrenLikeBaseClass (output);    
    Page_OnFormPostRender (output);
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


  #region Private implementation for all Render... (...) methods of base class

  /// <summary>
  /// Implements RenderAttributes from base class HtmlForm.
  /// </summary>
  /// <remarks>
  /// RenderAttributes from base class cannot be called, because base class would write 
  /// hardcoded "action" attribute.
  /// </remarks>
  private void RenderAttributesLikeBaseClass (HtmlTextWriter output)
  {
    // To avoid a HTML 4.0 error the name attribute is not being rendered
    //output.WriteAttribute ("name", Name);
    output.WriteAttribute ("method", Method);
    
    // Remove written attributes
    //Attributes.Remove ("name");
    Attributes.Remove ("method");
			
    string submitEvent = Page_ClientOnSubmitEvent;
    if (submitEvent != null && submitEvent != string.Empty) 
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

    // Is normally done by HtmlContainerControl
    ViewState.Remove ("innerhtml");

    Attributes.Render (output);
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
    SetHiddenFieldValue (typeof (Page), "_inOnFormRender", Page, true);
    Page_RenderHiddenFields (output);

    RenderViewState (output);

    if (Page_FRequirePostBackScript && !IsPostBackScriptRendered)
      Page_RenderPostBackScript (output, UniqueID);

    if (!AreRegisteredClientScriptBlocksRendered) 
    {
      Page_RenderScriptBlock (output, Page_RegisteredClientScriptBlocks);
      AreRegisteredClientScriptBlocksRendered = true;
    }
  }

  private void RenderViewState (HtmlTextWriter output) 
  {
    if (IsActionSetToCurrentPage)
    {
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
    }
  }

  private IDictionary CachedRegisteredHiddenFields
  {
    get 
    { 
      return (IDictionary) Page_ViewState["Rubicon.Web.UI.Controls.Form.CachedRegisteredHiddenFields"];
    }
    set
    {
      Page_ViewState["Rubicon.Web.UI.Controls.Form.CachedRegisteredHiddenFields"] = value;
    }

  }

  private bool IsActionSetToCurrentPage
  {
    get 
    { 
      return UrlUtility.GetAbsoluteUrlWithoutProtocol (Page, _action) == 
          UrlUtility.GetAbsoluteUrlWithoutProtocol (Page, UrlUtility.GetAbsolutePageUrl (Page));
    }
  }

  private bool AreRegisteredClientScriptBlocksRendered
  {
    get 
    { 
      return (Page_ViewState["Rubicon.Web.UI.Controls.Form.AreRegisteredClientScriptBlocksRendered"] != null 
          && (bool) Page_ViewState["Rubicon.Web.UI.Controls.Form.AreRegisteredClientScriptBlocksRendered"]);
    }
    set
    {
      Page_ViewState["Rubicon.Web.UI.Controls.Form.AreRegisteredClientScriptBlocksRendered"] = value;
    }
  }


  private bool IsPostBackScriptRendered
  {
    get 
    { 
      return (Page_ViewState["Rubicon.Web.UI.Controls.Form.IsPostBackScriptRendered"] != null 
        && (bool) Page_ViewState["Rubicon.Web.UI.Controls.Form.IsPostBackScriptRendered"]);
    }
    set
    {
      Page_ViewState["Rubicon.Web.UI.Controls.Form.IsPostBackScriptRendered"] = value;
    }
  }

  private bool AreRegisteredArrayDeclaresRendered
  {
    get 
    { 
      return (Page_ViewState["Rubicon.Web.UI.Controls.Form.AreRegisteredArrayDeclaresRendered"] != null 
        && (bool) Page_ViewState["Rubicon.Web.UI.Controls.Form.AreRegisteredArrayDeclaresRendered"]);
    }
    set
    {
      Page_ViewState["Rubicon.Web.UI.Controls.Form.AreRegisteredArrayDeclaresRendered"] = value;
    }
  }

  private bool AreRegisteredClientStartupScriptsRendered
  {
    get 
    { 
      return (Page_ViewState["Rubicon.Web.UI.Controls.Form.AreRegisteredClientStartupScriptsRendered"] != null 
        && (bool) Page_ViewState["Rubicon.Web.UI.Controls.Form.AreRegisteredClientStartupScriptsRendered"]);
    }
    set
    {
      Page_ViewState["Rubicon.Web.UI.Controls.Form.AreRegisteredClientStartupScriptsRendered"] = value;
    }
  }

  
  private void Page_RenderScriptBlock (HtmlTextWriter output, IDictionary scriptBlocks) 
  {
    Type pageType = typeof(Page);

    MethodInfo renderScriptBlockMethod = pageType.GetMethod (
      "RenderScriptBlock", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);

    renderScriptBlockMethod.Invoke (Page, new object[] {output, scriptBlocks});     
  }

  private void Page_OnFormPostRender (HtmlTextWriter output) 
  {
    if (Page_RegisteredArrayDeclares != null && !AreRegisteredArrayDeclaresRendered)
    {
      output.WriteLine ();
      output.WriteLine ("<script language=\"javascript\" type=\"text/javascript\">\r\n<!--");
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

      AreRegisteredArrayDeclaresRendered = true;
    }

    Page_RenderHiddenFields (output);

    if (Page_FRequirePostBackScript && !IsPostBackScriptRendered)
      Page_RenderPostBackScript (output, UniqueID);

    if (!AreRegisteredClientStartupScriptsRendered) 
    {
      Page_RenderScriptBlock (output, Page_RegisteredClientStartupScripts);
      AreRegisteredClientStartupScriptsRendered = true;
    }

    SetHiddenFieldValue (typeof (Page), "_inOnFormRender", Page, false);
  }

  private void Page_RenderPostBackScript (HtmlTextWriter output, string formUniqueID)
  {
    string postBackScript = @"
        <script language=""javascript"" type=""text/javascript"">
          <!--
	          function __doPostBack(eventTarget, eventArgument) 
            {
		          var controlID = eventTarget.split(""$"").join(""_"");
          		
              var control = document[controlID];

              if (control == null && document.all != null)
                control = document.all[controlID];
                    		  
              if (control == null && document.getElementById != null)
                control = document.getElementById (controlID);
                    		
              var theform = control.form;
              
              if (theform == null)
                theform = GetForm (control);
              
              theform.__EVENTTARGET.value = controlID;
              theform.__EVENTARGUMENT.value = eventArgument;
              theform.submit();
            }
          	          
            function GetForm (control)
            {
              var parentControl = control.parentNode;
              
              if (parentControl == null)
                parentControl = control.parentElement;
              
              if (parentControl != null)
              {
                if (parentControl.tagName.toLowerCase () == ""form"")
                  return parentControl;
                else
                  return GetForm (parentControl);
              }
              else
              {
                return null;
              }
            }
          // -->
        </script>
        ";
    
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
      return (string) GetHiddenPropertyValue (typeof(Page), "ClientOnSubmitEvent", Page);
    }
  }

  /// <summary>
  /// Uses reflection to access the _viewStateToPersist field of the Page class
  /// </summary>
  private object Page_ViewStateToPersist 
  {
    get 
    {
      return GetHiddenFieldValue (typeof(Page), "_viewStateToPersist", Page);
    }
  }

  /// <summary>
  /// Uses reflection to access the ViewState property of the Page class
  /// </summary>
  private StateBag Page_ViewState 
  {
    get 
    {
      return (StateBag) GetHiddenPropertyValue (typeof(Page), "ViewState", Page);
    }
  }

  /// <summary>
  /// Uses reflection to access the _registeredClientScriptBlocks field of the Page class
  /// </summary>
  private IDictionary Page_RegisteredClientScriptBlocks 
  {
    get 
    {
      return (IDictionary) GetHiddenFieldValue (typeof(Page), "_registeredClientScriptBlocks", Page);
    }
  }

  /// <summary>
  /// Uses reflection to access the _registeredHiddenFields field of the Page class
  /// </summary>
  private IDictionary Page_RegisteredHiddenFields 
  {
    get 
    {
      return (IDictionary) GetHiddenFieldValue (typeof(Page), "_registeredHiddenFields", Page);
    }
    set
    {
      SetHiddenFieldValue (typeof (Page), "_registeredHiddenFields", Page, value);
    }
  }

  /// <summary>
  /// Uses reflection to access the _registeredClientStartupScripts field of the Page class
  /// </summary>
  private IDictionary Page_RegisteredClientStartupScripts 
  {
    get 
    {
      return (IDictionary) GetHiddenFieldValue (typeof(Page), "_registeredClientStartupScripts", Page);
    }
  }

  /// <summary>
  /// Uses reflection to access the _registeredArrayDeclares field of the Page class
  /// </summary>
  private IDictionary Page_RegisteredArrayDeclares
  {
    get 
    {
      return (IDictionary) GetHiddenFieldValue (typeof(Page), "_registeredArrayDeclares", Page);
    }
  }
  /// <summary>
  /// Uses reflection to access the _fRequirePostBackScript field of the Page class
  /// </summary>
  private bool Page_FRequirePostBackScript 
  {
    get 
    {
      return (bool) GetHiddenFieldValue (typeof(Page), "_fRequirePostBackScript", Page);
    }
    set
    {
      SetHiddenFieldValue (typeof (Page), "_fRequirePostBackScript", Page, value);
    }
  }

  /// <summary>
  /// Uses reflection to access the _formatter field of the Page class
  /// </summary>
  private LosFormatter Page_Formatter 
  {
    get 
    {
      return (LosFormatter) GetHiddenFieldValue (typeof (Page), "_formatter", Page);
    }
  }
  /// <summary>
  /// Uses reflection to access the _renderMethod field of the Page class
  /// </summary>
  private RenderMethod Page_RenderMethod
  {
    get 
    {
      return (RenderMethod) GetHiddenFieldValue (typeof(Page), "_renderMethod", this);
    }
  }
  #endregion

  #region Reflection helpers
  private Object GetHiddenPropertyValue (Type targetType, String propertyName, Object target) 
  {
    PropertyInfo property = GetHiddenProperty (targetType, propertyName);

    if (property != null) 
      return property.GetValue (target, null);		
    else 
      return null; 
  }

  private PropertyInfo GetHiddenProperty (Type targetType, String propertyName) 
  {
    return targetType.GetProperty (
        propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);
  }

  private object GetHiddenFieldValue (Type targetType, String fieldName, Object target) 
  {
    FieldInfo field = GetHiddenField (targetType, fieldName);

    if (field != null) 
      return field.GetValue (target);		
    else 
      return null; 
  }

  private void SetHiddenFieldValue (Type targetType, String fieldName, Object target, object value) 
  {
    FieldInfo field = GetHiddenField (targetType, fieldName);

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

  private FieldInfo GetHiddenField (Type targetType, String fieldName) 
  {
    return targetType.GetField (
        fieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);
  }
  #endregion
}
}

