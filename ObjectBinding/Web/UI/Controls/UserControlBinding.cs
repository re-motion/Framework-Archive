using System;
using System.Web.UI;
using System.ComponentModel;
using Rubicon.ObjectBinding.Web;
using Rubicon.Web.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary>
/// Control that allows a User Control to be bound to a business object data source and property.
/// </summary>
public class UserControlBinding: BusinessObjectBoundModifiableWebControl
{
  private string _userControlPath = string.Empty;
  private IDataEditControl _userControl = null;
  private BusinessObjectReferenceDataSourceControl _referenceDataSource = null;

  public string UserControlPath
  {
    get { return _userControlPath; }
    set { _userControlPath = value; }
  }

  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IDataEditControl UserControl
  {
    get { return _userControl; }
  }

  protected override void OnInit (EventArgs e)
  {
    base.OnInit (e);
    if (! ControlHelper.IsDesignMode (this, Context))
    {
      TemplateControl control = (TemplateControl) Page.LoadControl (_userControlPath);
      Controls.Add (control);
      _userControl = control as IDataEditControl;

      if (_userControl != null && DataSource != null)
      {
        IBusinessObjectDataSource dataSourceControl = this.DataSource;
        if (Property != null)
        {
          _referenceDataSource = new BusinessObjectReferenceDataSourceControl ();
          _referenceDataSource.DataSource = this.DataSource;
          _referenceDataSource.Property = this.Property;
          _referenceDataSource.EditMode = this.DataSource.EditMode;
          dataSourceControl = _referenceDataSource;
          Controls.Add (_referenceDataSource);
        }

        _userControl.EditMode = dataSourceControl.EditMode;
        _userControl.BusinessObject = dataSourceControl.BusinessObject;
      }
    }
  }

  protected override void OnLoad (EventArgs e)
  {
    base.OnLoad (e);
    if (_referenceDataSource != null)
    {
      _referenceDataSource.EditMode = this.DataSource.EditMode;
    }
    _userControl.EditMode = this.DataSource.EditMode;
  }


  public override void LoadValue (bool interim)
  {
    if (_referenceDataSource == null)
      throw new NotImplementedException();

    _referenceDataSource.LoadValue (interim);
    _userControl.BusinessObject = _referenceDataSource.BusinessObject;
    _userControl.LoadValues (interim);
  }

  public override void SaveValue (bool interim)
  {
    _userControl.SaveValues (interim);
    _referenceDataSource.BusinessObject = _userControl.BusinessObject;
    _referenceDataSource.SaveValue (interim);
  }

  protected override object ValueImplementation
  {
    get
    {
      if (_referenceDataSource == null)
        throw new InvalidOperationException ("Cannot get value if no property is set.");
      return _referenceDataSource.Value;
    }
    set
    {
      if (_referenceDataSource == null)
        throw new InvalidOperationException ("Cannot set value if no property is set.");
      _referenceDataSource.Value = value;
    }
  }

  public override bool IsDirty
  {
    get
    {
      foreach (IBusinessObjectBoundControl control in _userControl.DataSource.BoundControls)
      {
        BusinessObjectBoundModifiableWebControl modifiableControl = control as BusinessObjectBoundModifiableWebControl;
        if (modifiableControl != null && modifiableControl.IsDirty)
          return true;
      }
      return false;
    }
    set
    {
      throw new NotSupportedException();
    }
  }

  protected override void Render (HtmlTextWriter writer)
  {
    if (ControlHelper.IsDesignMode (this, Context))
    {
      string type = "Unknown";
      IBusinessObjectReferenceProperty property = Property as IBusinessObjectReferenceProperty;
      if (property != null && property.ReferenceClass != null)
        type = property.ReferenceClass.Identifier;

      writer.Write (
          "<table style=\"font-family: arial; font-size: x-small; BORDER-RIGHT: gray 1px solid; BORDER-TOP: white 1px solid; BORDER-LEFT: white 1px solid; BORDER-BOTTOM: gray 1px solid; BACKGROUND-COLOR: #d4d0c8\">"
          + "<tr><td colspan=\"2\"><b>User Control</b></td></tr>"
          + "<tr><td>Data Source:</td><td>{0}</td></tr>"
          + "<tr><td>Property:</td><td>{1}</td></tr>"
          + "<tr><td>Type:</td><td>{2}</td></tr>"
          + "<tr><td>User Control:</td><td>{3}</td></tr>",
          DataSourceControl, 
          PropertyIdentifier,
          type,
          _userControlPath);
    }

    base.Render (writer);
  }

  protected override Type[] SupportedPropertyInterfaces
  {
    get { return new Type[] { typeof (IBusinessObjectReferenceProperty) }; }
  }

  protected override bool SupportsPropertyMultiplicity (bool isList)
  {
    return !isList;
  }


}

}
