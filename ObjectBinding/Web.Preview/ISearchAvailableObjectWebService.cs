using System;

namespace Rubicon.ObjectBinding.Web
{
  public interface ISearchAvailableObjectWebService
  {
    BusinessObjectWithIdentityProxy[] Search (
        string prefixText,
        int? completionSetCount,
        string businessObjectClass,
        string businessObjectProperty,
        string businessObjectID,
        string args);
  }
}
