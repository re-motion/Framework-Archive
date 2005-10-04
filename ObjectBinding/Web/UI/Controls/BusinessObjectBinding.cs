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

/// <summary>
///   Encapsulates the functionality required for configuring business object binding in an
///   <see cref="IBusinessObjectBoundWebControl"/>.
/// </summary>
public class BusinessObjectBinding
{
  private readonly IBusinessObjectBoundWebControl _control;

  private IBusinessObjectDataSource _dataSource;
  private string _dataSourceControl;  
  private bool _dataSourceChanged = false;

  private IBusinessObjectProperty _property;
  private string _propertyIdentifier;
  private bool _bindingChanged = false;
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

  /// <summary> Initializes a new instance of the <b>BusinessObjectBinding</b> class. </summary>
  public BusinessObjectBinding (IBusinessObjectBoundWebControl control)
  {
    _control = control;
  }

  /// <summary> The <see cref="IBusinessObjectBoundWebControl"/> whose binding this instance encapsulates. </summary>
  public IBusinessObjectBoundWebControl Control
  {
    get { return _control; }
  }

  /// <summary> 
  ///   Gets or sets the <see cref="IBusinessObjectDataSource"/> for this <see cref="BusinessObjectBinding"/>.
  /// </summary>
  /// <remarks>
  ///   Unless an <b>DataSource</b> is set, <see cref="DataSourceControl"/> is used to identify the data source.
  /// </remarks>
  /// <exception cref="ArgumentException"> Thrown if an attempt is made to set a self reference. </exception>
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

  /// <summary> Uses the value of <see cref="DataSourceControl"/> to set <see cref="DataSource"/>. </summary>
  /// <remarks> 
  ///   Due to <b>Design Mode</b> behavior of Visual Studio .NET the <see cref="IComponent.Site"/> property is
  ///   utilized for finding the data source during <b>Design Mode</b>.
  /// </remarks>
  /// <exception cref="HttpException"> 
  ///   Thrown if the <see cref="DataSourceControl"/> is not <see langword="null "/> and could not be evaluated 
  ///   to a valid <see cref="IBusinessObjectDataSourceControl"/> during <b>Run Time</b>.
  /// </exception>
  public void EnsureDataSource()
  {
    if (_dataSourceChanged)
    {
      // set _dataSource from ID in _dataSourceControl
      if (StringUtility.IsNullOrEmpty (_dataSourceControl))
      {
        SetDataSource (null);
      }
      else
      {
        bool isDesignMode = ControlHelper.IsDesignMode (_control);

        Control namingContainer = _control.NamingContainer;
        if (namingContainer == null)
        {
          if (! isDesignMode)
            throw new HttpException (string.Format ("Cannot evaluate data source because control {0} has no naming container.", _control.ID));

          //  HACK: Designmode Naming container
          //  Not completely sure that Components[0] will always be the naming container.
          if (_control.Site.Container.Components.Count > 0)
            namingContainer = (_control.Site.Container.Components[0]) as Control;
          else
            return;
        }

        Control control = ControlHelper.FindControl (namingContainer, _dataSourceControl);
        if (control == null)
        {
          if (! isDesignMode)
            throw new HttpException(string.Format ("Unable to find control id '{0}' referenced by the DataSourceControl property of '{1}'.", _dataSourceControl, _control.ID));

          foreach (IComponent component in namingContainer.Site.Container.Components)
          {
            if (   component is IBusinessObjectDataSourceControl 
                && component is Control
                && ((Control) component).ID == _dataSourceControl)
            {
              control = (Control) component;
              break;
            }
          }

          if (control == null)
          {
            SetDataSource (null);
            _dataSourceChanged = true;
            return;
          }
        }

        IBusinessObjectDataSourceControl dataSource = control as IBusinessObjectDataSourceControl;
        if (dataSource == null)
          throw new HttpException(string.Format ("The control with the id '{0}' referenced by the DataSourceControl property of '{1}' does not identify a control of type '{2}'.", _dataSourceControl, _control.ID, typeof (IBusinessObjectDataSourceControl)));

        SetDataSource (dataSource);
      }

      _dataSourceChanged = false;
    }
  }

  /// <summary> Sets the new value of the <see cref="DataSource"/> property. </summary>
  /// <param name="dataSource"> The new <see cref="IBusinessObjectDataSource"/>. Can be <see langword="null"/>. </param>
  private void SetDataSource (IBusinessObjectDataSource dataSource)
  {
    if (_control == dataSource && _control is IBusinessObjectReferenceDataSource)
      throw new ArgumentException ("Assigning a reference data source as its own data source is not allowed.", "value");

    if (_dataSource == dataSource)
      return;

    if (_dataSource != null)
      _dataSource.Unregister (Control);

    _dataSource = dataSource;

    if (dataSource != null)
      dataSource.Register (Control); 
    _bindingChanged = true;
  }

  /// <summary> The <b>ID</b> of the <see cref="DataSource"/>. </summary>
  /// <value> A string or <see langword="null"/> if no <see cref="DataSource"/> is set. </value>
  /// <exception cref="ArgumentException"> Thrown if an attempt is made to set a self reference. </exception>
  public string DataSourceControl
  {
    get { return _dataSourceControl; }

    set 
    { 
      if (_control.ID != null && _control.ID == value && _control is IBusinessObjectReferenceDataSource)
          throw new ArgumentException ("Assigning a reference data source as its own data source is not allowed.", "value");
      if (_dataSourceControl != value)
      {
        _dataSourceControl = value;
        _dataSourceChanged = true;
      }
    }
  }

  /// <summary> Gets or sets the <see cref="IBusinessObjectProperty"/> used in the business object binding. </summary>
  /// <remarks>
  ///   Unless an <b>Property</b> is set, <see cref="PropertyIdentifier"/> and <see cref="DataSource"/> are used to 
  ///   identify the property.
  /// </remarks>
  /// <exception cref="ArgumentException"> Thrown if the <see cref="Control"/> does not support the <b>Property</b>. </exception>
  /// <exception cref="InvalidOperationException"> Thrown if an invalid <b>Property</b> has been specifed by the <see cref="PropertyIdentifier"/>. </exception>
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
        if (   _property == null 
            && DataSource != null 
            && DataSource.BusinessObjectClass != null 
            && ! StringUtility.IsNullOrEmpty (_propertyIdentifier))
        {
          IBusinessObjectProperty property = 
              DataSource.BusinessObjectClass.GetPropertyDefinition (_propertyIdentifier); 
          if (property == null)
          {
            throw new InvalidOperationException (
                string.Format ("The business object class '{0}' bound to {1} '{2}' via the DataSource " + 
                        "does not support the business object property '{3}'.",
                    DataSource.BusinessObjectClass.Identifier, 
                    _control.GetType().Name, 
                    _control.ID, 
                    _propertyIdentifier));
          }
          if (! _control.SupportsProperty (property))
          {
            throw new InvalidOperationException (
                string.Format ("{0} '{1}' does not support the business object property '{2}'.", 
                    _control.GetType().Name, _control.ID, _propertyIdentifier));
          }
          _property = property;
        }

        _bindingChanged = false;
        if (_isDesignModePropertyInitalized)
          _hasDesignModePropertyChanged = false;

        OnBindingChanged();
      }

      return _property; 
    }

    set 
    {
      if (value != null)
      {
        if (! Control.SupportsProperty (value))
        {
          throw new ArgumentException (
              string.Format ("{0} '{1}' does not support the  business object property '{2}'.", 
                  this.GetType().Name, _control.ID, value.Identifier), 
              "value");
        }
      }

      _property = value; 
      _propertyIdentifier = (value == null) ? string.Empty : value.Identifier;
      _bindingChanged = true;
    }
  }

  /// <summary> Gets or sets the string representation of the <see cref="Property"/>. </summary>
  /// <value> 
  ///   A string that can be used to query the <see cref="IBusinessObjectClass.GetPropertyDefinition"/> method for
  ///   the <see cref="IBusinessObjectProperty"/>. 
  /// </value>
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

  /// <summary> Executed when the <see cref="Property"/> is assigned a new value. </summary>
  protected void OnBindingChanged()
  {
    if (BindingChanged != null)
      BindingChanged (this, EventArgs.Empty);
  }

  /// <summary> Raised when the <see cref="Property"/> is assigned a new value. </summary>
  /// <remarks> 
  ///   Register for this event to execute code updating the <see cref="Control"/>'s state for the new binding.
  /// </remarks>
  public event EventHandler BindingChanged;
}

}
