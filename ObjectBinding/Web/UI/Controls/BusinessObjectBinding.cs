using System;
using System.Reflection;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.Design;
using Rubicon.Web.Utilities;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{

public class BusinessObjectBinding
{
  private readonly IBusinessObjectBoundWebControl _control;

  private IBusinessObjectDataSource _dataSource;
  private string _dataSourceControl;  
  private bool _dataSourceChanged = false;

  private bool _bindingChanged = false;
  private IBusinessObjectProperty _property;
  private string _propertyIdentifier;
  /// <summary>
  ///   Set after the <see cref="DataSource"/> returned a value for the first time
  ///   in the <c>get accessor</c> of <see cref="Property"/>.
  /// </summary>
  private bool _isDesignModePropertyInitalized = false;
  /// <summary>
  ///   Set in the <c>get accessor</c> of <see cref="Property"/> when <see cref="_dataSourceChanged"/> is set.
  ///   Reset after the <see cref="Property"/> is bound.
  /// </summary>
  private bool _hasDesignModePropertyChanged = false;

  public BusinessObjectBinding (IBusinessObjectBoundWebControl control)
  {
    _control = control;
  }

  public IBusinessObjectBoundWebControl Control
  {
    get { return _control; }
  }

  public virtual IBusinessObjectDataSource DataSource
  {
    get 
    { 
      EnsureDataSource();
      return _dataSource; 
    }

    set
    {
      SetDataSource (value);

      Control dataSourceControl = value as Control;
      if (dataSourceControl != null)
        _dataSourceControl = dataSourceControl.ID;
      else
        _dataSourceControl = null;
    }
  }

  public void EnsureDataSource()
  {
    if (_dataSourceChanged)
    {
      // set _dataSource from ID in _dataSourceControl
      if (_dataSourceControl == null)
      {
        SetDataSource (null);
      }
      else
      {
        bool isDesignMode = ControlHelper.IsDesignMode (_control);

        if (_control.NamingContainer == null)
        {
          if (isDesignMode)
            return;
          else
            throw new HttpException (string.Format ("Cannot evaluate data source because control {0} has no naming container.", _control.ID));
        }

        Control control = ControlHelper.FindControl (_control.NamingContainer, _dataSourceControl);
        if (control == null)
        {
          if (isDesignMode)
          {
            // HACK: Find a way to restart the page's lifecycle.
            // On the first round-trip, not only the controls up to the current control exist.
            SetDataSource (null);
            _dataSourceChanged = true;
            return;
          }
          else
          {
            throw new HttpException(string.Format ("Unable to find control id '{0}' referenced by the DataSourceControl property of '{1}'.", _dataSourceControl, _control.ID));
          }
        }

        IBusinessObjectDataSourceControl dataSource = control as IBusinessObjectDataSourceControl;
        if (dataSource == null)
          throw new HttpException(string.Format ("The value '{0}' of the DataSource property of '{1}' cannot be converted to type '{2}'.", _dataSourceControl, _control.ID, typeof (IBusinessObjectDataSourceControl)));

        SetDataSource (dataSource);
      }

      _dataSourceChanged = false;
    }
  }

  private void SetDataSource (IBusinessObjectDataSource dataSource)
  {
    if (_dataSource == dataSource)
      return;

    if (_dataSource != null)
      _dataSource.Unregister (Control);

    _dataSource = dataSource;

    if (dataSource != null)
      dataSource.Register (Control); 
    _bindingChanged = true;
  }

  public string DataSourceControl
  {
    get { return _dataSourceControl; }

    set 
    { 
      if (_dataSourceControl != value)
      {
        _dataSourceControl = value;
        _dataSourceChanged = true;
      }
    }
  }

  public IBusinessObjectProperty Property
  {
    get 
    { 
      if (ControlHelper.IsDesignMode (_control))
      {
        if (! _isDesignModePropertyInitalized && DataSource != null)
          _isDesignModePropertyInitalized = true;
        _hasDesignModePropertyChanged |= _dataSourceChanged;
      }

      // evaluate binding
      if (_bindingChanged || _hasDesignModePropertyChanged && _isDesignModePropertyInitalized)
      {
        if (_property == null && DataSource != null && _propertyIdentifier != null && _propertyIdentifier.Length != 0)
        {
          IBusinessObjectProperty property = DataSource.BusinessObjectClass.GetPropertyDefinition (_propertyIdentifier); 
          if (! Control.SupportsProperty (property))
            throw new ArgumentException ("Property 'Property' does not support the property '" + _propertyIdentifier + "'.");
          _property = property;
        }

        _bindingChanged = false;
        if (_isDesignModePropertyInitalized)
          _hasDesignModePropertyChanged = false;

        this.OnBindingChanged();
      }

      return _property; 
    }

    set 
    {
      if (! Control.SupportsProperty (value))
        throw new ArgumentException ("Property 'Property' does not support this value.", "value");

      _property = value; 
      _propertyIdentifier = (value == null) ? string.Empty : value.Identifier;
      _bindingChanged = true;
    }
  }

  public string PropertyIdentifier
  {
    get { return _propertyIdentifier; }

    set 
    { 
      _propertyIdentifier = value;
      _property = null;
      _bindingChanged = true;
    }
  }

  [Obsolete]
  public void EvaluateBinding()
  {
  }

  protected void OnBindingChanged()
  {
    if (BindingChanged != null)
      BindingChanged (this, EventArgs.Empty);

  }

  public event EventHandler BindingChanged;
}

}
