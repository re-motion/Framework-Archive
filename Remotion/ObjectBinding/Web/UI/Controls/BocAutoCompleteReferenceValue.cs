// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.ComponentModel;
using System.Drawing.Design;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Design;
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  [ValidationProperty ("BusinessObjectUniqueIdentifier")]
  [DefaultEvent ("SelectionChanged")]
  [ToolboxItemFilter ("System.Web.UI")]
  [Designer (typeof (BocDesigner))]
  public class BocAutoCompleteReferenceValue
      :
          BocReferenceValueBase,
          IBocAutoCompleteReferenceValue,
          IFocusableControl
  {
    // constants

    /// <summary> The text displayed when control is displayed in desinger, is read-only, and has no contents. </summary>
    private const string c_designModeEmptyLabelContents = "##";

    private const string c_textBoxIDPostfix = "Boc_TextBox";
    private const string c_hiddenFieldIDPostfix = "Boc_HiddenField";
    private const string c_buttonIDPostfix = "Boc_DropDownButton";
    private const string c_iconIDPostfix = "Boc_Icon";

    // types

    // static members

    // member fields

    private readonly Style _commonStyle;
    private readonly SingleRowTextBoxStyle _textBoxStyle;
    private readonly Style _labelStyle;

    /// <summary> 
    ///   The object returned by <see cref="BocReferenceValue"/>. 
    ///   Does not require <see cref="System.Runtime.Serialization.ISerializable"/>. 
    /// </summary>
    private IBusinessObjectWithIdentity _value;

    /// <summary> The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> of the current object. </summary>
    private string _displayName;

    private string _serviceMethod = string.Empty;
    private string _servicePath = string.Empty;
    private string _args = string.Empty;
    private int _completionSetCount = 10;
    private int _dropDownDisplayDelay = 1000;
    private int _dropDownRefreshDelay = 2000;
    private int _selectionUpdateDelay = 200;


    // construction and disposing

    public BocAutoCompleteReferenceValue ()
    {
      _commonStyle = new Style();
      _textBoxStyle = new SingleRowTextBoxStyle();
      _labelStyle = new Style();

      EnableIcon = true;
      ShowOptionsMenu = true;
    }

    // methods and properties

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      base.RegisterHtmlHeadContents (htmlHeadAppender);

      var renderer = CreateRenderer();
      renderer.RegisterHtmlHeadContents (htmlHeadAppender);
    }

    protected override string ValueContainingControlID
    {
      get { return HiddenFieldUniqueID; }
    }

    protected override void OnDataChanged ()
    {
      _displayName = PageUtility.GetPostBackCollectionItem (Page, TextBoxClientID);
    }

    /// <summary> Called when the state of the control has changed between postbacks. </summary>
    protected override void RaisePostDataChangedEvent ()
    {
      if (!IsReadOnly && Enabled)
        OnSelectionChanged();
    }

    /// <summary> Checks whether the control conforms to the required WAI level. </summary>
    /// <exception cref="WcagException"> Thrown if the control does not conform to the required WAI level. </exception>
    protected virtual void EvaluateWaiConformity ()
    {
      if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
        WcagHelper.Instance.HandleError (1, this);
    }

    public override void PrepareValidation ()
    {
      base.PrepareValidation();

      if (!IsReadOnly)
        SetEditModeValue();
    }

    private void SetEditModeValue ()
    {
      IBusinessObjectWithIdentity obj = Value;
      if (obj != null)
        _displayName = GetDisplayName (obj);
    }

    protected override void OnPreRender (EventArgs e)
    {
      EnsureChildControls();
      base.OnPreRender (e);

      LoadResources (GetResourceManager());

      if (!IsDesignMode)
        PreRenderMenuItems();

      if (HasOptionsMenu)
        PreRenderOptionsMenu();

      if (Command != null)
        Command.RegisterForSynchronousPostBack (this, null, string.Format ("BocAutoCompleteReferenceValue '{0}', Object Command", ID));

      if (!IsReadOnly)
        PreRenderEditModeValue();
    }

    protected override string GetOptionsMenuTitle ()
    {
      return GetResourceManager().GetString (ResourceIdentifier.OptionsTitle);
    }

    protected override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      EvaluateWaiConformity ();

      var renderer = CreateRenderer();
      renderer.Render (CreateRenderingContext(writer));
    }

    protected virtual IBocAutoCompleteReferenceValueRenderer CreateRenderer ()
    {
      return ServiceLocator.GetInstance<IBocAutoCompleteReferenceValueRenderer> ();
    }

    protected virtual BocAutoCompleteReferenceValueRenderingContext CreateRenderingContext (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      return new BocAutoCompleteReferenceValueRenderingContext (Context, writer, this);
    }

    protected override void LoadControlState (object savedState)
    {
      object[] values = (object[]) savedState;

      base.LoadControlState (values[0]);
      InternalValue = (string) values[1];
      _displayName = (string) values[2];
    }

    protected override object SaveControlState ()
    {
      object[] values = new object[3];

      values[0] = base.SaveControlState();
      values[1] = InternalValue;
      values[2] = _displayName;

      return values;
    }


    /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='..\Web\doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/LoadValue/*' />
    public override void LoadValue (bool interim)
    {
      if (interim)
        return;

      if (Property == null)
        return;

      if (DataSource == null)
        return;

      IBusinessObjectWithIdentity value = null;

      if (DataSource.BusinessObject != null)
        value = (IBusinessObjectWithIdentity) DataSource.BusinessObject.GetProperty (Property);

      LoadValueInternal (value, false);
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value"> 
    ///   The object implementing <see cref="IBusinessObjectWithIdentity"/> to load, or <see langword="null"/>. 
    /// </param>
    /// <include file='..\Web\doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/LoadUnboundValue/*' />
    public void LoadUnboundValue (IBusinessObjectWithIdentity value, bool interim)
    {
      LoadValueInternal (value, interim);
    }

    /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
    protected virtual void LoadValueInternal (IBusinessObjectWithIdentity value, bool interim)
    {
      if (!interim)
      {
        Value = value;
        IsDirty = false;
      }
    }

    /// <summary> Saves the <see cref="Value"/> into the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='..\Web\doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/SaveValue/*' />
    public override void SaveValue (bool interim)
    {
      if (interim)
        return;

      if (IsDirty && SaveValueToDomainModel())
        IsDirty = false;
    }

    /// <summary> Returns the <see cref="IResourceManager"/> used to access the resources for this control. </summary>
    protected override IResourceManager GetResourceManager ()
    {
      return GetResourceManager (typeof (ResourceIdentifier));
    }

    string IBocReferenceValueBase.GetLabelText ()
    {
      string text;
      if (InternalValue != null)
        text = _displayName;
      else
        text = String.Empty;
      if (StringUtility.IsNullOrEmpty (text) && IsDesignMode)
      {
        text = c_designModeEmptyLabelContents;
        //  Too long, can't resize in designer to less than the content's width
        //  _label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
      }

      return text;
    }

    private void PreRenderEditModeValue ()
    {
      SetEditModeValue();
    }

    /// <summary> Gets or sets the current value. </summary>
    [Browsable (false)]
    public override IBusinessObjectWithIdentity Value
    {
      get
      {
        if (InternalValue == null)
          _value = null;
            //  Only reload if value is outdated
        else if (_value == null || _value.UniqueIdentifier != InternalValue)
        {
          if (Property != null)
            _value = ((IBusinessObjectClassWithIdentity) Property.ReferenceClass).GetObject (InternalValue);
          else if (DataSource != null)
            _value = ((IBusinessObjectClassWithIdentity) DataSource.BusinessObjectClass).GetObject (InternalValue);
        }
        return _value;
      }
      set
      {
        IsDirty = true;

        _value = value;

        if (value != null)
        {
          InternalValue = value.UniqueIdentifier;
          _displayName = GetDisplayName (value);
        }
        else
        {
          InternalValue = null;
          _displayName = null;
        }
      }
    }

    /// <summary>Gets a flag indicating whether the <see cref="BocAutoCompleteReferenceValue"/> contains a value. </summary>
    public override bool HasValue
    {
      get { return InternalValue != null; }
    }

    /// <summary> Intern
    ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user interface.
    /// </summary>
    /// <returns> 
    ///   A <see cref="String"/> <see cref="Array"/> containing the <see cref="Control.ClientID"/> of the
    ///   <see cref="DropDownList"/> if the control is in edit mode, or an empty array if the control is read-only.
    /// </returns>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.GetTrackedClientIDs">BusinessObjectBoundEditableWebControl.GetTrackedClientIDs</seealso>
    public override string[] GetTrackedClientIDs ()
    {
      return IsReadOnly ? new string[0] : new[] { TextBoxClientID };
    }

    /// <summary> The <see cref="BocReferenceValue"/> supports only scalar properties. </summary>
    /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="false"/>. </returns>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
    protected override bool SupportsPropertyMultiplicity (bool isList)
    {
      return !isList;
    }

    /// <summary>
    ///   Gets a flag that determines whether it is valid to generate HTML &lt;label&gt; tags referencing the
    ///   <see cref="BocReferenceValueBase.TargetControl"/>.
    /// </summary>
    /// <value> Returns always <see langword="true"/>. </value>
    public override bool UseLabel
    {
      get { return true; }
    }

    /// <summary> Gets the ID of the element to receive the focus when the page is loaded. </summary>
    /// <value>
    ///   Returns the <see cref="Control.ClientID"/> of the <see cref="DropDownList"/> if the control is in edit mode, 
    ///   otherwise <see langword="null"/>. 
    /// </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public string FocusID
    {
      get { return IsReadOnly ? null : TextBoxClientID; }
    }

    /// <summary>
    ///   Gets the style that you want to apply to the <see cref="TextBox"/> (edit mode) 
    ///   and the <see cref="Label"/> (read-only mode).
    /// </summary>
    /// <remarks>
    ///   Use the <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> to assign individual 
    ///   style settings for the respective modes. Note that if you set one of the <b>Font</b> 
    ///   attributes (Bold, Italic etc.) to <see langword="true"/>, this cannot be overridden using 
    ///   <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/>  properties.
    /// </remarks>
    [Category ("Style")]
    [Description ("The style that you want to apply to the TextBox (edit mode) and the Label (read-only mode).")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public Style CommonStyle
    {
      get { return _commonStyle; }
    }

    /// <summary> Gets the style that you want to apply to the <see cref="TextBox"/> (edit mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
    [Category ("Style")]
    [Description ("The style that you want to apply to the TextBox (edit mode) only.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public SingleRowTextBoxStyle TextBoxStyle
    {
      get { return _textBoxStyle; }
    }

    /// <summary> Gets the style that you want to apply to the <see cref="Label"/> (read-only mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
    [Category ("Style")]
    [Description ("The style that you want to apply to the Label (read-only mode) only.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public Style LabelStyle
    {
      get { return _labelStyle; }
    }

    [Category ("AutoComplete")]
    [DefaultValue ("")]
    public string ServiceMethod
    {
      get { return _serviceMethod; }
      set { _serviceMethod = StringUtility.NullToEmpty (value); }
    }

    [Editor (typeof (UrlEditor), typeof (UITypeEditor))]
    [Category ("AutoComplete")]
    [DefaultValue ("")]
    public string ServicePath
    {
      get { return _servicePath; }
      set { _servicePath = StringUtility.NullToEmpty (value); }
    }

    [Category ("AutoComplete")]
    [DefaultValue (10)]
    public int CompletionSetCount
    {
      get { return _completionSetCount; }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException ("value", "The CompletionSetCount must be greater than or equal to 0.");
        _completionSetCount = value;
      }
    }

    [Category ("AutoComplete")]
    [DefaultValue (1000)]
    [Description ("Time in milliseconds before the drop down is shown for the first time.")]
    public int DropDownDisplayDelay
    {
      get { return _dropDownDisplayDelay; }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException ("value", "The DropDownDisplayDelay must be greater than or equal to 0.");
        _dropDownDisplayDelay = value;
      }
    }

    [Category ("AutoComplete")]
    [DefaultValue (2000)]
    [Description ("Time in milliseconds before the contents of the drop down is refreshed.")]
    public int DropDownRefreshDelay
    {
      get { return _dropDownRefreshDelay; }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException ("value", "The DropDownRefreshDelay must be greater than or equal to 0.");
        _dropDownRefreshDelay = value;
      }
    }

    [Category ("AutoComplete")]
    [DefaultValue (200)]
    [Description ("Time in milliseconds before the user's input is used to select the best match in the drop down list.")]
    public int SelectionUpdateDelay
    {
      get { return _selectionUpdateDelay; }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException ("value", "The SelectionUpdateDelay must be greater than or equal to 0.");
        _selectionUpdateDelay = value;
      }
    }

    [Category ("AutoComplete")]
    [DefaultValue ("")]
    public string Args
    {
      get { return _args; }
      set { _args = value; }
    }

    bool IBocReferenceValueBase.IsCommandEnabled (bool readOnly)
    {
      return IsCommandEnabled (readOnly);
    }

    public string TextBoxUniqueID
    {
      get { return UniqueID + IdSeparator + c_textBoxIDPostfix; }
    }

    public string TextBoxClientID
    {
      get { return ClientID + ClientIDSeparator + c_textBoxIDPostfix; }
    }

    string IBocAutoCompleteReferenceValue.DropDownButtonClientID
    {
      get { return ClientID + ClientIDSeparator + c_buttonIDPostfix; }
    }

    public string HiddenFieldClientID
    {
      get { return ClientID + ClientIDSeparator + c_hiddenFieldIDPostfix; }
    }

    public string HiddenFieldUniqueID
    {
      get { return UniqueID + IdSeparator + c_hiddenFieldIDPostfix; }
    }

    string IBocReferenceValueBase.IconClientID
    {
      get { return ClientID + ClientIDSeparator + c_iconIDPostfix; }
    }

    string IBocReferenceValueBase.LabelClientID
    {
      get { return ClientID + ClientIDSeparator + "Boc_Label"; }
    }

    bool IBocRenderableControl.IsDesignMode
    {
      get { return base.IsDesignMode; }
    }

    IconInfo IBocReferenceValueBase.GetIcon ()
    {
      return GetIcon (Value, Property == null ? null : Property.ReferenceClass.BusinessObjectProvider);
    }

    DropDownMenu IBocReferenceValueBase.OptionsMenu
    {
      get { return OptionsMenu; }
    }

    bool IBocReferenceValueBase.HasOptionsMenu
    {
      get { return HasOptionsMenu; }
    }

    string IBocAutoCompleteReferenceValue.NullValueString
    {
      get { return c_nullIdentifier; }
    }

    protected override string GetSelectionCountFunction ()
    {
      return "function() { return BocAutoCompleteReferenceValue.GetSelectionCount ('" + HiddenFieldClientID + "', '" + c_nullIdentifier + "'); }";
    }
  }
}
