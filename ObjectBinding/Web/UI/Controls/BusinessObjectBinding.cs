using System;
using System.Reflection;
using System.ComponentModel;
using System.Web.UI;
using Rubicon.ObjectBinding;

namespace Rubicon.ObjectBinding.Web.Controls
{

// issues: data source and property identifier may be set/modified in any order
// only when a load/save request is made, the combination of both must be valid
// changes are noted in _bindingChanged, but only evaluated on EvaluateBinding().
public class BusinessObjectBinding
{
  private readonly IBusinessObjectBoundWebControl _control;

  private bool _bindingChanged = false;
  private IBusinessObjectDataSource _dataSource;
  private IBusinessObjectProperty _property;
  private string _propertyIdentifier;

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
      if (_dataSource == null)
      {
        ISite site = _control.Site;
        if (site != null && site.DesignMode)
        {
          System.Web.UI.DataBinding binding = _control.DataBindings ["DataSource"];
          if (binding != null)
          {
            Page page = ((Control)_control).Page;
            string name = binding.Expression.Trim();
            _dataSource = page.Site.Container.Components[name] as IBusinessObjectDataSource;
          }
        }
      }
      return _dataSource;
    }

    set
    {
      if (_dataSource != null)
        _dataSource.Unregister (Control);
      _dataSource = value; 
      if (value != null)
        value.Register (Control); 
      _bindingChanged = true;
    }
  }

  public IBusinessObjectProperty Property
  {
    get { return _property; }

    set 
    { 
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

  public void EvaluateBinding()
  {
    if (_bindingChanged && _property == null && DataSource != null && _propertyIdentifier != null && _propertyIdentifier.Length != 0)
    {
      _property = DataSource.BusinessObjectClass.GetProperty (_propertyIdentifier); 

      this.OnBindingChanged ();
      _bindingChanged = false;
    }
  }

  protected void OnBindingChanged()
  {
    if (BindingChanged != null)
      BindingChanged (this, new EventArgs());

  }

  public event EventHandler BindingChanged;
}

}
