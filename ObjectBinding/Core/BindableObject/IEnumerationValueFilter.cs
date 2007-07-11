using System;

namespace Rubicon.ObjectBinding.BindableObject
{
  public interface IEnumerationValueFilter
  {
    IEnumerationValueInfo[] GetEnabledValues (
        IEnumerationValueInfo[] values, IBusinessObject businessObject, IBusinessObjectEnumerationProperty property);
  }
}