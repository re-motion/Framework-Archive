using System;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Rubicon.ObjectBinding.Web.Design
{

public class TypeDescriptorContext : ITypeDescriptorContext
{
  private BocListDesigner _designer;
  private PropertyDescriptor _propertyDescriptor;

  public TypeDescriptorContext (
      BocListDesigner designer, 
      PropertyDescriptor propertyDescriptor)
  {
    _designer = designer;
    _propertyDescriptor = propertyDescriptor;
  }

  private IComponentChangeService ComponentChangeService
  {
    get { return (IComponentChangeService) this.GetService (typeof (IComponentChangeService)); }
  }

  public object GetService (Type serviceType)
  {
    return ((IServiceProvider)_designer).GetService(serviceType);
  }

  public IContainer Container
  {
    get { return _designer.Component.Site.Container; }
  }

  public object Instance
  {
    get { return _designer.Component; }
  }

  public PropertyDescriptor PropertyDescriptor
  {
    get { return _propertyDescriptor; }
  }

  public void OnComponentChanged()
  {
    if (ComponentChangeService != null)
      ComponentChangeService.OnComponentChanged (Instance, PropertyDescriptor, null, null);
  }

  public bool OnComponentChanging()
  {
    if (ComponentChangeService != null)
    {
      try
      {
        ComponentChangeService.OnComponentChanging (Instance, PropertyDescriptor);
      }
      catch (CheckoutException e)
      {
        if (e == CheckoutException.Canceled)
          return false;
        throw e;
      }
    }
    return true;
  }
}

}
