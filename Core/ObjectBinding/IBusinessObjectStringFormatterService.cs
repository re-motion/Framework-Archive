using System;

namespace Rubicon.ObjectBinding
{
  //TODO: doc
  public interface IBusinessObjectStringFormatterService : IBusinessObjectService
  {
    string GetPropertyString (IBusinessObject businessObject, IBusinessObjectProperty property, string format);
  }
}