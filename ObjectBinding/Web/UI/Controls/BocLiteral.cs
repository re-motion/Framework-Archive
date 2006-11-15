using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.Globalization;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.Utilities;

namespace Rubicon.ObjectBinding.Web.UI.Controls
{

  /// <summary> This control can be used to render text without any escaping applied. </summary>
  [ToolboxItemFilter ("System.Web.UI")]
  public class BocLiteral : BusinessObjectBoundWebControl
  {
    //  constants

    //  statics

    private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { typeof (IBusinessObjectStringProperty) };

    // types

    // fields

    private string _text = string.Empty;
    private LiteralMode _mode = LiteralMode.Transform;

    public BocLiteral ()
    {
    }

    protected override void AddAttributesToRender (HtmlTextWriter writer)
    {
      base.AddAttributesToRender (writer);

      if (StringUtility.IsNullOrEmpty (CssClass) && StringUtility.IsNullOrEmpty (Attributes["class"]))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);
    }

    protected override void RenderContents (HtmlTextWriter writer)
    {
      if (!string.IsNullOrEmpty (_text))
      {
        if (_mode != LiteralMode.Encode)
          writer.Write (_text);
        else
          HttpUtility.HtmlEncode (_text, writer);
      }
      else if (IsDesignMode)
      {
        writer.Write ("##");
      }
    }

    protected override void LoadViewState (object savedState)
    {
      object[] values = (object[]) savedState;
      base.LoadViewState (values[0]);
      _mode = (LiteralMode) values[1];
    }

    protected override object SaveViewState ()
    {
      object[] values = new object[4];
      values[0] = base.SaveViewState ();
      values[1] = _mode;
      return values;
    }


    /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
    public override void LoadValue (bool interim)
    {
      if (Property != null && DataSource != null && DataSource.BusinessObject != null)
      {
        object value = DataSource.BusinessObject.GetProperty (Property);
        LoadValueInternal (value, interim);
      }
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value"> A <see cref="String"/> to load or <see langword="null"/>. </param>
    public void LoadUnboundValue (object value, bool interim)
    {
      LoadValueInternal (value, interim);
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value"> A <see cref="String"/> to load or <see langword="null"/>. </param>
    public void LoadUnboundValue (string value, bool interim)
    {
      LoadValueInternal (value, interim);
    }

    /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
    protected virtual void LoadValueInternal (object value, bool interim)
    {
      Value = (string) value;
    }

    /// <summary> Gets or sets the current value. </summary>
    [Description ("Gets or sets the current value.")]
    [Browsable (false)]
    public new string Value
    {
      get { return _text; }
      set { _text = value; }
    }

    /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
    protected override object ValueImplementation
    {
      get { return Value; }
      set { Value = (string) value; }
    }

    /// <summary> Gets or sets the string representation of the current value. </summary>
    /// <value> 
    ///   An empty <see cref="String"/> if the control's value is <see langword="null"/> or empty. 
    ///   The default value is an empty <see cref="String"/>. 
    /// </value>
    [Description ("Gets or sets the string representation of the current value.")]
    [Category ("Data")]
    [DefaultValue ("")]
    public string Text
    {
      get { return StringUtility.NullToEmpty (_text); }
      set { _text = value; }
    }

    /// <summary> The <see cref="BocLiteral"/> supports only scalar properties. </summary>
    /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="false"/>. </returns>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
    protected override bool SupportsPropertyMultiplicity (bool isList)
    {
      return !isList;
    }

    /// <summary>
    ///   The <see cref="BocLiteral"/> supports properties of type <see cref="IBusinessObjectStringProperty"/>.
    /// </summary>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
    protected override Type[] SupportedPropertyInterfaces
    {
      get { return s_supportedPropertyInterfaces; }
    }

    /// <summary>
    ///   Gets a flag that determines whether it is valid to generate HTML &lt;label&gt; tags referencing the <see cref="TargetControl"/>.
    /// </summary>
    /// <value> Returns always <see langword="false"/>. </value>
    public override bool UseLabel
    {
      get { return false; }
    }

    /// <summary>
    ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its <see cref="Control.ClientID"/>.
    /// </summary>
    /// <value> Returns the control itself. </value>
    public override Control TargetControl
    {
      get { return (Control) this; }
    }

    public LiteralMode Mode
    {
      get { return _mode; }
      set { _mode = value; }
    }

    #region protected virtual string CssClass...
    /// <summary> Gets the CSS-Class applied to the <see cref="BocLiteral"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>bocLiteral</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    protected virtual string CssClassBase
    { get { return "bocLiteral"; } }
    #endregion
  }
}
