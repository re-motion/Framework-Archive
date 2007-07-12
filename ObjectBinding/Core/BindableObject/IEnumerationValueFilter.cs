using System;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public interface IEnumerationValueFilter
  {
    bool IsEnabled (IEnumerationValueInfo value, IBusinessObject businessObject, IBusinessObjectEnumerationProperty property);
  }
}