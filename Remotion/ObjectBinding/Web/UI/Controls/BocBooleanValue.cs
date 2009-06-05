// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.ServiceLocation;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocBooleanValue;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocBooleanValueBase;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> This control can be used to display or edit a tri-state value (true, false, and undefined). </summary>
  /// <include file='doc\include\UI\Controls\BocBooleanValue.xml' path='BocBooleanValue/Class/*' />
  [ValidationProperty ("ValidationValue")]
  [DefaultEvent ("SelectionChanged")]
  [ToolboxItemFilter ("System.Web.UI")]
  public class BocBooleanValue : BocBooleanValueBase, IBocBooleanValue
  {
    // constants

    private const string c_scriptFileUrl = "BocBooleanValue.js";

    private const string c_trueIcon = "CheckBoxTrue.gif";
    private const string c_falseIcon = "CheckBoxFalse.gif";
    private const string c_nullIcon = "CheckBoxNull.gif";

    private const string c_nullString = "null";

    private const string c_defaultResourceGroup = "default";

    // types

    /// <summary> A list of control specific resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources ("Remotion.ObjectBinding.Web.Globalization.BocBooleanValue")]
    protected enum ResourceIdentifier
    {
      /// <summary> The descripton rendered next the check box when it is checked. </summary>
      TrueDescription,
      /// <summary> The descripton rendered next the check box when it is not checked.  </summary>
      FalseDescription,
      /// <summary> The descripton rendered next the check box when its state is undefined.  </summary>
      NullDescription,
      /// <summary> The validation error message displayed when the null item is selected. </summary>
      NullItemValidationMessage
    }

    // static members

    private static readonly Type[] s_supportedPropertyInterfaces = new[]
                                                                   {
                                                                       typeof (IBusinessObjectBooleanProperty)
                                                                   };

    private static readonly string s_scriptFileKey = typeof (BocBooleanValue).FullName + "_Script";

    // member fields
    private bool? _value;

    private readonly Style _labelStyle;

    private bool _showDescription = true;

    private string _errorMessage;
    private readonly ArrayList _validators;

    // construction and disposing

    public BocBooleanValue ()
    {
      _labelStyle = new Style();
      _validators = new ArrayList();
    }

    // methods and properties

    public override void RenderControl (HtmlTextWriter writer)
    {
      EvaluateWaiConformity ();

      var factory = ServiceLocator.Current.GetInstance<IBocBooleanValueRendererFactory> ();
      var renderer = factory.CreateRenderer (new HttpContextWrapper (Context), writer, this);
      renderer.Render ();
    }

    /// <summary> Creates the list of validators required for the current binding and property settings. </summary>
    /// <include file='doc\include\UI\Controls\BocBooleanValue.xml' path='BocBooleanValue/CreateValidators/*' />
    public override BaseValidator[] CreateValidators ()
    {
      if (IsReadOnly || !IsRequired)
        return new BaseValidator[0];

      BaseValidator[] validators = new BaseValidator[1];

      CompareValidator notNullItemValidator = new CompareValidator ();
      notNullItemValidator.ID = ID + "_ValidatorNotNullItem";
      notNullItemValidator.ControlToValidate = ID;
      notNullItemValidator.ValueToCompare = c_nullString;
      notNullItemValidator.Operator = ValidationCompareOperator.NotEqual;
      if (StringUtility.IsNullOrEmpty (_errorMessage))
        notNullItemValidator.ErrorMessage = GetResourceManager ().GetString (ResourceIdentifier.NullItemValidationMessage);
      else
        notNullItemValidator.ErrorMessage = _errorMessage;
      validators[0] = notNullItemValidator;

      _validators.AddRange (validators);
      return validators;
    }
    
    public override void RegisterHtmlHeadContents (HttpContext context)
    {
      base.RegisterHtmlHeadContents (context);

      if (!HtmlHeadAppender.Current.IsRegistered (s_scriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (this, context, typeof (BocBooleanValue), ResourceType.Html, c_scriptFileUrl);
        HtmlHeadAppender.Current.RegisterJavaScriptInclude (s_scriptFileKey, scriptUrl);
      }
    }

    /// <summary> 
    ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user 
    ///   interface.
    /// </summary>
    /// <returns> 
    ///   A <see cref="String"/> <see cref="Array"/> containing the <see cref="Control.ClientID"/> of the
    ///   <see cref="HiddenField"/> if the control is in edit mode, or an empty array if the control is read-only.
    /// </returns>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.GetTrackedClientIDs">BusinessObjectBoundEditableWebControl.GetTrackedClientIDs</seealso>
    public override string[] GetTrackedClientIDs ()
    {
      return IsReadOnly ? new string[0] : new[] { GetHiddenFieldKey () };
    }

    public string GetHiddenFieldKey ()
    {
      return UniqueID + IdSeparator + "Boc_HiddenField";
    }

    public string GetHyperLinkKey ()
    {
      return UniqueID + IdSeparator + "Boc_HyperLink";
    }

    /// <summary> Gets or sets the current value. </summary>
    /// <value> The boolean value currently displayed or <see langword="null"/>. </value>
    /// <remarks> The dirty state is reset when the value is set. </remarks>
    [Browsable (false)]
    public override bool? Value
    {
      get { return _value; }
      set
      {
        IsDirty = true;
        _value = value;
      }
    }

    /// <summary> Gets or sets the validation error message. </summary>
    /// <value> 
    ///   The error message displayed when validation fails. The default value is an empty <see cref="String"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description ("Validation message displayed if there is an error.")]
    [Category ("Validator")]
    [DefaultValue ("")]
    public string ErrorMessage
    {
      get { return _errorMessage; }
      set
      {
        _errorMessage = value;
        for (int i = 0; i < _validators.Count; i++)
        {
          BaseValidator validator = (BaseValidator) _validators[i];
          validator.ErrorMessage = _errorMessage;
        }
      }
    }

    /// <summary>
    ///   Gets a flag that determines whether it is valid to generate HTML &lt;label&gt; tags referencing the
    ///   <see cref="TargetControl"/>.
    /// </summary>
    /// <value> Always <see langword="true"/>. </value>
    public override bool UseLabel
    {
      get { return true; }
    }

    /// <summary>
    ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its 
    ///   <see cref="Control.ClientID"/>.
    /// </summary>
    /// <value> The <see cref="HyperLink"/> if the control is in edit mode, otherwise the control itself. </value>
    public override Control TargetControl
    {
      get { return this; }
    }

    /// <summary> Gets the ID of the element to receive the focus when the page is loaded. </summary>
    /// <value>
    ///   Returns the <see cref="Control.ClientID"/> of the <see cref="HyperLink"/> if the control is in edit mode, 
    ///   otherwise <see langword="null"/>. 
    /// </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public override string FocusID
    {
      get { return IsReadOnly ? null : GetHyperLinkKey (); }
    }

    /// <summary> Gets the string representation of this control's <see cref="Value"/>. </summary>
    /// <remarks> 
    ///   <para>
    ///     Values can be <c>True</c>, <c>False</c>, and <c>null</c>. 
    ///   </para><para>
    ///     This property is used for validation.
    ///   </para>
    /// </remarks>
    [Browsable (false)]
    public string ValidationValue
    {
      get { return Value.HasValue ? Value.Value.ToString () : c_nullString; }
    }

    /// <summary>
    ///   Gets the <see cref="Style"/> that you want to apply to the <see cref="Label"/> used for displaying the 
    ///   description. 
    /// </summary>
    [Category ("Style")]
    [Description ("The style that you want to apply to the label used for displaying the description.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public override Style LabelStyle
    {
      get { return _labelStyle; }
    }

    /// <summary> Gets or sets the flag that determines whether to show the description next to the checkbox. </summary>
    /// <value> <see langword="true"/> to enable the description. The default value is <see langword="true"/>. </value>
    [Description ("The flag that determines whether to show the description next to the checkbox")]
    [Category ("Appearance")]
    [DefaultValue (true)]
    public bool ShowDescription
    {
      get { return _showDescription; }
      set { _showDescription = value; }
    }

    /// <summary>
    ///   The <see cref="BocBooleanValue"/> supports properties of type <see cref="IBusinessObjectBooleanProperty"/>.
    /// </summary>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
    protected override Type[] SupportedPropertyInterfaces
    {
      get { return s_supportedPropertyInterfaces; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      if (!IsDesignMode)
        Page.RegisterRequiresPostBack (this);
    }

    /// <summary>
    ///   Uses the <paramref name="postCollection"/> to determine whether the value of this control has been changed 
    ///   between postbacks.
    /// </summary>
    /// <include file='doc\include\UI\Controls\BocBooleanValue.xml' path='BocBooleanValue/LoadPostData/*' />
    protected override bool LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      string newValueAsString = PageUtility.GetPostBackCollectionItem (Page, GetHiddenFieldKey());
      bool? newValue = null;
      bool isDataChanged = false;
      if (newValueAsString != null)
      {
        if (newValueAsString != c_nullString)
          newValue = bool.Parse (newValueAsString);
        isDataChanged = _value != newValue;
      }
      if (isDataChanged)
      {
        _value = newValue;
        IsDirty = true;
      }
      return isDataChanged;
    }

    /// <summary> Checks whether the control conforms to the required WAI level. </summary>
    /// <exception cref="WcagException"> Thrown if the control does not conform to the required WAI level. </exception>
    protected virtual void EvaluateWaiConformity ()
    {
      if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
        WcagHelper.Instance.HandleError (1, this);
    }

    protected override void OnPreRender (EventArgs e)
    {
      EnsureChildControls();
      base.OnPreRender (e);

      IResourceManager resourceManager = GetResourceManager();
      LoadResources (resourceManager);
      DetermineClientScriptLevel();
    }

    protected override void LoadControlState (object savedState)
    {
      object[] values = (object[]) savedState;

      base.LoadControlState (values[0]);
      _value = (bool?) values[1];
    }

    protected override object SaveControlState ()
    {
      object[] values = new object[2];

      values[0] = base.SaveControlState();
      values[1] = _value;

      return values;
    }

    /// <summary>
    /// Override this method to change the default look of the <see cref="BocBooleanValue"/>
    /// </summary>
    protected virtual BocBooleanValueResourceSet CreateResourceSet ()
    {
      IResourceManager resourceManager = GetResourceManager ();

      BocBooleanValueResourceSet resourceSet = new BocBooleanValueResourceSet (
          c_defaultResourceGroup,
          ResourceUrlResolver.GetResourceUrl (this, Context, typeof (BocBooleanValue), ResourceType.Image, c_trueIcon),
          ResourceUrlResolver.GetResourceUrl (this, Context, typeof (BocBooleanValue), ResourceType.Image, c_falseIcon),
          ResourceUrlResolver.GetResourceUrl (this, Context, typeof (BocBooleanValue), ResourceType.Image, c_nullIcon),
          resourceManager.GetString (ResourceIdentifier.TrueDescription),
          resourceManager.GetString (ResourceIdentifier.FalseDescription),
          resourceManager.GetString (ResourceIdentifier.NullDescription)
          );

      return resourceSet;
    }

    /// <summary> Returns the <see cref="IResourceManager"/> used to access the resources for this control. </summary>
    protected virtual IResourceManager GetResourceManager ()
    {
      return GetResourceManager (typeof (ResourceIdentifier));
    }

    /// <summary> Loads the resources into the control's properties. </summary>
    protected override void LoadResources (IResourceManager resourceManager)
    {
      if (resourceManager == null)
        return;
      if (IsDesignMode)
        return;
      base.LoadResources (resourceManager);

      string key;
      key = ResourceManagerUtility.GetGlobalResourceKey (TrueDescription);
      if (! StringUtility.IsNullOrEmpty (key))
        TrueDescription = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (FalseDescription);
      if (! StringUtility.IsNullOrEmpty (key))
        FalseDescription = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (NullDescription);
      if (! StringUtility.IsNullOrEmpty (key))
        NullDescription = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (ErrorMessage);
      if (! StringUtility.IsNullOrEmpty (key))
        ErrorMessage = resourceManager.GetString (key);
    }

    /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
    protected override object ValueImplementation
    {
      get { return Value; }
      set { Value = ArgumentUtility.CheckType<bool?> ("value", value); }
    }

    /// <summary> The <see cref="BocCheckBox"/> supports only scalar properties. </summary>
    /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="false"/>. </returns>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
    protected override bool SupportsPropertyMultiplicity (bool isList)
    {
      return ! isList;
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocBooleanValue"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>bocBooleanValue</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    protected override string CssClassBase
    {
      get { return "bocBooleanValue"; }
    }

    private void DetermineClientScriptLevel ()
    {
      HasClientScript = !IsDesignMode;
    }

    string IBocBooleanValue.GetLabelClientID ()
    {
      return UniqueID + IdSeparator + "Boc_Label";
    }

    string IBocBooleanValue.GetImageClientID ()
    {
      return UniqueID + IdSeparator + "Boc_Image";
    }

    BocBooleanValueResourceSet IBocBooleanValue.CreateResourceSet ()
    {
      return CreateResourceSet ();
    }

    string IBocRenderableControl.CssClassBase
    {
      get { return CssClassBase; }
    }
  }
}