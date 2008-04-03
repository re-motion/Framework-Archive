using System;

namespace Remotion.ObjectBinding
{
  //TODO: doc
  public interface IBusinessObjectStringFormatterService : IBusinessObjectService
  {
    string GetPropertyString (IBusinessObject businessObject, IBusinessObjectProperty property, string format);
  }
}